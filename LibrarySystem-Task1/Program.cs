using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Borrower
{
    public int userid { get; set; }
    public Borrower(int id)
    {
        userid = id;
    }
}

class Library
{
    private List<Book> books;
    public Library()
    {
        books = new List<Book>();
    }

    public void AddBook(string title, string author, string isbn, int copyCount)
    {
        Book newBook = new Book(title, author, isbn, copyCount);
        books.Add(newBook);
        Console.WriteLine($"Kitap basariyla eklendi Başlik: {newBook.Title}, Yazar: {newBook.Author}, ISBN: {newBook.ISBN}, Kopya Sayisi: {newBook.CopyCount}");
    }

    public void DisplayBooks()
    {
        Console.WriteLine("Kütüphanedeki Kitaplar:");
        foreach (Book book in books)
        {
            Console.WriteLine($"Başlik: {book.Title}, Yazar: {book.Author}, ISBN: {book.ISBN}, Kopya Sayisi: {book.CopyCount}");
        }
    }

    public void BorrowBook(string title, string author, int userid)
    {
        Book selectedBook = books.Find(book => book.Title == title && book.Author == author);
        int copy = selectedBook.CopyCount;
        int borrowed = selectedBook.BorrowedCount;

        if (selectedBook == null)
        {
            Console.WriteLine("Belirtilen kitap bulunamadi.");
            return;
        }

        if ((copy - borrowed) == 0)
        {
            Console.WriteLine("Butun kitaplar odunc alinmis lutfen daha sonra tekrar deneyiniz.");
            return;
        }

        selectedBook.BorrowedCount = selectedBook.BorrowedCount + 1;
        DateTime dueDate = DateTime.Now.AddDays(14);
        selectedBook.DueDates.Add(userid, dueDate);
        Console.WriteLine($"Kitap basariyla odunc alindi. Odunc alan: {userid}, Iade Tarihi: {dueDate}");
    }

    public void ReturnBook(string title, string author, int userid)
    {
        Book selectedBook = books.Find(book => book.Title == title && book.Author == author);

        if (selectedBook == null)
        {
            Console.WriteLine("Belirtilen kitap bulunamadi.");
            return;
        }
        if (selectedBook.DueDates.ContainsKey(userid))
        {
            selectedBook.DueDates.Remove(userid);
            Console.WriteLine($"Kullanici ID {userid} için odunc kaydi silindi.");
            selectedBook.BorrowedCount = selectedBook.BorrowedCount - 1;
        }
        else
        {
            Console.WriteLine($"Kullanici ID {userid} için kayit bulunamadi.");
            return;
        }
    }

    public void Search(string key)
    {
        List<Book> searchedBooks = new List<Book>();

        foreach (var book in books)
        {
            if (book.Title == key || book.Author == key)
            {
                searchedBooks.Add(book);
            }
        }

        if (searchedBooks.Count > 0)
        {
            foreach (var book in searchedBooks)
            {
                Console.WriteLine($"{book.Title} - {book.Author} - Kutuphanede mevcuttur");
            }
        }
        else
        {
            Console.WriteLine($"Belirtilen anahtar ile eşleşen kitap bulunamadı.");
        }
    }

