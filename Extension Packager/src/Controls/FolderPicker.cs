// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Extension_Packager.src.Controls
{
    public class FolderPicker : IPicker
    {
        public async Task<string> GetPathDialog(string fileTypes)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            Windows.Storage.Pickers.FolderPicker picker = new();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.FileTypeFilter.Add("*");
            var folder = await picker.PickSingleFolderAsync();

            return folder?.Path;
        }
    }
}
