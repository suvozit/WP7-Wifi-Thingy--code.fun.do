using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml.Linq;

namespace PCController.Update
{
    /// <summary>
    /// Checks a remote source for available updates to the application.
    /// </summary>
    public class RemoteUpdate
    {
        private const string VersionElementName = "Version";
        private const string ChangeElementName = "Change";
        private const string VersionNumberAttributeName = "Number";
        private const string ReleaseDateAttributeName = "ReleaseDate";
        private const string InstallerUrlAttributeName = "InstallerUrl";

        private WebClient _webClient;

        /// <summary>
        /// Gets the remotely available versions after they have been retrieved.
        /// </summary>
        public IEnumerable<VersionData> AvailableVersions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message in case the versions download failed.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when version data has been successfully downloaded.
        /// </summary>
        public event EventHandler<EventArgs> VersionDataDownloaded;

        /// <summary>
        /// Occurs when an error occurred during the version download.
        /// </summary>
        public event EventHandler<EventArgs> Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteUpdate"/> class.
        /// </summary>
        public RemoteUpdate()
        {
            _webClient = new WebClient();
            _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
        }

        /// <summary>
        /// Downloads version data from the given url.
        /// </summary>
        /// <param name="url">The url to download version data from.</param>
        public void DownloadVersionData(string url)
        {
            if (_webClient.IsBusy)
            {
                throw new InvalidOperationException("A download of version data is already in progress.");
            }

            var uri = new Uri(url, UriKind.Absolute);
            _webClient.DownloadStringAsync(uri);
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessage = "An error occurred while downloading application version information: " + e.Error.Message;
                RaiseErrorEvent();
            }
            else
            {
                try
                {
                    var xml = XDocument.Parse(e.Result);
                    var versions = new List<VersionData>();
                    var versionElements = xml.Descendants(VersionElementName);
                    foreach (var versionElement in versionElements)
                    {
                        var versionData = new VersionData();
                        versions.Add(versionData);

                        // get attribute values
                        versionData.VersionNumber = versionElement.Attribute(VersionNumberAttributeName).Value;
                        var releaseDateText = versionElement.Attribute(ReleaseDateAttributeName).Value;
                        versionData.ReleaseDate = DateTime.ParseExact(releaseDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        versionData.InstallerUrl = versionElement.Attribute(InstallerUrlAttributeName).Value;

                        // changes
                        var changesList = new List<string>();
                        var changes = versionElement.Descendants(ChangeElementName);
                        foreach (var change in changes)
                        {
                            changesList.Add(change.Value);
                        }
                        versionData.Changes = changesList;
                    }

                    AvailableVersions = versions;
                    RaiseVersionDataDownloadedEvent();
                }
                catch (Exception ex)
                {
                    ErrorMessage = "An error occurred while parsing downloaded application version information: " + ex.Message;
                    RaiseErrorEvent();
                }
            }
        }

        private void RaiseErrorEvent()
        {
            var handlers = Error;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        private void RaiseVersionDataDownloadedEvent()
        {
            var handlers = VersionDataDownloaded;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }
    }
}
