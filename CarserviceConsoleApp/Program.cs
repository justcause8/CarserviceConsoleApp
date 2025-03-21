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
        // Настройка сервисов
        var serviceProvider = new ServiceCollection()
            .AddDbContextFactory<CarserviceContext>(options =>
                options.UseSqlServer("Server=localhost;Database=carservice;Trusted_Connection=True;TrustServerCertificate=true;Connect Timeout=60;"))
            .AddScoped<DatabaseOperations>() // Создается для каждого запроса
            .AddSingleton<DataGenerator>() // Создается один раз
            .AddSingleton<RequestGenerator>()
            .BuildServiceProvider(); // Нужен для создания serviceProvider

        // Получение сервисов
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
                        await generator.FillDatabaseAsync(
                            clientsCount: 1000,
                            carsCount: 3000,
                            employeesCount: 300,
                            ordersCount: 4000,
                            inventoryCount: 5000,
                            orderPartsCount: 5000,
                            orderServicesCount: 5000,
                            orderAssignmentsCount: 5000
                        );
                        Console.WriteLine("Генерация данных завершена.");
                        break;

                    case "2":
                        Console.WriteLine("Запуск клиентского приложения...\n");
                        await requestGenerator.GenerateRequestsAsync();
                        Console.WriteLine("Клиентское приложение завершило работу.");
                        break;

                    case "3":
                        Console.WriteLine("Очистка базы данных...");
                        await generator.ClearDatabaseAsync();
                        Console.WriteLine("База данных очищена.");
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