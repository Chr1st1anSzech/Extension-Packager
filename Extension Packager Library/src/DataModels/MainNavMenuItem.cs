// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Controls;
using System;

namespace Extension_Packager_Library.src.DataModels
{
    public class NavMenuItem : ListViewItem
    {
        public SymbolIcon Icon { get; set; }
        public Type Page { get; set; }
        public PageParameter PageParameter { get; set; }
    }
}
