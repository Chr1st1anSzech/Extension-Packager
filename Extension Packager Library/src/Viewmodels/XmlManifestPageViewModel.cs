// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Text;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Formatter;
using Extension_Packager_Library.src.Helper;
using log4net;
using System.Reflection;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.DataProcessing;
using System.Xml;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class XmlManifestPageViewModel : ViewModel
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

        private string _appId = string.Empty;
        public string AppId
        {
            get { return _appId; }
            set { SetField(ref _appId, value); }

        }


        private RichEditTextDocument _extensionXmlManifestDocument;
        public RichEditTextDocument ExtensionXmlManifestDocument
        {
            get { return _extensionXmlManifestDocument; }
            set
            {
                SetField(ref _extensionXmlManifestDocument, value);
            }
        }

        #endregion


        #region Private Fields

        private XmlManifestDataProcessing _dataProcessing;

        #endregion


        #region Static Fields


        #endregion


        #region Commands

        public MyCommand GoBackCommand { get; set; }
        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand PreviewXmlManifestCommand { get; set; }

        #endregion


        public void Init()
        {
            SetCommands();
            SetProperties();
            _dataProcessing = new(PageParameter, ShowWarning);
        }


        private void SetProperties()
        {
            AppId = PageParameter.Extension.Id;
        }


        private void SetCommands()
        {
            GoBackCommand = new MyCommand(GoBack);
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            PreviewXmlManifestCommand = new MyCommand(PreviewXmlManifest);
        }


        private void GoBack(object parameter = null)
        {
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("ManifestEditPage", PageParameter);
        }


        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("OptionalInfosPage", PageParameter);
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;

            bool successfulProcessing = await _dataProcessing.ProcessInput(); ;
            if (!successfulProcessing) return;

            IsBusy = false;

            GoForward();
        }


        private void PreviewXmlManifest(object parameter = null)
        {
            IsEditBoxReadOnly = false;


            string xmlManifest = _dataProcessing.CreateXmlManifest();
            if (xmlManifest == null) return;

            ExtensionXmlManifestDocument.SetText(TextSetOptions.FormatRtf, xmlManifest);

            Formatter.Formatter formatter = new XmlFormatter()
            {
                //AttributeColor = App.Current.Resources["KeyColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //ElementNameColor = App.Current.Resources["BoolNullColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //ValueColor = App.Current.Resources["StringColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //HighlightColor = App.Current.Resources["HighlightColor"] as Color? ?? Color.FromArgb(120, 120, 120, 120)
            };
            formatter.SyntaxHighlight(ExtensionXmlManifestDocument);

            IsEditBoxReadOnly = true;
        }
    }
}
