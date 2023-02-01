// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
            private set => DataContext = value;
        }


        public ExtensionListPage()
        {
            InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.XamlRoot = MainPageGrid.XamlRoot;
            };

        }
    }
}
