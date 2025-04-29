using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NReco.Logging.File;
using SmartBook.Helpers;
using SmartBook.SmartBookCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace SmartBook;
/*
 * Menus for the App
*/
public class LibraryApp
{
    //Filenames used 
    public static string BOOKS_FILE = "library.json";
    public static string CARDS_FILE = "cards.json";
    public static string EXPORTED_B = "book_on_loan.txt";
    public static string LOGFILE    = "log.txt";

    private readonly ILogger _logger;


    public Library MyLibrary { get; private set; }
    private LibraryCardHandler myCardHandler;

    public LibraryApp()
    {
        MyLibrary = new Library();
        myCardHandler = new LibraryCardHandler();
        _logger = GetLogger();
    }

    public ILogger GetLogger()
    {
            ILoggerFactory factory = new LoggerFactory();              
            factory.AddProvider(new FileLoggerProvider(LOGFILE));  
            return factory.CreateLogger<LibraryApp>();
    }

    public List<Card> GetCards()
    {
        return myCardHandler.GetCards();
    }
    public void Seed()
    {
        MyLibrary.AddBook(new Book(2, "aaa", "aaa", Category.Novels));
        MyLibrary.AddBook(new Book(3, "eee", "eee", Category.Novels));
        MyLibrary.AddBook(new Book(4, "www", "qqq", Category.Novels));
        MyLibrary.AddBook(new Book(5, "ppp", "ppp", Category.Novels));
        MyLibrary.AddBook(new Book(1, "bbb", "bbb", Category.Novels));
        MyLibrary.AddBook(new Book(3829030428, "Könemann, Ludwig", "Egypten Faraonernas värld", Category.History));
        MyLibrary.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
        //91 - 1 - 743092 - 5
    }
    #region Lånekortsmeny
    private enum AltMenuOptions
    {
        BORROW,
        RETURN_ALL,
        RETURN_SELECTED,
        EXIT
    }
    private Dictionary<AltMenuOptions, string> AltMenuText = new Dictionary<AltMenuOptions, string>()
    {
        { AltMenuOptions.BORROW,            "[B] Borrow books" },
        { AltMenuOptions.RETURN_ALL,        "[A] Return all books" },
        { AltMenuOptions.RETURN_SELECTED,   "[S] Select books to return" },
        { AltMenuOptions.EXIT,              "[X] Exit"},
    };
    private Dictionary<ConsoleKey, AltMenuOptions> AltMenuHotkeys = new Dictionary<ConsoleKey, AltMenuOptions>()
    {
        { ConsoleKey.B,         AltMenuOptions.BORROW },
        { ConsoleKey.A,         AltMenuOptions.RETURN_ALL },
        { ConsoleKey.S,         AltMenuOptions.RETURN_SELECTED },
        { ConsoleKey.X,         AltMenuOptions.EXIT },
        { ConsoleKey.Escape,    AltMenuOptions.EXIT },
    };
    private void ShowAltMenu(Card card, AltMenuOptions selected)
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine($"LOGGED IN AS {card.Name} ");
        Console.WriteLine("======================================");

