using SmartBook.Helpers;
using SmartBook.SmartBookCore;

namespace SmartBook;

internal class Program
{
    static void Main(string[] args)
    {
        WelcomeScreen();

        LibraryApp app = new LibraryApp();

        app.LoadData(); 

        app.MainMenu();
    }
    static void WelcomeScreen()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"

  _____                     _      ____               _   
/  ___|                    | |    |  _ \             | |       
| (___  __ ___   __ _ __ __| |_   | |_) | ___   ___  | |___
 \___ \ '_ ` _ \/ _` || '__| __|  |  _ < / _ \ / _ \ | '__|
 ____) | | | | | (_| || |  | |_   | |_) | (_) | (_) || |\ \ 
|_____/| |_| |_|\__,_||_|   \__|  |____/ \___/ \___/ |_| \_|

");

        Console.ResetColor();
    }
}
