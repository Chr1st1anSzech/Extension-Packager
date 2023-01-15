// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Extension_Packager.src.Views
{
    public sealed partial class SuccessPage : Page
    {
        public SuccessPageViewModel ViewModel
        {
            get => (SuccessPageViewModel)DataContext;
            set => DataContext = value;
        }
        public SuccessPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
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
