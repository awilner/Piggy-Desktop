using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace PiggyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<PiggyBL.Account> accounts = PiggyBL.PiggyManager.Instance.GetAccountList();
            foreach(PiggyBL.Account account in accounts)
            {
                accountsTreeView.Items.Add(account.Name);
            }
        }

        #region Commands

        #region Import

        /// <summary>
        /// This is the ICommand object that routes the import command through to this object's methods.
        /// </summary>
        public static RoutedCommand ImportCmd = new RoutedCommand();

        /// <summary>
        /// Import can occur at any time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Creates an import wizard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "All Data Sources (*.qif, *.ofx, *.csv, *.txt, *.xml)|*.qif;*.ofx;*.csv;*.txt*;*.xml|All Files|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // We receive an array of paths, but it must contain only one file.
                string[] filePathList = openFileDialog.FileNames;
                string filePath = filePathList[0];

                ImportExport.Wizard.WizardDialog importDialog = ImportExport.Import.ImportWizardFactory.Instance.CreateWizard(filePath);
                importDialog.ShowDialog();
            }
            e.Handled = true;
        }

        #endregion Import

        #endregion Commands
    }
}
