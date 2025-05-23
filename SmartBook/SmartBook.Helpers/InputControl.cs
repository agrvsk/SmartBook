﻿/*
 Helperklass för användarinmatning
 */
namespace SmartBook.Helpers;
public class InputControl
{
    /**
     * @input Prompt
     * @retur en validerad ifylld sträng. 
    **/
    public static string AskForString(string prompt)
    {
        do
        {
            Console.Write($"Enter {prompt}: ");
            string? retur = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(retur))
                return retur;

            Console.WriteLine("Not a valid value. Try again...");

        } while (true);
    }


    /*
     @input Prompt
     @retur En ulong
     */
    public static ulong AskForULong(string prompt)
    {
        do
        {
            if (ulong.TryParse(AskForString(prompt), out ulong retur))
                return retur;

            Console.WriteLine("Not a valid number. Try again...");

        } while (true);
    }
    /*
     @input Prompt
     @retur En validerad int
     */
    public static int AskForInt(string prompt)
    {
        do
        {
            if (int.TryParse(AskForString(prompt), out int retur))
                return retur;

            Console.WriteLine("Not a valid number. Try again...");

        } while (true);
    }
    /*
    @input Prompt
    @retur En int som  motsvarar ett alternativ i enum-listan
    */
    public static Object AskForEnum(Type type)
    {
        Console.WriteLine();
        foreach (var item in Enum.GetValues( type ))
            Console.WriteLine((int)item + " " + item);

        Console.WriteLine();
        do {
            int i = AskForInt(type.Name);

            if (Enum.IsDefined(type, i))
                return Enum.GetValues(type).GetValue(i);

        } while (true);
    }

}
