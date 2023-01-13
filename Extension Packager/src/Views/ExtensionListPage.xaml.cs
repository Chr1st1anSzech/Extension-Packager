// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Views
{
    public sealed partial class ExtensionListPage : Page
    {
        public ExtensionListPageViewModel ViewModel
        {
            get => (ExtensionListPageViewModel)DataContext;
            set => DataContext = value;
        }


        public ExtensionListPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            this.Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.XamlRoot = MainPageGrid.XamlRoot;
            };
            
        }
    }
}
