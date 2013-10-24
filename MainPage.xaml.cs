﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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

        public MainPage()
        {
            mainPage = this;

            InitializeComponent();
            game = BraceGame.get();
            game.Run(this);

            this.Children.Add(new MainMenu());
        }

        public static MainPage GetMainPage()
        {
            return mainPage;
        }
        
        public void ResetGame()
        {
            game.paused = true;
            this.Children.Add(new MainMenu());
        }

        public void StartGame()
        {
            game.Start();
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

            Button bExit = new Button() { Content = "Quit" };
            bExit.Click += bExit_OnClick;

            s.Children.Add(t);
            s.Children.Add(bExit);

            //now create the flyout
            Flyout f = new Flyout(
                new SolidColorBrush(Colors.White),//the foreground color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the background color of all flyouts
                new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)),//the theme brush of the app
                "Brace : Paused",
                FlyoutDimension.Narrow,
                s);
            f.OnClosing += f_OnClosing;
            f.ShowAsync();
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

        async void bExit_OnClick(object sender, RoutedEventArgs e)
        {
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Are you sure you want to quit?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand(
                "Quit",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand(
                "Cancel",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        void f_OnClosing(object sender, CloseReason reason, System.ComponentModel.CancelEventArgs cancelEventArgs)
        {
            this.ResumeGame();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label.Equals("Quit"))
            {
                game.Exit();
            }
            else
            {
                this.PauseGame();
            }
        }
    }
}
