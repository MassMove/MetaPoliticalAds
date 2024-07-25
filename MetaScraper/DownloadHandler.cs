using CefSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace MetaScraper
{
    public class DownloadHandler : IDownloadHandler
    {
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        public int Phase = 0;

        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            OnBeforeDownloadFired?.Invoke(this, downloadItem);

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    var fileName = downloadItem.SuggestedFileName.Replace(".zip", "");
                    fileName = fileName.Replace("FacebookAdLibraryReport_", "");
                    fileName = fileName.Replace("_lifelong_advertisers", "");
                    fileName = fileName.Replace("_lifelong", "");
                    fileName = fileName.Replace("last_30_days", "");
                    fileName = fileName.Replace(DateTime.UtcNow.ToString("yyyy-MM-dd"), "");
                    fileName = fileName.Replace(DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"), "");
                    fileName = fileName.Replace(DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd"), "");
                    fileName = fileName.Replace(DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd"), "");
                    fileName = fileName.Replace(DateTime.UtcNow.AddDays(-4).ToString("yyyy-MM-dd"), "");
                    fileName = fileName.Replace("_", "");

                    if (Phase == 0)
                    {
                        fileName = @"..\..\..\MetaData\Total\" + fileName + ".csv";
                    }
                    if (Phase == 30)
                    {
                        fileName = @"..\..\..\MetaData\Month\" + fileName + ".csv";
                    }

                    using (WebClient webClient = new WebClient())
                    {
                        var scData = webClient.DownloadData(downloadItem.OriginalUrl);
                        var zipStream = new MemoryStream(scData);

                        using (ZipArchive archive = new ZipArchive(zipStream))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.Name.Contains("_advertisers.csv"))
                                {
                                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                                    using (var stream = entry.Open())
                                    {
                                        stream.CopyTo(fileStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            OnDownloadUpdatedFired?.Invoke(this, downloadItem);
        }
    }
}
