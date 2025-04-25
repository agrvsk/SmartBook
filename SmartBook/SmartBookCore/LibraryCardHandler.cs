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
    private List<ulong> Cards;

    public LibraryCardHandler()
    {
        lastID = 0;
        Cards = new List<ulong>();
        //LoadFromFile();
        //lastID = max()..
    }

    public ulong getNewCard()
    {
        Cards.Add(++lastID);
        return lastID;
    }

    //public void LoadFromFile()
    //{
    //    LibraryCards = JsonSerializer.Deserialize<List<ulong>>(File.ReadAllText("cards.json"));
    //}
    //public void SaveToFile()
    //{
    //    File.WriteAllText("cards.json", JsonSerializer.Serialize(LibraryCards));
    //}
}
