using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Brace.Utils
{
    class OptionsManager
    {
        private static bool initialised = false;
        private static ApplicationDataContainer localSettings;

        public static void Init()
        {
            if (initialised)
            {
                return;
            }

            // Load data
            var applicationData = ApplicationData.Current;
            localSettings = applicationData.LocalSettings;
            localSettings.CreateContainer("options",
                Windows.Storage.ApplicationDataCreateDisposition.Always);

            if (!localSettings.Containers.ContainsKey("options"))
            {
                return; // error with containers
            }
            
            if (!localSettings.Containers["options"].Values.ContainsKey("name"))
            {
                localSettings.Containers["options"].Values["name"] = "El asesino cubo";
            }

            if (!localSettings.Containers["options"].Values.ContainsKey("challengemode"))
            {
                localSettings.Containers["options"].Values["challengemode"] = false;
            }

            initialised = true;
        }

        private static void CheckInit()
        {
            if (!initialised)
            {
                throw new Exception("HighScoreManager not initialised");
            }
        }

        public static string GetPlayerName()
        {
            CheckInit();
            return (string)localSettings.Containers["options"].Values["name"];
        }

        public static void SetPlayerName(string name)
        {
            CheckInit();
            localSettings.Containers["options"].Values["name"] = name;
        }

        public static bool ChallengeModeEnabled()
        {
            return (bool)localSettings.Containers["options"].Values["challengemode"];
        }

        public static void ChallengeModeEnabled(bool enabled)
        {
            localSettings.Containers["options"].Values["challengemode"] = enabled;
        }
    }
}
