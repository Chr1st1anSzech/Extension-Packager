// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Views
{
    public sealed partial class ImprintPage : Page
    {
        public ImprintPageViewModel ViewModel
        {
            get => (ImprintPageViewModel)DataContext;
            private set => DataContext = value;
        }

        public ImprintPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            LicenseEditBox.Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.LicenseDocument = LicenseEditBox.Document;
                ViewModel.LoadLicenseInfo();
            };
        }
    }
}
