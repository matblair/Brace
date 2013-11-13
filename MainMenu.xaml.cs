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
    
        public MainMenu()
        {
            this.InitializeComponent();
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
                pickHowTo();
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
            pickHowTo();
        }

        private void menuOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage parent = this.Parent as MainPage;
            parent.Children.Add(new Options());
        }

        private void pickHowTo()
        {
            MainPage parent = this.Parent as MainPage;
            if (BraceGame.get().input.hasAcceleromterSupport)
            {
                parent.Children.Add(new HowToIntro());
            }
            else
            {
                parent.Children.Add(new HowToIntroDesktop());
            }
        }


    }
}
