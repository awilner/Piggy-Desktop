﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Import
{
    class ChooseAccountPageViewModel : Wizard.WizardPageViewModel
    {
        #region Constructor

        public ChooseAccountPageViewModel() : base( Resources.Strings.ChooseAccount )
        {
        }

        #endregion // Constructor

        #region Properties

        internal override bool IsValid()
        {
            throw new NotImplementedException();
        }

        #endregion Properties
    }
}
