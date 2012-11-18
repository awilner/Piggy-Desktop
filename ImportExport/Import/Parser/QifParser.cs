using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;

namespace ImportExport.Import.Parser
{
    class QifParser
    {
        #region Fields;

        private string _file;

        private string _line;

        private StreamReader _reader;

        private Dictionary<string, PiggyDB.Account> _accounts = new Dictionary<string,PiggyDB.Account>();
        private List<PiggyDB.Category> _categories = new List<PiggyDB.Category>();
        private Dictionary<PiggyDB.Account,List<PiggyDB.Transaction>> _transactions = new Dictionary<PiggyDB.Account,List<PiggyDB.Transaction>>();
        private List<string> _tags = new List<string>();
        private List<string> _securities = new List<string>();
        private List<string> _memorized = new List<string>();
        private List<string> _prices = new List<string>();
        private List<string> _messages = new List<string>();

        private string _currentAccount;
        private bool _autoSwitch;

        #endregion // Fields

        #region Properties

        public string Report
        {
            get
            {
                string report = "";

                report += _accounts.Count + " accounts\n";
                report += _categories.Count + " categories\n";
                report += _transactions.Count + " transactions\n";
                report += _tags.Count + " tags\n";
                report += _securities.Count + " securities\n";
                report += _memorized.Count + " memorized\n";
                report += _prices.Count + " prices\n";
                report += "-------\nMessages:\n\n";
                foreach (string message in _messages)
                    report += message + "\n";

                return report;
            }
        }

        #endregion Properties

        #region Constructor

        public QifParser(string filePath)
        {
            _file = filePath;

            // First we try to load the file.
            _reader = new StreamReader(_file);
        }

        #endregion // Constructor

        #region Methods

        public void parse()
        {
            _line = _reader.ReadLine();
            while (_line != null)
            {
                switch (_line.Trim())
                {
                    case "!Type:Tag":
                        parseTags();
                        break;

                    case "!Type:Cat":
                        parseCategories();
                        break;

                    case "!Option:AutoSwitch":
                        _autoSwitch = true;
                        _line = _reader.ReadLine();
                        break;

                    case "!Clear:AutoSwitch":
                        _autoSwitch = false;
                        _currentAccount = null;
                        _line = _reader.ReadLine();
                        break;

                    case "!Account":
                        parseAccounts();
                        break;

                    case "!Type:Security":
                        parseSecurities();
                        break;

                    case "!Type:Bank":
                    case "!Type:CCard":
                    case "!Type:Oth":
                    case "!Type:Oth L":
                    case "!Type:Oth A":
                    case "!Type:Invst":
                    case "!Type:Cash":
                        parseTransactions();
                        break;

                    case "!Type:Memorized":
                    case "!Type:Prices":
                        // Ignored for now.
                        _line = _reader.ReadLine();
                        while (_line != null && _line[0] != '!')
                            _line = _reader.ReadLine();
                        break;

                    default:
                        // Unknown tag. Log and skip.
                        _messages.Add("Unknown tag \"" + _line + "\"");
                        _line = _reader.ReadLine();
                        break;
                }
            }

        }

        private void parseTransactions()
        {
            PiggyDB.Transaction transaction = new PiggyDB.Transaction();
            PiggyDB.Account currentAccount = _accounts[_currentAccount];
            while ((_line = _reader.ReadLine()) != null)
            {
                switch (_line[0])
                {
                    case 'D':
                        // Transaction date.
                        transaction.date = parseDate(_line.Substring(1));
                        break;

                    case 'T':
                        // Ammount.
                        transaction.value = parseMoney(_line.Substring(1));
                        break;

                    case 'P':
                        // Payee.
                        transaction.payee = _line.Substring(1);
                        break;

                    case 'M':
                    case 'E': // Memo for split.
                        // Memo
                        transaction.memo = _line.Substring(1);
                        break;

                    case 'L':
                    case 'S': // Split
                        // Category (or transfer)
                        break;

                    case 'C': // Cleared status
                        break;

                    case 'I': // Share price (for security transactions).
                    case 'Y': // Security name.
                    case 'Q': // Quantity of shares.
                    case 'O': // Commission cost.
                    case 'N': // Check number or operation type.
                    case 'U': // Don't know what it is, but seems to repeat T (value).
                    case 'A': // Address, 5 lines max.
                    case '$': // Value for split.
                        // TODO: ignored for now.
                        break;

                    case '^':
                        // End of record. Store the current transaction and start a new one.
                        if (!_transactions.ContainsKey(currentAccount))
                            _transactions[currentAccount] = new List<PiggyDB.Transaction>();

                        _transactions[currentAccount].Add(transaction);
                        transaction = new PiggyDB.Transaction();
                        break;

                    case '!':
                        // We're done, there's another section coming up.
                        return;

                    default:
                        // Unknown field.
                        _messages.Add("Parsing transactions - unknown field. Line is \"" + _line);
                        break;

                }
            }
       }

