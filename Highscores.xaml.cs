using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Core;
using TCD.Controls;
using Windows.UI;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Brace
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Highscores : Brace.Common.LayoutAwarePage
    {
        private TCD.Controls.Flyout f;

        ApplicationDataContainer localSettings;

        public Highscores()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += BraceCommandsRequested;
          
            loadScores();
        }

        private void loadScores()
        {
            SerializableDictionary<DateTime, int> scores = Utils.HighScoreManager.Scores;

            this.pageTitle.Text = "High Scores: Local";
            this.scoreList.Items.Clear();

            if (scores == null)
            {
                this.scoreList.Items.Add("Error loading scores");
            }

            else if (scores.Count > 0)
            {
                List<KeyValuePair<DateTime, int>> list = scores.ToList();

                list.Sort((firstPair, nextPair) =>
                {
                    return -firstPair.Value.CompareTo(nextPair.Value);
                }
                );

                int i = 1;
                foreach (var item in list)
                {
                    var str = string.Format("{0}:\t{1}\t{2}", i, item.Value, item.Key);
                    this.scoreList.Items.Add(str);
                    i++;
                }
            }

            else
            {
                this.scoreList.Items.Add("No scores yet!");
            }
        }

        private void loadOnlineScores()
        {
            KeyValuePair<int, string>[] scores = Utils.HighScoreManager.OnlineScores;

            this.pageTitle.Text = "High Scores: Global";
            this.scoreList.Items.Clear();

            if (scores == null)
            {
                this.scoreList.Items.Add("Error loading scores");
            }

            else if (scores.Length > 0)
            {
                int i = 1;
                foreach (var item in scores)
                {
                    var str = string.Format("{0}:\t{1}\t{2}", i, item.Key, item.Value);
                    this.scoreList.Items.Add(str);
                    i++;
                }
            }

            else
            {
                this.scoreList.Items.Add("No scores yet!");
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainPage parent = this.Parent as MainPage;
            parent.Children.Remove(this);
        }

        private void localButton_Click(object sender, RoutedEventArgs e)
        {
            loadScores();
        }

        private void onlineButton_Click(object sender, RoutedEventArgs e)
        {
            loadOnlineScores();
        }

        void BraceCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();
            SettingsCommand privacyPolicy = new SettingsCommand(
                "PrivacyPolicy",
                "Privacy Policy", (uiCommand) =>
                {
                    ShowSettingsPanel();
                });

            args.Request.ApplicationCommands.Add(privacyPolicy);
        }

        private void ShowSettingsPanel()
        {
            //make up some content (this can be a user control as well!)
            StackPanel s = new StackPanel();

            //  Game description
            TextBlock t = new TextBlock() { TextWrapping = TextWrapping.Wrap };
            t.Text = "In order to provide a competitive game environment, Brace will use your windows profile name when it uploads your highscore to our servers. No other information will be sent, other than your profile name and the score achieved. The data does not include any location information or any other identifying features. If you do not wish your name to be transmitted, enable Anonymous HighScores in the option menu. ";
            t.Margin = new Thickness(4);
            t.FontSize = 14;
            s.Children.Add(t);

            //now create the flyout
            f = new TCD.Controls.Flyout(
                new SolidColorBrush(Colors.White),//the foreground color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the background color of all flyouts
                new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)),//the theme brush of the app
                "Brace : Privacy Policy",
                FlyoutDimension.Narrow,
                s);
            f.ShowAsync();
        }
    }
}
