using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTCoreDefaultApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserPage : Page
    {
        private HttpCredentialsHeaderValue authHeader;

        public BrowserPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var Uri = e.Parameter as String;
            Browser.Settings.IsJavaScriptEnabled = true;
            if (Uri != null)
            {
                UrlBar.Text = Uri;
                Title.Text = Uri;

                if (Uri.Contains("127.0.0.1"))
                {
                    if (authHeader == null) { 
                        var msg = new PromptDialog("Authentication", "Please enter your Admin Password");
                        var result = await msg.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            var text = msg.Text;

                            var filter = new HttpBaseProtocolFilter();
                            filter.ServerCredential = new Windows.Security.Credentials.PasswordCredential(Uri, "Administrator", text);

                            Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient(filter);
                            var response = await client.GetAsync(new Uri(Uri));
                            Browser.Source = new Uri(Uri);

                            authHeader = CreateBasicHeader("Administrator", text);
                        }
                    }

                    //navigateWithAuth(new Uri(Uri));
                }
                else
                {
                    Browser.Navigate(new Uri(Uri));
                }
            }
        }

        private async void navigateWithAuth(Uri uri)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequestMessage.Headers.Authorization = authHeader;
            Browser.NavigateWithHttpRequestMessage(httpRequestMessage);
            System.Diagnostics.Debug.WriteLine(uri);

            await Task.Delay(1);

            Browser.NavigationStarting += Browser_NavigationStarting;
        }

        public HttpCredentialsHeaderValue CreateBasicHeader(string username, string password)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);

            String logindata = (username + ":" + password);
            var value = new HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(byteArray));
            System.Diagnostics.Debug.WriteLine("AuthenticationHeaderValue: " + value);

            return value;
        }

        private void WebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            UrlBar.Text = e.Uri.ToString();
            Title.Text = Browser.DocumentTitle;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationUtils.GoBack();
        }

        private void BrowserBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack) Browser.GoBack();
        }

        private void BrowserForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward) Browser.GoForward();
        }

        private void BrowserGoButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri;
            if (Uri.TryCreate(UrlBar.Text, UriKind.Absolute, out uri))
            {
                Browser.Navigate(uri);
            }
            else
            {
                var msg = new Windows.UI.Popups.MessageDialog("Not a valid URL! Loading Google...");
                Browser.Navigate(new Uri("http://google.com"));

                msg.ShowAsync();
            }
            Title.Text = "Loading...";
        }

        private void Browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Browser.NavigationStarting -= Browser_NavigationStarting;
            args.Cancel = true;
            navigateWithAuth(args.Uri);
        }
    }
}
