// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Brace;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Core;
using TCD.Controls;
using System;

namespace Brace
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private BraceGame game;
        private static MainPage mainPage;
      
        private TCD.Controls.Flyout f;
        private TCD.Controls.Flyout p;

        public MainPage()
        {
            mainPage = this;

            InitializeComponent();
            game = BraceGame.get();
            Utils.OptionsManager.Init();
            game.Run(this);
          
            Utils.SoundManager.GetCurrent().PlaySound(Utils.SoundManager.SoundsEnum.Rain);
            this.Children.Add(new MainMenu());
            
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }

        public static MainPage GetMainPage()
        {
            return mainPage;
        }
        
        public void ResetGame()
        {
            game.paused = true;
            //this.rainAudio.Play();
            this.Children.Add(new EndGame());
        }

        public void StartGame()
        {
           
            game.Start();
            Utils.SoundManager.GetCurrent().PlaySound(Utils.SoundManager.SoundsEnum.Thunder2);
            this.gamePauseButton.Visibility = Utils.OptionsManager.ChallengeModeEnabled() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PauseGame()
        {
            game.paused = true;
            //make up some content (this can be a user control as well!)
            StackPanel s = new StackPanel();

            //  Game description
            TextBlock t = new TextBlock() { TextWrapping = TextWrapping.Wrap };
            t.Text = "This is Brace. A pretty cool game.\n\nMade by Benedict, Mathew, Sindre and James.\n\n\n\n";
            t.Margin = new Thickness(4);
            t.FontSize = 14;

            Button bExit = new Button() { Content = "Concede" };
            bExit.Click += TerminateGame;

            s.Children.Add(t);
            s.Children.Add(bExit);

            //now create the flyout
            f = new TCD.Controls.Flyout(
                new SolidColorBrush(Colors.White),//the foreground color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the background color of all flyouts
                new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)),//the theme brush of the app
                "Brace : Paused",
                FlyoutDimension.Narrow,
                s);
            f.OnClosing += f_OnClosing;
            f.ShowAsync();
        }

        private void TerminateGame(object sender, RoutedEventArgs e)
        {
            if (f != null)
            {
                f.Hide(CloseReason.Other);
                f = null;
            }

            game.getPlayer().addHealth(-10000);
            game.paused = false;

        }

        private void ResumeGame()
        {
            game.paused = false;
        }

        private void menuPlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartGame();
        }

        private void gamePauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (game.paused)
            {
                this.ResumeGame();
            }
            else
            {
                this.PauseGame();
            }
        }

        void f_OnClosing(object sender, CloseReason reason, System.ComponentModel.CancelEventArgs cancelEventArgs)
        {
            this.ResumeGame();
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();
            if(this.game.IsActive){
                game.paused = true;
            }
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
            p = new TCD.Controls.Flyout(
                new SolidColorBrush(Colors.White),//the foreground color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the background color of all flyouts
                new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)),//the theme brush of the app
                "Brace : Privacy Policy",
                FlyoutDimension.Narrow,
                s);
            p.ShowAsync();
        }
    }

}
