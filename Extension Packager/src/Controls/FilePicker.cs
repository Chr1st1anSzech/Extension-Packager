// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace Extension_Packager.src.Controls
{
    public class FilePicker : IPicker
    {
        public async Task<string> GetPathDialog(string fileTypes)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            FileOpenPicker picker = new();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            SetFileTypes(picker, fileTypes);
            var file = await picker.PickSingleFileAsync();

            return file?.Path;
        }

        private void SetFileTypes(FileOpenPicker picker, string fileTypes)
        {
            string[] fileTypesArray = fileTypes.Split(',');
            foreach (string fileType in fileTypesArray)
            {
                picker.FileTypeFilter.Add(fileType);
            }

        }
    }
}
