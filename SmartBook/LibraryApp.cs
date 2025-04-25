using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SmartBook.Helpers;
using SmartBook.SmartBookCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace SmartBook;
/*
 * Menus for the App
*/
internal class LibraryApp
{
    private Library myLibrary;
    private LibraryCardHandler myCardHandler;
    public LibraryApp()
    {
        myLibrary = new Library();
        myCardHandler = new LibraryCardHandler();

        LoadFromFile();
        if(myLibrary.GetBooks().Count() == 0)
            seed();
    }

    public void seed()
    {
        myLibrary.AddBook(new Book(2, "aaa", "aaa", Category.Novels));
        myLibrary.AddBook(new Book(3, "eee", "eee", Category.Novels));
        myLibrary.AddBook(new Book(4, "www", "qqq", Category.Novels));
        myLibrary.AddBook(new Book(5, "ppp", "ppp", Category.Novels));
        myLibrary.AddBook(new Book(1, "bbb", "bbb", Category.Novels));
        myLibrary.AddBook(new Book(0, "hhh", "hhh", Category.Novels));
        myLibrary.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
        //91 - 1 - 743092 - 5
    }
    #region Huvudmeny
    // Menyalternativ för huvudmenyn
    private enum MainMenuOptions
    {
        ADD,
        DELETE,
        LIST,
        FIND,
        GETCARD,
        ON_LOAN,
        SAVE,
        EXIT
    }
     // Ledtexter för ovanstående menyalternativ
    private Dictionary<MainMenuOptions, string> MainMenuText = new Dictionary<MainMenuOptions, string>()
    {
        { MainMenuOptions.ADD,     "[A] Add a book" },
        { MainMenuOptions.DELETE,  "[D] Delete a book"},
        { MainMenuOptions.LIST,    "[L] List all the books"},
        { MainMenuOptions.FIND,    "[F] Find a book"},
        { MainMenuOptions.GETCARD, "[C] Get new library card."},
        { MainMenuOptions.ON_LOAN, "[W] Show books on loan"},
        { MainMenuOptions.SAVE,    "[S] Save all"},
        { MainMenuOptions.EXIT,    "[X] Exit"},

    };
    //Hotkeys för ovanstående menyalternativ
    private Dictionary<ConsoleKey, MainMenuOptions> MainMenuHotkeys = new Dictionary<ConsoleKey, MainMenuOptions>()
    {
        { ConsoleKey.A,         MainMenuOptions.ADD },
        { ConsoleKey.D,         MainMenuOptions.DELETE },
        { ConsoleKey.L,         MainMenuOptions.LIST },
        { ConsoleKey.F,         MainMenuOptions.FIND },
        { ConsoleKey.C,         MainMenuOptions.GETCARD },
        { ConsoleKey.W,         MainMenuOptions.ON_LOAN },
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
        ConsoleKeyInfo enter = new ConsoleKeyInfo(' ', ConsoleKey.Enter, false, false, false);
        bool bAvsluta = false;
        MainMenuOptions selected = MainMenuOptions.ADD;
        do
        {
            ShowMainMenu(selected);
            ConsoleKeyInfo cki = Console.ReadKey();

            //Menu-navigation w arrows
            (bool, int) ArrowAction = ArrowHandled(typeof(MainMenuOptions), (int)selected, cki.Key);
            if(ArrowAction.Item1)
            {
                ShowMainMenu(selected = (MainMenuOptions)ArrowAction.Item2);
                continue;
            }

            //Enter or hotkey for accept
            if (cki.Key == ConsoleKey.Enter
            || MainMenuHotkeys.ContainsKey(cki.Key) )
            {
                //Set selected row according to used hotkey
                if(cki.Key != ConsoleKey.Enter)
                selected = MainMenuHotkeys.GetValueOrDefault(cki.Key);

                switch (selected)
                {
                    case MainMenuOptions.ADD:
                        AddBook();
                        break;

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
                        getNewCard();
                        
                        break;

                    case MainMenuOptions.ON_LOAN:
                        BorrowBook();
                        OnLoan();
                        break;

                    case MainMenuOptions.SAVE:
                        SaveToFile();
                        break;

                    case MainMenuOptions.EXIT:
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

    //Menu navigation with arrowkeys - used in multiple menus.
    private (bool, int) ArrowHandled(Type enumType, int selected, ConsoleKey key)
    {
        //Move selection up
        if (key == ConsoleKey.UpArrow
        ||  key == ConsoleKey.LeftArrow)
        {
            selected--;
            if (!Enum.IsDefined(enumType, selected))
                selected = (int)Enum.GetValues(enumType)
                      .GetValue(Enum.GetValues(enumType).Length-1);
            return (true, selected);
        }

        //Move selection down
        if (key == ConsoleKey.DownArrow
        ||  key == ConsoleKey.RightArrow)
        {
            selected++;
            if (!Enum.IsDefined(enumType, selected))
                selected = (int)Enum.GetValues(enumType).GetValue(0);

            return(true, selected);
        }
        return (false, selected);
    }


    private void ListBooks()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"List of books ");
        Console.WriteLine("================================");

        foreach (Book book in myLibrary.GetBooks())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("");
        Console.WriteLine($"{myLibrary.GetBooks().Count} books found.");
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
    }
    private void FindBooks()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"Search for Books (use * wildcard)");
        Console.WriteLine("================================");
        string author = InputControl.AskForString("Author");
        string title = InputControl.AskForString("Title");

        List<Book> urval = myLibrary.FindBooks(author, title);
        Console.WriteLine();
        foreach (Book book in urval)
        {
            Console.WriteLine(book);
        }
        Console.WriteLine("");
        Console.WriteLine($"{urval.Count} Books found.");
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();

    }

