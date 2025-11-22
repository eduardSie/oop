using LibraryManager.Models;
using LibraryManager.Services;
using System.Collections.ObjectModel;

namespace LibraryManager
{
    public partial class MainPage : ContentPage
    {
        private readonly JsonService _jsonService;
        private ObservableCollection<Book> _allBooks;
        private ObservableCollection<Book> _filteredBooks;
        private string _currentFilePath;
        private Book _selectedBook;
        private int _selectedRowIndex = -1;

        public MainPage()
        {
            InitializeComponent();
            _jsonService = new JsonService();
            _allBooks = new ObservableCollection<Book>();
            _filteredBooks = new ObservableCollection<Book>();

            // Підписка на зміни колекції для автоматичного оновлення Grid
            _filteredBooks.CollectionChanged += (s, e) => UpdateGridTable();

            InitializeGenrePicker();
            UpdateStatus("Завантажте JSON файл для початку роботи");
        }

        private void InitializeGenrePicker()
        {
            GenrePicker.Items.Add("Всі жанри");
            GenrePicker.SelectedIndex = 0;
        }

        // Метод оновлення Grid таблиці
        private void UpdateGridTable()
        {
            // Видаляємо всі рядки крім заголовка (рядки 0 і 1)
            var itemsToRemove = BooksGrid.Children
                .Where(x =>
                {
                    if (x is BindableObject bindable)
                    {
                        var row = (int)bindable.GetValue(Microsoft.Maui.Controls.Grid.RowProperty);
                        return row >= 2;
                    }
                    return false;
                })
                .ToList();

            foreach (var item in itemsToRemove)
            {
                BooksGrid.Children.Remove(item);
            }

            // Видаляємо визначення рядків (залишаємо тільки 2: заголовок і лінію)
            while (BooksGrid.RowDefinitions.Count > 2)
            {
                BooksGrid.RowDefinitions.RemoveAt(BooksGrid.RowDefinitions.Count - 1);
            }

            // Додаємо рядки для кожної книги
            int rowIndex = 2;
            foreach (var book in _filteredBooks)
            {
                // Додаємо визначення рядка
                BooksGrid.RowDefinitions.Add(new RowDefinition { Height = 50 });

                // Фон рядка
                var bgBox = new BoxView
                {
                    BackgroundColor = rowIndex % 2 == 0 ? Colors.White : Color.FromRgb(248, 248, 248)
                };
                bgBox.SetValue(Microsoft.Maui.Controls.Grid.RowProperty, rowIndex);
                bgBox.SetValue(Microsoft.Maui.Controls.Grid.ColumnSpanProperty, 8);
                BooksGrid.Children.Add(bgBox);

                // Додаємо обробник кліку
                var tapGesture = new TapGestureRecognizer();
                int currentRow = rowIndex;
                Book currentBook = book;
                tapGesture.Tapped += (s, e) => OnRowTapped(currentRow, currentBook);
                bgBox.GestureRecognizers.Add(tapGesture);

                // Створюємо та додаємо Label для кожної колонки
                AddCellToGrid(book.Id.ToString(), rowIndex, 0, false);
                AddCellToGrid(book.Title, rowIndex, 1, true);
                AddCellToGrid(book.Author, rowIndex, 2, false);
                AddCellToGrid(book.Year.ToString(), rowIndex, 3, false);
                AddCellToGrid(book.Genre, rowIndex, 4, false);
                AddCellToGrid(book.Isbn, rowIndex, 5, false);
                AddCellToGrid(book.Pages.ToString(), rowIndex, 6, false);
                AddCellToGrid(book.Publisher, rowIndex, 7, false);

                // Роздільна лінія
                var separator = new BoxView
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.FromRgb(224, 224, 224),
                    VerticalOptions = LayoutOptions.End
                };
                separator.SetValue(Microsoft.Maui.Controls.Grid.RowProperty, rowIndex);
                separator.SetValue(Microsoft.Maui.Controls.Grid.ColumnSpanProperty, 8);
                BooksGrid.Children.Add(separator);

                rowIndex++;
            }
        }

