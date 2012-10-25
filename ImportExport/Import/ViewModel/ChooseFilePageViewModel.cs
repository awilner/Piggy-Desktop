using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Import
{
    class ChooseFilePageViewModel : Wizard.WizardPageViewModel
    {
        #region Constructor

        public ChooseFilePageViewModel() : base( Resources.Strings.ChooseFile )
        {
        }

        #endregion // Constructor

        internal override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
