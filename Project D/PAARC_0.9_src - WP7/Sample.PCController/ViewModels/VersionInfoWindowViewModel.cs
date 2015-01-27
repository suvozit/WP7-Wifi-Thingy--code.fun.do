
using System;
using System.Collections.Generic;
using System.Linq;
using PCController.Update;

namespace PCController.ViewModels
{
    public class VersionInfoWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the current version of the application.
        /// </summary>
        public string CurrentVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the newest version available remotely.
        /// </summary>
        public string NewestVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the download Uri for the newest remotely available version.
        /// </summary>
        public Uri DownloadUri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of new versions available and newer than the current version.
        /// </summary>
        public IEnumerable<VersionData> Versions
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfoWindowViewModel"/> class and creates some design-time data.
        /// </summary>
        public VersionInfoWindowViewModel()
        {
            // this (hopefully) is only used in design mode
            var versions = new List<VersionData>
                           {
                               new VersionData
                                   {
                                       VersionNumber = "4.0.0.0",
                                       ReleaseDate = new DateTime(2011, 12, 09),
                                       InstallerUrl = "http://www.example.com/version4000.msi",
                                       Changes = new List<string>
                                                     {
                                                         "Tremendously improved the automated version number increase mechanism",
                                                         "Added world domination feature",
                                                         "Fixed bug #555 that could cause nuclear winter in rare cases"
                                                     }
                                   },
                                new VersionData
                                   {
                                       VersionNumber = "3.5.0.1",
                                       ReleaseDate = new DateTime(2011, 12, 02),
                                       InstallerUrl = "http://www.example.com/version3501.msi",
                                       Changes = new List<string>
                                                     {
                                                         "Quick bugfix release to fix close button causing cancer for user",
                                                     }
                                   },
                                new VersionData
                                   {
                                       VersionNumber = "3.5.0.0",
                                       ReleaseDate = new DateTime(2011, 12, 01),
                                       InstallerUrl = "http://www.example.com/version3500.msi",
                                       Changes = new List<string>
                                                     {
                                                         "Removed dependency on external library for mouse pointer movement",
                                                         "Added all glory to the hypnotoad",
                                                         "Fixed bug #42 that caused some users to recognize the answer to the ultimate question of life, the universe and everything.",
                                                         "Improved the startup time by 1093 seconds"
                                                     }
                                   },
                           };

            SetupBaseData(versions);
            SetupVersions(versions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfoWindowViewModel"/> class.
        /// </summary>
        /// <param name="availableVersions">The remotely available versions.</param>
        public VersionInfoWindowViewModel(IEnumerable<VersionData> availableVersions)
        {
            SetupBaseData(availableVersions);
            SetupVersions(availableVersions);
        }

        private void SetupBaseData(IEnumerable<VersionData> availableVersions)
        {
            var newestVersion = availableVersions.FirstOrDefault();

            if (newestVersion == null || !VersionHelper.IsNewerThanCurrentVersion(newestVersion.VersionNumber))
            {
                throw new ArgumentException("The version info provided is not newer than what is already installed.");
            }

            CurrentVersion = VersionHelper.ApplicationVersion.ToString();
            NewestVersion = newestVersion.VersionNumber;
            DownloadUri = new Uri(newestVersion.InstallerUrl, UriKind.Absolute);
        }

        private void SetupVersions(IEnumerable<VersionData> availableVersions)
        {
            var versions = new List<VersionData>();

            foreach (var version in availableVersions)
            {
                // discard all versions that are older than what we already have
                if (VersionHelper.IsNewerThanCurrentVersion(version.VersionNumber))
                {
                    versions.Add(version);
                }
            }

            Versions = versions;
        }
    }
}
