// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Navigation
{
    public interface INavigationService
    {
        void Navigate(string page);
        void Navigate(string page, object param);
        void GoBack();
    }
}
