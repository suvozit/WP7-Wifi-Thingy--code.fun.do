using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sample.LogService
{
    internal static class SimpleApplicationSettings
    {
        private const string FileName = "AppSettings.cfg";
        private static Dictionary<string, string> _settings;

        private static void EnsureDictionary()
        {
            if (_settings == null)
            {
                try
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForAssembly())
                    {
                        if (store.FileExists(FileName))
                        {

                            using (var stream = store.OpenFile("AppSettings.cfg", FileMode.OpenOrCreate, FileAccess.Read))
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                _settings = (Dictionary<string, string>)formatter.Deserialize(stream);
                            }

                        }
                        else
                        {
                            _settings = new Dictionary<string, string>();
                        }
                    }
                }
                catch (Exception)
                {
                    _settings = new Dictionary<string, string>();
                }
            }
        }

        public static void Save(string key, string value)
        {
            EnsureDictionary();

            _settings[key] = value;

            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (store.FileExists(FileName))
                    {
                        store.DeleteFile(FileName);
                    }

                    using (var stream = store.OpenFile("AppSettings.cfg", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, _settings);
                    }
                }
            }
            catch (Exception)
            {
                // consciously re-throw
                throw;
            }
        }

        public static string GetValue(string key)
        {
            EnsureDictionary();

            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }

            return null;
        }
    }
}
