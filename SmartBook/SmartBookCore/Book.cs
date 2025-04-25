using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBook.SmartBookCore;

public record Book(ulong ISBN, string Author, string Title, Category Category, bool IsOnLoan = false)
{
    //public ulong ISBN { get; set; }
    //public string Author { get; set; }
    //public string Title { get; set; }
    //public Category Category { get; set; }
    //public bool IsOnLoan { get; set; }
    public ulong BorrowedBy { get; set; }

    //public Book(ulong isbn, string author, string title, Category cat )
    //{
    //    ISBN = isbn;
    //    Author = author;
    //    Title = title;
    //    Category = cat;
    //    IsOnLoan = false;
    //}

    public Book() : this(0,null,null,Category.Chapter_books)
    {

    }
    public Book(ulong isbn, string author, string title, Category cat, ulong borrowedBy) : this(isbn,author,title,cat, true)
    {
        BorrowedBy = borrowedBy;
    }

    public override string ToString()
    {
        return $"{Author} {Title} {Category} {ISBN} {IsOnLoan} {BorrowedBy}";
    }

    public Book checkOut(ulong id)
    {
        return new(ISBN, Author, Title, Category, id);
    }
    public Book checkIn() 
    {
        return new(ISBN, Author, Title, Category);
    }
}
