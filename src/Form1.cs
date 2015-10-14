using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using Microsoft.Win32;

// Logitech SDK C# wrapper
using LedCSharp;
using RegistryUtils;

namespace Ambiance
{
    public partial class mainWindow : Form
    {
        #region class variables

        private int ledSDK_majVer = 0;
        private int ledSDK_minVer = 0;
        private int ledSDK_buildNumber = 0;

        // RGB values to send to the photon and the keyboard (0-255)
        private int redValue = 0;
        private int greenValue = 0;
        private int blueValue = 0;

        private static string accentColor = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent";
        private static string accentPalette = "AccentPalette";

        private RegistryMonitor regMon;

        #endregion

        #region internal functions

        public mainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            regMon = new RegistryMonitor(RegistryHive.CurrentUser, accentColor);
            regMon.RegChangeNotifyFilter = RegChangeNotifyFilter.Value;
            regMon.RegChanged += new EventHandler<OnRegChangedEventArgs>(onRegistryChange);

            initLEDSDK();
        }
        
        /// <summary>
        /// Initializes the Logitech LED SDK, sets the SDK version, and saves the current keyboard lighting state.
        /// </summary>
        private void initLEDSDK()
        {
            LogitechGSDK.LogiLedInit();

            // sleep for a bit because the LogiLedInit() function needs time
            Thread.Sleep(100);

            // get the SDK version information
            LogitechGSDK.LogiLedGetSdkVersion(ref ledSDK_majVer, ref ledSDK_minVer, ref ledSDK_buildNumber);

            // print the SDK version on the form
            this.sdkVersionLabel.Text = String.Format("{0}.{1}.{2}", ledSDK_majVer, ledSDK_minVer, ledSDK_buildNumber);

            // save the current lighting scheme
            LogitechGSDK.LogiLedSaveCurrentLighting();
        }

        /// <summary>
        /// Rescale an RGB value (0-255) to a percentage (0-100)
        /// </summary>
        /// <param name="colorValue">The RGB value to convert; should be between 0 and 255</param>
        /// <returns></returns>
        private static int convertRGBtoPercent(int colorValue)
        {
            float percentValue = (float)colorValue / 255.0f;

            return (int)(percentValue * 100);
        }

        /// <summary>
        /// sets the Logitech keyboard backlight according to the current local RGB values
        /// </summary>
        private void setKeyboardColor()
        {
            int redPercent = convertRGBtoPercent(redValue);
            int greenPercent = convertRGBtoPercent(greenValue);
            int bluePercent = convertRGBtoPercent(blueValue);
            LogitechGSDK.LogiLedSetLighting(redPercent, greenPercent, bluePercent);
        }

        /// <summary>
        /// When registry monitoring is enabled, this function is called to match lighting to the current registry value
        /// </summary>
        private void setInitialColor()
        {
            // get the AccentPalette subkey value
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(accentColor, false);            
            object subKeyValue = regKey.GetValue(accentPalette);

            setColor((byte[])subKeyValue);

            regKey.Close();
        }

        /// <summary>
        /// Updates the local color values, resets all GUI elements to match, and changes keyboard lighting.
        /// </summary>
        /// <param name="registryData">The byte array data from AccentPalette</param>
        private void setColor(byte[] registryData)
        {
            // get the new color
            redValue = registryData[12];
            greenValue = registryData[13];
            blueValue = registryData[14];

            // update the display and the preview box
            redValueBox.Value = redValue;
            greenValueBox.Value = greenValue;
            blueValueBox.Value = blueValue;
            colorPreviewBox.BackColor = Color.FromArgb(redValue, greenValue, blueValue);

            // update the keyboard lighting
            setKeyboardColor();
        }

        /// <summary>
        /// Event handler for registry changes. Extracts the value of AccentPalette and passes it along
        /// </summary>
        /// <param name="e">OnRegChangedEvents object that contains the changed registry key</param>
        private void onRegistryChange(object sender, OnRegChangedEventArgs e)
        {
            // get the value from the RegistryKey object
            setColor((byte[])e.changedKey.GetValue(accentPalette));
        }

