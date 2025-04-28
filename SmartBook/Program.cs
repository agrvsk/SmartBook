using SmartBook.Helpers;
using SmartBook.SmartBookCore;

namespace SmartBook;

internal class Program
{
    static void Main(string[] args)
    {
        WelcomeScreen();
        LibraryApp app = new LibraryApp();


        //Låter anv. läsa splahs-screen samt om laddning gick bra
        Console.WriteLine("");
        Console.WriteLine("Press any key to continue...");
        ConsoleKeyInfo ki =  Console.ReadKey();

        //Console.WriteLine("************************************");
        //Console.WriteLine("Press [1] if you have a librarycard!");
        //if (ki.Key == ConsoleKey.D1 || ki.Key == ConsoleKey.NumPad1)
        //{
        //    ulong no = InputControl.AskForULong("Library Card No");
        //    Card xx = app.GetCards().Find(c=>c.ID==no);
        //    if (xx != null)
        //        app.AltMenu( xx );
        //}

        app.MainMenu();
    }
    static void WelcomeScreen()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"

  _____                     _      ____               _   
 / ____                    | |    |  _ \             | |       
| (___  __ ___   __ _ __ __| |_   | |_) | ___   ___  | |___
 \___ \ '_ ` _ \/ _` || '__| __|  |  _ < / _ \ / _ \ | '__|
 ____) | | | | | (_| || |  | |_   | |_) | (_) | (_) || |\ \ 
|_____/| |_| |_|\__,_||_|   \__|  |____/ \___/ \___/ |_| \_|

");

        Console.ResetColor();
        //Console.WriteLine("Press enter key to start logging in!");
        //Console.ReadLine();
        //Console.ReadKey();
        //Console.Clear();

    }
}
