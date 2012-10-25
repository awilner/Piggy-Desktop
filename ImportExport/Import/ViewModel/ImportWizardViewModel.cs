using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ImportExport.Import
{
    public class ImportWizardViewModel : Wizard.WizardViewModel
    {
        #region Pages

        static internal List<Wizard.WizardPageViewModel> GeneratePageList()
        {
            List<Wizard.WizardPageViewModel> pageList = new List<Wizard.WizardPageViewModel>();
            pageList.Add(new ChooseFilePageViewModel());

            return pageList;
        }

        readonly static ReadOnlyCollection<Wizard.WizardPageViewModel> _pages = new ReadOnlyCollection<Wizard.WizardPageViewModel>(GeneratePageList());
        
        #endregion // Pages

        #region Constructor

        public ImportWizardViewModel() : base(Resources.Strings.ImportWizard, _pages)
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
