// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class MainWindowViewModel : ViewModel
    {

        #region Public Properties

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        private IMenuItemsProvider _menuItems;
        public IMenuItemsProvider MenuItems
        {
            get { return _menuItems; }
            set { SetField(ref _menuItems, value); }

        }

        #endregion


        public MainWindowViewModel()
        {

        }
    }
}
