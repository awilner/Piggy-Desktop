CREATE TABLE Currency (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  symbol VARCHAR(3) UNIQUE NOT NULL,
  name VARCHAR(40) UNIQUE NOT NULL
);
CREATE TABLE FinancialInstitution (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  name VARCHAR(40) UNIQUE NOT NULL,
  ofx_bankid VARCHAR(40) NULL
);
CREATE TABLE AccountType (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  name VARCHAR(40) UNIQUE NOT NULL,
  ofx_type VARCHAR(20) NULL,
  CHECK (ofx_type IN ('CHECKING','SAVINGS','MONEYMRKT','CREDITLINE'))
);
CREATE TABLE Account (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  name VARCHAR(40) UNIQUE NOT NULL,
  account_type INTEGER NOT NULL,
  financial_institution INTEGER NOT NULL,
  currency INTEGER NOT NULL,
  ofx_acct_id VARCHAR(40) NULL,
  acct_limit NUMERIC(19,4) NULL,
  FOREIGN KEY (account_type) REFERENCES AccountType (id) ON DELETE CASCADE,
  FOREIGN KEY (financial_institution) REFERENCES FinancialInstitution (id) ON DELETE CASCADE,
  FOREIGN KEY (currency) REFERENCES Currency (id) ON DELETE CASCADE
);
CREATE TABLE Balance (
 account INTEGER NOT NULL,
 date DATE NOT NULL,
 balance DECIMAL (19,4) NOT NULL,
 PRIMARY KEY (account,date),
 FOREIGN KEY (account) REFERENCES Account (id) ON DELETE CASCADE
);
CREATE TABLE Category (
 id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
 parent INTEGER DEFAULT NULL,
 name VARCHAR(40) NOT NULL,
 UNIQUE (name,parent) ON CONFLICT ROLLBACK,
 FOREIGN KEY (parent) REFERENCES Category (id) ON DELETE CASCADE
);
CREATE TABLE Preferences (
  default_currency INTEGER NOT NULL,
  FOREIGN KEY (default_currency) REFERENCES Currencies (id) ON DELETE CASCADE
);
CREATE TABLE Transactions (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  from_account INTEGER,
  to_account INTEGER,
  date DATE NOT NULL,
  value DECIMAL (19,4) NOT NULL,
  payee VARCHAR(100),
  category INTEGER,
  memo VARCHAR(255),
  from_ofx_fitid VARCHAR(255),
  to_ofx_fitid VARCHAR(255),
  last_modified TIMESTAMP NOT NULL,
  status CHAR(1) NOT NULL DEFAULT ' ',
  FOREIGN KEY (from_account) REFERENCES Account (id) ON DELETE CASCADE,
  FOREIGN KEY (to_account) REFERENCES Account (id) ON DELETE CASCADE,
  FOREIGN KEY (category) REFERENCES Category (id) ON DELETE CASCADE,
  --CHECK (status IN(' ','CLEARED','RECONCILED','PENDING'))
  CHECK (status IN(' ','C','R','P'))
);
CREATE TRIGGER insert_transaction_from AFTER INSERT ON Transactions
 FOR EACH ROW WHEN NEW.from_account NOT NULL AND NEW.status<>'P' BEGIN

   -- Insert a new balance for the debit account, if needed.
   INSERT OR REPLACE INTO Balance
     SELECT account, curr_date, balance FROM
     (
       SELECT pref, account, curr_date, balance FROM
       (
         SELECT 1 as pref, NEW.date AS curr_date, *
           FROM Balance
         WHERE
           account = NEW.from_account AND date <= NEW.date
         ORDER BY date DESC
         LIMIT 1
       )
       UNION
       SELECT 2 as pref, NEW.from_account, NEW.date, 0
       ORDER BY pref
       LIMIT 1
     );

   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance - NEW.value
     WHERE account = NEW.from_account AND date >= NEW.date;
 END;
CREATE TRIGGER insert_transaction_to AFTER INSERT ON Transactions
 FOR EACH ROW WHEN NEW.to_account NOT NULL AND NEW.status<>'P' BEGIN

   -- Insert a new balance for the credit account, if needed.
   INSERT OR REPLACE INTO Balance
     SELECT account, curr_date, balance FROM
     (
       SELECT pref, account, curr_date, balance FROM
       (
         SELECT 1 as pref, NEW.date AS curr_date, *
           FROM Balance
         WHERE
           account = NEW.to_account AND date <= NEW.date
         ORDER BY date DESC
         LIMIT 1
       )
       UNION
       SELECT 2 as pref, NEW.to_account, NEW.date, 0
       ORDER BY pref
       LIMIT 1
     );

   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance + NEW.value
     WHERE  account = NEW.to_account AND date >= NEW.date;

 END;
CREATE TRIGGER delete_transaction AFTER DELETE ON Transactions
 FOR EACH ROW WHEN OLD.status<>'P' BEGIN

   -- Delete the debit account's balance for the date if the deleted transaction was the only one for the date.
   DELETE FROM Balance
   WHERE
     OLD.from_account NOT NULL
     AND NOT EXISTS (SELECT * FROM Transactions WHERE date = OLD.date AND account = OLD.from_account)
     AND account = OLD.from_account AND date = OLD.date;

   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance + OLD.value
     WHERE account = OLD.from_account AND date >= OLD.date;

   -- Delete the credit account's balance for the date if the deleted transaction was the only one for the date.
   DELETE FROM Balance
   WHERE
     OLD.to_account NOT NULL
     AND NOT EXISTS (SELECT * FROM Transactions WHERE date = OLD.date AND account = OLD.to_account)
     AND account = OLD.to_account AND date = OLD.date;

   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance - OLD.value
     WHERE account = OLD.to_account AND date >= OLD.date;

 END;
