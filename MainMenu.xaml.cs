using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Core;
using TCD.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Brace
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : Page
    {
        private TCD.Controls.Flyout f;

        public MainMenu()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += BraceCommandsRequested;
         
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void menuPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent == null || this.Parent.GetType() != typeof(MainPage))
                return;

            if (Utils.OptionsManager.isFirstPlay())
            {
                //Need to show tutorial first
                MainPage parent = this.Parent as MainPage;
                parent.Children.Add(new HowToIntro());
            }
            else
            {
                MainPage parent = this.Parent as MainPage;
                parent.StartGame();
                parent.Children.Remove(this);
            }
        }

        private void menuHighScoreButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage parent = this.Parent as MainPage;
            parent.Children.Add(new Highscores());
        }

        private void menuHowToButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage parent = this.Parent as MainPage;
            parent.Children.Add(new HowToIntro());
        }

        private void menuOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage parent = this.Parent as MainPage;
            parent.Children.Add(new Options());
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
