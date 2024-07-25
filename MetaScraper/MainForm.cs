using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MetaScraper
{
    public partial class MainForm : Form
    {
        int countries;
        const int totalCountries = 204;
        int downloadTicks;

        int shortSleep = 32;

        Phase phase;
        DownloadHandler downloadHandler = new DownloadHandler();

        public MainForm()
        {
            InitializeComponent();

            chromiumWebBrowser.DownloadHandler = downloadHandler;
        }

        private void initTimer_Tick(object sender, EventArgs e)
        {
            initTimer.Enabled = false;

            if (countries > totalCountries)
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
            this.Text = $"MetaScraper - {countries}: {phase}";

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

        private void SelectCountry()
        {
            Thread.Sleep(1024);
            foreach (var i in Enumerable.Range(0, 21))
            {
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendKeys.SendWait(" ");
            SendKeys.SendWait("Albania");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{ENTER}");

            countries++;

            Thread.Sleep(2048);
            phase = Phase.SelectedFirstCountry;
        }

        private void SelectAllDates()
        {
            foreach (var i in Enumerable.Range(0, 30))
            {
                SendKeys.SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendKeys.SendWait(" ");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(shortSleep);

            phase = Phase.SelectedDates;
        }

        private void DownloadReport()
        {
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(shortSleep);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(shortSleep);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(shortSleep);
            downloadTicks = 0;
        }

        private void Select30Days()
        {
            foreach (var i in Enumerable.Range(0, 2))
            {
                SendKeys.SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendKeys.SendWait(" ");
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(shortSleep);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(shortSleep);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(1024);

            phase = Phase.Selected30Days;
        }

        private void SelectNextCountry()
        {
            foreach (var i in Enumerable.Range(0, 28))
            {
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendKeys.SendWait(" ");
            Thread.Sleep(1024);

            countries++;

            foreach (var i in Enumerable.Range(0, countries))
            {
                SendKeys.SendWait("{DOWN}");
                Thread.Sleep(shortSleep);
            }

            SendKeys.SendWait("{ENTER}");

            Thread.Sleep(4096);
            phase = Phase.SelectedNextCountry;
        }

        private void SelectAllDatesAgain()
        {
            foreach (var i in Enumerable.Range(0, 30))
            {
                SendKeys.SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            SendKeys.SendWait(" ");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(1024);

            phase = Phase.SelectedDatesAgain;
        }

        private void Reselect30Days()
        {
            foreach (var i in Enumerable.Range(0, 11))
            {
                SendKeys.SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            phase = Phase.Selected30Days;
        }

        private void ReselectAllDates()
        {
            foreach (var i in Enumerable.Range(0, 11))
            {
                SendKeys.SendWait("+{TAB}");
                Thread.Sleep(shortSleep);
            }
            phase = Phase.SelectedDatesAgain;
        }
    }
}
