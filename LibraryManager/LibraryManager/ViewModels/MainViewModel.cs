using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LibraryManager.Models;
using LibraryManager.Services;

namespace LibraryManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly JsonService _jsonService;
        private ObservableCollection<Book> _books;
        private ObservableCollection<Book> _filteredBooks;
        private string _searchQuery = string.Empty;
        private string _selectedCriteria = "Title";
        private Book? _selectedBook;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set { _books = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Book> FilteredBooks
        {
            get => _filteredBooks;
            set { _filteredBooks = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        public string SelectedCriteria
        {
            get => _selectedCriteria;
            set
            {
                _selectedCriteria = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        public Book? SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        public ICommand LoadSampleDataCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public MainViewModel()
        {
            _jsonService = new JsonService();
            _books = new ObservableCollection<Book>();
            _filteredBooks = new ObservableCollection<Book>();

            LoadSampleDataCommand = new Command(LoadSampleData);
            OpenFileCommand = new Command(async () => await OpenFile());
            SaveFileCommand = new Command(async () => await SaveFile());
            AddBookCommand = new Command(async () => await AddBook());
            EditBookCommand = new Command<Book>(async (book) => await EditBook(book));
            DeleteBookCommand = new Command<Book>(DeleteBook);
        }

        private void LoadSampleData()
        {
            var sampleBooks = _jsonService.GetSampleData();
            Books.Clear();
            foreach (var book in sampleBooks)
            {
                Books.Add(book);
            }
            PerformSearch();
        }

        private async Task OpenFile()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Виберіть JSON файл",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".json" } },
                        { DevicePlatform.Android, new[] { "application/json" } },
                        { DevicePlatform.iOS, new[] { "public.json" } },
                        { DevicePlatform.MacCatalyst, new[] { "json" } }
                    })
                });

                if (result != null)
                {
                    var books = await _jsonService.LoadBooksAsync(result.FullPath);
                    Books.Clear();
                    foreach (var book in books)
                    {
                        Books.Add(book);
                    }
                    PerformSearch();
                    await Application.Current.MainPage.DisplayAlert("Успіх", $"Завантажено {books.Count} книг", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async Task SaveFile()
        {
            try
            {
                var fileName = $"library_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                var success = await _jsonService.SaveBooksAsync(Books.ToList(), filePath);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Успіх",
                        $"Файл збережено:\n{filePath}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async Task AddBook()
        {
            var newBook = new Book
            {
                Id = Books.Any() ? Books.Max(b => b.Id) + 1 : 1,
                Available = true
            };

            // Тут буде відкриватися форма редагування
            // Для простоти зараз додаємо з діалогом
            string title = await Application.Current.MainPage.DisplayPromptAsync("Нова книга", "Назва:");
            if (!string.IsNullOrEmpty(title))
            {
                newBook.Title = title;
                newBook.Author = await Application.Current.MainPage.DisplayPromptAsync("Автор", "Введіть автора:") ?? "";
                Books.Add(newBook);
                PerformSearch();
            }
        }

        private async Task EditBook(Book book)
        {
            if (book == null) return;

            string newTitle = await Application.Current.MainPage.DisplayPromptAsync("Редагування", "Нова назва:", initialValue: book.Title);
            if (!string.IsNullOrEmpty(newTitle))
            {
                book.Title = newTitle;
                OnPropertyChanged(nameof(FilteredBooks));
            }
        }

        private async void DeleteBook(Book book)
        {
            if (book == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Підтвердження",
                $"Видалити книгу '{book.Title}'?", "Так", "Ні");

            if (confirm)
            {
                Books.Remove(book);
                PerformSearch();
            }
        }

        // LINQ запити для пошуку
        private void PerformSearch()
        {
            IEnumerable<Book> results = Books;

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();

                results = SelectedCriteria switch
                {
                    "Title" => Books.Where(b => b.Title.ToLower().Contains(query)),
                    "Author" => Books.Where(b => b.Author.ToLower().Contains(query)),
                    "Genre" => Books.Where(b => b.Genre.ToLower().Contains(query)),
                    "Year" => Books.Where(b => b.Year.ToString() == SearchQuery),
                    "Available" => Books.Where(b => b.Available),
                    "Unavailable" => Books.Where(b => !b.Available),
                    _ => Books
                };
            }

            FilteredBooks.Clear();
            foreach (var book in results)
            {
                FilteredBooks.Add(book);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}