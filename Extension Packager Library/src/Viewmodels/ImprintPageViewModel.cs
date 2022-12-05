// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Microsoft.UI.Text;
using System.IO;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class ImprintPageViewModel : ViewModel
    {
        #region Properties

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        private bool _isEditBoxReadOnly = true;
        public bool IsEditBoxReadOnly
        {
            get { return _isEditBoxReadOnly; }
            set { SetField(ref _isEditBoxReadOnly, value); }

        }


        private RichEditTextDocument _licenseDocument;
        public RichEditTextDocument LicenseDocument
        {
            get { return _licenseDocument; }
            set
            {
                SetField(ref _licenseDocument, value);
            }
        }

        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        #endregion

        public void LoadLicenseInfo()
        {
            IsEditBoxReadOnly = false;
            try
            {
                string text = File.ReadAllText(ResourceFile.GetFullPath("Third Party Notices.txt"));
                LicenseDocument.SetText(TextSetOptions.FormatRtf, text);
            }
            catch
            {

            }
            IsEditBoxReadOnly = true;
        }
    }
}
