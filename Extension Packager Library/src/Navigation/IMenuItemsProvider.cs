// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.Collections.Generic;

namespace Extension_Packager_Library.src.Navigation
{
    public interface IMenuItemsProvider
    {
        #region Public Properties

        public List<NavMenuItem> FooterNavMenuItems { get; }

        public List<NavMenuItem> MainNavMenuItems { get; }

        #endregion
    }
}
