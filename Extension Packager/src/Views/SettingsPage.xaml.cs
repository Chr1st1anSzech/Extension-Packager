// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel
        {
            get => (SettingsViewModel)DataContext;
            private set => DataContext = value;
        }
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
        }
    }
}
