using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class ExtensionListPageViewModel : ViewModel
    {

        #region Public Properties

        //public List<string> BrowserFilterList { get; } = new() { "Alle", "Google Chrome", "Microsoft Edge" };

        private List<DataModels.Extension> _allExtensions = new();
        public List<DataModels.Extension> AllExtensions
        {
            get { return _allExtensions; }
            set { SetField(ref _allExtensions, value); }
        }

        private DataModels.Extension _selectedExtension;
        public DataModels.Extension SelectedExtension
        {
            get { return _selectedExtension; }
            set { SetField(ref _selectedExtension, value); }
        }

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetField(ref _errorMessage, value); }

        }

        private bool _errorOccured = false;
        public bool ErrorOccured
        {
            get { return _errorOccured; }
            set { SetField(ref _errorOccured, value); }

        }

        private XamlRoot _xamlRoot;
        public XamlRoot XamlRoot
        {
            get { return _xamlRoot; }
            set { SetField(ref _xamlRoot, value); }

        }

        #endregion

        #region Private Fields

        private List<DataModels.Extension> _notFilteredLastModifiedExtensions;

        #endregion

        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Commands

        public MyCommand CreateCommand { get; set; }
        public MyCommand AddCommand { get; set; }
        public MyCommand ChangeCommand { get; set; }
        public MyCommand AskDeleteCommand { get; set; }

        private void SetCommands()
        {
            CreateCommand = new MyCommand(Create);
            AddCommand = new MyCommand(Add);
            ChangeCommand = new MyCommand(Change);
            AskDeleteCommand = new MyCommand(AskDelete);
        }

        #endregion

        public ExtensionListPageViewModel()
        {
            SetCommands();
            LoadExtensions();
        }

        private void LoadExtensions()
        {
            IExtensionStorage extensionStorage = new DatabaseStorage();
            AllExtensions = extensionStorage.GetAll();
        }

        private void Add(object parameter = null)
        {
            _navigationService.Navigate("AddExtensionPage");
        }

        private void Create(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage");
        }

        private void Change(object parameter = null)
        {
            if (SelectedExtension == null) return;

            _navigationService.Navigate("CrxSelectPage", new PageParameter()
            {
                Extension = SelectedExtension,
                IsUpdate = true
            });
        }

        private async void AskDelete(object parameter = null)
        {
            if(XamlRoot == null)
            {
                ErrorMessage = StringResources.Get(this, 7);
                _log.Warn($"{XamlRoot} must not be NULL.");
                return;
            }

            ContentDialog confirmationDialog = new()
            {
                PrimaryButtonText = StringResources.Get(this, 3),
                PrimaryButtonCommand = new MyCommand(Delete),
                CloseButtonText = StringResources.Get(this, 4),
                Title = StringResources.Get(this, 5, SelectedExtension.Name),
                Content = StringResources.Get(this, 6, SelectedExtension.DeployementDir, SelectedExtension.BackupDir),
                XamlRoot = XamlRoot
            };
            await confirmationDialog.ShowAsync();

            
        }

        private void Delete(object parameter = null)
        {
            if (!RemoveFromStorage()) return;

            if (!DeleteDir(SelectedExtension.DeployementDir)) return;
            if (!DeleteDir(SelectedExtension.BackupDir)) return;

            LoadExtensions();
        }

        private bool RemoveFromStorage()
        {
            try
            {
                IExtensionStorage storage = new DatabaseStorage();
                storage.Delete(SelectedExtension.Id);
                return true;
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 2, exception.Message);
                _log.Warn(exception);
                return false;
            }
        }

        private bool DeleteDir(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
                return true;
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 1, exception.Message);
                _log.Warn(exception);
                return false;
            }
        }

    }
}
