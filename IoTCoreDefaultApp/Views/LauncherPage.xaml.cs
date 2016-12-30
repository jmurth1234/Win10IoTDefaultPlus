using IoTCoreDefaultApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTCoreDefaultApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LauncherPage : Page
    {
        public ObservableCollection<AppListItem> AppItemList;
        public static LauncherPage Current;
        private DispatcherTimer timer;

        public Frame rootFrame { get { return SideFrame; } }

        public LauncherPage()
        {
            this.InitializeComponent();
            Current = this;

            PopulateAppsList();

            this.Loaded += (sender, e) =>
            {
                UpdateDateTime();

                appList.ItemsSource = AppItemList;

                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Start();
            };
            this.Unloaded += (sender, e) =>
            {
                timer.Stop();
                timer = null;
            };
        }

        private void timer_Tick(object sender, object e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            // Using DateTime.Now is simpler, but the time zone is cached. So, we use a native method insead.
            SYSTEMTIME localTime;
            NativeTimeMethods.GetLocalTime(out localTime);

            DateTime t = localTime.ToDateTime();
            CurrentTime.Text = t.ToString("t", CultureInfo.CurrentCulture) + Environment.NewLine + t.ToString("d", CultureInfo.CurrentCulture);
        }

        private async Task PopulateAppsList()
        {
            AppItemList = new ObservableCollection<AppListItem>();

            var packageManager = new PackageManager();

            var packages = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            foreach (var package in packages)
            {
                var tasks = await package.GetAppListEntriesAsync();

                foreach (var task in tasks)
                {
                    try
                    {
                        var info = task.DisplayInfo;
                        var logo = info.GetLogo(new Size(64, 64));

                        var item = new AppListItem
                        {
                            Name = info.DisplayName,
                            PackageFullName = package.Id.FullName,
                            AppEntry = task,
                            ImgSrc = new BitmapImage()
                        };

                        var logoImg = await logo.OpenReadAsync();
                        await item.ImgSrc.SetSourceAsync(logoImg);

                        AppItemList.Add(item);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        Debug.WriteLine(e.Data);
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
#if !FORCE_OOBE_WELCOME_SCREEN
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.HasDoneOOBEKey))
            {
                SideFrame.Navigate(typeof(MainPage), e.Content);
            }
            else
#endif
            {
                SideFrame.Navigate(typeof(OOBEWelcome), e.Content);
            }
        }

        private void appList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AppListItem;
            item.AppEntry.LaunchAsync();
        }

        private void CortanaButton_Click(object sender, RoutedEventArgs e)
        {
            CortanaHelper.LaunchCortanaToAboutMeAsync();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            CrappyStart.IsPaneOpen = StartButton.IsChecked.Value;
        }

        private void CrappyStart_PaneClosed(SplitView sender, object args)
        {
            StartButton.IsChecked = false;
        }
    }

    public class AppListItem
    {
        public BitmapImage ImgSrc { get; set; }

        public String Name { get; set; }

        public String PackageFullName { get; set; }

        public AppListEntry AppEntry { get; set; }

    }
}
