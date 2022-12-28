// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Extension_Packager.src.Views
{
    public sealed partial class XmlManifestPage : Page
    {
        public XmlManifestPageViewModel ViewModel
        {
            get => (XmlManifestPageViewModel)DataContext;
            set => DataContext = value;
        }

        public XmlManifestPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            ExtensionManifestTextBox.Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.ExtensionXmlManifestDocument = ExtensionManifestTextBox.Document;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is bool isStepBack && isStepBack)
            {
                ViewModel.IsStepBack = true;
            }
        }
    }
}
