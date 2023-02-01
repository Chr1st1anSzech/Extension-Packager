// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Views;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Navigation;
using log4net;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension_Packager.src.Navigation
{
    internal class NavigationService : INavigationService
    {
        #region Private Fields

        private IMenuItemsProvider _menuItems;

        private Frame _frame;

        private NavigationView _navView;

        private Dictionary<string, Type> _pageTypes = new() {
            { nameof(MainPage), typeof(MainPage) },
            { nameof(CrxSelectPage), typeof(CrxSelectPage) },
            { nameof(ManifestEditPage), typeof(ManifestEditPage) },
            { nameof(SettingsLoadingPage), typeof(SettingsLoadingPage) },
            { nameof(SettingsPage), typeof(SettingsPage) },
            { nameof(SuccessPage), typeof(SuccessPage) },
            { nameof(XmlManifestPage), typeof(XmlManifestPage) },
            { nameof(ImprintPage), typeof(ImprintPage) },
            { nameof(ExtensionListPage), typeof(ExtensionListPage) },
            { nameof(OptionalInfosPage), typeof(OptionalInfosPage) }
        };

        #endregion


        #region Static Fields

        private static NavigationService _instance;
        internal static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new ArgumentNullException("The navigation manager was not initialized.");
                }
                else
                {
                    return _instance;
                }
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #endregion
        

        private NavigationService() { }


        internal static void Initialize(Frame frame, NavigationView navigationView, IMenuItemsProvider menuItems)
        {
            _instance = new()
            {
                _menuItems = menuItems,
                _frame = frame,
                _navView = navigationView
            };
        }


        public void Navigate(string page, object param = null)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page == "SettingsPage")
            {
                _frame.Navigate(typeof(SettingsPage));
                _navView.SelectedItem = _navView.SettingsItem;
            }
            else
            {
                if (!_pageTypes.ContainsKey(page))
                {
                    _log.Warn($"There is no page named \"{page}\"");
                    return;
                }

                NavMenuItem MainNavMenuItem = GetMainNavigationItem(page);
                if (MainNavMenuItem != null)
                {
                    _navView.SelectedItem = MainNavMenuItem;
                }
                _frame.Navigate(_pageTypes[page], param);
            }
        }


        public void Navigate(string page)
        {
            Navigate(page, null);
        }


        public void GoBack()
        {
            _frame.GoBack();
        }


        internal NavMenuItem GetMainNavigationItem(string pageName)
        {
            return _menuItems.MainNavMenuItems.FirstOrDefault(view => view.Tag.Equals(pageName));
        }
    }
}