        #endregion

        #region form element actions

        /// <summary>
        /// Set the keyboard backlight color to the values in redValue, greenValue, and blueValue.
        /// </summary>
        private void setKeyboardButton_Click(object sender, EventArgs e)
        {
            //int redPercent = convertRGBtoPercent(redValue);
            //int greenPercent = convertRGBtoPercent(greenValue);
            //int bluePercent = convertRGBtoPercent(blueValue);
            //LogitechGSDK.LogiLedSetLighting(redPercent, greenPercent, bluePercent);
            setKeyboardColor();
        }

        /// <summary>
        /// Minimize the application to the system tray and display a notification telling
        /// the user that the application is still running.
        /// </summary>
        private void minimizeButton_Click(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                trayIcon.Visible = true;
                trayIcon.ShowBalloonTip(750);
                this.ShowInTaskbar = false;
            }
        }

        /// <summary>
        /// Bring the main window back to the screen from the system tray
        /// </summary>
        private void trayIcon_DoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            trayIcon.Visible = false;
        }

        /// <summary>
        /// Right-click menu's exit button. Resets keyboard lighting and terminates the application.
        /// </summary>
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            // reset the lighting to pre-Ambiance state
            LogitechGSDK.LogiLedRestoreLighting();
            regMon.Dispose();

            Application.Exit();
        }    

        /// <summary>
        /// Update the stored value for redValue and reset the preview box color.
        /// </summary>
        private void redValueBox_ValueChanged(object sender, EventArgs e)
        {
            if (redValueBox.ReadOnly)
            {
                if (redValueBox.Value != redValue)
                {
                    redValueBox.Value = redValue;
                    MessageBox.Show("Currently using the registry to determine colors",
                                    "Ambiance",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            else
            {
                redValue = (int)redValueBox.Value;
                this.colorPreviewBox.BackColor = Color.FromArgb(redValue, greenValue, blueValue);
            }
        }

        /// <summary>
        /// Update the stored value for greenValue and reset the preview box color.
        /// </summary>
        private void greenValueBox_ValueChanged(object sender, EventArgs e)
        {
            if (greenValueBox.ReadOnly)
            {
                if (greenValueBox.Value != greenValue)
                {
                    greenValueBox.Value = greenValue;
                    MessageBox.Show("Currently using the registry to determine colors",
                                    "Ambiance",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            else
            {
                greenValue = (int)greenValueBox.Value;
                this.colorPreviewBox.BackColor = Color.FromArgb(redValue, greenValue, blueValue);
            }
        }

        /// <summary>
        /// Update the stored value for blueValue and reset the preview box color.
        /// </summary>
        private void blueValueBox_ValueChanged(object sender, EventArgs e)
        {
            if(blueValueBox.ReadOnly)
            {
                if(blueValueBox.Value != blueValue)
                {
                    blueValueBox.Value = blueValue;
                    MessageBox.Show("Currently using the registry to determine colors",
                                    "Ambiance",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            else
            {
                blueValue = (int)blueValueBox.Value;
                this.colorPreviewBox.BackColor = Color.FromArgb(redValue, greenValue, blueValue);
            }
        }        

        /// <summary>
        /// Starts/stops the registry monitor
        /// </summary>
        /// <note>Starting the registry monitor disables the NumericUpDown controls</note>
        private void autoColorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(autoColorCheckBox.Checked)
            {
                redValueBox.ReadOnly = true;
                greenValueBox.ReadOnly = true;
                blueValueBox.ReadOnly = true;

                setInitialColor();

                regMon.Start();
            }
            else if(!autoColorCheckBox.Checked)
            {
                redValueBox.ReadOnly = false;
                greenValueBox.ReadOnly = false;
                blueValueBox.ReadOnly = false;
                regMon.Stop();                
            }
        }

        #endregion
    }
}
