using System;
using System.Threading;
using SmartBook.SmartBookCore;
using Xunit;
namespace SmartBook.Test;

public class LibraryTest
{

    [Fact]
    public void WillAllowInsertOfBook()
    {
        
        Book b = new(1,"Author","Title",Category.Novels);

        Library lib = new Library();
        lib.AddBook( b );

        Assert.Contains( b, lib.GetBooks());
    }

    [Fact]
    public void WillAllowDeletOfBook()
    {
        Book b = new(1, "Author", "Title", Category.Novels);

        Library lib = new Library();
        lib.AddBook(b);
        lib.DeleteBook(b);

        Assert.DoesNotContain(b, lib.GetBooks());
    }

    [Fact]
    public void WillNotAllowDuplicateISBN()
    {
        Book b1 = new(1, "Author", "Title", Category.Novels);
        Book b2 = new(1, "Author2", "Title2", Category.History);

        Library lib = new Library();
        lib.AddBook(b1);

        var ex = Assert.Throws<ArgumentException>("ISBN", () => lib.AddBook(b2));
        Assert.Equal("This book already exist in this library! (Parameter 'ISBN')", ex.Message);
    }

    [Fact]
    public void CanSaveBooks()
    {
        Library lib = new Library();
        Seed(lib);
        //File.Delete(lib.GetFile());
        //Assert.False(File.Exists(lib.GetFile()));

        string json2Save = lib.GetJsonFromBooks();
        //lib.SaveToFile();

        //Assert.True(File.Exists(lib.GetFile()));
        Assert.Equal(Json(), json2Save);
    }
    public void Seed(Library lib)
    {
        lib.AddBook(new Book(3829030428, "Könemann, Ludwig", "Egypten Faraonernas värld", Category.History));
        lib.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
    }
    public string Json()
    {
        return "[{\"ISBN\":3829030428,\"Author\":\"K\\u00F6nemann, Ludwig\",\"Title\":\"Egypten Faraonernas v\\u00E4rld\",\"Category\":5,\"IsOnLoan\":false,\"BorrowedBy\":0},{\"ISBN\":9117430925,\"Author\":\"Zassenhaus, Hiltgunt\",\"Title\":\"Muren och stranden\",\"Category\":4,\"IsOnLoan\":false,\"BorrowedBy\":0}]";
    }


    [Fact]
    public void CanLoadBooks()
    {
        //Arrange - skapar filen!
        CanSaveBooks();

        //Arrange
        Library lib = new Library();
        Assert.Empty(lib.GetBooks());

        //Act
        //lib.LoadFromFile();
        lib.LoadBooksFromJson(Json());

        //Assert
        Assert.NotEmpty(lib.GetBooks());
        
    }

    //public void xxx()
    //{
    //    LibraryApp
    //}

}
