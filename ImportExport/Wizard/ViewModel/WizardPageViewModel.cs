using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Wizard
{
    public abstract class WizardPageViewModel
    {
        #region Fields

        bool _isCurrentPage;

        readonly string _displayName;

        #endregion // Fields

        #region Constructor

        protected WizardPageViewModel(string displayName)
        {
            _displayName = displayName;
        }

        #endregion // Constructor

        #region Properties

        public string DisplayName
        {
            get { return _displayName; }
        }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set 
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        #endregion // Methods

    }
}
