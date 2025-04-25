using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SmartBook.Helpers;
using static SmartBook.LibraryApp;

namespace SmartBook.SmartBookCore;

public class Library
{
    private List<Book> Books { get; set; }

    public Library()
    {
        Books = new List<Book>();
    }

    public void LoadFromFile()
    {
        Books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText("library.json"));
    }
    public void SaveToFile()
    {
        File.WriteAllText("library.json", JsonSerializer.Serialize(Books));
    }
    public string GetFile()
    {
        return "library.json";
    }


    public void AddBook(Book book)
    {
        Validate(book);
        Books.Add(book);
    }
    public void Validate(Book book)
    {
        if (Books.Find(b => b.ISBN == book.ISBN) != null)
            throw new ArgumentException("This book already exist in this library!", "ISBN");
    }
    public bool BorrowBook(Book book, ulong borrowedBy)
    {
        bool retur = false;
        Book borrowed = book.checkOut(borrowedBy);
        if (borrowed == null) return retur;
        if (Books.Remove(book))
        {
            Books.Add(borrowed);
            return true;
        }
        return retur;
    }
    public bool ReturnBook(Book returned)
    {
        bool retur = false;
        Book book = returned.checkIn();
        if (book == null) return retur;

        if (Books.Remove(returned))
        {
            Books.Add(book);
            return true;
        }

        return retur;
    }



    public void DeleteBook(Book book)
    {
        Books.Remove(book);
    }

    public List<Book> GetBooks()
    {
        Books.Sort( (x,y) => x.Author.CompareTo(y.Author));
        return Books.ToList();
    }

    public List<Book> FindBooks(string author, string title)
    {
        string[] author_parts = author.Split(['*']);
        string[] title_parts = title.Split(['*']);
        int author_max = author_parts.Length - 1;
        int title_max = title_parts.Length - 1;

        //Start with a copy and narrow it down...
        List<Book> urval = Books.ToList();

        if (author_parts[0].Trim().Length > 0)
        {
            urval = urval.Where(a => a.Author.StartsWith(author_parts[0])).ToList();
        }
        for (int i = 1; i < author_max; i++)
        {
            urval = urval.Where(a => a.Author.Contains(author_parts[i])).ToList();
        }
        if (author_parts[author_max].Trim().Length > 0)
        {
            urval = urval.Where(a => a.Author.EndsWith(author_parts[author_max])).ToList();
        }

        if (title_parts[0].Trim().Length > 0)
        {
            urval = urval.Where(t => t.Title.StartsWith(title_parts[0])).ToList();
        }
        for (int i = 1; i < title_max; i++)
        {
            urval = urval.Where(t => t.Title.Contains(title_parts[i])).ToList();
        }
        if (title_parts[title_max].Trim().Length > 0)
        {
            urval = urval.Where(a => a.Title.EndsWith(title_parts[title_max])).ToList();
        }
        return urval;
    }





}