        // Допоміжний метод для додавання комірки
        private void AddCellToGrid(string text, int row, int column, bool isBold)
        {
            var label = new Label
            {
                Text = text,
                FontSize = 14,
                Padding = new Thickness(10, 0),
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation,
                TextColor = isBold ? Color.FromRgb(25, 118, 210) : Colors.Black
            };

            if (isBold)
            {
                label.FontAttributes = FontAttributes.Bold;
            }

            label.SetValue(Microsoft.Maui.Controls.Grid.RowProperty, row);
            label.SetValue(Microsoft.Maui.Controls.Grid.ColumnProperty, column);
            BooksGrid.Children.Add(label);

            // Alternative: attach gesture to label (instead of InputTransparent)
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s,e) => OnRowTapped(row, _filteredBooks[row - 2]); // compute book index accordingly
            label.GestureRecognizers.Add(tap);
        }

        // Обробка кліку на рядок
        private void OnRowTapped(int rowIndex, Book book)
        {
            // Знімаємо виділення з попереднього рядка
            if (_selectedRowIndex >= 2)
            {
                var oldBg = BooksGrid.Children
                    .FirstOrDefault(x =>
                        x is BoxView &&
                        Microsoft.Maui.Controls.Grid.GetRow((BindableObject)x) == _selectedRowIndex &&
                        Microsoft.Maui.Controls.Grid.GetColumnSpan((BindableObject)x) == 8);

                if (oldBg is BoxView oldBox)
                {
                    oldBox.BackgroundColor = _selectedRowIndex % 2 == 0 ? Colors.White : Color.FromRgb(248, 248, 248);
                }
            }

            // Виділяємо новий рядок
            var newBg = BooksGrid.Children
                .FirstOrDefault(x =>
                    x is BoxView &&
                    Microsoft.Maui.Controls.Grid.GetRow((BindableObject)x) == rowIndex &&
                    Microsoft.Maui.Controls.Grid.GetColumnSpan((BindableObject)x) == 8);

            if (newBg is BoxView newBox)
            {
                newBox.BackgroundColor = Color.FromRgb(227, 242, 253);
            }

            _selectedRowIndex = rowIndex;
            _selectedBook = book;
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Оберіть JSON файл з книгами",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".json" } },
                        { DevicePlatform.macOS, new[] { "json" } },
                        { DevicePlatform.iOS, new[] { "public.json" } },
                        { DevicePlatform.Android, new[] { "application/json" } }
                    })
                });

                if (result != null)
                {
                    _currentFilePath = result.FullPath;
                    var books = await _jsonService.LoadBooksFromFileAsync(_currentFilePath);

                    _allBooks.Clear();
                    foreach (var book in books)
                    {
                        _allBooks.Add(book);
                    }

                    UpdateGenreFilter();
                    ApplyFilters();
                    UpdateStatus($"Завантажено {_allBooks.Count} книг з файлу: {Path.GetFileName(_currentFilePath)}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося відкрити файл: {ex.Message}", "OK");
            }
        }

        private async void OnSaveFileClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                await DisplayAlert("Увага", "Спочатку відкрийте файл", "OK");
                return;
            }

            try
            {
                await _jsonService.SaveBooksToFileAsync(_currentFilePath, _allBooks.ToList());
                UpdateStatus($"Дані збережено у файл: {Path.GetFileName(_currentFilePath)}");
                await DisplayAlert("Успіх", "Зміни успішно збережено!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося зберегти файл: {ex.Message}", "OK");
            }
        }

        private async void OnExportFileClicked(object sender, EventArgs e)
        {
            try
            {
                string fileName = $"books_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                await _jsonService.ExportBooksAsync(filePath, _allBooks.ToList());

                await DisplayAlert("Успіх", $"Дані експортовано у файл:\n{filePath}", "OK");
                UpdateStatus($"Експортовано {_allBooks.Count} книг");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося експортувати файл: {ex.Message}", "OK");
            }
        }

        private async void OnAddBookClicked(object sender, EventArgs e)
        {
            var editPage = new BookEditPage();
            editPage.BookSaved += (s, book) =>
            {
                book.Id = _allBooks.Any() ? _allBooks.Max(b => b.Id) + 1 : 1;
                _allBooks.Add(book);
                UpdateGenreFilter();
                ApplyFilters();
                UpdateStatus($"Додано книгу: {book.Title}");
            };

            await Navigation.PushModalAsync(editPage);
        }

        private async void OnEditBookClicked(object sender, EventArgs e)
        {
            if (_selectedBook == null)
            {
                await DisplayAlert("Увага", "Оберіть книгу для редагування", "OK");
                return;
            }

            var editPage = new BookEditPage(_selectedBook);
            editPage.BookSaved += (s, book) =>
            {
                var index = _allBooks.IndexOf(_selectedBook);
                if (index >= 0)
                {
                    _allBooks[index] = book;
                    ApplyFilters();
                    UpdateStatus($"Відредаговано книгу: {book.Title}");
                }
            };

            await Navigation.PushModalAsync(editPage);
        }

        private async void OnDeleteBookClicked(object sender, EventArgs e)
        {
            if (_selectedBook == null)
            {
                await DisplayAlert("Увага", "Оберіть книгу для видалення", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Підтвердження",
                $"Видалити книгу '{_selectedBook.Title}'?", "Так", "Ні");

            if (confirm)
            {
                string title = _selectedBook.Title;
                _allBooks.Remove(_selectedBook);
                _selectedBook = null;
                _selectedRowIndex = -1;
                ApplyFilters();
                UpdateStatus($"Видалено книгу: {title}");
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnGenreFilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void OnYearFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Застосування LINQ фільтрів
        private void ApplyFilters()
        {
            var query = _allBooks.AsEnumerable();

            // Критерій 1: Пошук за текстом (назва, автор, жанр)
            if (!string.IsNullOrWhiteSpace(SearchEntry.Text))
            {
                string searchText = SearchEntry.Text.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchText) ||
                    b.Author.ToLower().Contains(searchText) ||
                    b.Genre.ToLower().Contains(searchText));
            }

            // Критерій 2: Фільтр за жанром
            if (GenrePicker.SelectedIndex > 0)
            {
                string selectedGenre = GenrePicker.SelectedItem.ToString();
                query = query.Where(b => b.Genre == selectedGenre);
            }

            // Критерій 3: Фільтр за діапазоном років
            if (int.TryParse(YearFromEntry.Text, out int yearFrom))
            {
                query = query.Where(b => b.Year >= yearFrom);
            }
            if (int.TryParse(YearToEntry.Text, out int yearTo))
            {
                query = query.Where(b => b.Year <= yearTo);
            }

            _filteredBooks.Clear();
            foreach (var book in query)
            {
                _filteredBooks.Add(book);
            }

            UpdateStatus($"Показано {_filteredBooks.Count} з {_allBooks.Count} книг");
        }

        private void UpdateGenreFilter()
        {
            var genres = _allBooks.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();

            GenrePicker.Items.Clear();
            GenrePicker.Items.Add("Всі жанри");
            foreach (var genre in genres)
            {
                GenrePicker.Items.Add(genre);
            }
            GenrePicker.SelectedIndex = 0;
        }

        private void UpdateStatus(string message)
        {
            StatusLabel.Text = message;
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AboutPage());
        }
    }
}