using LibraryManager.Models;
using System.Formats.Tar;

namespace LibraryManager
{
    public partial class BookEditPage : ContentPage
    {
        private Book _book;
        private bool _isNewBook;

        public event EventHandler<Book> BookSaved;

        // Конструктор для додавання нової книги
        public BookEditPage()
        {
            InitializeComponent();
            _isNewBook = true;
            _book = new Book();
            Title = "Додавання книги";
        }

        // Конструктор для редагування існуючої книги
        public BookEditPage(Book book)
        {
            InitializeComponent();
            _isNewBook = false;
            _book = book;
            Title = "Редагування книги";
            LoadBookData();
        }

        // Завантаження даних книги у форму
        private void LoadBookData()
        {
            TitleEntry.Text = _book.Title;
            AuthorEntry.Text = _book.Author;
            YearEntry.Text = _book.Year.ToString();
            GenreEntry.Text = _book.Genre;
            IsbnEntry.Text = _book.Isbn;
            PagesEntry.Text = _book.Pages.ToString();
            PublisherEntry.Text = _book.Publisher;
        }

        // Збереження книги
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Валідація полів
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Помилка", "Введіть назву книги", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(AuthorEntry.Text))
            {
                await DisplayAlert("Помилка", "Введіть автора книги", "OK");
                return;
            }

            if (!int.TryParse(YearEntry.Text, out int year) || year < 1000 || year > DateTime.Now.Year)
            {
                await DisplayAlert("Помилка", "Введіть коректний рік видання", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(GenreEntry.Text))
            {
                await DisplayAlert("Помилка", "Введіть жанр книги", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(IsbnEntry.Text))
            {
                await DisplayAlert("Помилка", "Введіть ISBN книги", "OK");
                return;
            }

            if (!int.TryParse(PagesEntry.Text, out int pages) || pages <= 0)
            {
                await DisplayAlert("Помилка", "Введіть коректну кількість сторінок", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PublisherEntry.Text))
            {
                await DisplayAlert("Помилка", "Введіть видавництво", "OK");
                return;
            }

            // Створення або оновлення об'єкта книги
            var book = new Book
            {
                Id = _book.Id,
                Title = TitleEntry.Text.Trim(),
                Author = AuthorEntry.Text.Trim(),
                Year = year,
                Genre = GenreEntry.Text.Trim(),
                Isbn = IsbnEntry.Text.Trim(),
                Pages = pages,
                Publisher = PublisherEntry.Text.Trim()
            };

            // Виклик події збереження
            BookSaved?.Invoke(this, book);

            // Закриття форми
            await Navigation.PopModalAsync();
        }

        // Скасування
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження",
                "Скасувати зміни?", "Так", "Ні");

            if (confirm)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}