using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ImportExport.Import
{
    public class ImportWizardViewModel : Wizard.WizardViewModel
    {
        readonly static ReadOnlyCollection<Wizard.WizardPageViewModel> pages = new ReadOnlyCollection<Wizard.WizardPageViewModel>(new List<Wizard.WizardPageViewModel>());
        #region Constructor

        public ImportWizardViewModel() : base(pages)
        {
        }

        #endregion // Constructor

        #region Methods

        protected override void ApplyChanges()
        {
        }

        #endregion // Methods

    }
}
