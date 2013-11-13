using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace Brace.Utils
{
    using Parse;

    public class HighScoreManager
    {
        private static bool initialised = false;
        private static readonly int MAX_SCORES = 10;
        private static ApplicationDataContainer localSettings;

        public static SerializableDictionary<DateTime, int> Scores { get; private set; }
        public static KeyValuePair<int, string>[] OnlineScores { get; private set; }

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

            var onlinedata = (KeyValuePair<int, string>[])localSettings.Containers["highScores"].Values["onlineScores"];
            OnlineScores = onlinedata;

            UpdateOnlineScores();

            initialised = true;
        }

        async public static void UpdateOnlineScores()
        {
            var query = from onlineScore in ParseObject.GetQuery("HighScore")
                        orderby onlineScore.Get<int>("Score") descending, onlineScore.Get<DateTime>("createdAt")
                        select onlineScore;
            query.Limit(10);

            try
            {
                IEnumerable<ParseObject> results = await query.FindAsync();

                var elements = results.ToArray();
                int countElements = elements.Length;
                OnlineScores = new KeyValuePair<int, string>[countElements];

                for (int i = 0; i < countElements; i++)
                {
                    int score = elements[i].Get<int>("Score");
                    string name = elements[i].Get<string>("Name");
                    OnlineScores[i] = new KeyValuePair<int, string>(score, name);
                }

                localSettings.Containers["highScores"].Values["onlineScores"] = OnlineScores;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void CheckInit()
        {
            if (!initialised)
            {
                throw new Exception("HighScoreManager not initialised");
            }
        }

        public static void AddScore(int score)
        {
            if (OptionsManager.isFirstPlay())
            {
                OptionsManager.isFirstPlay(false);
            }
            AddScore(score, DateTime.Now);
        }

        async public static void AddScore(int score, DateTime dateTime)
        {
            CheckInit();

            if (OptionsManager.isFirstPlay())
            {
                OptionsManager.isFirstPlay(false);
              
            }
            // There's no score list, so error
            if (Scores == null)
            {
                return;
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

            // Online score
            try
            {
                ParseObject newScore = new ParseObject("HighScore");
                if (OptionsManager.IsAnonymous())
                {
                    newScore["Name"] = "Anonymous";
                }
                else {
                    newScore["Name"] =
                   OptionsManager.GetPlayerName();
                }
               
                newScore["Score"] = score;
                await newScore.SaveAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
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
