using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class Account
{
    public string AccountNumber { get; set; }
    public string HolderName { get; set; }
    public string AccountType { get; set; }
    public decimal Balance { get; set; }
    private List<Transaction> Transactions { get; set; } = new List<Transaction>();

    public Account(string accountNumber, string holderName, string type, decimal initialDeposit)
    {
        AccountNumber = accountNumber;
        HolderName = holderName;
        AccountType = type;
        Balance = initialDeposit;
    }
    public void Deposit(decimal amount)
    {
        Balance += amount;
        Transactions.Add(new Transaction("Deposit", amount));
    }
    public bool Withdraw(decimal amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            Transactions.Add(new Transaction("Withdraw", amount));
            return true;
        }
        Console.WriteLine("Not enough balance.");
        return false;
    }
    public void GenerateStatement()
    {
        Console.WriteLine($"Statement for Account: {AccountNumber}");
        Console.WriteLine("Date\t\tType\tAmount");
        Console.WriteLine("-------------------------------");
        foreach (var transaction in Transactions)
        {
            Console.WriteLine($"{transaction.Date.ToShortDateString()}\t{transaction.Type}\t{transaction.Amount}");
        }
    }

}

public class Transaction
{
    public string TransactionId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }

    public Transaction(string type, decimal amount)
    {
        TransactionId = Guid.NewGuid().ToString();
        Date = DateTime.Now;
        Type = type;
        Amount = amount;
    }
}

public class BankingSystem
{
    private List<User> users = new List<User>();

    public void RegisterUser()
    {
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine();

        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Please try a different one.");
            return;
        }

        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        User newUser = new User(username, password);
        users.Add(newUser);
        Console.WriteLine($"User {username} registered successfully!");
    }

    public User LoginUser()
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine();

        User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            Console.WriteLine($"Welcome, {user.Username}!");
            return user;
        }
        else
        {
            Console.WriteLine("Invalid credentials.");
            return null;
        }
    }

    public void CreateAccount(User user)
    {
        Console.WriteLine("Enter the account number:");
        string accountNumber = Console.ReadLine();
        Console.WriteLine("Enter account type (e.g., Savings, Checking):");
        string accountType = Console.ReadLine();
        Console.WriteLine("Enter initial deposit amount:");
        decimal initialDeposit = decimal.Parse(Console.ReadLine());

        Account newAccount = new Account(accountNumber, user.Username, accountType, initialDeposit);
        user.Accounts.Add(newAccount);
        Console.WriteLine("Account created successfully!");
    }

    public void DepositToAccount(User user)
    {
        Console.WriteLine("Enter account number:");
        string accountNumber = Console.ReadLine();

        Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return;
        }

        Console.WriteLine("Enter deposit amount:");
        decimal amount = decimal.Parse(Console.ReadLine());
        account.Deposit(amount);
        Console.WriteLine("Deposit successful!");
    }

    public void WithdrawFromAccount(User user)
    {
        Console.WriteLine("Enter account number:");
        string accountNumber = Console.ReadLine();

        Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return;
        }

        Console.WriteLine("Enter withdrawal amount:");
        decimal amount = decimal.Parse(Console.ReadLine());
        if (account.Withdraw(amount))
        {
            Console.WriteLine("Withdrawal successful!");
        }
    }

    public void ViewBalance(User user)
    {
        Console.WriteLine("Enter account number:");
        string accountNumber = Console.ReadLine();

        Account account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            Console.WriteLine("Account not found.");
        }
        else
        {
            Console.WriteLine($"Account Balance: {account.Balance}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        BankingSystem bankingSystem = new BankingSystem();
        User currentUser = null;

        while (true)
        {
            Console.WriteLine("\n--- Banking Application ---");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    bankingSystem.RegisterUser();
                    break;
                case "2":
                    currentUser = bankingSystem.LoginUser();
                    if (currentUser != null)
                    {
                        bool loggedIn = true;
                        while (loggedIn)
                        {
                            Console.WriteLine("\n1. Create Account");
                            Console.WriteLine("2. Deposit");
                            Console.WriteLine("3. Withdraw");
                            Console.WriteLine("4. View Statement");
                            Console.WriteLine("5. View Balance");
                            Console.WriteLine("6. Logout");
                            Console.Write("Choose an option: ");
                            string userOption = Console.ReadLine();

                            switch (userOption)
                            {
                                case "1":
                                    bankingSystem.CreateAccount(currentUser);
                                    break;
                                case "2":
                                    bankingSystem.DepositToAccount(currentUser);
                                    break;
                                case "3":
                                    bankingSystem.WithdrawFromAccount(currentUser);
                                    break;
                                case "4":
                                    if(currentUser.Accounts.Count == 0)
                                    {
                                        Console.WriteLine("No accounts found");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Select an account to view the statement:");
                                        for (int i = 0; i < currentUser.Accounts.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. Account Number: {currentUser.Accounts[i].AccountNumber}");
                                        }
                                        int accountChoice;
                                        if (int.TryParse(Console.ReadLine(), out accountChoice) && accountChoice > 0 && accountChoice <= currentUser.Accounts.Count)
                                        {
                                            var selectedAccount = currentUser.Accounts[accountChoice - 1];
                                            selectedAccount.GenerateStatement();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid account selection.");
                                        }
                                    }
                                    break;
                                case "5":
                                    bankingSystem.ViewBalance(currentUser);
                                    break;
                                case "6":
                                    loggedIn = false;
                                    currentUser = null;
                                    Console.WriteLine("Logged out.");
                                    break;
                                default:
                                    Console.WriteLine("Invalid option.");
                                    break;
                            }
                        }
                    }
                    break;
                case "3":
                    Console.WriteLine("Exiting application.");
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
