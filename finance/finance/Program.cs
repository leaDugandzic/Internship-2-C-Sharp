using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Dictionary<int, (string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts)> users = new();
    static int userIdCounter = 1;
    static int transactionIdCounter = 1;
    static void Main(string[] args)
    {
        InitializeData();
        while (true)
        {
            ShowMainMenu();
        }
    }

    static void InitializeData()
    {
        AddUser("Ivan", "Ivić", new DateTime(1990, 5, 15));
        AddUser("Ana", "Anić", new DateTime(1985, 3, 10));
    }

    static void ShowMainMenu()
    {
        Console.WriteLine("\nGlavni izbornik:");
        Console.WriteLine("1 - Korisnici");
        Console.WriteLine("2 - Računi");
        Console.WriteLine("0 - Izlaz");
        Console.Write("Odabir: ");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ManageUsers();
                break;
            case "2":
                ManageAccounts();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Neispravan unos. Pokušajte ponovo.");
                break;
        }
    }

    static void ManageUsers()
    {
        while (true)
        {
            Console.WriteLine("\nUpravljanje korisnicima:");
            Console.WriteLine("1 - Unos novog korisnika");
            Console.WriteLine("2 - Brisanje korisnika");
            Console.WriteLine("3 - Pregled korisnika");
            Console.WriteLine("4 - Posebni pregledi korisnika");
            Console.WriteLine("0 - Povratak na glavni izbornik");
            Console.Write("Odabir: ");

            var choice = Console.ReadLine();
            if (choice == "0") return;

            switch (choice)
            {
                case "1":
                    Console.Write("Ime: ");
                    var firstName = Console.ReadLine();
                    Console.Write("Prezime: ");
                    var lastName = Console.ReadLine();
                    Console.Write("Datum rođenja (yyyy-mm-dd): ");
                    if (DateTime.TryParse(Console.ReadLine(), out var birthDate))
                    {
                        AddUser(firstName, lastName, birthDate);
                        Console.WriteLine("Korisnik uspješno dodan.");
                    }
                    else
                    {
                        Console.WriteLine("Neispravan datum. Pokušajte ponovo.");
                    }
                    break;

                case "2":
                    Console.Write("Unesite ID korisnika za brisanje: ");
                    if (int.TryParse(Console.ReadLine(), out var idToDelete) && users.ContainsKey(idToDelete))
                    {
                        users.Remove(idToDelete);
                        Console.WriteLine("Korisnik uspješno obrisan.");
                    }
                    else
                    {
                        Console.WriteLine("Korisnik s unesenim ID-om ne postoji.");
                    }
                    break;

                case "3":
                    foreach (var user in users.OrderBy(u => u.Value.LastName))
                    {
                        Console.WriteLine($"ID: {user.Key}, Ime: {user.Value.FirstName}, Prezime: {user.Value.LastName}, Datum rođenja: {user.Value.BirthDate.ToShortDateString()}");
                    }
                    break;

                case "4":
                    Console.WriteLine("1 - Korisnici stariji od 30 godina");
                    Console.WriteLine("2 - Korisnici s računima u minusu");
                    Console.WriteLine("0 - Povratak");
                    Console.Write("Odabir: ");
                    var subChoice = Console.ReadLine();
                    if (subChoice == "0") continue;

                    if (subChoice == "1")
                    {
                        var over30 = users.Where(u => DateTime.Now.Year - u.Value.BirthDate.Year > 30);
                        foreach (var user in over30)
                        {
                            Console.WriteLine($"{user.Key}: {user.Value.FirstName} {user.Value.LastName}");
                        }
                    }
                    else if (subChoice == "2")
                    {
                        var inDebt = users.Where(u => u.Value.Accounts.Any(acc => acc.Value.Sum(t => t.Amount) < 0));
                        foreach (var user in inDebt)
                        {
                            Console.WriteLine($"{user.Key}: {user.Value.FirstName} {user.Value.LastName}");
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Neispravan unos. Pokušajte ponovo.");
                    break;
            }
        }
    }

    static void ManageAccounts()
    {
        while (true)
        {
            Console.Write("Unesite ID korisnika (ili 0 za povratak): ");
            if (!int.TryParse(Console.ReadLine(), out var userId) || userId == 0) return;

            if (users.ContainsKey(userId))
            {
                var user = users[userId];
                Console.WriteLine($"Računi korisnika {user.FirstName} {user.LastName}:");
                foreach (var account in user.Accounts)
                {
                    Console.WriteLine($"- {account.Key}: {account.Value.Sum(t => t.Amount):0.00} EUR");
                }

                Console.WriteLine("1 - Dodavanje transakcije");
                Console.WriteLine("2 - Pregled transakcija");
                Console.WriteLine("3 - Financijsko izvješće");
                Console.WriteLine("4 - Brisanje transakcija");
                Console.WriteLine("5 - Interni/eksterni prijenos");
                Console.WriteLine("0 - Povratak");
                Console.Write("Odabir: ");
                var choice = Console.ReadLine();
                if (choice == "0") continue;

                switch (choice)
                {
                    case "1":
                        AddTransaction(user);
                        break;

                    case "2":
                        ViewTransactions(user);
                        break;

                    case "3":
                        GenerateReport(user);
                        break;

                    case "4":
                        DeleteTransaction(user);
                        break;

                    case "5":
                        TransferFunds(user);
                        break;

                    default:
                        Console.WriteLine("Neispravan unos.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Korisnik ne postoji.");
            }
        }
    }

    static void AddTransaction((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.Write("Odaberite račun (tekući, žiro, prepaid): ");
        var accountType = Console.ReadLine();
        if (user.Accounts.ContainsKey(accountType))
        {
            Console.Write("Unesite iznos: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                Console.Write("Opis: ");
                var description = Console.ReadLine();
                Console.Write("Tip (prihod/rashod): ");
                var type = Console.ReadLine();
                Console.Write("Kategorija: ");
                var category = Console.ReadLine();
                user.Accounts[accountType].Add((transactionIdCounter++, amount, description, type, category, DateTime.Now));
                Console.WriteLine("Transakcija uspješno dodana.");
            }
            else
            {
                Console.WriteLine("Neispravan iznos.");
            }
        }
        else
        {
            Console.WriteLine("Račun ne postoji.");
        }
    }

    static void ViewTransactions((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        foreach (var account in user.Accounts)
        {
            Console.WriteLine($"Transakcije za račun {account.Key}:");
            foreach (var transaction in account.Value)
            {
                Console.WriteLine($"- {transaction.Id}, {transaction.Amount:0.00} EUR, {transaction.Description}, {transaction.Type}, {transaction.Category}, {transaction.DateTime}");
            }
        }
    }

    static void GenerateReport((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.Write("Odaberite račun za izvješće (tekući, žiro, prepaid): ");
        var accountType = Console.ReadLine();

        if (user.Accounts.ContainsKey(accountType))
        {
            var transactions = user.Accounts[accountType];
            var balance = transactions.Sum(t => t.Amount);
            Console.WriteLine($"Trenutno stanje računa: {balance:0.00} EUR");
            if (balance < 0) Console.WriteLine("Upozorenje: Račun je u minusu!");

            var totalIncome = transactions.Where(t => t.Type == "prihod").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "rashod").Sum(t => t.Amount);

            Console.WriteLine($"Broj transakcija: {transactions.Count}");
            Console.WriteLine($"Prihodi: {totalIncome:0.00} EUR");
            Console.WriteLine($"Rashodi: {totalExpense:0.00} EUR");

            Console.Write("Unesite mjesec i godinu (yyyy-mm) za analizu: ");
            if (DateTime.TryParse(Console.ReadLine() + "-01", out var monthYear))
            {
                var monthlyTransactions = transactions.Where(t => t.DateTime.Month == monthYear.Month && t.DateTime.Year == monthYear.Year);
                var monthlyIncome = monthlyTransactions.Where(t => t.Type == "prihod").Sum(t => t.Amount);
                var monthlyExpense = monthlyTransactions.Where(t => t.Type == "rashod").Sum(t => t.Amount);

                Console.WriteLine($"Prihodi za {monthYear.ToString("MMMM yyyy")}: {monthlyIncome:0.00} EUR");
                Console.WriteLine($"Rashodi za {monthYear.ToString("MMMM yyyy")}: {monthlyExpense:0.00} EUR");
            }
            else
            {
                Console.WriteLine("Neispravan unos datuma.");
            }
        }
        else
        {
            Console.WriteLine("Račun ne postoji.");
        }
    }

    static void DeleteTransaction((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.Write("Odaberite račun za brisanje transakcije (tekući, žiro, prepaid): ");
        var accountType = Console.ReadLine();

        if (user.Accounts.ContainsKey(accountType))
        {
            Console.Write("Unesite ID transakcije za brisanje: ");
            if (int.TryParse(Console.ReadLine(), out var transactionId))
            {
                var account = user.Accounts[accountType];
                var transaction = account.FirstOrDefault(t => t.Id == transactionId);
                if (transaction != default)
                {
                    account.Remove(transaction);
                    Console.WriteLine("Transakcija uspješno obrisana.");
                }
                else
                {
                    Console.WriteLine("Transakcija s tim ID-em ne postoji.");
                }
            }
            else
            {
                Console.WriteLine("Neispravan ID transakcije.");
            }
        }
        else
        {
            Console.WriteLine("Račun ne postoji.");
        }
    }

    static void TransferFunds((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.Write("Unesite naziv računa iz kojeg se prebacuju sredstva (tekući, žiro, prepaid): ");
        var fromAccountType = Console.ReadLine();

        Console.Write("Unesite naziv računa na koji se prebacuju sredstva (tekući, žiro, prepaid): ");
        var toAccountType = Console.ReadLine();

        if (user.Accounts.ContainsKey(fromAccountType) && user.Accounts.ContainsKey(toAccountType))
        {
            Console.Write("Unesite iznos za prijenos: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                if (user.Accounts[fromAccountType].Sum(t => t.Amount) >= amount)
                {
                    user.Accounts[fromAccountType].Add((transactionIdCounter++, -amount, "Interni prijenos", "rashod", "Transfer", DateTime.Now));
                    user.Accounts[toAccountType].Add((transactionIdCounter++, amount, "Interni prijenos", "prihod", "Transfer", DateTime.Now));
                    Console.WriteLine("Prijenos uspješno izvršen.");
                }
                else
                {
                    Console.WriteLine("Nema dovoljno sredstava na računu.");
                }
            }
            else
            {
                Console.WriteLine("Neispravan iznos.");
            }
        }
        else
        {
            Console.WriteLine("Jedan ili oba računa ne postoje.");
        }
    }

    static void AddUser(string firstName, string lastName, DateTime birthDate)
    {
        users.Add(userIdCounter++, (firstName, lastName, birthDate, new Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>>()));
    }
}
