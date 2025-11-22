using System.Text.Json;
using LibraryManager.Models;

namespace LibraryManager.Services
{
    public class JsonService
    {
        private readonly JsonSerializerOptions _options;

        public JsonService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        // Десеріалізація - читання з JSON файлу
        public async Task<List<Book>> LoadBooksFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<Book>();
                }

                using var stream = File.OpenRead(filePath);
                var books = await JsonSerializer.DeserializeAsync<List<Book>>(stream, _options);
                return books ?? new List<Book>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при читанні файлу: {ex.Message}", ex);
            }
        }

        // Серіалізація - збереження у JSON файл
        public async Task SaveBooksToFileAsync(string filePath, List<Book> books)
        {
            try
            {
                using var stream = File.Create(filePath);
                await JsonSerializer.SerializeAsync(stream, books, _options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при збереженні файлу: {ex.Message}", ex);
            }
        }

        // Експорт у новий файл
        public async Task ExportBooksAsync(string filePath, List<Book> books)
        {
            await SaveBooksToFileAsync(filePath, books);
        }

        // Десеріалізація з рядка JSON
        public List<Book> DeserializeBooks(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<Book>>(json, _options) ?? new List<Book>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка десеріалізації: {ex.Message}", ex);
            }
        }

        // Серіалізація у рядок JSON
        public string SerializeBooks(List<Book> books)
        {
            try
            {
                return JsonSerializer.Serialize(books, _options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка серіалізації: {ex.Message}", ex);
            }
        }
    }
}