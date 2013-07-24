using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiggyBL
{
    /// <summary>
    /// Object that represents an account of any kind (checking, cash, savings, investment, etc).
    /// </summary>
    public class Account
    {
        #region Fields

        /// <summary>
        /// Unique id of the account.
        /// </summary>
        private readonly long _id;

        /// <summary>
        /// Unique name of the account.
        /// </summary>
        private string _name;

        /// <summary>
        /// Financial institution where the account is located.
        /// </summary>
        private FinancialInstitution _institution;

        /// <summary>
        /// Unique OFX acct_id
        /// </summary>
        private string _ofxId;

        private double _currentBalance;
        private double _finalBalance;

        #endregion

        #region Properties
        double CurrentBalance
        {
            get
            {
                return _currentBalance;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        #endregion

        #region Constructor
        public Account(long id, string name)
        {
            _id = id;
            _name = name;
        }
        #endregion
        
        #region Methods
        #endregion

    }
}
