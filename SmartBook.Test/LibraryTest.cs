using System;
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
        seed(lib);
        File.Delete(lib.GetFile());

        Assert.False(File.Exists(lib.GetFile()));
        lib.SaveToFile();
        Assert.True(File.Exists(lib.GetFile()));
    }
    public void seed(Library lib)
    {
        lib.AddBook(new(1, "Author", "Title", Category.Novels));
        lib.AddBook(new(2, "Author2", "Title2", Category.History));
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
        lib.LoadFromFile();

        //Assert
        Assert.NotEmpty(lib.GetBooks());
        
    }

}
