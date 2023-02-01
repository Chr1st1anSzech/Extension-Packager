// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.DataModels;
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
            private set => DataContext = value;
        }

        public XmlManifestPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
            XmlManifestTextBox.Loaded += (object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
            {
                ViewModel.ExtensionXmlManifestDocument = XmlManifestTextBox.Document;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is PageParameter param)
            {
                ViewModel.PageParameter = param;
            }

            ViewModel.Init();
        }
    }
}
