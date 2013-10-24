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

            initialised = true;
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

            // Insert the new score
            Scores.Add(score, dateTime);

            // Drop keys while there's too many
            while (Scores.Count > MAX_SCORES)
            {
                Scores.Remove(Scores.Keys.Min());
            }

            return true;
        }

        public static int HighestScore()
        {
            return Scores.Keys.Max();
        }
    }
}
