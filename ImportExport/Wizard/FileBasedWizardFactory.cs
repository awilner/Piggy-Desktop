using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Wizard
{
    /// <summary>
    /// Factory that produces wizards whose constructor takes a file path as parameter.
    /// For now this is used only for import/export.
    /// The file extension determines the wizard that will be created, and the wizard ViewModel
    /// will be created with the file path as parameter.
    /// </summary>
    /// <remarks>
    /// Ideally, derived classes should be singletons, and no extra implementation should be needed
    /// (apart from the singleton implemebntation, that is).
    /// This should serve to group similar wizards, for example, grouping import wizards for various
    /// file types under one factory, and export wizards for the same types under another factory.
    /// </remarks>
    public abstract class FileBasedWizardFactory
    {
        #region WizardList

        private Dictionary<string, FileBasedWizardViewModelFactory> WizardList = new Dictionary<string,FileBasedWizardViewModelFactory>();

        internal void RegisterWizard(string fileType, FileBasedWizardViewModelFactory factory)
        {
            WizardList[fileType] = factory;
        }
        
        #endregion // WizardList

        /// <summary>
        /// Creates a wizard based on the file type. The file type determines the wizard ViewModel
        /// to be created, and the ViewModel takes the file path as parameter during construction.
        /// </summary>
        /// <param name="filePath">Path of the file for which the wizard is being created.</param>
        /// <returns>A wizard dialog with the proper wizard ViewModel set as data context.</returns>
        public WizardDialog CreateWizard(string filePath)
        {
            // Determine the file extension.
            string fileType = System.IO.Path.GetExtension(filePath).ToLower();

            // Retrieve the proper WizardViewModel factory.
            // This throws if no factory exists for the given extension.
            FileBasedWizardViewModelFactory factory = WizardList[fileType];

            // Create the ViewModel.
            FileBasedWizardViewModel viewModel = factory.CreateWizardViewModel(filePath);

            // Finally create the dialog.
            return new WizardDialog(viewModel);
        }
    }
}