        private decimal parseMoney(string value)
        {
            // First detect if the decimal mark is a point or a comma (QIF always has decimal marks).
            if (Regex.IsMatch(value, @"\.\d\d$"))
            {
                // The decimal mark is a point. Strip everything that is not a point, a digit or a signal.
                return decimal.Parse(Regex.Replace(value, @"[^\-\+\d\.]", ""));
            }
            else if (Regex.IsMatch(value, @",\d\d$"))
            {
                // The decimal mark is a comma. Strip everything that is not a comma, a digit or a signal.
                return decimal.Parse(Regex.Replace(value, @"[^\-\+\d,]", ""));
            }
            else
            {
                // No decimal mark, just strip everything that is not a digit or signal.
                return decimal.Parse(Regex.Replace(value, @"[^\-\+\d]", ""));
            }

        }

        private DateTime parseDate(string value)
        {
            // Use a regexp to parse QIF's bizarre format.
            Match match = Regex.Match(value, @"(\d\d?)\/\s?(\d\d?)'\s?(\d+)");
            if (match.Success)
            {
                int day = int.Parse(match.Groups[2].Value);
                int month = int.Parse(match.Groups[1].Value);
                int year = int.Parse(match.Groups[3].Value);

                // Adjust year, if the year is given in two digits.
                if (year < 70)
                    year += 2000;
                else if (year < 100)
                    year += 1900;

                return new DateTime(year, month, day);
            }

            return new DateTime();
        }

        private void parseSecurities()
        {
            // Securities are ignored for now.
            while ((_line = _reader.ReadLine()) != null)
            {
                switch (_line[0])
                {
                    case 'N':
                        // The name of the security.
                        //name = _line.Substring(1);
                        break;

                    case 'S':
                        // Security symbol - ignored for now.
                        break;

                    case 'T':
                        // Security type (mutual fund, stock,market index etc).
                        break;

                    case '^':
                        // End of record. Store the current security and start a new one.
                        break;

                    case '!':
                        // We're done, there's another section coming up.
                        return;

                    default:
                        // Unknown field.
                        _messages.Add("Parsing securities - unknown field. Line is \"" + _line);
                        break;

                }
            }
        }

        private void parseAccounts()
        {
            PiggyDB.Account account = new PiggyDB.Account();
            while ((_line = _reader.ReadLine()) != null)
            {
                switch (_line[0])
                {
                    case 'N':
                        // The name of the account.
                        account.name = _line.Substring(1);
                        break;

                    case 'D':
                        // Account description - ignored for now.
                        break;

                    case 'T':
                        // Account type.
                        break;

                    case 'L':
                        // Account limit - ignored for now.
                        break;

                    case '^':
                        // End of record. Store the current account and start a new one.
                        // Check if there already is an account with this name in the parse results.
                        if (!_accounts.ContainsKey(account.name))
                        {
                            _accounts.Add(account.name, account);
                        }

                        if (_autoSwitch)
                            _currentAccount = account.name;

                        // Create new account.
                        account = new PiggyDB.Account();
                        break;

                    case '!':
                        // We're done, there's another section coming up.
                        return;

                    default:
                        // Unknown field.
                        _messages.Add("Parsing accounts - unknown field. Line is \"" + _line);
                        break;

                }
            }
        }

        private void parseCategories()
        {
            PiggyDB.Category category = new PiggyDB.Category();
            while ((_line = _reader.ReadLine()) != null)
            {
                switch (_line[0])
                {
                    case 'N':
                        // The name of the category. We still need to parse the name for subcategories.
                        category.name = _line.Substring(1);
                        break;

                    case 'D':
                        // Category description - ignored for now.
                        break;

                    case 'T':
                        // Indicates taxable - ignored for now.
                        break;

                    case 'R':
                        // Indicates the code used for the American IRS - ignored for now.
                        break;

                    case 'I':
                        // Indicates income category - ignored for now.
                        break;

                    case 'E':
                        // Indicates expense category - ignored for now.
                        break;

                    case 'B':
                        // Budget set for the category - ignored for now.
                        break;

                    case '^':
                        // End of record. Store the current category and start a new one.
                        _categories.Add(category);
                        category = new PiggyDB.Category();
                        break;

                    case '!':
                        // We're done, there's another section coming up.
                        return;

                    default:
                        // Unknown field.
                        _messages.Add("Parsing categories - unknown field. Line is \"" + _line);
                        break;

                }
            }
        }

        private void parseTags()
        {
            while ((_line = _reader.ReadLine()) != null)
            {
                switch (_line[0])
                {
                    case 'N':
                        // The name of the tag.
                        _tags.Add(_line.Substring(1));
                        break;

                    case '^':
                        // End of record.
                        break;

                    case '!':
                        // We're done, there's another section coming up.
                        return;

                    default:
                        // Unknown field.
                        _messages.Add("Parsing tags - unknown field. Line is \"" + _line);
                        break;

                }
            }
        }

        #endregion Methods
    }
}
