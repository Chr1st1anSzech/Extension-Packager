// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.DataProcessing;
using Extension_Packager_Library.src.Formatter;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Validation;
using log4net;
using Microsoft.UI.Text;
using System;
using System.IO;
using System.Reflection;
using static Extension_Packager_Library.src.DataModels.Constants;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class ManifestEditPageViewModel : ViewModel
    {
        #region Public Properties

        private PageParameter _pageParameter;
        public PageParameter PageParameter
        {
            get { return _pageParameter; }
            set { SetField(ref _pageParameter, value); }
        }

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

        private bool _isNameValide = true;
        public bool IsNameValid
        {
            get { return _isNameValide; }
            set { SetField(ref _isNameValide, value); }

        }

        private bool _isShortNameValide = true;
        public bool IsShortNameValid
        {
            get { return _isShortNameValide; }
            set { SetField(ref _isShortNameValide, value); }

        }


        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                SetField(ref _name, value);
                ShortName = new ShortNameFormatter().Format(value);
            }
        }


        private string _version = string.Empty;
        public string Version
        {
            get { return _version; }
            set
            {
                SetField(ref _version, value);
            }
        }


        private string _shortName = string.Empty;
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                SetField(ref _shortName, value);
            }
        }


        private RichEditTextDocument _extensionManifestDocument;
        public RichEditTextDocument ExtensionManifestDocument
        {
            get { return _extensionManifestDocument; }
            set
            {
                SetField(ref _extensionManifestDocument, value);
            }
        }


        #endregion


        #region Private Fields

        private ManifestEditDataProcessing _dataProcessing;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands


        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand GoBackCommand { get; set; }
        public MyCommand ManifestPreviewCommand { get; set; }
        public MyCommand ResetValuesCommand { get; set; }


        private void SetCommands()
        {
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            GoBackCommand = new MyCommand(GoBack);
            ManifestPreviewCommand = new MyCommand(ShowManifest);
            ResetValuesCommand = new MyCommand(SetProperties);
        }


        #endregion


        public void Init()
        {
            SetCommands();
            SetProperties();
            CompareToRecognizedShortname();
            _dataProcessing = new(ShortName, PageParameter, ShowWarning);
        }


        private void GoBack(object parameter = null)
        {
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("CrxSelectPage", PageParameter);
        }


        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("XmlManifestPage", PageParameter);
        }


        public void Reset()
        {
            if (PageParameter.Get<string>("TmpPackedCrxFile") == null) return;
            try
            {
                File.Delete(PageParameter.Get<string>("TmpPackedCrxFile"));
                if (PageParameter.Get<string>("PrivateKeyFile") != null)
                {
                    File.Delete(PageParameter.Get<string>("PrivateKeyFile"));
                }
            }
            catch (Exception exception)
            {
                WarningMessage = StringResources.GetWithReason(this, 9, exception.Message);
            }
        }


        private void SetProperties(object parameter = null)
        {
            Name = PageParameter.Extension.Name ?? string.Empty;
            ShortName = PageParameter.Extension.ShortName ?? string.Empty;
            Version = PageParameter.Extension.Version ?? string.Empty;
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            SetExtensionValues();

            bool successfulProcessing = await _dataProcessing.ProcessInput();
            if (!successfulProcessing) return;

            IsBusy = false;

            GoForward();
        }


        private bool IsInputValide()
        {
            bool areAllInputsValid = IsValidOrWarn(() =>
            {
                return InputValidator.IsNameValid(Name);
            }, StringResources.Get(this, 7));


            areAllInputsValid &= IsValidOrWarn(() =>
            {
                return InputValidator.IsShortNameValide(ShortName);
            }, StringResources.Get(this, 8));


            areAllInputsValid &= IsValidOrWarn(() =>
            {
                return !IsDuplicateShortname() || PageParameter.IsUpdate;
            }, StringResources.Get(this, 10));

            if (!areAllInputsValid) return false;

            IsNameValid = IsShortNameValid = true;
            IsWarningVisible = false;
            return true;
        }


        private bool IsDuplicateShortname()
        {
            IExtensionStorage storage = new DatabaseStorage();
            int count = storage.GetCountByShortname(ShortName);
            return count > 0;
        }


        private void CompareToRecognizedShortname()
        {
            string shortname1 = PageParameter.Get<string>("Shortname1");
            string shortname2 = PageParameter.Get<string>("Shortname2");

            ExtInOutputDir result = ExtInOutputDir.None;
            if (shortname1 != null || shortname2 != null)
            {
                result = ExtInOutputDir.Partial;

                string errorMessage = StringResources.Get(this, 11);
                if (shortname1.Equals(shortname2))
                {
                    result = ExtInOutputDir.Full;
                    errorMessage = StringResources.Get(this, 12);
                }
                ShowWarning(errorMessage);
            }
            PageParameter.Set("ExtInOutputDir", result);
        }


        private void SetExtensionValues()
        {
            PageParameter.Extension.Name = Name;
            PageParameter.Extension.ShortName = ShortName;
        }


        private void ShowManifest(object parameter = null)
        {
            IsEditBoxReadOnly = false;

            if (!IsInputValide())
            {
                ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, string.Empty);
                IsEditBoxReadOnly = true;
                return;
            }

            _dataProcessing.ChangeManifest(ShortName);

            ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, PageParameter.Get<string>("ManifestContent"));

            Formatter.Formatter formatter = new JSONFormatter()
            {
                //KeyColor = App.Current.Resources["KeyColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //BoolNullColor = App.Current.Resources["BoolNullColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //NumberColor = App.Current.Resources["NumberColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //StringColor = App.Current.Resources["StringColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //HighlightColor = App.Current.Resources["HighlightColor"] as Color? ?? Color.FromArgb(120, 120, 120, 120)
            };
            formatter.SyntaxHighlight(ExtensionManifestDocument);
            IsEditBoxReadOnly = true;
        }
    }
}
