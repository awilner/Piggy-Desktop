using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ImportExport.Wizard
{
    /// <summary>
    /// The ViewModel for a wizard. This class controls only navigating through the given collection of pages.
    /// Derived classes need to implement the specific data context that should be maintained accross all pages.
    /// The changes to the underlying system should be kept in that data context until the user finishes the wizard,
    /// so that the 
    /// </summary>
    public abstract class WizardViewModel
    {
        #region Fields

        /// <summary>
        /// Current page index. -1 indicates an invalid state or an empty page collection.
        /// </summary>
        int _currentPage = -1;

        /// <summary>
        /// List of pages. This actually contains the ViewModels for each page.
        /// </summary>
        ReadOnlyCollection<WizardPageViewModel> _pages;

        /// <summary>
        /// The name of the wizard.
        /// </summary>
        string _wizardName;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardName">Name of the wizard</param>
        /// <param name="pages">List of page ViewModels</param>
        public WizardViewModel(string wizardName,ReadOnlyCollection<WizardPageViewModel> pages)
        {
            _wizardName = wizardName;
            _pages = pages;

            // Start with the first page, obviously.
            CurrentPageIndex = 0;
        }

        #endregion // Constructor

        #region Methods

        protected abstract void ApplyChanges();

        #endregion // Methods

        #region Commands

        #region PreviousPageCommand

        /// <summary>
        /// Determine whether we can move to a previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _currentPage > 0;
            e.Handled = true;
        }

        /// <summary>
        /// Moves to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentPageIndex--;
            e.Handled = true;
        }

        #endregion // PreviousPageCommand

        #region NextPageCommand

        private void NextPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_currentPage > -1 && _pages[_currentPage].IsValid());
            e.Handled = true;
        }

        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsOnLastPage)
            {
                ApplyChanges();
                OnRequestClose();
            }
            else
                CurrentPageIndex++;

            e.Handled = true;
        }

        #endregion // NextPageCommand

        #region CancelCommand

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // We can always cancel.
            e.CanExecute = true;
            e.Handled = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OnRequestClose();
            e.Handled = true;
        }

        #endregion // CancelCommand

        #endregion // Commands

        #region Properties

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        public WizardPageViewModel CurrentPage
        {
            get { return _pages[_currentPage]; }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return _currentPage == _pages.Count - 1; }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public ReadOnlyCollection<WizardPageViewModel> Pages
        {
            get { return _pages; }
        }

        public int CurrentPageIndex
        {
            get
            {
                return _currentPage;
            }

            private set
            {
                if (value == _currentPage)
                    return;

                if( _currentPage >= 0 && _currentPage < _pages.Count )
                    _pages[_currentPage].IsCurrentPage = false;

                _currentPage = value;

                if (_currentPage >= 0 && _currentPage < _pages.Count)
                    _pages[_currentPage].IsCurrentPage = true;
            }

        }

        public string WizardName
        {
            get { return _wizardName; }
        }

        #endregion // Properties

        #region Events

        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        #endregion // Events

        #region Private Helpers

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // Private Helpers


        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


    }
}
