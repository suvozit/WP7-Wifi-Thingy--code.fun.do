using System;
using System.Reflection;

namespace PCController.Update
{
    /// <summary>
    /// A helper class for handling application/assembly version information.
    /// </summary>
    public static class VersionHelper
    {
        private static Version _applicationVersion;

        /// <summary>
        /// Gets the current application version, determined from the currently executing assembly.
        /// </summary>
        public static Version ApplicationVersion
        {
            get
            {
                if (_applicationVersion == null)
                {
                    _applicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
                }

                return _applicationVersion;
            }
        }

        /// <summary>
        /// Determines whether the given version is newer than the current application version.
        /// </summary>
        /// <param name="versionText">The version text to use for comparison.</param>
        /// <returns>
        ///   <c>true</c> if the given version is newer than the current application version; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNewerThanCurrentVersion(string versionText)
        {
            Version version;
            if (!Version.TryParse(versionText, out version))
            {
                return false;
            }

            return ApplicationVersion < version;
        }
    }
}