        foreach (AltMenuOptions item in Enum.GetValues(typeof(AltMenuOptions)))
        {
            if (item == selected)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(AltMenuText.GetValueOrDefault(item));
                Console.ResetColor();
            }
            else
                Console.WriteLine(AltMenuText.GetValueOrDefault(item));
        }
    }
    #endregion 

    #region Huvudmeny
    // Menyalternativ för huvudmenyn
    private enum MainMenuOptions
    {
        ADD,
        DELETE,
        LIST,
        FIND,
        GETCARD,
        MENU,
        ON_LOAN_SHOW,
        ON_LOAN_SAVE,
        SAVE,
        EXIT
    }
    // Ledtexter för ovanstående menyalternativ
    private Dictionary<MainMenuOptions, string> MainMenuText = new Dictionary<MainMenuOptions, string>()
    {
        { MainMenuOptions.ADD,          "[A] Add a book" },
        { MainMenuOptions.DELETE,       "[D] Delete a book"},
        { MainMenuOptions.LIST,         "[L] Show all books"},
        { MainMenuOptions.FIND,         "[F] Find a book"},
        { MainMenuOptions.GETCARD,      "[C] Issue a new library card"},
        { MainMenuOptions.MENU,         "[M] Borrow / return books"},
        { MainMenuOptions.ON_LOAN_SHOW, "[W] Show borrowed books"},
        { MainMenuOptions.ON_LOAN_SAVE, "[P] export borrowed books to a file"},
        { MainMenuOptions.SAVE,         "[S] Save all books"},
        { MainMenuOptions.EXIT,         "[X] Exit"},

    };
    //Hotkeys för ovanstående menyalternativ
    private Dictionary<ConsoleKey, MainMenuOptions> MainMenuHotkeys = new Dictionary<ConsoleKey, MainMenuOptions>()
    {
        { ConsoleKey.A,         MainMenuOptions.ADD },
        { ConsoleKey.D,         MainMenuOptions.DELETE },
        { ConsoleKey.L,         MainMenuOptions.LIST },
        { ConsoleKey.F,         MainMenuOptions.FIND },
        { ConsoleKey.C,         MainMenuOptions.GETCARD },
        { ConsoleKey.M,         MainMenuOptions.MENU },
        { ConsoleKey.W,         MainMenuOptions.ON_LOAN_SHOW },
        { ConsoleKey.P,         MainMenuOptions.ON_LOAN_SAVE },
        { ConsoleKey.S,         MainMenuOptions.SAVE },
        { ConsoleKey.X,         MainMenuOptions.EXIT },
        { ConsoleKey.Escape,    MainMenuOptions.EXIT },

    };

    private void ShowMainMenu(MainMenuOptions selected)
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine("MAIN MENU");
        Console.WriteLine("======================================");

        foreach (MainMenuOptions item in Enum.GetValues(typeof(MainMenuOptions)))
        {
            if (item == selected)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(MainMenuText.GetValueOrDefault(item));
                Console.ResetColor();
            }
            else
                Console.WriteLine(MainMenuText.GetValueOrDefault(item));
        }
    }
    #endregion


    public void MainMenu()
    {
        //ConsoleKeyInfo enter = new ConsoleKeyInfo(' ', ConsoleKey.Enter, false, false, false);
        bool bAvsluta = false;
        MainMenuOptions selected = MainMenuOptions.ADD;
        do
        {
            ShowMainMenu(selected);
            ConsoleKeyInfo cki = Console.ReadKey();

            //Menu-navigation w arrows
            (bool, int) ArrowAction = ArrowHandled(typeof(MainMenuOptions), (int)selected, cki.Key);
            if (ArrowAction.Item1)
            {
                ShowMainMenu(selected = (MainMenuOptions)ArrowAction.Item2);
                continue;
            }

            //Enter or hotkey for accept
            if (cki.Key == ConsoleKey.Enter
            || MainMenuHotkeys.ContainsKey(cki.Key))
            {
                //Set selected row according to used hotkey
                if (cki.Key != ConsoleKey.Enter)
                    selected = MainMenuHotkeys.GetValueOrDefault(cki.Key);

                switch (selected)
                {
                    case MainMenuOptions.ADD:
                        AddBook();
                        continue;

                    case MainMenuOptions.DELETE:
                        DeleteBook();
                        break;

                    case MainMenuOptions.LIST:
                        ListBooks();
                        break;

                    case MainMenuOptions.FIND:
                        FindBooks();
                        break;

                    case MainMenuOptions.GETCARD:
                        GenerateLibraryCard();
                        break;

                    case MainMenuOptions.MENU:
                        Menu();
                        continue;

                    case MainMenuOptions.ON_LOAN_SHOW:
                        OnLoan(export:false);
                        break;

                    case MainMenuOptions.ON_LOAN_SAVE:
                        OnLoan(export:true);
                        break;


                    case MainMenuOptions.SAVE:
                        SaveToFile();
                        break;

                    case MainMenuOptions.EXIT:
                        bAvsluta = true;
                        continue;

                    default:
                        Console.WriteLine("Menu option not implemented! ");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        } while (!bAvsluta);
    }


    public void AltMenu(Card card)
    {
        bool bAvsluta = false;
        AltMenuOptions selected = AltMenuOptions.BORROW;
        do
        {
            ShowAltMenu(card, selected);
            ConsoleKeyInfo cki = Console.ReadKey();

            //Menu-navigation w arrows
            (bool, int) ArrowAction = ArrowHandled(typeof(MainMenuOptions), (int)selected, cki.Key);
            if (ArrowAction.Item1)
            {
                ShowAltMenu(card, selected = (AltMenuOptions)ArrowAction.Item2);
                continue;
            }

            //Enter (or hotkey for accept)
            if (cki.Key == ConsoleKey.Enter
            || AltMenuHotkeys.ContainsKey(cki.Key))
            {
                //Set selected row according to used hotkey
                if (cki.Key != ConsoleKey.Enter)
                    selected = AltMenuHotkeys.GetValueOrDefault(cki.Key);

                switch (selected)
                {
                    case AltMenuOptions.RETURN_ALL:
                        ReturnBooks(card, alla:true );
                        break;

                    case AltMenuOptions.RETURN_SELECTED:
                        ReturnBooks(card, alla:false);
                        break;

                    case AltMenuOptions.BORROW:
                        BorrowBooks(card);
                        break;


                    case AltMenuOptions.EXIT:
                        bAvsluta = true;
                        break;

                    default:
                        Console.WriteLine("Menu option not implemented! Press any key...");
                        Console.ReadKey();
                        break;
                }
            }

        } while (!bAvsluta);
    }

    public void Menu()
    {
        ulong no = InputControl.AskForULong("Library Card No");
        Card xx = GetCards().Find(c => c.ID == no);
        if (xx != null)
            AltMenu(xx);
        else
        {
            Console.WriteLine("No librarycard with number {no} has been issued!");
            Console.ReadKey();
        }
            

    }



    //Menu navigation with arrowkeys - used in multiple menus.
    private (bool, int) ArrowHandled(Type enumType, int selected, ConsoleKey key)
    {
        //Move selection up
        if (key == ConsoleKey.UpArrow
        || key == ConsoleKey.LeftArrow)
        {
            selected--;
            if (!Enum.IsDefined(enumType, selected))
                selected = (int)Enum.GetValues(enumType)
                      .GetValue(Enum.GetValues(enumType).Length - 1);
            return (true, selected);
        }

        //Move selection down
        if (key == ConsoleKey.DownArrow
        || key == ConsoleKey.RightArrow)
        {
            selected++;
            if (!Enum.IsDefined(enumType, selected))
                selected = (int)Enum.GetValues(enumType).GetValue(0);

            return (true, selected);
        }
        return (false, selected);
    }


    private void ListBooks()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"List of books ");
        Console.WriteLine("================================");

        foreach (Book book in MyLibrary.GetBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("");
        Console.WriteLine($"{MyLibrary.GetBooks().Count} books found.");
        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();
    }
    private void FindBooks()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"Search for Books (use * wildcard)");
        Console.WriteLine("================================");
        string author = InputControl.AskForString("Author");
        string title = InputControl.AskForString("Title");

        List<Book> urval = MyLibrary.FindBooks(author, title);
        Console.WriteLine();
        foreach (Book book in urval)
        {
            Console.WriteLine(book);
        }
        Console.WriteLine("");
        Console.WriteLine($"{urval.Count} Books found.");
        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();

    }

    private void GenerateLibraryCard()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("Library Card Application:");
        Console.WriteLine("================================");

        string namn = InputControl.AskForString("Name");
        ulong cardNo = myCardHandler.GetNewCard(namn);
        string jsontext = myCardHandler.SaveCardsToJson();
        File.WriteAllText(LibraryApp.CARDS_FILE, jsontext);
        _logger.Log(LogLevel.Information, "Card no {cardNo} was issued to {namn}.", [cardNo,namn] );

        //Console.WriteLine("");
        //Console.WriteLine("================================");
        Console.WriteLine($"Card issued: {cardNo}");
        Console.WriteLine("================================");

        Console.WriteLine("");
        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();

    }

    private void OnLoan(bool export)
    {
        StringBuilder sb = new StringBuilder();
        sb = sb.AppendLine("================================");
        sb = sb.AppendLine("Books OnLoan");
        sb = sb.AppendLine("================================");

        var urval = MyLibrary.GetBooks()
            .Where(b => b.IsOnLoan == true)
            .ToList();

        foreach (Book book in urval)
            sb = sb.AppendLine(book.ToString());

        sb = sb.AppendLine();
        sb = sb.AppendLine($"{urval.Count} Books found.");


        if (export)
        {
            File.WriteAllText(EXPORTED_B, sb.ToString() );
            Console.WriteLine($"{urval.Count} books was exported to {EXPORTED_B}");
        }
        else
        {
            Console.Clear();
            Console.Write(sb.ToString());
            //Console.WriteLine($"{urval.Count} Books found.");

        }


        //Console.WriteLine("================================");
        //Console.WriteLine($"Books OnLoan");
        //Console.WriteLine("================================");

        //var urval = myLibrary.GetBooks()
        //.Where(b => b.IsOnLoan == true)
        //.ToList();

        //foreach (Book book in urval)
        //    Console.WriteLine(book);


        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();

    }



    private void AddBook()
    {
        int iAdderat = 0;
        bool bAvsluta = false;
        do
        {
            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine("ADD BOOKS");
            Console.WriteLine("======================================");
            try
            {
                ulong isbn = InputControl.AskForULong("ISBN");
                string author = InputControl.AskForString("Author");
                string title = InputControl.AskForString("Title");
                Category cat = (Category)InputControl.AskForEnum(typeof(Category));

                Book book = new Book(isbn, author, title, cat);
                MyLibrary.AddBook(book);

                iAdderat++;
                Console.WriteLine("");
                Console.WriteLine($"{iAdderat} book(s) added.");
                _logger.Log(LogLevel.Information, "The book \"{title}\" was added", book.Title);

            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("X to exit. Press any key to continue...");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.X)
            {
                bAvsluta = true;
                continue;
            }
                

        } while (!bAvsluta);
    }


    private void DeleteBook()
    {
        int idxCurrent = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("Select the book to delete and press <delete> ");
            Console.WriteLine("or press <Q> or <T> to enter isbn/title for the book to delete.");
            Console.WriteLine("================================");

            foreach ((int, Book) tup in MyLibrary.GetBooks().Index())
            {
                if (idxCurrent == tup.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{tup.Item2}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{tup.Item2}");
            }
            Console.WriteLine("");
            Console.WriteLine($"{MyLibrary.GetBooks().Count} Books was found.");

            //Enter ISBN to delete the book and end the loop
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
            {
                ulong isbn = InputControl.AskForULong("ISBN");
                bool status = MyLibrary.DeleteBookByIsbn(isbn);
                Console.WriteLine($"The book was {((status) ? "removed." : "not removed.")} ");
                if(status) _logger.Log(LogLevel.Information, "The book with isbn {isbn} was deleted", isbn);

                break;
            }
            if (key.Key == ConsoleKey.T)
            {
                string title = InputControl.AskForString("Title");
                bool status = MyLibrary.DeleteBookByTitle(title);
                Console.WriteLine($"The book was {((status) ? "removed." : "not removed.")} ");
                if (status) _logger.Log(LogLevel.Information, "The book with title {title} was deleted.", title);

                break;
            }

            //Let the user navigate in the list above.
            if (key.Key == ConsoleKey.UpArrow
            || key.Key == ConsoleKey.LeftArrow)
            {
                if (idxCurrent > 0)
                    idxCurrent--;
            }

            if (key.Key == ConsoleKey.DownArrow
            || key.Key == ConsoleKey.RightArrow)
            {
                if (idxCurrent < MyLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Delete)
            {
                Book b = MyLibrary.GetBooks().ElementAt(idxCurrent);
                if (b != null)
                {
                    bool status = MyLibrary.DeleteBookByIsbn(b.ISBN);
                    if (status) _logger.Log(LogLevel.Information, "{title} was deleted.", b.Title);
                }
            }

        } while (true);

        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();
    }
    private void ReturnBooks(Card card, bool alla)
    {
        int idxCurrent = 0;
        List<Book> myBorrowedBooks = MyLibrary.GetBooks()
            .Where(b => b.BorrowedBy == card.ID).ToList();
        
        if (myBorrowedBooks.Count() == 0)
        {
            Console.WriteLine("");
            Console.WriteLine("Borrow some books first!");
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
            return; 
        }

        if (alla)
        {
            int returned = 0;
            int total = myBorrowedBooks.Count();
            Console.WriteLine($"{card.Name} returns all.");

            foreach (Book b in myBorrowedBooks)
            {
                if (MyLibrary.ReturnBook(b))
                {
                    returned++;
                }
            }
            _logger.Log(LogLevel.Information, "{name} returned {cnt} Book(s)", card.Name, returned);
            Console.WriteLine($"{card.Name} returned {returned} of {total} books.");

            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
            return;

        }
        do
        {
            if (myBorrowedBooks.Count() == 0) return;

            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("Select the book to return and press");
            Console.WriteLine("<enter> or <space> to select it.");
            Console.WriteLine("================================");

            //List all the books for the user to choose from.
            foreach ((int, Book) tup in myBorrowedBooks.Index())
            {
                if (idxCurrent == tup.Item1)
                {   //Currently selected index is color-marked
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
            }
            Console.WriteLine("");
            Console.WriteLine($"{myBorrowedBooks.Count} Books was found.");

            //Let the user navigate in the list above by changing the current index.
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow
            || key.Key == ConsoleKey.LeftArrow)
            {
                if (idxCurrent > 0)
                    idxCurrent--;
            }

            if (key.Key == ConsoleKey.DownArrow
            || key.Key == ConsoleKey.RightArrow)
            {
                if (idxCurrent < myBorrowedBooks.Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter
            ||  key.Key == ConsoleKey.Spacebar  )
            {
                Book selectedBook = myBorrowedBooks.ElementAt(idxCurrent);

                //mark the selected book as returned.
                if (selectedBook != null)
                {
                    bool status = MyLibrary.ReturnBook(selectedBook);
                    if(status )_logger.Log(LogLevel.Debug, "{name} returned {title}", card.Name, selectedBook.Title);

                    myBorrowedBooks = MyLibrary.GetBooks()
                                    .Where(b => b.BorrowedBy == card.ID).ToList();
                }
            }




        } while (true);
    }


    private void BorrowBooks(Card card)
    {
        int idxCurrent = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("Select the book to borrow and press <enter>");
            Console.WriteLine("================================");

            //List all the books for the user to choose from.
            List<Book> booksAtHand = MyLibrary.GetBooks().Where(b => b.IsOnLoan == false).ToList();

            foreach ((int, Book) tup in booksAtHand.Index())
            {
                    if (idxCurrent == tup.Item1)
                {   //Currently selected index is color-marked
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
            }
            Console.WriteLine("");
            Console.WriteLine($"{booksAtHand.Count} Books was found.");

            //Let the user navigate in the list above by changing the current index.
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow
            || key.Key == ConsoleKey.LeftArrow)
            {
                if (idxCurrent > 0)
                    idxCurrent--;
            }

            if (key.Key == ConsoleKey.DownArrow
            || key.Key == ConsoleKey.RightArrow)
            {
                if (idxCurrent < MyLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter
            ||  key.Key == ConsoleKey.Spacebar)
            {
                Book selectedBook = booksAtHand.ElementAt(idxCurrent);
                //mark the selected book as borrowed.
                if (selectedBook != null)
                {
                    bool status = MyLibrary.BorrowBook(selectedBook, card.ID);
                    if (status) _logger.Log(LogLevel.Debug, "{name} borrowed {Title}.", card.Name, selectedBook.Title  );

                }
            }

        } while (true);

    }

    public void LoadBooks()
    {
        try
        {
            string json = File.ReadAllText(LibraryApp.BOOKS_FILE);
            MyLibrary.LoadBooksFromJson(json);
            Console.WriteLine($"{MyLibrary.GetBooks().Count()} books was loaded from file.");
        }
        catch (Exception e) 
        {
            throw new Exception($"Could not read {LibraryApp.BOOKS_FILE}.", e);
        }
    }
    public void LoadCards()
    {
        try
        {
            string json = File.ReadAllText(LibraryApp.CARDS_FILE);
            myCardHandler.LoadCardsFromJson(json);
            Console.WriteLine($"{myCardHandler.GetCards().Count()} Library cards was loaded from file. ");
        }
        catch (Exception e)
        {
            throw new Exception($"Could not read {LibraryApp.CARDS_FILE}.", e);
        }
    }

    public void LoadData()
    {
        try
        {
            if (File.Exists(LibraryApp.BOOKS_FILE))
                LoadBooks();
            else
                Console.WriteLine($"The file \"{LibraryApp.BOOKS_FILE}\" was not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        if (MyLibrary.GetBooks().Count() == 0)
        {
            Seed();
            Console.WriteLine($"The empty library was seeded with {MyLibrary.GetBooks().Count()} books.");
        }
            

        try
        {
            if (File.Exists(LibraryApp.CARDS_FILE))
            LoadCards();
        else
            Console.WriteLine($"The file \"{LibraryApp.CARDS_FILE}\" was not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        //Låter anv. kontrollera om dataladdningen gick bra.
        Console.WriteLine($"{Environment.NewLine}Press any key to continue...");
        Console.ReadKey();

    }
    public void SaveToFile()
    {
        File.WriteAllText(LibraryApp.BOOKS_FILE, MyLibrary.GetJsonFromBooks() );
        Console.WriteLine($"{MyLibrary.GetBooks().Count()} books was saved to file.");
    }



}
