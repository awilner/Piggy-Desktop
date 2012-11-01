using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ImportExport.Wizard
{
    /// <summary>
    /// Wizard ViewModel that expects a file as constructor parameter.
    /// </summary>
    abstract class FileBasedWizardViewModel : WizardViewModel
    {
        #region Fields;

        protected string _file;

        #endregion // Fields

        #region Constructor

        internal FileBasedWizardViewModel(string wizardName, ReadOnlyCollection<WizardPageViewModel> pages, string filePath)
            : base(wizardName, pages)
        {
            _file = filePath;
        }

        #endregion // Constructor
    }
}
