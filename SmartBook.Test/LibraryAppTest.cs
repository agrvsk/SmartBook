using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBook.SmartBookCore;
using SmartBook;

namespace SmartBook.Test;

public class LibraryAppTest
{
    [Fact]
    public void CanSaveBooks()
    {
        LibraryApp lib = new LibraryApp();

        File.Delete(LibraryApp.BOOKS_FILE);
        Assert.False(File.Exists(LibraryApp.BOOKS_FILE));

        Seed(lib.MyLibrary);
        lib.SaveToFile();

        Assert.True(File.Exists(LibraryApp.BOOKS_FILE));
        Assert.Equal(Json(), File.ReadAllText(LibraryApp.BOOKS_FILE));
    }

    [Fact]
    public void CanLoadBooks()
    {
        //Arrange
        File.WriteAllText(LibraryApp.BOOKS_FILE, Json() );
        LibraryApp lib = new LibraryApp();

        //Act
        lib.LoadBooks();

        //Assert
        Assert.NotEmpty(lib.MyLibrary.GetBooks());

        Assert.Collection(lib.MyLibrary.GetBooks(), Actions4Test() 
        //e => { Assert.Equal(3829030428ul, e.ISBN); },
        //e => { Assert.Equal(9117430925ul, e.ISBN); }
        );
    }
    [Fact]
    public void CanNotLoadBooksIfNoFile()
    {
        //Arrange
        LibraryApp lib = new LibraryApp();
        File.Delete(LibraryApp.BOOKS_FILE);

        //Act

        //Assert
        var ex = Assert.Throws<Exception>( () => lib.LoadBooks() );

        Assert.Equal("Could not read library.json.", ex.Message);

        Assert.Empty(lib.MyLibrary.GetBooks());

    }

    private void Seed(Library lib)
    {
        lib.AddBook(new Book(3829030428, "Könemann, Ludwig", "Egypten Faraonernas värld", Category.History));
        lib.AddBook(new Book(9117430925, "Zassenhaus, Hiltgunt", "Muren och stranden", Category.Biographies_and_memoirs));
    }
    private static string Json()
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
