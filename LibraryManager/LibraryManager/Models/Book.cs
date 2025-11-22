using System.Text.Json.Serialization;

namespace LibraryManager.Models
{
    public class Book
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("isbn")]
        public string Isbn { get; set; } = string.Empty;

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; } = string.Empty;

        // Конструктор за замовчуванням
        public Book() { }

        // Конструктор для зручності створення нових книг
        public Book(int id, string title, string author, int year, string genre,
                    string isbn, int pages, string publisher)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
            Genre = genre;
            Isbn = isbn;
            Pages = pages;
            Publisher = publisher;
        }

        // Для зручного відображення у Grid
        public override string ToString()
        {
            return $"{Title} - {Author} ({Year})";
        }
    }
}