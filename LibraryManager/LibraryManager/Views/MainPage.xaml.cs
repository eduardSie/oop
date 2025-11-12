using System.Collections.ObjectModel;
using System.Text.Json;
using LibraryManager.Models;

namespace LibraryManager.Views
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Book> books = new();

        public MainPage()
        {
            InitializeComponent();
            BooksCollection.ItemsSource = books;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            StatusLabel.Text = $"Всього книг: {books.Count}";
        }

        // ВАРІАНТ 1: Стандартний FilePicker
        private async void OnOpenClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Виберіть JSON файл (у діалозі виберіть 'All files')"
                });

                if (result != null)
                {
                    await LoadJsonFile(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        

        // Спільний метод завантаження JSON
        private async Task LoadJsonFile(string filePath)
        {
            try
            {
                StatusLabel.Text = "Завантаження...";
                StatusLabel.TextColor = Colors.Orange;

                var json = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<LibraryData>(json);

                books.Clear();
                if (data?.Books != null)
                {
                    foreach (var book in data.Books)
                    {
                        books.Add(book);
                    }
                }

                UpdateStatus();
                StatusLabel.TextColor = Colors.Green;

                await DisplayAlert("Успіх ✓",
                    $"Завантажено: {books.Count} книг\n\nФайл: {Path.GetFileName(filePath)}",
                    "OK");
            }
            catch (JsonException ex)
            {
                StatusLabel.Text = "Помилка завантаження";
                StatusLabel.TextColor = Colors.Red;
                await DisplayAlert("Помилка JSON", $"Невалідний JSON формат:\n{ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Помилка";
                StatusLabel.TextColor = Colors.Red;
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (books.Count == 0)
                {
                    await DisplayAlert("Попередження", "Немає даних для збереження", "OK");
                    return;
                }

                var data = new LibraryData
                {
                    Books = books.ToList(),
                    Metadata = new Metadata
                    {
                        Count = books.Count,
                        Date = DateTime.Now
                    }
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(data, options);

                // Зберігаємо на Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"library_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(desktopPath, fileName);

                await File.WriteAllTextAsync(filePath, json);

                StatusLabel.Text = "Файл збережено";
                StatusLabel.TextColor = Colors.Green;

                bool openFolder = await DisplayAlert("Успіх ✓",
                    $"Файл збережено:\n{fileName}\n\nШлях:\n{filePath}\n\nКількість книг: {books.Count}",
                    "Відкрити папку",
                    "OK");

                if (openFolder)
                {
                    // Відкрити папку з файлом (тільки для Windows)
                    System.Diagnostics.Process.Start("explorer.exe", desktopPath);
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Помилка збереження";
                StatusLabel.TextColor = Colors.Red;
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            try
            {
                string title = await DisplayPromptAsync("Нова книга", "Назва книги:");
                if (string.IsNullOrEmpty(title))
                    return;

                string author = await DisplayPromptAsync("Автор", "Ім'я автора:") ?? "";
                string yearStr = await DisplayPromptAsync("Рік", "Рік видання:",
                    keyboard: Keyboard.Numeric,
                    initialValue: DateTime.Now.Year.ToString()) ?? DateTime.Now.Year.ToString();
                string genre = await DisplayPromptAsync("Жанр", "Жанр книги:") ?? "";
                string pagesStr = await DisplayPromptAsync("Сторінки", "Кількість сторінок:",
                    keyboard: Keyboard.Numeric,
                    initialValue: "0") ?? "0";
                string isbn = await DisplayPromptAsync("ISBN", "ISBN код:") ?? "";

                var newBook = new Book
                {
                    Id = books.Any() ? books.Max(b => b.Id) + 1 : 1,
                    Title = title,
                    Author = author,
                    Year = int.TryParse(yearStr, out int year) ? year : DateTime.Now.Year,
                    Genre = genre,
                    Pages = int.TryParse(pagesStr, out int pages) ? pages : 0,
                    ISBN = isbn,
                    Available = true
                };

                books.Add(newBook);
                UpdateStatus();

                await DisplayAlert("Успіх", $"✓ Книгу '{title}' додано\nID: {newBook.Id}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void OnLoadSampleClicked(object sender, EventArgs e)
        {
            books.Clear();

            var sampleBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", Year = 1840, Genre = "Поезія", Pages = 240, ISBN = "978-966-03-1234-5", Available = true },
                new Book { Id = 2, Title = "Тіні забутих предків", Author = "Михайло Коцюбинський", Year = 1911, Genre = "Повість", Pages = 128, ISBN = "978-966-03-2345-6", Available = true },
                new Book { Id = 3, Title = "Захар Беркут", Author = "Іван Франко", Year = 1883, Genre = "Історичний роман", Pages = 320, ISBN = "978-966-03-3456-7", Available = false },
                new Book { Id = 4, Title = "Лісова пісня", Author = "Леся Українка", Year = 1911, Genre = "Драма", Pages = 96, ISBN = "978-966-03-4567-8", Available = true },
                new Book { Id = 5, Title = "Собор", Author = "Олесь Гончар", Year = 1968, Genre = "Роман", Pages = 456, ISBN = "978-966-03-5678-9", Available = true }
            };

            foreach (var book in sampleBooks)
            {
                books.Add(book);
            }

            UpdateStatus();
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Book book)
            {
                string newTitle = await DisplayPromptAsync("Редагування назви", "Нова назва:", initialValue: book.Title);
                if (!string.IsNullOrEmpty(newTitle))
                {
                    book.Title = newTitle;

                    // Оновлення відображення
                    var temp = BooksCollection.ItemsSource;
                    BooksCollection.ItemsSource = null;
                    BooksCollection.ItemsSource = temp;

                    await DisplayAlert("Успіх", "Книгу оновлено", "OK");
                }
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Book book)
            {
                bool confirm = await DisplayAlert("Підтвердження",
                    $"Видалити книгу:\n'{book.Title}'\nАвтор: {book.Author}?",
                    "Так, видалити",
                    "Скасувати");

                if (confirm)
                {
                    books.Remove(book);
                    UpdateStatus();
                    await DisplayAlert("Успіх", "Книгу видалено", "OK");
                }
            }
        }
    }
}