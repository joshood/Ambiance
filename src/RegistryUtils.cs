/// Thomas Freudenberg's RegistryMonitor, with some modifications to pass data when change events occur.
/// <a href="http://www.codeproject.com/Articles/4502/RegistryMonitor-a-NET-wrapper-class-for-RegNotifyC">RegistryMonitor</a>
/// Used under <a href="http://www.codeproject.com/info/cpol10.aspx">The Code Project Open License (CPOL)</a>

using System;
using System.ComponentModel; // InvalidEnumArgumentException
using System.IO; // ErrorEventHandler
using System.Runtime.InteropServices; // DllImport
using System.Threading;

using Microsoft.Win32; // RegistryKey, RegistryValueKind
using Microsoft.Win32.SafeHandles;

namespace RegistryUtils
{

    /// <summary>
    /// Filter for notifications reported by <see cref="RegistryMonitor"/>
    /// </summary>
    [Flags]
    public enum RegChangeNotifyFilter
    {
        /// <summary>Subkey is added or deleted</summary>
        Key = 1,
        /// <summary>Attributes of the key are changed, such as security descriptor info</summary>
        Attribute = 2,
        /// <summary>Value of the key changed (including adding/deleting a value)</summary>
        Value = 4,
        /// <summary>Security descriptor of the key has changed</summary>
        Security = 8,
    }

    /// <summary>
    /// This class is used to pass the registry key when a change event happens, to avoid having to go query what changed.
    /// </summary>
    public class OnRegChangedEventArgs : EventArgs
    {
        public DateTime timeChanged { get; set; }
        public RegistryKey changedKey { get; set; }
    }

