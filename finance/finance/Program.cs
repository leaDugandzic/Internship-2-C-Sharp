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
            if (DateTime.TryParse(Console.ReadLine() + "-01", out var selectedMonth))
            {
                var monthlyTransactions = transactions.Where(t => t.DateTime.Year == selectedMonth.Year && t.DateTime.Month == selectedMonth.Month);
                Console.WriteLine($"Ukupan broj transakcija u {selectedMonth:yyyy-MM}: {monthlyTransactions.Count()}");
                Console.WriteLine($"Ukupno za mjesec: {monthlyTransactions.Sum(t => t.Amount):0.00} EUR");
                if (monthlyTransactions.Any())
                {
                    Console.WriteLine($"Prosječni iznos transakcije: {monthlyTransactions.Average(t => t.Amount):0.00} EUR");
                }
            }
        }
        else
        {
            Console.WriteLine("Račun ne postoji.");
        }
    }

    static void DeleteTransaction((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.Write("Odaberite račun (tekući, žiro, prepaid): ");
        var accountType = Console.ReadLine();

        if (user.Accounts.ContainsKey(accountType))
        {
            Console.WriteLine("Opcije za brisanje transakcija:");
            Console.WriteLine("1 - Po ID-u");
            Console.WriteLine("2 - Ispod određenog iznosa");
            Console.WriteLine("3 - Sve prihode");
            Console.WriteLine("4 - Sve rashode");
            Console.WriteLine("0 - Povratak");
            Console.Write("Odabir: ");
            var choice = Console.ReadLine();

            if (choice == "0") return;

            var transactions = user.Accounts[accountType];

            switch (choice)
            {
                case "1":
                    Console.Write("Unesite ID transakcije za brisanje: ");
                    if (int.TryParse(Console.ReadLine(), out var idToDelete))
                    {
                        var transaction = transactions.FirstOrDefault(t => t.Id == idToDelete);
                        if (transaction != default)
                        {
                            transactions.Remove(transaction);
                            Console.WriteLine("Transakcija uspješno obrisana.");
                        }
                        else
                        {
                            Console.WriteLine("Transakcija s tim ID-om ne postoji.");
                        }
                    }
                    break;

                case "2":
                    Console.Write("Unesite maksimalni iznos transakcija za brisanje: ");
                    if (decimal.TryParse(Console.ReadLine(), out var maxAmount))
                    {
                        transactions.RemoveAll(t => t.Amount < maxAmount);
                        Console.WriteLine("Transakcije ispod unesenog iznosa su obrisane.");
                    }
                    break;

                case "3":
                    transactions.RemoveAll(t => t.Type == "prihod");
                    Console.WriteLine("Svi prihodi su obrisani.");
                    break;

                case "4":
                    transactions.RemoveAll(t => t.Type == "rashod");
                    Console.WriteLine("Svi rashodi su obrisani.");
                    break;

                default:
                    Console.WriteLine("Neispravan unos.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Račun ne postoji.");
        }
    }

    static void TransferFunds((string FirstName, string LastName, DateTime BirthDate, Dictionary<string, List<(int Id, decimal Amount, string Description, string Type, string Category, DateTime DateTime)>> Accounts) user)
    {
        Console.WriteLine("1 - Interni prijenos");
        Console.WriteLine("2 - Eksterni prijenos");
        Console.Write("Odabir: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Izvorni račun (tekući, žiro, prepaid): ");
            var sourceAccount = Console.ReadLine();
            Console.Write("Ciljani račun (tekući, žiro, prepaid): ");
            var targetAccount = Console.ReadLine();

            if (user.Accounts.ContainsKey(sourceAccount) && user.Accounts.ContainsKey(targetAccount))
            {
                Console.Write("Unesite iznos za prijenos: ");
                if (decimal.TryParse(Console.ReadLine(), out var transferAmount) && transferAmount > 0)
                {
                    user.Accounts[sourceAccount].Add((transactionIdCounter++, -transferAmount, "Interni prijenos", "rashod", "transfer", DateTime.Now));
                    user.Accounts[targetAccount].Add((transactionIdCounter++, transferAmount, "Interni prijenos", "prihod", "transfer", DateTime.Now));
                    Console.WriteLine("Interni prijenos uspješno obavljen.");
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
        else if (choice == "2")
        {
            Console.Write("Unesite ID primatelja: ");
            if (int.TryParse(Console.ReadLine(), out var recipientId) && users.ContainsKey(recipientId))
            {
                var recipient = users[recipientId];
                Console.Write("Ciljani račun primatelja (tekući, žiro, prepaid): ");
                var targetAccount = Console.ReadLine();

                if (recipient.Accounts.ContainsKey(targetAccount))
                {
                    Console.Write("Unesite iznos za prijenos: ");
                    if (decimal.TryParse(Console.ReadLine(), out var transferAmount) && transferAmount > 0)
                    {
                        user.Accounts["tekući"].Add((transactionIdCounter++, -transferAmount, "Eksterni prijenos", "rashod", "transfer", DateTime.Now));
                        recipient.Accounts[targetAccount].Add((transactionIdCounter++, transferAmount, "Eksterni prijenos", "prihod", "transfer", DateTime.Now));
                        Console.WriteLine("Eksterni prijenos uspješno obavljen.");
                    }
                    else
                    {
                        Console.WriteLine("Neispravan iznos.");
                    }
                }
                else
                {
                    Console.WriteLine("Račun primatelja ne postoji.");
                }
            }
            else
            {
                Console.WriteLine("Primatelj s unesenim ID-om ne postoji.");
            }
        }
        else
        {
            Console.WriteLine("Neispravan unos.");
        }
    }

    static void AddUser(string firstName, string lastName, DateTime birthDate)
    {
        users[userIdCounter++] = (firstName, lastName, birthDate, new Dictionary<string, List<(int, decimal, string, string, string, DateTime)>>
        {
            { "tekući", new List<(int, decimal, string, string, string, DateTime)>() { (transactionIdCounter++, 100.00m, "Početno stanje", "prihod", "init", DateTime.Now) } },
            { "žiro", new List<(int, decimal, string, string, string, DateTime)>() },
            { "prepaid", new List<(int, decimal, string, string, string, DateTime)>() }
        });
    }
}
