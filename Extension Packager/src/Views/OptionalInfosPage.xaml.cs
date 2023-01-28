// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Extension_Packager.src.Navigation;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Viewmodels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Extension_Packager.src.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionalInfosPage : Page
    {
        public OptionalInfosPageViewModel ViewModel
        {
            get => (OptionalInfosPageViewModel)DataContext;
            set => DataContext = value;
        }

        public OptionalInfosPage()
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
        }
    }
}
