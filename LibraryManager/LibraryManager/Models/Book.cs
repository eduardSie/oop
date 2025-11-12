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

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("isbn")]
        public string ISBN { get; set; } = string.Empty;

        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

    public class LibraryData
    {
        [JsonPropertyName("books")]
        public List<Book> Books { get; set; } = new();

        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}