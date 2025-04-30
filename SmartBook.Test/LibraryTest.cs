using System;
using System.Threading;
using SmartBook.SmartBookCore;
using Xunit;
namespace SmartBook.Test;

public class LibraryTest
{

    [Fact]
    public void AllowInsertOfBook()
    {
        //Arrange
        Book b = new(1,"Author","Title",Category.Novels);
        Library lib = new Library();
        
        //Act
        lib.AddBook( b );

        //Assert
        Assert.Contains( b, lib.GetBooks());
    }

    [Fact]
    public void AllowDeleteOfBook()
    {
        //Arrange
        Book b = new(1, "Author", "Title", Category.Novels);
        Library lib = new Library();
        lib.AddBook(b);

        //Act
        lib.DeleteBook(b);
        //lib.DeleteBookByIsbn(1);
        //lib.DeleteBookByTitle("Muren och stranden");

        //Assert
        Assert.DoesNotContain(b, lib.GetBooks());
    }
    [Fact]
    public void AllowDeleteOfBookByIsbn()
    {
        //Arrange
        Library lib = new Library();
        Seed(lib);
        Book b = lib.GetBooks().Where(b => b.ISBN== 9117430925).FirstOrDefault();

        //Act
        lib.DeleteBookByIsbn(9117430925);

        //Assert
        Assert.DoesNotContain(b, lib.GetBooks());
    }
    [Fact]
    public void AllowDeleteOfBookByTitle()
    {
        //Arrange
        Library lib = new Library();
        Seed(lib);
        Book b = lib.GetBooks().Where(b => b.ISBN == 9117430925).FirstOrDefault();

        //Act
        lib.DeleteBookByTitle("Muren och stranden");

        //Assert
        Assert.DoesNotContain(b, lib.GetBooks());
    }



    [Fact]
    public void NotAllowDuplicateISBN()
    {
        //Arrange
        //Same ISBN
        Book b1 = new(1, "Author", "Title", Category.Novels);
        Book b2 = new(1, "Author2", "Title2", Category.History);
        Library lib = new Library();
        lib.AddBook(b1); 

        //Act & Assign
        var ex = Assert.Throws<ArgumentException>("ISBN", () => lib.AddBook(b2));
        Assert.Equal("This book already exist in this library! (Parameter 'ISBN')", ex.Message);
    }

    [Fact]
    public void CanSaveBooksToJson()
    {
        //Arrange
        Library lib = new Library();
        Seed(lib);

        //Act
        string json2Save = lib.GetJsonFromBooks();
        
        //Assert
        Assert.Equal(Json(), json2Save);
    }


    [Fact]
    public void CanLoadBooksFromJson()
    {
        //Arrange
        Library lib = new Library();

        //Act
        lib.LoadBooksFromJson(Json());

        //Assert
        Assert.NotEmpty(lib.GetBooks());
        Assert.Collection(lib.GetBooks(), Actions4Test());
    }






    private void Seed(Library lib)
    {
        lib.AddBook(new Book(3829030428, "Könemann, Ludwig", "Egypten Faraonernas värld", Category.History));
        lib.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
    }
    private string Json()
    {
        return "[{\"ISBN\":3829030428,\"Author\":\"K\\u00F6nemann, Ludwig\",\"Title\":\"Egypten Faraonernas v\\u00E4rld\",\"Category\":5,\"IsOnLoan\":false,\"BorrowedBy\":0}," +
                "{\"ISBN\":9117430925,\"Author\":\"Zassenhaus, Hiltgunt\",\"Title\":\"Muren och stranden\",\"Category\":4,\"IsOnLoan\":false,\"BorrowedBy\":0}]";
    }
    private Action<Book>[] Actions4Test()
    {
        Action<Book>[] actions = new Action<Book>[2];
        actions[0] = a => { Assert.IsType<Book>(a); Assert.Equal(3829030428ul, a.ISBN); };
        actions[1] = a => { Assert.IsType<Book>(a); Assert.Equal(9117430925ul, a.ISBN); };
        return actions;
    }
}
