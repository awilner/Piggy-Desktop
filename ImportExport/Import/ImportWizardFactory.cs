using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Import
{
    /// <summary>
    /// Wizard factory for import wizards.
    /// </summary>
    public sealed class ImportWizardFactory : Wizard.FileBasedWizardFactory
    {
        #region Fields

        static readonly ImportWizardFactory instance = new ImportWizardFactory();

        #endregion Fields

        #region Constructor

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ImportWizardFactory()
        {
        }

        ImportWizardFactory()
        {
            // Register import wizards.
            RegisterWizard(".qif", new ImportQifWizardViewModelFactory());
        }

        #endregion Constructor

        #region Properties

        public static ImportWizardFactory Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion Properties
    }
}
