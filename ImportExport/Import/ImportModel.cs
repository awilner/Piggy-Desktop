using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportExport.Import
{
    /// <summary>
    /// Class that holds the data being processed during import.
    /// </summary>
    class ImportModel
    {
        #region Fields

        private Dictionary<string, PiggyDB.Account> _accounts = new Dictionary<string, PiggyDB.Account>();
        private List<PiggyDB.Category> _categories = new List<PiggyDB.Category>();
        private Dictionary<PiggyDB.Account, List<PiggyDB.Transaction>> _transactions = new Dictionary<PiggyDB.Account, List<PiggyDB.Transaction>>();
        private List<string> _tags = new List<string>();
        private List<string> _securities = new List<string>();
        private List<string> _memorized = new List<string>();
        private List<string> _prices = new List<string>();

        #endregion Fields

        #region Properties

        public Dictionary<string, PiggyDB.Account> Accounts
        {
            get
            {
                return _accounts;
            }
        }

        public List<PiggyDB.Category> Categories
        {
            get
            {
                return _categories;
            }
        }

        public Dictionary<PiggyDB.Account, List<PiggyDB.Transaction>> Transactions
        {
            get
            {
                return _transactions;
            }
        }

        public List<string> Tags
        {
            get
            {
                return _tags;
            }
        }

        public List<string> Securities
        {
            get
            {
                return _securities;
            }
        }

        public List<string> Memorized
        {
            get
            {
                return _memorized;
            }
        }

        public List<string> Prices
        {
            get
            {
                return _prices;
            }
        }

        #endregion Properites
    }
}
