using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace DarkMultiPlayer
{
    public class Language
    {
        // Language
        public static string language = "english";

        // General
        public static string cancelBtn = "Cancel";
        public static string connectBtn = "Connect";
        public static string disconnectBtn = "Disconnect";
        public static string randomBtn = "Random";
        public static string setBtn = "Set";
        public static string closeBtn = "Close";
        public static string removeBtn = "Remove";

        // Main Menu
        public static string addServer = "Add";
        public static string editServer = "Edit";
        public static string serverNameLabel = "Name:";
        public static string serverAddressLabel = "Address:";
        public static string serverPortLabel = "Port:";
        public static string optionsBtn = "Options";
        public static string playerNameLabel = "Player name:";
        public static string serversLabel = "Servers:";
        public static string serverAddEdit = "server";
        public static string noServers = "(None - Add a server first)";

        // Options
        public static string options = "Options";
        public static string playerNameColor = "Player name color:";
        public static string cacheSizeLabel = "Cache size";
        public static string currentCacheSizeLabel = "Current size:";
        public static string maxCacheSizeLabel = "Max size:";
        public static string expireCacheButton = "Expire cache";
        public static string deleteCacheButton = "Delete cache";
        public static string setChatKeyBtn = "Set chat key";
        public static string currentKey = "current:";
        public static string setScrnShotKeyBtn = "Set screenshot key";
        public static string generateModCntrlLabel = "Generate a server DMPModControl:";
        public static string generateModBlacklistBtn = "Generate blacklist DMPModControl.txt";
        public static string generateModWhitelistBtn = "Generate whitelist DMPModControl.txt";
        public static string generateUniverseSavedGameBtn = "Generate Universe from saved game";
        public static string resetDisclaimerBtn = "Reset disclaimer";
        public static string enableCompressionBtn = "Enable compression";

    }

    public class LanguageWorker
    {
        //LanguageWorker
        private static LanguageWorker singleton = new LanguageWorker();
        private string dataLocation;
        private string langFile;
        private const string LANGUAGE_FILE = "language.txt";

        private bool loadedSettings = false;

        private static Dictionary<string, string> languageStrings;

        public static LanguageWorker fetch
        {
            get
            {
                return singleton;
            }
        }

        public LanguageWorker()
        {
            dataLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data");
            langFile = Path.Combine(dataLocation, LANGUAGE_FILE);
            languageStrings = new Dictionary<string, string>();
            LoadLanguage();
        }

        public void LoadLanguage()
        {
            if (!File.Exists(langFile))
            {
                using (StreamWriter sw = new StreamWriter(langFile))
                {
                    foreach(FieldInfo fieldInfo in typeof(Language).GetFields())
                    {
                        sw.WriteLine(fieldInfo.Name + "=" + fieldInfo.GetValue(null));
                    }
                }
            }
            using (StreamReader sr = new StreamReader(langFile))
            {
                string currentLine;

                // darklight's key=value parser
                while ((currentLine = sr.ReadLine()) != null)
                {
                    try
                    {
                        string key = currentLine.Substring(0, currentLine.IndexOf("=")).Trim();
                        string value = currentLine.Substring(currentLine.IndexOf("=") + 1).Trim();
                        languageStrings.Add(key, value);
                    }
                    catch (Exception e)
                    {
                        DarkLog.Debug("Error while reading language file: " + e);
                    }
                }
            }
            DarkLog.Debug("Loaded " + languageStrings.Count + " language strings");
            loadedSettings = true;
        }

        public string GetString(string key)
        {
            if (languageStrings.ContainsKey(key))
            {
                return languageStrings[key];
            }
            return key;
        }
    }
}
