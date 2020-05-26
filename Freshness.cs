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
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal class Freshness
    {
        private string UpdateURL;
        private string UpdateVER;
        private string ThisVersion;
        private string ThisApplication;
        private string WebUpdateFile;

        public Freshness(string updateURL, string updateVer, string thisVersion, string thisApplication, string webUpdateFile)
        {
            UpdateURL = updateURL;
            UpdateVER = updateVer;
            ThisVersion = thisVersion;
            ThisApplication = thisApplication;
            WebUpdateFile = webUpdateFile;
        }
        private void DisplayUpdates(bool silentMode, bool popupOK)
        {
            if (UpdateURL != "")
            {
                if (popupOK) // only popup if code editor has focus (otherwise, we might be doing something that we shouldn't interrupt)
                {
                    if (FlexibleMessageBox.Show("An update to CodeLab is available.\n\nWould you like to download CodeLab v" + UpdateVER + "?\n\n(This will not close your current CodeLab session.)", "CodeLab Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        LaunchUrl(UpdateURL);
                    }
                    else
                    {
                        UpdateURL = "";
                    }
                }
            }
            else if (!silentMode)
            {
                if (UpdateVER == ThisVersion)
                {
                    FlexibleMessageBox.Show("You are up-to-date!", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    FlexibleMessageBox.Show("I'm not sure if you are up-to-date.\n\nI was not able to reach the update website.\n\nTry again later.", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void GoCheckForUpdates(bool silentMode, bool force)
        {
            UpdateVER = "";
            UpdateURL = "";

            if (!force)
            {
                // only check for updates every 7 days
                if (Math.Abs((Settings.LatestUpdateCheck - DateTime.Today).TotalDays) < 7)
                {
                    return; // not time yet
                }
            }

            Random r = new Random(); // defeat any cache by appending a random number to the URL

            WebClient web = new WebClient();
            web.OpenReadAsync(new Uri(WebUpdateFile + "?r=" + r.Next(int.MaxValue).ToString()));

            web.OpenReadCompleted += (sender, e) =>
            {
                try
                {
                    string text = "";
                    Stream stream = e.Result;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                    }
                    string[] lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] data = lines[i].Split(';');
                        if (data.Length >= 2)
                        {
                            if (data[0].Trim() == ThisApplication.Trim())
                            {
                                UpdateVER = data[1].Trim();
                                if (data[1].Trim() != ThisVersion.Trim())
                                {
                                    UpdateURL = data[2].Trim();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    UpdateVER = "";
                    UpdateURL = "";
                }

                Settings.LatestUpdateCheck = DateTime.Now;

                DisplayUpdates(silentMode, true);
            };
        }
        private void LaunchUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

    }
}
