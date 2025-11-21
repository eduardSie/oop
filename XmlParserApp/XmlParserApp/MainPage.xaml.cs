using System.Diagnostics;

namespace XmlParserApp;

public partial class MainPage : ContentPage
{
    private XmlParserContext _parserContext;
    private XmlAttributeLoader _attributeLoader;
    private XslTransformer _xslTransformer;
    private string _currentXmlFile;
    private Dictionary<string, string> _searchCriteria;

    public MainPage()
    {
        InitializeComponent();

        _parserContext = new XmlParserContext();
        _attributeLoader = new XmlAttributeLoader();
        _xslTransformer = new XslTransformer();
        _searchCriteria = new Dictionary<string, string>();

        // Встановлюємо стратегію за замовчуванням
        StrategyPicker.SelectedIndex = 0;
    }

    private async void OnSelectFileClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xml" } },
                    { DevicePlatform.macOS, new[] { "xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } },
                    { DevicePlatform.Android, new[] { "text/xml" } }
                }),
                PickerTitle = "Виберіть XML файл"
            });

            if (result != null)
            {
                _currentXmlFile = result.FullPath;
                FilePathEntry.Text = result.FileName;

                // Завантажуємо доступні значення атрибутів
                await LoadAvailableAttributes();

                await DisplayAlert("Успіх", "Файл успішно завантажено!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося завантажити файл: {ex.Message}", "OK");
        }
    }

    private async Task LoadAvailableAttributes()
    {
        try
        {
            if (string.IsNullOrEmpty(_currentXmlFile))
                return;

            // Завантажуємо факультети
            var faculties = _attributeLoader.GetUniqueValues(_currentXmlFile, "faculty");
            FacultyPicker.ItemsSource = faculties;

            // Завантажуємо кафедри
            var departments = _attributeLoader.GetUniqueValues(_currentXmlFile, "department");
            DepartmentPicker.ItemsSource = departments;

            // Завантажуємо семестри
            var semesters = _attributeLoader.GetUniqueValues(_currentXmlFile, "semester");
            SemesterPicker.ItemsSource = semesters;

            // Завантажуємо предмети
            var subjects = _attributeLoader.GetUniqueValues(_currentXmlFile, "subject");
            SubjectPicker.ItemsSource = subjects;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося завантажити атрибути: {ex.Message}", "OK");
        }
    }

    private void OnFacultyChanged(object sender, EventArgs e)
    {
        if (FacultyPicker.SelectedItem != null)
            _searchCriteria["Faculty"] = FacultyPicker.SelectedItem.ToString();
        else
            _searchCriteria.Remove("Faculty");
    }

    private void OnDepartmentChanged(object sender, EventArgs e)
    {
        if (DepartmentPicker.SelectedItem != null)
            _searchCriteria["Department"] = DepartmentPicker.SelectedItem.ToString();
        else
            _searchCriteria.Remove("Department");
    }

    private void OnSemesterChanged(object sender, EventArgs e)
    {
        if (SemesterPicker.SelectedItem != null)
            _searchCriteria["Semester"] = SemesterPicker.SelectedItem.ToString();
        else
            _searchCriteria.Remove("Semester");
    }

    private void OnSubjectChanged(object sender, EventArgs e)
    {
        if (SubjectPicker.SelectedItem != null)
            _searchCriteria["Subject"] = SubjectPicker.SelectedItem.ToString();
        else
            _searchCriteria.Remove("Subject");
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentXmlFile))
        {
            await DisplayAlert("Попередження", "Спочатку виберіть XML файл!", "OK");
            return;
        }

        try
        {
            // Вибираємо стратегію
            IXmlParsingStrategy strategy = StrategyPicker.SelectedIndex switch
            {
                0 => new SaxParsingStrategy(),
                1 => new DomParsingStrategy(),
                2 => new LinqToXmlParsingStrategy(),
                _ => new LinqToXmlParsingStrategy()
            };

            _parserContext.SetStrategy(strategy);

            // Вимірюємо час виконання
            var stopwatch = Stopwatch.StartNew();
            var results = _parserContext.ExecuteParsing(_currentXmlFile, _searchCriteria);
            stopwatch.Stop();

            // Виводимо результати
            ResultCountLabel.Text = $"{results.Count} записів";
            ExecutionTimeLabel.Text = $"({stopwatch.ElapsedMilliseconds} мс)";

            if (results.Count > 0)
            {
                var output = string.Join("\n\n", results.Select((s, i) =>
                    $"{i + 1}. {s.FullName}\n" +
                    $"   Факультет: {s.Faculty}\n" +
                    $"   Кафедра: {s.Department}\n" +
                    $"   Предмет: {s.Subject}\n" +
                    $"   Оцінка: {s.Grade}\n" +
                    $"   Семестр: {s.Semester}"));

                ResultsEditor.Text = output;
            }
            else
            {
                ResultsEditor.Text = "Не знайдено жодного запису за заданими критеріями.";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Помилка пошуку: {ex.Message}", "OK");
        }
    }

    private async void OnTransformClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentXmlFile))
        {
            await DisplayAlert("Попередження", "Спочатку виберіть XML файл!", "OK");
            return;
        }

        try
        {
            // Сначала выполняем поиск с текущими критериями
            IXmlParsingStrategy strategy = StrategyPicker.SelectedIndex switch
            {
                0 => new SaxParsingStrategy(),
                1 => new DomParsingStrategy(),
                2 => new LinqToXmlParsingStrategy(),
                _ => new LinqToXmlParsingStrategy()
            };

            _parserContext.SetStrategy(strategy);
            var results = _parserContext.ExecuteParsing(_currentXmlFile, _searchCriteria);

            if (results.Count == 0)
            {
                var proceed = await DisplayAlert(
                    "Попередження",
                    "За заданими критеріями не знайдено жодного запису. Трансформувати весь файл?",
                    "Так",
                    "Ні");

                if (!proceed)
                    return;

                // Если пользователь согласен, получаем все записи
                results = _parserContext.ExecuteParsing(_currentXmlFile, new Dictionary<string, string>());
            }

            // Запрашиваем XSL файл
            var xslResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xsl", ".xslt" } },
                { DevicePlatform.macOS, new[] { "xsl", "xslt" } },
                { DevicePlatform.iOS, new[] { "public.xsl" } },
                { DevicePlatform.Android, new[] { "text/xml" } }
            }),
                PickerTitle = "Виберіть XSL файл"
            });

            if (xslResult == null)
                return;

            // Создаем выходной HTML файл
            string outputPath = Path.Combine(
                Path.GetDirectoryName(_currentXmlFile),
                "output_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");

            var stopwatch = Stopwatch.StartNew();
            // Передаем отфильтрованные результаты вместо целого файла
            _xslTransformer.TransformToHtml(results, xslResult.FullPath, outputPath);
            stopwatch.Stop();

            var openFile = await DisplayAlert(
                "Успіх",
                $"HTML файл створено з {results.Count} записів!\nШлях: {outputPath}\nЧас: {stopwatch.ElapsedMilliseconds} мс\n\nВідкрити файл?",
                "Так",
                "Ні");

            if (openFile)
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(outputPath)
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Помилка трансформації: {ex.Message}", "OK");
        }
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        // Очищаємо критерії пошуку
        _searchCriteria.Clear();

        FacultyPicker.SelectedIndex = -1;
        DepartmentPicker.SelectedIndex = -1;
        SemesterPicker.SelectedIndex = -1;
        SubjectPicker.SelectedIndex = -1;

        // Очищаємо результати
        ResultsEditor.Text = string.Empty;
        ResultCountLabel.Text = "0 записів";
        ExecutionTimeLabel.Text = string.Empty;

        await DisplayAlert("Інформація", "Поля очищено!", "OK");
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            var result = await DisplayAlert(
                "Підтвердження",
                "Чи дійсно ви хочете завершити роботу з програмою?",
                "Так",
                "Ні");

            if (result)
            {
                Application.Current.Quit();
            }
        });

        return true; // Запобігаємо стандартній поведінці
    }
}