using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
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
        public MyCommand UpdateCommand { get; set; }
        public MyCommand AskDeleteCommand { get; set; }

        private void SetCommands()
        {
            CreateCommand = new MyCommand(Create);
            AddCommand = new MyCommand(Add);
            UpdateCommand = new MyCommand(Update);
            AskDeleteCommand = new MyCommand(AskDelete);
        }

        #endregion


        public ExtensionListPageViewModel()
        {
            SetCommands();
            LoadExtensions();
        }


        /// <summary>
        /// Loads all stored extensions.
        /// </summary>
        private void LoadExtensions()
        {
            try
            {
                IExtensionStorage extensionStorage = new DatabaseStorage();
                AllExtensions = extensionStorage.GetAll();
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 8, exception.Message);
                _log.Warn(exception);
            }
        }


        /// <summary>
        /// Navigates to the page for adding an existing extension.
        /// </summary>
        private void Add(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage", new PageParameter()
            {
                Extension = SelectedExtension,
                IsPageBack = false,
                IsAddition = true,
                IsUpdate = false
            });
        }


        /// <summary>
        /// Navigates to the page for creating a new extension.
        /// </summary>
        private void Create(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage", new PageParameter()
            {
                IsUpdate = false,
                IsAddition = false,
                IsPageBack = false
            });
        }


        /// <summary>
        /// Navigates to the page for updating the extension.
        /// </summary>
        private void Update(object parameter = null)
        {
            if (SelectedExtension == null) return;

            _navigationService.Navigate("CrxSelectPage", new PageParameter()
            {
                Extension = SelectedExtension,
                IsPageBack = false,
                IsAddition = false,
                IsUpdate = true
            });
        }


        /// <summary>
        /// Asks whether the extension should be deleted and triggers the deletion after confirmation.
        /// </summary>
        private async void AskDelete(object parameter = null)
        {
            if(SelectedExtension== null) return;

            if (XamlRoot == null)
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


        /// <summary>
        /// Removes the extension from the storage and deletes the associated directories.
        /// </summary>
        private void Delete(object parameter = null)
        {
            if (!RemoveFromStorage()) return;

            if (!DeleteDir(SelectedExtension.DeployementDir)) return;
            if (!DeleteDir(SelectedExtension.BackupDir)) return;

            LoadExtensions();
        }


        /// <summary>
        /// Removes the extension from the storage.
        /// </summary>
        /// <returns>The result of whether the action was carried out successfully.</returns>
        private bool RemoveFromStorage()
        {
            try
            {
                IExtensionStorage storage = new DatabaseStorage();
                storage.DeleteLastModified(SelectedExtension.Id);
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


        /// <summary>
        /// Deletes the directory recursively.
        /// </summary>
        /// <param name="dir">The directory to be deleted.</param>
        /// <returns>The result of whether the action was carried out successfully.</returns>
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