    public void OutdatedBooks()
    {
        foreach (var book in books)
        {
            foreach (var dueDateEntry in book.DueDates)
            {
                if (dueDateEntry.Value < DateTime.Now)
                {
                    Console.WriteLine($"Kullanici {dueDateEntry.Key} tarafindan odunc alinan '{book.Title}' kitabi teslim etmede gecikmistir. Son Tarih: {dueDateEntry.Value}");
                }
            }
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int CopyCount { get; set; }
    public int BorrowedCount { get; set; }
    public Dictionary<int, DateTime> DueDates { get; set; }

    public Book(string title, string author, string isbn, int copyCount)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        CopyCount = copyCount;
        BorrowedCount = 0;
        DueDates = new Dictionary<int, DateTime>();
    }
}


class LibrarySystem
{
    static void Main()
    {
        Library library = new Library();

        while (true)
        {
            Console.WriteLine("\n Uyari! Bu sistemde Turkce karakter kullanmamaniz onemle rica olunur.");
            Console.WriteLine("\nKUTUPHANE YONETIM SISTEMI");
            Console.WriteLine("1. Yeni Kitap Ekle");
            Console.WriteLine("2. Tum Kitaplari Listele");
            Console.WriteLine("3. Kitap Ara");
            Console.WriteLine("4. Kitap Odunç Al");
            Console.WriteLine("5. Kitap Iade Et");
            Console.WriteLine("6. Suresi Gecmis Kitaplari Goruntule");
            Console.WriteLine("7. Cikis");
            Regex regex = new Regex("^[a-zA-Z ]+$");
            Regex regexwnum = new Regex("^[a-zA-Z0-9 ]+$");

            Console.Write("Gerceklestirmek istediginiz islemin numarasini giriniz: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    Console.Write("Eklenecek kitabin basligi: ");
                    string newTitle = Console.ReadLine()??"";                    
                    Console.Write("Eklenecek kitabin yazari: ");
                    string newAuthor = Console.ReadLine() ?? "";
                    Console.Write("Kitabin ISBN nosunu giriniz: ");
                    string newISBN = Console.ReadLine() ?? "";
                    Console.Write("Kitaptan kac tane eklendigini giriniz: ");

                    if (int.TryParse(Console.ReadLine(), out int newCopyCount)==false)
                    {
                        Console.Write("Kitaptan kac tane eklediginizi sayi olarak yazip tekrar deneyiniz.");
                        break;
                    }

                    if(regexwnum.IsMatch(newTitle)==false || regexwnum.IsMatch(newISBN) == false)
                    {
                        Console.Write("Baslikta ve ISBN numarasinda yalnizca harf veya sayi kullanabilirsiniz, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }
                    if (regex.IsMatch(newAuthor) == false)
                    {
                        Console.Write("Yazar isminde yalnizca harf, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }

                    library.AddBook(newTitle, newAuthor, newISBN, newCopyCount);
                    break;

                case "2":
                    library.DisplayBooks();
                    break;

                case "3":
                    Console.Write("Aranacak kelimeyi giriniz: ");
                    string searchKeyword = Console.ReadLine() ?? "";

                    if (regexwnum.IsMatch(searchKeyword) == false)
                    {
                        Console.Write("Gecersiz anahtar kelime girdiniz, degerin harf ve sayi disinda karakter icermediginden emin olduktan sonra tekrar deneyiniz.");
                        break;
                    }
                    library.Search(searchKeyword);
                    break;

                case "4":
                    Console.Write("Odunc almak istediginiz kitabin basligini giriniz: ");
                    string borrowTitle = Console.ReadLine() ?? "";
                    Console.Write("Odunc almak istediginiz kitabin yazarini giriniz: ");
                    string borrowAuthor = Console.ReadLine() ?? "";
                    Console.Write("Kitabi odunc alan kisinin kullanici numarasini giriniz: ");

                    if (int.TryParse(Console.ReadLine(), out int borrowerId) == false)
                    {
                        Console.Write("Uye numarasi rakamlardan olusur, gecerli bir uye numarasi girip tekrar deneyiniz.");
                        break;
                    }

                    if (regexwnum.IsMatch(borrowTitle) == false)
                    {
                        Console.Write("Baslikta yalnizca harf veya sayi kullanabilirsiniz, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }
                    if (regexwnum.IsMatch(borrowAuthor) == false )
                    {
                        Console.Write("Yazar isminde yalnizca harf kullanabilirsiniz, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }

                    library.BorrowBook(borrowTitle, borrowAuthor, borrowerId);
                    break;

                case "5":
                    Console.Write("Iade etmek istediginiz kitabin basligini giriniz: ");
                    string returnTitle = Console.ReadLine() ?? "";
                    Console.Write("Iade etmek istediginiz kitabin yazarini giriniz: ");
                    string returnAuthor = Console.ReadLine() ?? "";
                    Console.Write("Kitabi odunc almis kisinin kullanici numarasini giriniz: ");

                    if (int.TryParse(Console.ReadLine(), out int returnId) == false)
                    {
                        Console.Write("Uye numarasi rakamlardan olusur, gecerli bir uye numarasi girip tekrar deneyiniz.");
                        break;
                    }

                    if (regexwnum.IsMatch(returnTitle) == false)
                    {
                        Console.Write("Baslikta yalnizca harf veya sayi kullanabilirsiniz, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }
                    if (regexwnum.IsMatch(returnAuthor) == false)
                    {
                        Console.Write("Yazar isminde yalnizca harf kullanabilirsiniz, girdiklerinizi kontrol edip tekrar deneyin. ");
                        break;
                    }

                    library.ReturnBook(returnTitle, returnAuthor, returnId);
                    break;

                case "6":
                    library.OutdatedBooks();
                    break;

                case "7":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Gecersiz secenek, lutfen tekrar deneyin.");
                    break;
            }
        }
    }
}