    /// <summary>
    /// <b>RegistryMonitor</b> allows monitoring specific registry keys, using Win32 APIs.
    /// </summary>
    /// <remarks>
    /// Monitored registry keys generate events when changed. You can subscribe to these events
    /// by adding a delegate to <see cref="RegChanged"/>.
    /// <para>
    /// This class acts as a wrapper for thw Win32 API 
    /// <a href="http://msdn.microsoft.com/library/en-us/sysinfo/base/regnotifychangekeyvalue.asp">
    /// RegNotifyChangeKeyValue</a>, which is not covered by <see cref="Microsoft.Win32.RegistryKey"/>.
    /// </para>
    /// <example>
    /// This sample shows how to monitor <c>HKEY_CURRENT_USER\Environment</c>
    /// <code>
    /// public class MonitorExample
    /// {
    ///     static void Main()
    ///     {
    ///         RegistryMonitor regMon = new RegistryMonitor(RegistryHive.CurrentUser, "Environment");
    ///         regMon.RegChanged += new EventHandler(OnRegChanged);
    ///         regMon.Start();
    /// 
    ///         while(true);
    ///     }
    /// 
    ///     private void OnRegChanged(object sender, EventArgs e)
    ///     {
    ///         Console.WriteLine("Registry key changed");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public class RegistryMonitor : IDisposable
    {
        #region P/Invoke

        // advapi32.dll functions
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int RegOpenKeyEx(IntPtr hKey,
                                               string subKey,
                                               uint options,
                                               int samDesired,
                                               out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegNotifyChangeKeyValue(IntPtr hKey,
                                                          bool bWatchSubtree,
                                                          RegChangeNotifyFilter dwNotifyFilter,
                                                          SafeWaitHandle hEvent,
                                                          bool fAsynchronous);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(IntPtr hKey);

        // advapi32.dll constants
        private const int KEY_QUERY_VALUE = 0x0001;
        private const int KEY_NOTIFY = 0x0010;
        private const int STANDARD_RIGHTS_READ = 0x00020000;

        // registry hives
        private static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(unchecked((int)0x80000000));
        private static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(unchecked((int)0x80000001));
        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((int)0x80000002));
        private static readonly IntPtr HKEY_USERS = new IntPtr(unchecked((int)0x80000003));
        private static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(unchecked((int)0x80000004));
        private static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(unchecked((int)0x80000005));
        private static readonly IntPtr HKEY_DYN_DATA = new IntPtr(unchecked((int)0x80000006));
        #endregion

        #region private variables

        private IntPtr registryHive;
        private string registrySubName;

        private RegChangeNotifyFilter regFilter = RegChangeNotifyFilter.Key | 
                                                  RegChangeNotifyFilter.Attribute | 
                                                  RegChangeNotifyFilter.Value | 
                                                  RegChangeNotifyFilter.Security;
        private object threadLock = new object();
        private Thread monitorThread;
        private bool disposed = false;
        private ManualResetEvent eventTerminate = new ManualResetEvent(false);

        #endregion

        #region event handling

        /// <summary>This event occurs when a registry key changes</summary>
        public event EventHandler<OnRegChangedEventArgs> RegChanged;

        /// <summary>This event occurs when registry access fails</summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Raises a <see cref="RegChanged"/> event
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>OnRegChanged</b> is called when the registry key changes
        /// </para>
        /// </remarks>
        /// <param name="e">object containing details of the raised event (registry changes)</param>
        protected virtual void OnRegChanged(OnRegChangedEventArgs e)
        {
            EventHandler<OnRegChangedEventArgs> handler = RegChanged;
            
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Error"/> event
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>OnError</b> is called when an exception occurs while monitoring the registry
        /// </para>
        /// </remarks>
        /// <param name="e">The <see cref="Exception"/> which occured while monitoring the registry</param>
        protected virtual void OnError(Exception e)
        {
            ErrorEventHandler handler = Error;

            if(handler != null)
            {
                handler(this, new ErrorEventArgs(e));
            }
        }

        #endregion

        #region constructors and initialization

        // Constructors

        /// <summary>
        /// Initialize a new <see cref="RegistryMonitor"/> object
        /// </summary>
        /// <param name="registryKey">The registry key to monitor</param>
        public RegistryMonitor(RegistryKey registryKey)
        {
            initRegistryKey(registryKey.Name);
        }

        /// <summary>
        /// Initialize a new <see cref="RegistryMonitor"/> object
        /// </summary>
        /// <param name="name">The name to monitor</param>
        public RegistryMonitor(string registryKey)
        {
            if(registryKey == null || registryKey.Length == 0)
            {
                throw new ArgumentNullException("registryKey");
            }

            initRegistryKey(registryKey);
        }

        /// <summary>
        /// Initialize a new <see cref="RegistryMonitor"/> object
        /// </summary>
        /// <param name="hive">The desired registry hive</param>
        /// <param name="subKey">The subkey within that hive</param>
        public RegistryMonitor(RegistryHive hive, string subKey)
        {
            initRegistryKey(hive, subKey);
        }

        /// <summary>
        /// Set private variables to appropriate values based on the input
        /// </summary>
        /// <param name="hive">The hive to monitor</param>
        /// <param name="subKeyName">The subkey to monitor</param>
        private void initRegistryKey(RegistryHive hive, string subKeyName)
        {
            // since we're using Win32 APIs, we need to associated the RegistryHive object
            // with the appropriate pointer value.
            switch(hive)
            {
                case RegistryHive.ClassesRoot:
                    registryHive = HKEY_CLASSES_ROOT;
                    break;
                case RegistryHive.CurrentConfig:
                    registryHive = HKEY_CURRENT_CONFIG;
                    break;
                case RegistryHive.CurrentUser:
                    registryHive = HKEY_CURRENT_USER;
                    break;
                case RegistryHive.DynData:
                    registryHive = HKEY_DYN_DATA;
                    break;
                case RegistryHive.LocalMachine:
                    registryHive = HKEY_LOCAL_MACHINE;
                    break;
                case RegistryHive.PerformanceData:
                    registryHive = HKEY_PERFORMANCE_DATA;
                    break;
                case RegistryHive.Users:
                    registryHive = HKEY_USERS;
                    break;
                default:
                    throw new InvalidEnumArgumentException("hive", (int)hive, typeof(RegistryHive));
            }

            registrySubName = subKeyName;

        }

        /// <summary>
        /// Set private variables to appropriate values based on the input
        /// </summary>
        /// <param name="regKeyString">The registry key to monitor, as a string</param>
        private void initRegistryKey(string regKeyString)
        {
            // break the full string into components to parse
            string[] regKeyComponents = regKeyString.Split('\\');

            // choose the hive based on the first part of the string
            switch(regKeyComponents[0])
            {
                case "HKEY_CLASSES_ROOT":
                case "HKCR":
                    registryHive = HKEY_CLASSES_ROOT;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryHive = HKEY_CURRENT_CONFIG;
                    break;
                case "HKEY_CURRENT_USER":
                case "HKCU":
                    registryHive = HKEY_CURRENT_USER;
                    break;
                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    registryHive = HKEY_LOCAL_MACHINE;
                    break;
                case "HKEY_USERS":
                    registryHive = HKEY_USERS;
                    break;
                default:
                    registryHive = IntPtr.Zero;
                    throw new ArgumentException(String.Format("{0} is an unsupported hive", regKeyComponents[0]));
            }

            // reconstruct the string we split apart for the remainder of the key
            registrySubName = String.Join("\\", regKeyComponents, 1, regKeyComponents.Length - 1);
        }

        #endregion

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            Stop();
            disposed = true;
            eventTerminate.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get/Set the instance's <see cref="RegChangeNotifyFilter"/>RegChangeNotifyFilter</see>
        /// </summary>
        public RegChangeNotifyFilter RegChangeNotifyFilter
        {
            get
            {
                return regFilter;
            }

            set
            {
                // enter critical section
                lock(threadLock)
                {
                    if(isMonitoring)
                    {
                        throw new InvalidOperationException("Monitoring thread is already running");                        
                    }

                    regFilter = value;
                }
            }
        }

        /// <summary>
        /// Returns <b>true</b> if this <see cref="RegistryMonitor"/> is currently active; <b>false</b> otherwise.
        /// </summary>
        public bool isMonitoring
        {
            get
            {
                return monitorThread != null;
            }
        }

        #region thread functions

        /// <summary>
        /// Start the monitoring thread
        /// </summary>
        public void Start()
        {
            // make sure this instance isn't disposed
            if(disposed)
            {
                throw new ObjectDisposedException(null, "This instance is already disposed");
            }

            // enter critical section
            lock(threadLock)
            {
                if(!isMonitoring)
                {
                    // make sure the terminate event isn't signaled
                    eventTerminate.Reset();

                    // initialize the thread
                    monitorThread = new Thread(new ThreadStart(MonitorThread));
                    monitorThread.IsBackground = true;
                    monitorThread.Start();
                }
            }
        }

        /// <summary>
        /// Stops the monitoring thread
        /// </summary>
        public void Stop()
        {
            // make sure this instance isn't disposed
            if(disposed)
            {
                throw new ObjectDisposedException(null, "This instance is already disposed");
            }

            // enter critical section
            lock(threadLock)
            {
                Thread thread = monitorThread;
                if(thread != null)
                {
                    // signal the terminate event
                    eventTerminate.Set();
                    // block until the thread terminates
                    thread.Join();
                }
            }
        }

        private void MonitorThread()
        {
            try
            {
                ThreadLoop();
            }
            catch(Exception e)
            {
                OnError(e);
            }

            monitorThread = null;
        }

        private void ThreadLoop()
        {
            // make the RegOpenKeyEx call
            IntPtr registryKey;
            int result = RegOpenKeyEx(registryHive,
                                      registrySubName,
                                      0,
                                      STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_NOTIFY,
                                      out registryKey);
            if(result != 0)
            {
                throw new Win32Exception(result);
            }

            try
            {
                AutoResetEvent eventNotify = new AutoResetEvent(false);

                // set up the events that we act on
                WaitHandle[] waitHandles = new WaitHandle[] { eventNotify, eventTerminate };

                while(!eventTerminate.WaitOne(0, true))
                {
                    // make the RegNotifyChangeKeyValue call to monitor the key
                    result = RegNotifyChangeKeyValue(registryKey,
                                                     true,
                                                     regFilter,
                                                     eventNotify.SafeWaitHandle,
                                                     true);
                    if(result != 0)
                    {
                        throw new Win32Exception(result);
                    }

                    // the signal we waited on was an eventNotify
                    if(WaitHandle.WaitAny(waitHandles) == 0)
                    {
                        // create a RegistryKey object using the handle we got earlier
                        RegistryKey changedRegKey = RegistryKey.FromHandle(new SafeRegistryHandle(registryKey, true));

                        // send this RegistryKey object back to the event handler
                        OnRegChangedEventArgs args = new OnRegChangedEventArgs();
                        // for our purposes, the time the key changed was now
                        args.timeChanged = DateTime.Now;
                        args.changedKey = changedRegKey;

                        // call the handler and signal the event
                        OnRegChanged(args);
                        eventNotify.Set();
                    }
                }
            }
            finally
            {
                // close the key
                if(registryKey != IntPtr.Zero)
                {
                    RegCloseKey(registryKey);
                }
            }

        }

        #endregion
    }
}