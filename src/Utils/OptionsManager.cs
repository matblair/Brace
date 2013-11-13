using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

            if (!localSettings.Containers["options"].Values.ContainsKey("volume"))
            {
                localSettings.Containers["options"].Values["volume"] = 100;
            }

            if (!localSettings.Containers["options"].Values.ContainsKey("anonymous"))
            {
                localSettings.Containers["options"].Values["anonymous"] = true;
            }

            if (!localSettings.Containers["options"].Values.ContainsKey("asked"))
            {
                localSettings.Containers["options"].Values["asked"] = false;
            }


            if (!localSettings.Containers["options"].Values.ContainsKey("firstPlay"))
            {
                localSettings.Containers["options"].Values["firstPlay"] = true;
            }

            SetVolume((int)localSettings.Containers["options"].Values["volume"]);

            initialised = true;
        }

        private static void CheckInit()
        {
            if (!initialised)
            {
                throw new Exception("HighScoreManager not initialised");
            }
        }

        public static bool isFirstPlay()
        {
            CheckInit();
            return (bool)localSettings.Containers["options"].Values["firstPlay"];
        }

        public static bool hasAsked()
        {
            //CheckInit();
            return false;
            //return (bool)localSettings.Containers["options"].Values["asked"];
        }

        public static void hasAsked(bool val)
        {
            localSettings.Containers["options"].Values["asked"] = val;
        }

        public static void isFirstPlay(bool val)
        {
           localSettings.Containers["options"].Values["firstPlay"] = val;
        }

        public static string GetPlayerName()
        {
            CheckInit();
            return (string)localSettings.Containers["options"].Values["name"];
        }

        public static bool IsAnonymous()
        {
            CheckInit();
            return (bool)localSettings.Containers["options"].Values["anonymous"];
        }

        public static void SetAnonymous(bool value)
        {
            CheckInit();
            localSettings.Containers["options"].Values["anonymous"] = value;
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

        public static int Volume()
        {
            return (int)localSettings.Containers["options"].Values["volume"];
        }

        public static void SetVolume(int vol)
        {
            localSettings.Containers["options"].Values["volume"] = vol;

            // Calculate the volume that's being set
            double newVolume = ushort.MaxValue * vol / 100.0;

            uint v = ((uint)newVolume) & 0xffff;
            uint vAll = v | (v << 16);

            // Set the volume
            //int retVal = NativeMethods.WaveOutSetVolume(IntPtr.Zero, vAll);
        }
    }

    static class NativeMethods
    {

        //[DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        //public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);

        //[DllImport("winmm.dll", SetLastError = true)]
        //public static extern bool Play(string pszSound, IntPtr hmod, uint fdwSound);
    }
}
