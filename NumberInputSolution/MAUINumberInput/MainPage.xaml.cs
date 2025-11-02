namespace MAUINumberInput;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        try
        {
            string input = NumberEntry.Text?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                await DisplayAlert("Помилка", "Будь ласка, введіть число!", "OK");
                return;
            }

            if (int.TryParse(input, out int number))
            {
                ResultLabel.Text = $"Ви ввели число {number}";
                ResultLabel.IsVisible = true;

                await DisplayAlert("Результат", $"Ви ввели число {number}", "OK");

                // Очищення поля після успішного введення (опціонально)
                // NumberEntry.Text = string.Empty;
                // ResultLabel.IsVisible = false;
            }
            else
            {
                await DisplayAlert("Помилка", "Введіть коректне ціле число!", "OK");
                ResultLabel.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Виникла помилка: {ex.Message}", "OK");
        }
    }
}
