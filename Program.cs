namespace BankingApplication
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastInterestDate { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class Transaction
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
    class Program
    {
        static List<User> users = new List<User>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Banking Application ---");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser();
                        break;
                    case "2":
                        User loggedInUser = Login();
                        if (loggedInUser != null)
                        {
                            UserMenu(loggedInUser);
                        }
                        break;
                    case "3":
                        Console.WriteLine("Thank you for using the banking application. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void RegisterUser()
        {
            Console.WriteLine("--- User Registration ---");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Please choose a different username.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (!PasswordStrengthCheck(password))
            {
                Console.WriteLine("Password is not secure. It must be at least 8 characters long and include:");
                Console.WriteLine("- At least one lowercase letter");
                Console.WriteLine("- At least one uppercase letter");
                Console.WriteLine("- At least one number");
                Console.WriteLine("- At least one special character");
                return;
            }

            users.Add(new User { Username = username, Password = password });
            Console.WriteLine("Registration successful! You can now log in.");
        }

        static bool PasswordStrengthCheck(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasLowercase = false;
            bool hasUppercase = false;
            bool hasNumber = false;
            bool hasSpecialChar = false;

            foreach (char c in password)
            {
                if (char.IsLower(c))
                    hasLowercase = true;
                else if (char.IsUpper(c))
                    hasUppercase = true;
                else if (char.IsDigit(c))
                    hasNumber = true;
                else if (!char.IsLetterOrDigit(c)) 
                    hasSpecialChar = true;
            }
            return hasLowercase && hasUppercase && hasNumber && hasSpecialChar;
        }


        static User Login()
        {
            Console.WriteLine("--- User Login ---");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Console.WriteLine("Login successful!");
                return user;
            }
            else
            {
                Console.WriteLine("Invalid username or password. Please try again.");
                return null;
            }
        }

        static void UserMenu(User user)
        {
            while (true)
            {
                Console.WriteLine($"\n--- Welcome, {user.Username} ---");
                Console.WriteLine("1. Open Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. View Statement");
                Console.WriteLine("5. Add Monthly Interest (Savings Only)");
                Console.WriteLine("6. Check Balance");
                Console.WriteLine($"7. List {user.Username}'s accounts");
                Console.WriteLine("8. Logout");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        OpenAccount(user);
                        break;
                    case "2":
                        ProcessTransaction(user, "Deposit");
                        break;
                    case "3":
                        ProcessTransaction(user, "Withdrawal");
                        break;
                    case "4":
                        ViewStatement(user);
                        break;
                    case "5":
                        AddMonthlyInterest(user);
                        break;
                    case "6":
                        CheckBalance(user);
                        break;
                    case "7":
                        ListAccounts(user);
                        break;
                    case "8":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void OpenAccount(User user)
        {
            Console.WriteLine("--- Open New Account ---");
            Console.Write("Enter account holder's name: ");
            string holderName = Console.ReadLine();
            Console.Write("Enter account type (1. Savings/ 2. Checking): ");
            string accountType = Console.ReadLine().Trim();

            switch (accountType)
            {
                case "1":
                    accountType = "Savings";
                    break;
                case "2":
                    accountType = "Checking";
                    break;
                default:
                    Console.WriteLine("Invalid account type. Please enter either 'Savings' or 'Checking'.");
                    return;
            }



            Console.Write("Enter initial deposit amount: ");
            string input = Console.ReadLine();
            decimal initialDeposit;
            bool isValidDeposit = decimal.TryParse(input, out initialDeposit);
            if (!isValidDeposit || initialDeposit < 0)
            {
                Console.WriteLine("Invalid deposit amount. Please enter a valid number.");
                return;
            }

            string accountNumber = "ACC" + new Random().Next(1000000, 9999999);
            Account newAccount = new Account
            {
                AccountNumber = accountNumber,
                AccountHolderName = holderName,
                AccountType = accountType,
                Balance = initialDeposit,
                LastInterestDate = DateTime.MinValue
            };

            user.Accounts.Add(newAccount);
            Console.WriteLine($"Account created successfully! Your account number is: {accountNumber}");
        }

        static void ProcessTransaction(User user, string transactionType)
        {
            Console.WriteLine($"--- {transactionType} ---");
            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine();

            Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found. Please check the account number.");
                return;
            }

            Console.Write($"Enter amount to {transactionType.ToLower()}: ");
            decimal amount;
            if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a valid number.");
                return;
            }

            if (transactionType == "Withdrawal" && amount > account.Balance)
            {
                Console.WriteLine("Insufficient funds. Withdrawal amount exceeds the current balance.");
                return;
            }

            if (transactionType == "Deposit")
            {
                account.Balance += amount;
            }
            else if (transactionType == "Withdrawal")
            {
                account.Balance -= amount;
            }

            account.Transactions.Add(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Type = transactionType,
                Amount = amount
            });

            Console.WriteLine($"{transactionType} successful! Your new balance is: Rs.{account.Balance}");
        }

        static void ViewStatement(User user)
        {
            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine();

            Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found. Please check the account number.");
                return;
            }

            Console.WriteLine($"\n--- Transaction History for Account {accountNumber} ---");
            foreach (var transaction in account.Transactions)
            {
                Console.WriteLine($"{transaction.Date} | {transaction.Type} | Amount: Rs.{transaction.Amount}");
            }

            if (account.Transactions.Count == 0)
            {
                Console.WriteLine("No transactions found for this account.");
            }
        }

        static void AddMonthlyInterest(User user)
        {
            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine();

            Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber && a.AccountType.ToLower() == "savings");
            if (account == null)
            {
                Console.WriteLine("Savings account not found or the account type is not a savings account.");
                return;
            }

            if (DateTime.Now.Subtract(account.LastInterestDate).Days < 30)
            {
                Console.WriteLine("Interest can only be added once per month. Please try again later.");
                return;
            }

            decimal interestRate = 0.01m; 
            decimal interest = account.Balance * interestRate;
            account.Balance += interest;
            account.LastInterestDate = DateTime.Now;

            account.Transactions.Add(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Type = "Interest",
                Amount = interest
            });

            Console.WriteLine($"Interest of Rs.{interest} added successfully! New balance: Rs.{account.Balance}");
        }

        static void CheckBalance(User user)
        {
            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine();

            Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found. Please check the account number.");
                return;
            }

            Console.WriteLine($"The current balance for account {accountNumber} is: Rs.{account.Balance}");

        }

        static void ListAccounts(User user)
        {
            if (user.Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts.");
                return;
            }

            Console.WriteLine("\n--- Your Accounts ---");
            foreach (var account in user.Accounts)
            {
                Console.WriteLine($"Account Number: {account.AccountNumber}, Type: {account.AccountType}, Balance: Rs.{account.Balance}");
            }
        }
    }


}
