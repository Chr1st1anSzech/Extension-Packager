// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

namespace Extension_Packager.src.Views
{
    public sealed partial class CrxSelectPage : Page
    {
        public CrxSelectPageViewModel ViewModel
        {
            get => (CrxSelectPageViewModel)DataContext;
            set => DataContext = value;
        }

        public CrxSelectPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = NavigationService.Instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is bool isStepBack && isStepBack)
            {
                ViewModel.IsStepBack= true;
            }
            else if( e.Parameter is Extension_Packager_Library.src.DataModels.Extension ext)
            {
                ExtensionManager.Instance.CurrentExtension = ext;
            }
        }
    }
}
