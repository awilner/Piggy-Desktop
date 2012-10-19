//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Windows.Input;

namespace ImportExport
{
    /// <summary>
    /// Class that handles all data imports in a wizard fashion.
    /// </summary>
    public class ImportWizardView
    {
        /// <summary>
        /// This is the ICommand object that routes the import command through to this object's methods.
        /// </summary>
        public static RoutedCommand ImportCmd = new RoutedCommand();

        public ImportWizardView()
        {
            int test = 0;
        }
    }
}
