using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class OptionalInfosPageViewModel : ViewModel
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

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }

        private bool _comment;
        public bool Comment
        {
            get { return _comment; }
            set { SetField(ref _comment, value); }
        }


        #endregion


        #region Private Fields

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands


        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand GoBackCommand { get; set; }



        private void SetCommands()
        {
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            GoBackCommand = new MyCommand(GoBack);
        }


        #endregion

        public OptionalInfosPageViewModel()
        {
            SetCommands();
        }


        private void GoBack(object parameter = null)
        {
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("XmlManifestPage", PageParameter);
        }

        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("SuccessPage", PageParameter);
        }



        private void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;



            IsBusy = false;

            GoForward();
        }


    }
}
