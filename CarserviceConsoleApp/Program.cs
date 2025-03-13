using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarserviceConsoleApp.Models;
using System;
using System.Threading.Tasks;
using CarserviceConsoleApp;

class Program
{
    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContextFactory<CarserviceContext>(options =>
                options.UseSqlServer("Server=localhost;Database=carservice;Trusted_Connection=True;TrustServerCertificate=true;Connect Timeout=60;"))
            .AddScoped<DatabaseOperations>()
            .AddSingleton<DataGenerator>()
            .AddSingleton<RequestGenerator>()
            .BuildServiceProvider();

        var generator = serviceProvider.GetRequiredService<DataGenerator>();
        var requestGenerator = serviceProvider.GetRequiredService<RequestGenerator>();

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Запустить генерацию данных");
            Console.WriteLine("2. Запустить клиентское приложение");
            Console.WriteLine("3. Очистить базу данных");
            Console.WriteLine("q. Выход");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(choice))
            {
                Console.WriteLine("Пожалуйста, введите корректное значение.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Запуск генерации данных...");
                        await generator.GenerateDataAsync();
                        Console.WriteLine("Генерация данных завершена.");
                        break;

                    case "2":
                        Console.WriteLine("Запуск клиентского приложение...");
                        await requestGenerator.GenerateRequestsAsync();
                        Console.WriteLine("Клиентское приложение завершило работу.");
                        break;

                    case "3":
                        await generator.ClearDatabaseAsync();
                        break;

                    case "q":
                        exit = true;
                        Console.WriteLine("Выход из программы.");
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"Детали: {ex.InnerException?.Message}");
            }

            Console.WriteLine();
        }
    }
}