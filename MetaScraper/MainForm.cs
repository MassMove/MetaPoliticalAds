using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MetaScraper
{
    public partial class MainForm : Form
    {
        int country;
        const int totalCountries = 204;
        int downloadTicks;

        int shortSleep = 32;

        bool closing;

        Phase phase;
        DownloadHandler downloadHandler = new DownloadHandler();

        string StatusMessage = "";

        public MainForm()
        {
            InitializeComponent();

            chromiumWebBrowser.DownloadHandler = downloadHandler;
            chromiumWebBrowser.StatusMessage += ChromiumWebBrowser_StatusMessage;
        }

        private void ChromiumWebBrowser_StatusMessage(object sender, CefSharp.StatusMessageEventArgs e)
        {
            StatusMessage = e.Value;
        }

        private void initTimer_Tick(object sender, EventArgs e)
        {
            initTimer.Enabled = false;

            if (closing) return;

            if (country > totalCountries)
            {
                return;
            }
            if (phase == Phase.Init)
            {
                phase = Phase.LoadUrl;
                Scrape();
            }
            if (phase == Phase.Loaded)
            {
                phase = Phase.SelectCountry;
                SelectCountry();
            }
            if (phase == Phase.SelectedFirstCountry)
            {
                phase = Phase.SelectDates;
                SelectAllDates();
            }
            if (phase == Phase.SelectedDates)
            {
                phase = Phase.Download;
                downloadHandler.Phase = 0;
                DownloadReport();
            }
            if (phase == Phase.Downloaded)
            {
                phase = Phase.Select30Days;
                Select30Days();
            }
            if (phase == Phase.Selected30Days)
            {
                phase = Phase.Download30Days;
                downloadHandler.Phase = 30;
                DownloadReport();
            }
            if (phase == Phase.Downloaded30Days)
            {
                phase = Phase.SelectNextCountry;
                SelectNextCountry();
            }
            if (phase == Phase.SelectedNextCountry)
            {
                phase = Phase.SelectDatesAgain;
                SelectAllDatesAgain();
            }
            if (phase == Phase.SelectedDatesAgain)
            {
                phase = Phase.Download;
                downloadHandler.Phase = 0;
                DownloadReport();
            }

            if (phase == Phase.Download30Days)
            {
                downloadTicks++;
                this.Text += $" {downloadTicks}";

                if (downloadTicks >= 4)
                {
                    Reselect30Days();
                }
            }

            if (phase == Phase.Download)
            {
                downloadTicks++;
                this.Text += $" {downloadTicks}";

                if (downloadTicks >= 4)
                {
                    ReselectAllDates();
                }
            }

            initTimer.Enabled = true;
        }

        private void Scrape()
        {
            var reportUrl = "https://www.facebook.com/ads/library/report";
            chromiumWebBrowser.LoadUrl(reportUrl);
        }

        private void chromiumWebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                if (phase == Phase.LoadUrl)
                {
                    phase = Phase.Loaded;
                }
                if (phase == Phase.Download)
                {
                    phase = Phase.Downloaded;
                }
                if (phase == Phase.Download30Days)
                {
                    phase = Phase.Downloaded30Days;
                }
            }
        }

        public void SetTitle(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(SetTitle), new object[] { value });
                return;
            }
            this.Text = $"MetaScraper - {value}";
        }

        private void SendWait(string keys)
        {
            SetTitle($"{country}: {phase}: {keys} {StatusMessage}");
            SendKeys.SendWait(keys);
            Thread.Sleep(8);
        }

        private void SelectCountry()
        {
            phase = Phase.SelectedFirstCountry;
            Thread.Sleep(1024);
            foreach (var i in Enumerable.Range(0, 23))
            {
                SendWait("{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendWait(" ");
            SendWait("Albania");
            SendWait("{DOWN}");
            SendWait("{ENTER}");

            country++;

            Thread.Sleep(2048);
        }

        private void SelectAllDates()
        {
            phase = Phase.SelectedDates;

            int reportStatus = 0;
            while (true)
            {
                SendWait("+{TAB}");
                Thread.Sleep(shortSleep);

                if (StatusMessage.Contains("/report#"))
                {
                    reportStatus++;;
                }
                if (reportStatus == 3)
                {
                    SendWait("+{TAB}");
                    Thread.Sleep(shortSleep);
                    SendWait("+{TAB}");
                    Thread.Sleep(shortSleep);
                    break;
                }
                if (closing) return;
            }

            SendWait(" ");
            Thread.Sleep(shortSleep);
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{ENTER}");
            Thread.Sleep(shortSleep);
        }

        private void DownloadReport()
        {
            SendWait("{TAB}");
            Thread.Sleep(shortSleep);
            SendWait("{TAB}");
            Thread.Sleep(shortSleep);
            SendWait("{ENTER}");
            Thread.Sleep(shortSleep);
            downloadTicks = 0;
        }

        private void Select30Days()
        {
            foreach (var i in Enumerable.Range(0, 2))
            {
                SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendWait(" ");
            SendWait("{DOWN}");
            Thread.Sleep(shortSleep);
            SendWait("{DOWN}");
            Thread.Sleep(shortSleep);
            SendWait("{ENTER}");
            Thread.Sleep(1024);

            phase = Phase.Selected30Days;
        }

        private void SelectNextCountry()
        {
            foreach (var i in Enumerable.Range(0, 30))
            {
                SendWait("{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendWait(" ");
            Thread.Sleep(1024);

            country++;

            foreach (var i in Enumerable.Range(0, country))
            {
                SendWait("{DOWN}");
                Thread.Sleep(shortSleep);
            }

            SendWait("{ENTER}");

            Thread.Sleep(4096);
            phase = Phase.SelectedNextCountry;
        }

        private void SelectAllDatesAgain()
        {
            int reportStatus = 0;
            while (true)
            {
                SendWait("+{TAB}");
                Thread.Sleep(shortSleep);

                if (StatusMessage.Contains("/report#"))
                {
                    reportStatus++; ;
                }
                if (reportStatus == 3)
                {
                    SendWait("+{TAB}");
                    Thread.Sleep(shortSleep);
                    SendWait("+{TAB}");
                    Thread.Sleep(shortSleep);
                    break;
                }
                if (closing) return;
            }

            SendWait(" ");
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{DOWN}");
            SendWait("{ENTER}");
            Thread.Sleep(1024);

            phase = Phase.SelectedDatesAgain;
        }

        private void Reselect30Days()
        {
            foreach (var i in Enumerable.Range(0, 11))
            {
                SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            phase = Phase.Selected30Days;
        }

        private void ReselectAllDates()
        {
            foreach (var i in Enumerable.Range(0, 11))
            {
                SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            phase = Phase.SelectedDatesAgain;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closing = true;
        }
    }
}
