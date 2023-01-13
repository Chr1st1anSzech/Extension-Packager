// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Views
{
    public sealed partial class AddExtensionPage : Page
    {
        public AddExtensionPageViewModel ViewModel
        {
            get => (AddExtensionPageViewModel)DataContext;
            set => DataContext = value;
        }
        public AddExtensionPage()
        {
            this.InitializeComponent();
        }
    }
}
