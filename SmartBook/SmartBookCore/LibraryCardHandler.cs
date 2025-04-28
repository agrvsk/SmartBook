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
    public ulong LastID { get; private set; }
    private List<Card> Cards;

    public LibraryCardHandler()
    {
        LastID = 0;
        Cards = new List<Card>();
    }

    public ulong GetNewCard(string namn)
    {
        Cards.Add(new Card(++LastID, namn));
        return LastID;
    }
    public List<Card> GetCards()
    {
        return Cards.ToList();
    }

    public void LoadCardsFromJson(string json)
    {
        ulong retur = 0;

        if (!string.IsNullOrWhiteSpace(json))
            Cards = JsonSerializer.Deserialize<List<Card>>(json);

        if (Cards != null && Cards.Count > 0)
        {
            LastID = Cards.Max(x => x.ID);
            //return LastID;
        }

        //return retur;
    }


    public string SaveCardsToJson()
    {
        return JsonSerializer.Serialize(Cards);
    }


}
