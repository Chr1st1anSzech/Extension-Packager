﻿// Copyright (c) Christian Szech
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
    public class NavigationService : INavigationService
    {
        #region Private Fields

        private IMenuItemsProvider _menuItems;

        private Frame _frame;

        private NavigationView _navView;

        private Dictionary<string, Type> _pageTypes = new() {
            { "MainPage", typeof(MainPage) },
            { "CrxSelectPage", typeof(CrxSelectPage) },
            { "ManifestEditPage", typeof(ManifestEditPage) },
            { "SettingsLoadingPage", typeof(SettingsLoadingPage) },
            { "SettingsPage", typeof(SettingsPage) },
            { "SuccessPage", typeof(SuccessPage) },
            { "XmlManifestPage", typeof(XmlManifestPage) },
            { "ImprintPage", typeof(ImprintPage) },
            { "ExtensionListPage", typeof(ExtensionListPage) },
            { "AddExtensionPage", typeof(AddExtensionPage) }
        };

        #endregion


        #region Static Fields

        private static NavigationService _instance;
        public static NavigationService Instance
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


        public static void Initialize(Frame frame, NavigationView navigationView, IMenuItemsProvider menuItems)
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


        public NavMenuItem GetMainNavigationItem(string pageName)
        {
            return _menuItems.MainNavMenuItems.FirstOrDefault(view => view.Tag.Equals(pageName));
        }
    }
}
