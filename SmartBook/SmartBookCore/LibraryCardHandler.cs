using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace SmartBook.SmartBookCore;

public class LibraryCardHandler
{
    private ulong lastID = 0;
    private List<Card> Cards;

    public LibraryCardHandler()
    {
        lastID = 0;
        Cards = new List<Card>();
        //LoadFromFile();
    }

    public ulong getNewCard(string namn)
    {
        Cards.Add(new Card(++lastID, namn));
        //SaveToFile();
        return lastID;
    }
    public List<Card> getCards()
    {
        return Cards.ToList();
    }

    //public void LoadFromFile()
    //{
    //    if (File.Exists(LibraryApp.CARDS_FILE))
    //        Cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText(LibraryApp.CARDS_FILE));
    //    if (Cards != null && Cards.Count > 0)
    //    {
    //        lastID = Cards.Max(x => x.ID);
    //        Console.WriteLine($"{Cards.Count} Library cards was loaded. ");
    //        Console.WriteLine($"Next number will be: {lastID + 1}.");
    //    }
    //}
    public ulong LoadFromJson(string json)
    {
        ulong retur = 0;

        if (!string.IsNullOrWhiteSpace(json))
            Cards = JsonSerializer.Deserialize<List<Card>>(json);

        if (Cards != null && Cards.Count > 0)
        {
            lastID = Cards.Max(x => x.ID);
            return lastID;
        }

        return retur;
    }

    //public void SaveToFile()
    //{
    //    File.WriteAllText(LibraryApp.CARDS_FILE, JsonSerializer.Serialize(Cards));
    //}

    public string SaveToJson()
    {
        return JsonSerializer.Serialize(Cards);
    }


}
