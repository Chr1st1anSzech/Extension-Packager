// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Views
{
    public sealed partial class ManifestEditPage : Page
    {
        public ManifestEditPageViewModel ViewModel
        {
            get => (ManifestEditPageViewModel)DataContext;
            set => DataContext = value;
        }

        public ManifestEditPage()
        {
            InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            ExtensionManifestTextBox.Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.ExtensionManifestDocument = ExtensionManifestTextBox.Document;
            };
        }
    }
}
