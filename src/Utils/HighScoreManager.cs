using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Brace.Utils
{
    public class HighScoreManager
    {
        private static bool initialised = false;
        private static readonly int MAX_SCORES = 10;

        public static SerializableDictionary<int, DateTime> Scores { get; private set; }

        public static void Init()
        {
            if (initialised)
            {
                return;
            }

            // Load data
            var applicationData = ApplicationData.Current;
            var localSettings = applicationData.LocalSettings;
            localSettings.CreateContainer("highScores",
                Windows.Storage.ApplicationDataCreateDisposition.Always);

            if (!localSettings.Containers.ContainsKey("highScores"))
            {
                return; // error with containers
            }

            if (!localSettings.Containers["highScores"].Values.ContainsKey("scores"))
            {
                Scores = new SerializableDictionary<int, DateTime>();
                localSettings.Containers["highScores"].Values["scores"] = App.SerializeToString(Scores);
            }

            var data = (string)localSettings.Containers["highScores"].Values["scores"];
            Scores = App.DeserializeFromString<SerializableDictionary<int, DateTime>>(data);
        }

        private static void CheckInit()
        {
            if (!initialised)
            {
                throw new Exception("HighScoreManager not initialised");
            }
        }

        public static bool AddScore(int score)
        {
            return AddScore(score, DateTime.Now);
        }

        public static bool AddScore(int score, DateTime dateTime)
        {
            CheckInit();

            // There's no score list, so error
            if (Scores == null)
            {
                return false;
            }

            List<int> keys = Scores.Keys.ToList();

            // Insert the new score
            if (score > keys.Min())
            {
                Scores.Remove(keys.Min());
                Scores.Add(score, dateTime);
            }

            // Drop keys while there's too many
            while (keys.Count > MAX_SCORES)
            {
                int k = keys.Min();
                keys.Remove(k);
                Scores.Remove(k);
            }

            return true;
        }
    }
}
