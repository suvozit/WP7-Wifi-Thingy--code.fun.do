using System;
using System.Collections.Generic;

namespace PCController.Update
{
    /// <summary>
    /// A data container for information about a single application version.
    /// </summary>
    public class VersionData
    {
        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public string VersionNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the release date of the version.
        /// </summary>
        public DateTime ReleaseDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the changes introduced by the version, compared to the previous one.
        /// </summary>
        public IEnumerable<string> Changes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the url where the installer for this version can be obtained from.
        /// </summary>
        public string InstallerUrl
        {
            get;
            set;
        }
    }
}
