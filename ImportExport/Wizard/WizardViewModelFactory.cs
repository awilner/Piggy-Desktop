using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Wizard
{
    abstract class FileBasedWizardViewModelFactory
    {
        abstract internal FileBasedWizardViewModel CreateWizardViewModel(string filePath);
    }
}
