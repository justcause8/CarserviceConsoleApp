using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarserviceConsoleApp.Models;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<CarserviceContext>(options =>
                options.UseSqlServer("Server=localhost;Database=carservice;Trusted_Connection=True;TrustServerCertificate=true;"))
            .AddSingleton<DataGenerator>()
            .BuildServiceProvider();

        var generator = serviceProvider.GetRequiredService<DataGenerator>();

        try
        {
            Console.WriteLine("Приложение запущено");
            await generator.GenerateDataAsync(); // Дождаться выполнения метода
            Console.WriteLine("Генерация данных завершена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

       
        //generator.ClearDatabase();
    }
}
