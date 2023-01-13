// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Views;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace Extension_Packager.src.Navigation
{
    public class MenuItemsProvider : IMenuItemsProvider
    {
        #region Public Properties

        private readonly List<NavMenuItem> _footerNavMenuItems = new();
        public List<NavMenuItem> FooterNavMenuItems
        {
            get { return _footerNavMenuItems; }
        }

        private readonly List<NavMenuItem> _mainNavMenuItems = new();
        public List<NavMenuItem> MainNavMenuItems
        {
            get { return _mainNavMenuItems; }

        }
        
        #endregion

        public MenuItemsProvider()
        {
            SetMainNavItems();
            SetFooterNavItems();
        }

        private void SetMainNavItems()
        {
            MainNavMenuItems.Add(new NavMenuItem()
            {
                Content = StringResources.Get(this, 1),
                Icon = new SymbolIcon((Symbol)0xE80F),
                Page = typeof(MainPage),
                Tag = "MainPage"
            });

            MainNavMenuItems.Add(new NavMenuItem()
            {
                Content = StringResources.Get(this, 4),
                Icon = new SymbolIcon((Symbol)0xE71D),
                Page = typeof(ExtensionListPage),
                Tag = "ExtensionListPage"
            });

            MainNavMenuItems.Add(new NavMenuItem()
            {
                Content = StringResources.Get(this, 2),
                Icon = new SymbolIcon((Symbol)0xE7C3),
                Page = typeof(CrxSelectPage),
                Tag = "CrxSelectPage"
            });

            MainNavMenuItems.Add(new NavMenuItem()
            {
                Content = StringResources.Get(this, 5),
                Icon = new SymbolIcon((Symbol)0xE8E5),
                Page = typeof(CrxSelectPage),
                Tag = "AddExtensionPage"
            });
        }

        private void SetFooterNavItems()
        {
            FooterNavMenuItems.Add(new NavMenuItem()
            {
                Content = StringResources.Get(this, 3),
                Icon = new SymbolIcon((Symbol)0xE946),
                Page = typeof(ImprintPage),
                Tag = "ImprintPage"
            });
        }
    }
}
