using System;

namespace ConsoleNumberInput
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Програма введення числа ===");
            Console.WriteLine();

            while (true)
            {
                try
                {
                    // Запит на введення числа
                    Console.Write("Введіть ціле число (або 'exit' для виходу): ");
                    string input = Console.ReadLine();

                    // Перевірка на вихід
                    if (input?.ToLower() == "exit")
                    {
                        Console.WriteLine("До побачення!");
                        break;
                    }

                    // Спроба перетворити введене значення на ціле число
                    if (int.TryParse(input, out int number))
                    {
                        Console.WriteLine($"Ви ввели число {number}");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Помилка! Будь ласка, введіть коректне ціле число.");
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Виникла помилка: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }
    }
}
