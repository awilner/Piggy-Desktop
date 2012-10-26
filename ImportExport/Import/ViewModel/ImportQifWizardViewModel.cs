using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ImportExport.Import
{
    internal class ImportQifWizardViewModel : Wizard.FileBasedWizardViewModel
    {
        #region Pages

        static private List<Wizard.WizardPageViewModel> GeneratePageList()
        {
            List<Wizard.WizardPageViewModel> pageList = new List<Wizard.WizardPageViewModel>();
            pageList.Add(new ChooseFilePageViewModel());

            return pageList;
        }

        readonly static private ReadOnlyCollection<Wizard.WizardPageViewModel> _pages = new ReadOnlyCollection<Wizard.WizardPageViewModel>(GeneratePageList());
        
        #endregion // Pages

        #region Constructor

        public ImportQifWizardViewModel(string filePath) : base(Resources.Strings.ImportWizard, _pages, filePath)
        {
        }

        #endregion // Constructor

        #region Methods

        protected override void ApplyChanges()
        {
        }

        #endregion // Methods
    }

    internal class ImportQifWizardViewModelFactory : Wizard.FileBasedWizardViewModelFactory
    {

        internal override Wizard.FileBasedWizardViewModel CreateWizardViewModel(string filePath)
        {
            return new ImportQifWizardViewModel(filePath);
        }
    }
}
