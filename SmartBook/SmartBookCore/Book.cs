using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartBook.SmartBookCore;

public record Book(ulong ISBN, string Author, string Title, Category Category, bool IsOnLoan = false)
{
    public ulong BorrowedBy { get; set; }

    //Tom Konstruktor krävs tydligen för Json Deserialize
    public Book() : this(0,null,null,Category.Chapter_books)    {    }

    //[JsonConstructor]?
    //Alternativ konstruktor för den utlånade boken.
    public Book(ulong isbn, string author, string title, Category cat, ulong borrowedBy) : this(isbn,author,title,cat, true)
    {
        BorrowedBy = borrowedBy;
    }

    public override string ToString()
    {
        return $"{Author,-25} {Title,-35} {ISBN,-13} {Category}  {IsOnLoan} {BorrowedBy}";
    }

    public Book CheckOut(ulong id)
    {
        return new(ISBN, Author, Title, Category, id);
    }
    public Book CheckIn() 
    {
        return new(ISBN, Author, Title, Category);
    }
}
