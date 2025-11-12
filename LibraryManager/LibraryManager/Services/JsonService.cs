using System.Text.Json;
using LibraryManager.Models;

namespace LibraryManager.Services
{
    public class JsonService
    {
        public async Task<List<Book>> LoadBooksAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<LibraryData>(json);
                return data?.Books ?? new List<Book>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading: {ex.Message}");
                return new List<Book>();
            }
        }

        public async Task<bool> SaveBooksAsync(List<Book> books, string filePath)
        {
            try
            {
                var data = new LibraryData
                {
                    Books = books,
                    Metadata = new Metadata
                    {
                        Count = books.Count,
                        Date = DateTime.Now
                    }
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving: {ex.Message}");
                return false;
            }
        }

        public List<Book> GetSampleData()
        {
            return new List<Book>
            {
                new Book { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", Year = 1840, Genre = "Поезія", Pages = 240, ISBN = "978-966-03-1234-5", Available = true },
                new Book { Id = 2, Title = "Тіні забутих предків", Author = "Михайло Коцюбинський", Year = 1911, Genre = "Повість", Pages = 128, ISBN = "978-966-03-2345-6", Available = true },
                new Book { Id = 3, Title = "Захар Беркут", Author = "Іван Франко", Year = 1883, Genre = "Історичний роман", Pages = 320, ISBN = "978-966-03-3456-7", Available = false },
                new Book { Id = 4, Title = "Лісова пісня", Author = "Леся Українка", Year = 1911, Genre = "Драма", Pages = 96, ISBN = "978-966-03-4567-8", Available = true },
                new Book { Id = 5, Title = "Собор", Author = "Олесь Гончар", Year = 1968, Genre = "Роман", Pages = 456, ISBN = "978-966-03-5678-9", Available = true }
            };
        }
    }
}