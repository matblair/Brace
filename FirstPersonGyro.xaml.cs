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
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Core;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Brace
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FirstPersonGyro : Brace.Common.LayoutAwarePage
    {
        private CoreWindow window;

        public FirstPersonGyro()
        {
            this.InitializeComponent();

            if (Utils.OptionsManager.isFirstPlay())
            {
                this.endButton.Content = "Play";
            }
            else
            {
                this.endButton.Content = "Return";
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

 
        private void TextBlock_SelectionChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void endButtonPressed(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Utils.OptionsManager.isFirstPlay())
            {
                MainPage parent = this.Parent as MainPage;
                int size = parent.Children.Count();
                for (int i = (size - 1); i > 0; i--)
                {
                    parent.Children.RemoveAt(i);
                }
                parent.StartGame();
            }
            else
            {
                MainPage parent = this.Parent as MainPage;
                int size = parent.Children.Count();
                for (int i = (size-1); i > 1; i--)
                {
                    parent.Children.RemoveAt(i);
                }
            }
        }
    }
}
