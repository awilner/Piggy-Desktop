using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
//using System.Text.RegularExpressions;
//using System.IO;

namespace ImportExport.Import
{
    internal class ImportQifWizardViewModel : Wizard.FileBasedWizardViewModel
    {
        #region Fields;

        protected Parser.QifParser _parser;

        #endregion // Fields

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
            // Parse the file.
            _parser = new Parser.QifParser(filePath);
            _parser.parse();

            Text = _parser.Report;
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
