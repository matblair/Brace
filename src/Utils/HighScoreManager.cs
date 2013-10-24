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
        private static ApplicationDataContainer localSettings;

        public static SerializableDictionary<DateTime, int> Scores { get; private set; }
        public static KeyValuePair<DateTime, int> LastScore { get; private set; }

        public static void Init()
        {
            if (initialised)
            {
                return;
            }

            // Load data
            var applicationData = ApplicationData.Current;
            localSettings = applicationData.LocalSettings;
            localSettings.CreateContainer("highScores",
                Windows.Storage.ApplicationDataCreateDisposition.Always);

            if (!localSettings.Containers.ContainsKey("highScores"))
            {
                return; // error with containers
            }

            if (!localSettings.Containers["highScores"].Values.ContainsKey("scores"))
            {
                Scores = new SerializableDictionary<DateTime, int>();
                localSettings.Containers["highScores"].Values["scores"] = App.SerializeToString(Scores);
            }

            var data = (string)localSettings.Containers["highScores"].Values["scores"];
            Scores = App.DeserializeFromString<SerializableDictionary<DateTime, int>>(data);

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
            LastScore = new KeyValuePair<DateTime, int>(dateTime, score);
            Scores.Add(dateTime, score);

            // Drop keys while there's too many
            while (Scores.Count > MAX_SCORES)
            {
                var min = Scores.Values.Min();
                var item = Scores.First(kvp => kvp.Value == min);
                Scores.Remove(item.Key);
            }

            SaveScores();

            return true;
        }

        public static int HighestScore()
        {
            return Scores.Values.Max();
        }

        private static void SaveScores()
        {
            localSettings.Containers["highScores"].Values["scores"] = App.SerializeToString(Scores);
        }
    }
}