CREATE TRIGGER update_transaction_debit_account AFTER UPDATE OF from_account ON Transactions
 -- This trigger is fired when the debit account changes. In this case, we do the same as if deleting
 -- the transaction from the old account and inserting into the new one.
 FOR EACH ROW WHEN OLD.from_account <> NEW.from_account
 BEGIN

   -- Delete the old debit account's balance for the date if the updated transaction was the only one for the date.
   DELETE FROM Balance
   WHERE
     OLD.from_account NOT NULL
     AND NOT EXISTS (SELECT * FROM Transactions WHERE date = OLD.date AND account = OLD.from_account)
     AND account = OLD.from_account AND date = OLD.date;

   -- Now update every balance for the old debit account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance + OLD.value
     WHERE account = OLD.from_account AND date >= OLD.date;

   -- Insert a new balance for the new debit account, if needed.
   INSERT OR ROLLBACK INTO Balance
     SELECT NEW.from_account, NEW.date, balance
     FROM Balance
     WHERE 
       NOT EXISTS (SELECT * FROM Balance WHERE date = NEW.date AND account_id = NEW.from_account)
       AND account_id = NEW.from_account AND date < NEW.date
     ORDER BY date DESC
     LIMIT 1;
 
   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance - NEW.value
     WHERE account_id = NEW.from_account AND date >= NEW.date;

 END;
CREATE TRIGGER update_transaction_credit_account AFTER UPDATE OF to_account ON Transactions
 -- This trigger is fired when the credit account changes. In this case, we do the same as if deleting
 -- the transaction from the old account and inserting into the new one.
 FOR EACH ROW WHEN OLD.to_account <> NEW.to_account
 BEGIN

   -- Delete the old credit account's balance for the date if the updated transaction was the only one for the date.
   DELETE FROM Balance
   WHERE
     OLD.to_account NOT NULL
     AND NOT EXISTS (SELECT * FROM Transactions WHERE date = OLD.date AND account = OLD.to_account)
     AND account = OLD.to_account AND date = OLD.date;

   -- Now update every balance for the old credit account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance - OLD.value
     WHERE account = OLD.to_account AND date >= OLD.date;

   -- Insert a new balance for the new credit account, if needed.
   INSERT OR ROLLBACK INTO Balance
     SELECT NEW.to_account, NEW.date, balance
     FROM Balance
     WHERE 
       NOT EXISTS (SELECT * FROM Balance WHERE date = NEW.date AND account_id = NEW.to_account)
       AND account_id = NEW.to_account AND date < NEW.date
     ORDER BY date DESC
     LIMIT 1;
 
   -- Now update every balance for this account from the current date onward.
   UPDATE OR ROLLBACK Balance SET balance = balance + NEW.value
     WHERE  account_id = NEW.to_account AND date >= NEW.date;

 END;
CREATE TRIGGER update_transaction_debit AFTER UPDATE OF date, value ON Transactions
 -- This trigger is fired when the date and/or value change but the debit account stays the same.
 -- In this case, we might have to insert or remove a balance and update some others in the interval between
 -- the old and the new dates, but it's better than doing a full delete and insert.
 FOR EACH ROW WHEN OLD.from_account = NEW.from_account
 BEGIN

   -- Update the values in the interval between the affected dates.
   UPDATE OR ROLLBACK Balance SET balance = balance +
     -- Define the value to alter based on whether the transaction moved forward or backward in time.
     CASE
       WHEN OLD.date < NEW.date THEN OLD.value
       WHEN OLD.date > NEW.date THEN -NEW.value
       ELSE 0
     END
     WHERE
       OLD.date <> NEW.date       AND
       account = OLD.from_account AND
       date BETWEEN MIN(OLD.date, NEW.date) AND MAX(OLD.date, NEW.date);

   -- If the value changed, update all subsequent balances.
   UPDATE OR ROLLBACK Balance SET balance = balance + OLD.value - NEW.value
     WHERE
       OLD.value <> NEW.value     AND
       account = OLD.from_account AND
       date >= MAX(OLD.date,NEW.date);

 END;
CREATE TRIGGER update_transaction_credit AFTER UPDATE OF date, value ON Transactions
 -- This trigger is fired when the date and/or value change but the credit account stays the same.
 -- In this case, we might have to insert or remove a balance and update some others in the interval between
 -- the old and the new dates, but it's better than doing a full delete and insert.
 FOR EACH ROW WHEN OLD.to_account = NEW.to_account
 BEGIN

   -- Update the values in the interval between the affected dates.
   UPDATE OR ROLLBACK Balance SET balance = balance +
     -- Define the value to alter based on whether the transaction moved forward or backward in time.
     CASE
       WHEN OLD.date < NEW.date THEN -OLD.value
       WHEN OLD.date > NEW.date THEN NEW.value
       ELSE 0
     END
     WHERE
       OLD.date <> NEW.date       AND
       account = OLD.to_account AND
       date BETWEEN MIN(OLD.date, NEW.date) AND MAX(OLD.date, NEW.date);

   -- If the value changed, update all subsequent balances.
   UPDATE OR ROLLBACK Balance SET balance = balance - OLD.value + NEW.value
     WHERE
       OLD.value <> NEW.value     AND
       account = OLD.to_account AND
       date >= MAX(OLD.date,NEW.date);

 END;