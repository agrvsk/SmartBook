using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
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
    public static string BOOKS_FILE = "library.json";
    public static string CARDS_FILE = "cards.json";
    public static string EXPORTED_B = "book_on_loan.txt";

    private Library myLibrary;
    private LibraryCardHandler myCardHandler;

    public LibraryApp()
    {
        myLibrary = new Library();
        myCardHandler = new LibraryCardHandler();

        LoadData();

        //if (myLibrary.GetBooks().Count() == 0)
        //    seed();
    }
    public List<Card> GetCards()
    {
        return myCardHandler.getCards();
    }
    public void seed()
    {
        myLibrary.AddBook(new Book(2, "aaa", "aaa", Category.Novels));
        myLibrary.AddBook(new Book(3, "eee", "eee", Category.Novels));
        myLibrary.AddBook(new Book(4, "www", "qqq", Category.Novels));
        myLibrary.AddBook(new Book(5, "ppp", "ppp", Category.Novels));
        myLibrary.AddBook(new Book(1, "bbb", "bbb", Category.Novels));
        myLibrary.AddBook(new Book(3829030428, "Könemann, Ludwig", "Egypten Faraonernas värld", Category.History));
        myLibrary.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
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
        { AltMenuOptions.RETURN_ALL,        "[A] Return books" },
        { AltMenuOptions.RETURN_SELECTED,   "[S] Return books" },
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
        { MainMenuOptions.LIST,         "[L] List all the books"},
        { MainMenuOptions.FIND,         "[F] Find a book"},
        { MainMenuOptions.GETCARD,      "[C] Get new library card."},
        { MainMenuOptions.MENU,         "[M] Borrow / return books."},
        { MainMenuOptions.ON_LOAN_SHOW, "[W] Show books on loan"},
        { MainMenuOptions.ON_LOAN_SAVE, "[P] export books on loan"},
        { MainMenuOptions.SAVE,         "[S] Save all"},
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
                        GenerateLibraryCard();
                        break;

                    case MainMenuOptions.MENU:
                        Menu();
                        break;

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
                        break;

                    default:
                        Console.WriteLine("Menu option not implemented! Press any key...");
                        Console.ReadKey();
                        break;
                }
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
            bool alla = true;
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
                        ReturnBooks(card, alla );
                        break;

                    case AltMenuOptions.RETURN_SELECTED:
                        ReturnBooks(card, !alla);
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

    private void GenerateLibraryCard()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("Library Card Application:");
        Console.WriteLine("================================");

        string namn = InputControl.AskForString("Name");
        ulong cardNo = myCardHandler.getNewCard(namn);

        string jsontext = myCardHandler.SaveToJson();
        File.WriteAllText(LibraryApp.CARDS_FILE, jsontext);

        Console.WriteLine("");
        Console.WriteLine("================================");
        Console.WriteLine($"{namn}, you got Card no: {cardNo} ");
        Console.WriteLine("================================");

        Console.WriteLine("");
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();

    }

    private void OnLoan(bool export)
    {
        StringBuilder sb = new StringBuilder();
        sb = sb.AppendLine("================================");
        sb = sb.AppendLine("Books OnLoan");
        sb = sb.AppendLine("================================");

        var urval = myLibrary.GetBooks()
            .Where(b => b.IsOnLoan == true)
            .ToList();

        foreach (Book book in urval)
            sb = sb.AppendLine(book.ToString());

        sb = sb.AppendLine();
        sb = sb.AppendLine($"{urval.Count} Books found.");


        if (export)
        {
            File.WriteAllText(EXPORTED_B, sb.ToString() );
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
            Console.WriteLine("Select the book to delete and press <delete> ");
            Console.WriteLine("or press <Q> to enter isbn for the book to delete.");
            Console.WriteLine("================================");

            foreach ((int, Book) tup in myLibrary.GetBooks().Index())
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
            Console.WriteLine($"{myLibrary.GetBooks().Count} Books was found.");

            //Enter ISBN delete the book and end the loop
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
            {
                ulong isbn = InputControl.AskForULong("ISBN");
                bool status = myLibrary.DeleteBook(isbn);
                Console.WriteLine($"The book was {((status) ? "removed." : "not removed.")} ");
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
                if (idxCurrent < myLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Delete)
            {
                Book b = myLibrary.GetBooks().ElementAt(idxCurrent);
                if(b != null)
                myLibrary.DeleteBook(b.ISBN);
            }

        } while (true);

        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
    }
    private void ReturnBooks(Card card, bool alla)
    {
        int idxCurrent = 0;
        List<Book> myBorrowedBooks = myLibrary.GetBooks()
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
                if (myLibrary.ReturnBook(b))
                    returned++;
            }
            Console.WriteLine($"{returned} of {total} books was returned.");
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
            Console.WriteLine("press <enter> or <space> to select it.");
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
                    myLibrary.ReturnBook(selectedBook);
                    myBorrowedBooks = myLibrary.GetBooks()
                                    .Where(b => b.BorrowedBy == card.ID).ToList();
                }
            }




        } while (true);
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
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
            foreach ((int, Book) tup in myLibrary.GetBooks().Index())
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
            Console.WriteLine($"{myLibrary.GetBooks().Count} Books was found.");

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
                if (idxCurrent < myLibrary.GetBooks().Count() - 1)
                    idxCurrent++;
            }

            if (key.Key == ConsoleKey.X
            || key.Key == ConsoleKey.Escape)
                break;

            if (key.Key == ConsoleKey.Enter
            ||  key.Key == ConsoleKey.Spacebar)
            {
                Book selectedBook = myLibrary.GetBooks().ElementAt(idxCurrent);
                //mark the selected book as borrowed.
                if(selectedBook != null)
                myLibrary.BorrowBook(selectedBook, card.ID);
            }

        } while (true);

        //Console.WriteLine("press any key to continue...");
        //Console.ReadKey();
    }


    private void LoadData()
    {
        myLibrary.LoadFromFile();
        Console.WriteLine($"{myLibrary.GetBooks().Count} books was loaded from file.");
        
        if (myLibrary.GetBooks().Count() == 0)
            seed();


        if (File.Exists(LibraryApp.CARDS_FILE))
        {
            string json = File.ReadAllText(LibraryApp.CARDS_FILE);
            ulong lastNo = myCardHandler.LoadFromJson(json);
            if (lastNo > 0)
            {
                Console.WriteLine($"{lastNo} Library cards was loaded. ");
                Console.WriteLine($"Next number will be: {lastNo + 1}."); //It is not possible to delete cards.
            }
        }



    }
    private void SaveToFile()
    {
        myLibrary.SaveToFile();
        Console.WriteLine($"{myLibrary.GetBooks().Count} books was saved to file.");
        Console.WriteLine($"Press any key to continue...");
        Console.ReadKey();
    }



}