    private void getNewCard()
    {
        ulong cardNo = myCardHandler.getNewCard();

        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"Card generated: {cardNo} ");
        Console.WriteLine("================================");

        Console.WriteLine("");
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();

    }

    private void OnLoan()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine($"Books OnLoan");
        Console.WriteLine("================================");
        var urval = myLibrary.GetBooks()
            .Where(b => b.IsOnLoan == true)
            .ToList();

        foreach (Book book in urval)
            Console.WriteLine(book);

        Console.WriteLine("");
        Console.WriteLine($"{urval.Count} Books found.");
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();

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
                myLibrary.AddBook(book);

                iAdderat++;
                Console.WriteLine("");
                Console.WriteLine($"{iAdderat} book(s) added.");

            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("X to exit. Press any key to continue...");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.X)
                bAvsluta = true;

        } while (!bAvsluta);
    }


    private void DeleteBook()
    {
        int idxCurrent = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("Select the book to delete and press <delete>");
            Console.WriteLine("================================");
            foreach ((int, Book) tup in myLibrary.GetBooks().Index())
            {
                if (idxCurrent == tup.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
            }
            Console.WriteLine("");
            Console.WriteLine($"{myLibrary.GetBooks().Count} Books was found.");

            //Let the user navigate in the list above.
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
                if (idxCurrent < myLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Delete)
            {
                myLibrary.DeleteBook(myLibrary.GetBooks().ElementAt(idxCurrent));
            }

        } while (true);

        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
    }

    private void BorrowBook()
    {
        int idxCurrent = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("Select the book to borrow and press <enter>");
            Console.WriteLine("================================");
            foreach ((int, Book) tup in myLibrary.GetBooks().Index())
            {
                if (idxCurrent == tup.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"{tup.Item1} {tup.Item2}");
            }
            Console.WriteLine("");
            Console.WriteLine($"{myLibrary.GetBooks().Count} Books was found.");

            //Let the user navigate in the list above.
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
                if (idxCurrent < myLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter)
            {
                myLibrary.BorrowBook(myLibrary.GetBooks().ElementAt(idxCurrent), 777 );
            }

        } while (true);

        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
    }


    private void LoadFromFile()
    {
        myLibrary.LoadFromFile();
    }
    private void SaveToFile()
    {
        myLibrary.SaveToFile();
    }



}
