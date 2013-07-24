using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiggyBL
{
    public class PiggyManager
    {
        #region Fields
        private PiggyDB.PiggyContext _context;

        private static PiggyManager _Instance;
        #endregion

        #region Properties
        public static PiggyManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PiggyManager();
                }

                return _Instance;
            }
        }
        #endregion

        #region Constructor
        private PiggyManager()
        {
            _context = new PiggyDB.PiggyContext();
        }
        #endregion

        #region Methods
        public List<Account> GetAccountList()
        {
            List<Account> list = new List<Account>();
            System.Data.Objects.ObjectSet<PiggyDB.Account> accounts = _context.Accounts;
            foreach (PiggyDB.Account account in accounts)
            {
                list.Add(new Account(account.id, account.name));
            }
            return list;
        }
        #endregion
    }
}
