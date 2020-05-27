/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2020 BoltBait. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal static class Freshness
    {
        private const string ThisVersion = CodeLab.Version;
        private const string WebUpdateFile = "https://www.boltbait.com/versions.txt"; // The web site to check for updates
        private const string ThisApplication = "1"; // in the WebUpadteFile, CodeLab is application #1
        // format of the versions.txt file:  application number;current version;URL to download current version
        // for example: 1;2.13;https://boltbait.com/pdn/CodeLab/CodeLab213.zip
        // each application on its own line

        private static string updateURL;
        private static string updateVER;
        private static bool needToShowNotification = false;

        internal static void DisplayUpdateNotification()
        {
            if (needToShowNotification)
            {
                if (FlexibleMessageBox.Show("An update to CodeLab is available.\n\nWould you like to download CodeLab v" + updateVER + "?\n\n(This will not close your current CodeLab session.)", "CodeLab Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    LaunchUrl(updateURL);
                }

                needToShowNotification = false;
            }
        }

        internal static void GoCheckForUpdates(bool silentMode, bool force)
        {
            updateURL = string.Empty;
            updateVER = string.Empty;
            needToShowNotification = false;

            if (!force)
            {
                // only check for updates every 7 days
                if (Math.Abs((Settings.LatestUpdateCheck - DateTime.Today).TotalDays) < 7)
                {
                    //return; // not time yet
                }
            }

            Random r = new Random(); // defeat any cache by appending a random number to the URL

            WebClient web = new WebClient();
            web.DownloadStringAsync(new Uri(WebUpdateFile + "?r=" + r.Next(int.MaxValue).ToString()));

            web.DownloadStringCompleted += (sender, e) =>
            {
                UpdateStatus updateStatus = UpdateStatus.Unknown;

                if (!e.Cancelled && e.Error == null)
                {
                    Settings.LatestUpdateCheck = DateTime.Now;

                    foreach (string line in e.Result.Split('\n'))
                    {
                        string[] data = line.Split(';');
                        if (data.Length >= 2 && data[0].Trim() == ThisApplication.Trim())
                        {
                            if (data[1].Trim() != ThisVersion.Trim())
                            {
                                updateStatus = UpdateStatus.UpdateAvailable;
                                updateVER = data[1].Trim();
                                updateURL = data[2].Trim();
                            }
                            else
                            {
                                updateStatus = UpdateStatus.UpToDate;
                            }

                            break;
                        }
                    }
                }

                if (silentMode)
                {
                    needToShowNotification = updateStatus == UpdateStatus.UpdateAvailable;
                }
                else
                {
                    switch (updateStatus)
                    {
                        case UpdateStatus.Unknown:
                            FlexibleMessageBox.Show("I'm not sure if you are up-to-date.\n\nI was not able to reach the update website.\n\nTry again later.", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        case UpdateStatus.UpToDate:
                            FlexibleMessageBox.Show("You are up-to-date!", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case UpdateStatus.UpdateAvailable:
                            if (FlexibleMessageBox.Show("An update to CodeLab is available.\n\nWould you like to download CodeLab v" + updateVER + "?\n\n(This will not close your current CodeLab session.)", "CodeLab Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                            {
                                LaunchUrl(updateURL);
                            }
                            break;
                    }
                }
            };
        }

        private static void LaunchUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        private enum UpdateStatus
        {
            Unknown,
            UpToDate,
            UpdateAvailable
        }
    }
}
