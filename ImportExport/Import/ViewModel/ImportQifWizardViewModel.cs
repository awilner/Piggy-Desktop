using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;

namespace ImportExport.Import
{
    internal class ImportQifWizardViewModel : Wizard.FileBasedWizardViewModel
    {
        #region Pages

        static private List<Wizard.WizardPageViewModel> GeneratePageList()
        {
            List<Wizard.WizardPageViewModel> pageList = new List<Wizard.WizardPageViewModel>();
            pageList.Add(new ChooseAccountPageViewModel());

            return pageList;
        }

        readonly static private ReadOnlyCollection<Wizard.WizardPageViewModel> _pages = new ReadOnlyCollection<Wizard.WizardPageViewModel>(GeneratePageList());
        
        #endregion // Pages

        #region Constructor

        public ImportQifWizardViewModel(string filePath) : base(Resources.Strings.ImportWizard, _pages, filePath)
        {
            // Parse the file before continuing.
            parse();
        }

        #endregion // Constructor

        #region Methods

        protected override void ApplyChanges()
        {
        }

        private void parse()
        {
            // First we try to load the file.
            StreamReader reader = new StreamReader(_file);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Text += line;
            }
            
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
