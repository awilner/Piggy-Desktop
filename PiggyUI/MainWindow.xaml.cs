﻿using System;
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
            //PiggyDB.PiggyContext context = new PiggyDB.PiggyContext();
            //var query = from c in context.Currencies select c;
            //transactionsDataGrid.ItemsSource = query;
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
            ImportExport.Wizard.WizardDialog importDialog = new ImportExport.Wizard.WizardDialog(new ImportExport.Import.ImportWizardViewModel());
            importDialog.ShowDialog();
            e.Handled = true;
        }

        #endregion // Import

        #endregion // Commands
    }
}
