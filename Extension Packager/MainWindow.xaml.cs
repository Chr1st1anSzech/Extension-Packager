using Extension_Packager.src.Navigation;
using Extension_Packager.src.Startup;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Settings;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Extension_Packager
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)rootPanel.DataContext;
            set => rootPanel.DataContext = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            SetSize(1400, 950);
            SetTitleBar();
            MainNavigationView.Loaded += async (object sender, RoutedEventArgs e) =>
            {
                ViewModel.MenuItems = new MenuItemsProvider();
                NavigationService.Initialize(MainWindowFrame, MainNavigationView, ViewModel.MenuItems);
                await PrepareView();
            };
        }
        private void SetTitleBar()
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(MyTitleBar);
        }

        private void SetSize(int width, int height)
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = width, Height = height });
        }

        private async Task PrepareView()
        {
            Startup startup = new ();
            NavigationService.Instance.Navigate("SettingsLoadingPage");
            startup.SetupSettingsRepo(SettingsReaderFactory.Create(), SettingsWriterFactory.Create());
            await Task.Delay(250);
            if (startup.AreSettingsValide())
            {
                NavigationService.Instance.Navigate("MainPage");
            }
            else
            {
                NavigationService.Instance.Navigate("SettingsPage");
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationService.Instance.Navigate("SettingsPage");
            }
            if(sender.SelectedItem is NavMenuItem mainNavMenuItem)
            {
                NavigationService.Instance.Navigate(mainNavMenuItem.Tag.ToString(), mainNavMenuItem.PageParameter);
            }
        }

        private void SetNavViewAccessKeys()
        {
            MainNavigationView.GetVisualInternal();
        }
    }
}
