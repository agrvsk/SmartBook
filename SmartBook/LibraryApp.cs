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
    public static string LOGFILE = "log.txt";

    private readonly ILogger _logger;
    private static ILoggerFactory factory=null;

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
        if (factory == null)
        {
            factory = new LoggerFactory();
            factory.AddProvider(new FileLoggerProvider(LOGFILE));
        }
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
        Console.WriteLine("======================================" +
                         $"{Environment.NewLine}LOGGED IN AS {card.Name}{Environment.NewLine}" +
                          "======================================");

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
        BORROWED_SHOW,
        BORROWED_SAVE,
        SAVE,
        SHOW_LOG,
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
        { MainMenuOptions.BORROWED_SHOW, "[W] Show borrowed books"},
        { MainMenuOptions.BORROWED_SAVE, "[P] export borrowed books to a file"},
        { MainMenuOptions.SAVE,         "[S] Save all books"},
        { MainMenuOptions.SHOW_LOG,     "[H] Show logfile"},
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
        { ConsoleKey.W,         MainMenuOptions.BORROWED_SHOW },
        { ConsoleKey.P,         MainMenuOptions.BORROWED_SAVE },
        { ConsoleKey.S,         MainMenuOptions.SAVE },
        { ConsoleKey.H,         MainMenuOptions.SHOW_LOG },
        { ConsoleKey.X,         MainMenuOptions.EXIT },
        { ConsoleKey.Escape,    MainMenuOptions.EXIT },

    };

    private void ShowMainMenu(MainMenuOptions selected)
    {
        Console.Clear();
        Console.WriteLine("======================================" +
                         $"{Environment.NewLine}MAIN MENU{Environment.NewLine}" +
                          "======================================");

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
                        break;   

                    case MainMenuOptions.DELETE:
                        DeleteBook();
                        continue;   //will not wait for keypress

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
                        continue; //will not wait for keypress

                    case MainMenuOptions.BORROWED_SHOW:
                        Borrowed(export: false);
                        break;

                    case MainMenuOptions.BORROWED_SAVE:
                        Borrowed(export: true);
                        break;


                    case MainMenuOptions.SAVE:
                        SaveToFile();
                        break;

                    case MainMenuOptions.SHOW_LOG:
                        ShowLog();
                        break;
                    case MainMenuOptions.EXIT:
                        bAvsluta = true;
                        continue; //Will not wait for keypress..

                    default:
                        Console.WriteLine("Menu option not implemented! ");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        } while (!bAvsluta);
    }

    private void ShowLog()
    {
        if (File.Exists(LibraryApp.LOGFILE))
        {
            //The file is open in another process (by _Logger )
            FileStream logFileStream=null;
            StreamReader logFileReader = null;

            try
            {
                Console.Clear();
                logFileStream = new FileStream(LibraryApp.LOGFILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                logFileReader = new StreamReader(logFileStream);

                while (!logFileReader.EndOfStream)
                    Console.WriteLine(logFileReader.ReadLine());
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally 
            {
                // Clean up
                if(logFileReader != null) logFileReader.Close();
                if(logFileStream != null) logFileStream.Close();
            }


        }
        
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
                        ReturnBooks(card, alla: true);
                        break;

                    case AltMenuOptions.RETURN_SELECTED:
                        ReturnBooks(card, alla: false);
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

    //Menu navigation with arrowkeys - used in both menus.
    private (bool, int) ArrowHandled(List<Book> list, int selected, ConsoleKey key)
    {
        //Move selection up
        if (key == ConsoleKey.UpArrow
        || key == ConsoleKey.LeftArrow)
        {
            selected--;

            if (selected < 0)
                selected = list.Count - 1;

            return (true, selected);
        }
        //Move selection down
        if (key == ConsoleKey.DownArrow
        || key == ConsoleKey.RightArrow)
        {
            selected++;

            if (selected > list.Count - 1)  
                selected = 0;

            return (true, selected);
        }
        return (false, selected);

    }

    //Menu navigation with arrowkeys - used in both menus.
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
        Console.WriteLine("================================" +
                         $"{Environment.NewLine}List of books {Environment.NewLine}" +
                          "================================");

        foreach (Book book in MyLibrary.GetBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("");
        Console.WriteLine($"{MyLibrary.GetBooks().Count} books found.");
    }
    private void FindBooks()
    {
        Console.Clear();
        Console.WriteLine("================================" +
                         $"{Environment.NewLine}Search for Books (use * wildcard){Environment.NewLine}" +
                          "================================");
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

    }

    private void GenerateLibraryCard()
    {
        Console.Clear();
        Console.WriteLine("================================" +
                         $"{Environment.NewLine}Library Card Application:{Environment.NewLine}" +
                          "================================");

        string namn = InputControl.AskForString("Name");
        ulong cardNo = myCardHandler.GetNewCard(namn);
        string jsontext = myCardHandler.SaveCardsToJson();
        File.WriteAllText(LibraryApp.CARDS_FILE, jsontext);
        _logger.Log(LogLevel.Information, "Card no {cardNo} was issued to {namn}.", [cardNo, namn]);

        Console.WriteLine($"Card issued: {cardNo}{Environment.NewLine}" +
                           "================================");

        Console.WriteLine("");

    }

    private void Borrowed(bool export)
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
            File.WriteAllText(EXPORTED_B, sb.ToString());
            Console.WriteLine($"{urval.Count} books was exported to {EXPORTED_B}");
        }
        else
        {
            Console.Clear();
            Console.Write(sb.ToString());
        }
    }



    private void AddBook()
    {
            Console.Clear();
            Console.WriteLine("======================================" +
                             $"{Environment.NewLine}ADD BOOKS{Environment.NewLine}" +
                             $"======================================");

            try
            {
                ulong isbn = InputControl.AskForULong("ISBN");
                string author = InputControl.AskForString("Author");
                string title = InputControl.AskForString("Title");
                Category cat = (Category)InputControl.AskForEnum(typeof(Category));

                Book book = new Book(isbn, author, title, cat);
                MyLibrary.AddBook(book);

                Console.WriteLine("");
                Console.WriteLine($"The book was added.");
            _logger.Log(LogLevel.Information, "The book \"{title}\" was added", book.Title);

            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(e.Message);
            }
    }


    private void DeleteBook()
    {
        int idxCurrent = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("================================" +
                             $"{Environment.NewLine}Select the book to delete and press <delete>" +
                             $"{Environment.NewLine}or press <Q> or <T> to enter isbn/title for the book to delete." +
                             $"{Environment.NewLine}================================");

            List<Book> List4Delete = MyLibrary.GetBooks().ToList();
            ShowListWithCurrentRowSelected(List4Delete, idxCurrent);
            //foreach ((int, Book) tup in List4Delete.Index())
            //{
            //    if (idxCurrent == tup.Item1)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        Console.WriteLine($"{tup.Item2}");
            //        Console.ResetColor();
            //    }
            //    else
            //        Console.WriteLine($"{tup.Item2}");
            //}
            //Console.WriteLine("");
            //Console.WriteLine($"{List4Delete.Count()} Books was found.");

            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.X
             || key.Key == ConsoleKey.Escape)
                break;

            //Let the user navigate in the list above.
            (bool, int) ArrowAction = ArrowHandled(List4Delete, idxCurrent, key.Key );
            if (ArrowAction.Item1)
            {
                idxCurrent = ArrowAction.Item2;
                continue;
            }

            if (key.Key == ConsoleKey.Q)
            {
                ulong isbn = InputControl.AskForULong("ISBN");
                bool status = MyLibrary.DeleteBookByIsbn(isbn);
                Console.WriteLine($"The book was {((status) ? "removed." : "not removed.")} ");
                if (status) _logger.Log(LogLevel.Information, "The book with isbn {isbn} was deleted", isbn);
                continue;
            }
            if (key.Key == ConsoleKey.T)
            {
                string title = InputControl.AskForString("Title");
                bool status = MyLibrary.DeleteBookByTitle(title);
                Console.WriteLine($"The book was {((status) ? "removed." : "not removed.")} ");
                if (status) _logger.Log(LogLevel.Information, "The book with title {title} was deleted.", title);
                continue;
            }


            if (key.Key == ConsoleKey.Delete)
            {
                Book b = List4Delete.ElementAt(idxCurrent);
                if (b != null)
                {
                    bool status = MyLibrary.DeleteBook(b);
                    if (status) _logger.Log(LogLevel.Information, "{title} was deleted.", b.Title);
                }
            }

        } while (true);

    }

    private void ShowListWithCurrentRowSelected(List<Book> list, int idxCurrent)
    {
        foreach ((int, Book) tup in list.Index())
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
        Console.WriteLine($"{list.Count()} Books was found.");
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
            Console.WriteLine("================================" +
                             "Select the book to return and press" +
                             $"{Environment.NewLine}<enter> or <space> to select it.{Environment.NewLine}" +
                             "================================");

            //List all the books for the user to choose from.
            ShowListWithCurrentRowSelected(myBorrowedBooks, idxCurrent);
            //foreach ((int, Book) tup in myBorrowedBooks.Index())
            //{
            //    if (idxCurrent == tup.Item1)
            //    {   //Currently selected index is color-marked
            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        Console.WriteLine($"{tup.Item1} {tup.Item2}");
            //        Console.ResetColor();
            //    }
            //    else
            //        Console.WriteLine($"{tup.Item1} {tup.Item2}");
            //}
            //Console.WriteLine("");
            //Console.WriteLine($"{myBorrowedBooks.Count} Books was found.");

            //Let the user navigate in the list above by changing the current index.
            ConsoleKeyInfo key = Console.ReadKey();
            //Let the user navigate in the list above.
            (bool, int) ArrowAction = ArrowHandled(myBorrowedBooks, idxCurrent, key.Key);
            if (ArrowAction.Item1)
            {
                idxCurrent = ArrowAction.Item2;
                continue;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter
            || key.Key == ConsoleKey.Spacebar)
            {
                Book selectedBook = myBorrowedBooks.ElementAt(idxCurrent);

                //mark the selected book as returned.
                if (selectedBook != null)
                {
                    bool status = MyLibrary.ReturnBook(selectedBook);
                    if (status) _logger.Log(LogLevel.Debug, "{name} returned {title}", card.Name, selectedBook.Title);

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
            Console.WriteLine("================================" +
                             $"{Environment.NewLine}Select the book to borrow and press <enter> or <space> {Environment.NewLine}" +
                              "================================");

            //List of all books not already borrowed.
            List<Book> booksAtHand = MyLibrary.GetBooks().Where(b => b.IsOnLoan == false).ToList();
            ShowListWithCurrentRowSelected(booksAtHand, idxCurrent);

            //Let the user navigate in the list above by changing the current index.
            ConsoleKeyInfo key = Console.ReadKey();

            (bool, int) ArrowAction = ArrowHandled(booksAtHand, idxCurrent, key.Key);
            if (ArrowAction.Item1)
            {
                idxCurrent = ArrowAction.Item2;
                continue;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter
            || key.Key == ConsoleKey.Spacebar)
            {
                Book selectedBook = booksAtHand.ElementAt(idxCurrent);
                //mark the selected book as borrowed.
                if (selectedBook != null)
                {
                    bool status = MyLibrary.BorrowBook(selectedBook, card.ID);
                    if (status) _logger.Log(LogLevel.Debug, "{name} borrowed {Title}.", card.Name, selectedBook.Title);
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
        File.WriteAllText(LibraryApp.BOOKS_FILE, MyLibrary.GetJsonFromBooks());
        Console.WriteLine($"{MyLibrary.GetBooks().Count()} books was saved to file.");
    }



}
