using Bogus;
using Bogus.DataSets;
using CarserviceConsoleApp;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RequestGenerator
{
    private readonly IDbContextFactory<CarserviceContext> _contextFactory;
    private readonly DatabaseOperations _databaseOperations;
    private readonly Random _random;

    public RequestGenerator(IDbContextFactory<CarserviceContext> contextFactory, DatabaseOperations databaseOperations)
    {
        _contextFactory = contextFactory;
        _databaseOperations = databaseOperations;
        _random = new Random();
    }

    public async Task GenerateRequestsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        for (int i = 0; i < 10000; i++)
        {
            int operationType = _random.Next(1, 101); // Случайное число для выбора типа операции

            if (operationType <= 80) // шанс на SELECT (чтение)
            {
                await GenerateSelectQueryAsync(context);
            }
            else if (operationType <= 95) //шанс на UPDATE (обновление)
            {
                await GenerateUpdateQueryAsync(context);
            }
            else // шанс на DELETE (удаление)
            {
                await GenerateDeleteQueryAsync(context);
            }
        }
    }

    private async Task GenerateSelectQueryAsync(CarserviceContext context)
    {
        // Случайный выбор клиента
        var clients = await _databaseOperations.GetClientsAsync();
        if (clients.Count == 0) return;

        var randomClient = clients[_random.Next(clients.Count)];

        // Случайный выбор автомобиля клиента
        var cars = await _databaseOperations.GetCarsByClientIdAsync(randomClient.Id);
        if (cars.Count == 0) return;

        var randomCar = cars[_random.Next(cars.Count)];

        // Случайный выбор заказа для автомобиля
        var orders = await _databaseOperations.GetOrdersByClientIdAsync(randomClient.Id);
        if (orders.Count == 0) return;

        var randomOrder = orders[_random.Next(orders.Count)];

        // Выбор случайного сценария
        int scenarioType = _random.Next(1, 5); // Случайное число для выбора типа запроса

        switch (scenarioType)
        {
            case 1:
                // Сценарий 1: Вывод общей суммы заказа
                var orderParts = await _databaseOperations.GetOrderPartsByOrderIdAsync(randomOrder.Id);
                var orderServices = await _databaseOperations.GetOrderServicesByOrderIdAsync(randomOrder.Id);

                decimal totalCost = 0;

                if (!orderParts.Any() && !orderServices.Any())
                {
                    Console.WriteLine($"Заказ ID {randomOrder.Id} не содержит деталей или услуг.");
                    break;
                }

                foreach (var part in orderParts)
                {
                    var partDetails = await context.Parts.FindAsync(part.PartId);
                    totalCost += partDetails.Price * part.Quantity;
                }

                foreach (var service in orderServices)
                {
                    var serviceDetails = await context.Services.FindAsync(service.ServiceId);
                    totalCost += serviceDetails.Price;
                }

                Console.WriteLine($"Общая стоимость заказа ID {randomOrder.Id} для клиента \"{randomClient.Name}\":");
                Console.WriteLine($"- Автомобиль: {randomCar.Brand} {randomCar.Model}");
                Console.WriteLine($"- Итоговая стоимость: {totalCost}");
                Console.WriteLine();
                break;

            case 2:
                // Сценарий 2: Вывод имени и должности работника, который выполнял заказ
                var assignments = await _databaseOperations.GetOrderAssignmentsByOrderIdAsync(randomOrder.Id);
                if (assignments.Count > 0)
                {
                    var randomAssignment = assignments[_random.Next(assignments.Count)];
                    var employee = await context.Employees.FindAsync(randomAssignment.EmployeeId);

                    Console.WriteLine($"Заказ ID {randomOrder.Id} выполнял сотрудник:");
                    Console.WriteLine($"- Имя: {employee.Name}");
                    Console.WriteLine($"- Должность: {employee.Position}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"Заказ ID {randomOrder.Id} не содержит работника {assignments}.");
                }
                break;

            case 3:
                // Сценарий 3: Вывод списка деталей, включенных в заказ, и их количество
                var partsInOrder = await _databaseOperations.GetOrderPartsByOrderIdAsync(randomOrder.Id);

                Console.WriteLine($"Список деталей в заказе ID {randomOrder.Id}:");
                foreach (var part in partsInOrder)
                {
                    var partDetails = await context.Parts.FindAsync(part.PartId);
                    Console.WriteLine($"- Деталь: {partDetails.Name}, Количество: {part.Quantity}");
                }
                Console.WriteLine();
                break;

            case 4:
                // Сценарий 4: Вывод информации о машине, связанной с заказом
                Console.WriteLine($"Информация о машине, связанной с заказом ID {randomOrder.Id}:");
                Console.WriteLine($"- Марка: {randomCar.Brand}");
                Console.WriteLine($"- Модель: {randomCar.Model}");
                Console.WriteLine($"- Год выпуска: {randomCar.Year}");
                Console.WriteLine();
                break;
        }
    }

    private async Task GenerateUpdateQueryAsync(CarserviceContext context)
    {
        int updateType = _random.Next(1, 6); // Случайное число для выбора типа обновления

        switch (updateType)
        {
            case 1:
                // Сценарий: Обновление имени клиента
                var clients = await _databaseOperations.GetClientsAsync();
                if (clients.Count == 0) return;

                var randomClient = clients[_random.Next(clients.Count)];
                string newName = new Name().FirstName(); // Генерация нового имени

                Console.WriteLine($"Обновление имени клиента с ID {randomClient.Id}. Старое имя: \"{randomClient.Name}\", новое имя: \"{newName}\".");
                await _databaseOperations.UpdateClientAsync(randomClient.Id, newName);
                break;

            case 2:
                // Сценарий: Обновление цены запчасти
                var parts = await _databaseOperations.GetPartsAsync();
                if (parts.Count == 0) return;

                var randomPart = parts[_random.Next(parts.Count)];
                decimal newPartPrice = _random.Next(50, 500);

                Console.WriteLine($"Обновление цены запчасти ID {randomPart.Id}:");
                Console.WriteLine($"- Старая цена: {randomPart.Price}");
                Console.WriteLine($"- Новая цена: {newPartPrice}");
                await _databaseOperations.UpdatePartAsync(randomPart.Id, newPartPrice);
                break;

            case 3:
                // Сценарий: Обновление количества запчастей на складе
                var inventories = await _databaseOperations.GetInventoriesAsync();
                if (inventories.Count == 0) return;

                var randomInventory = inventories[_random.Next(inventories.Count)];
                int newStock = _random.Next(1, 100);

                Console.WriteLine($"Обновление количества запчастей на складе для ID {randomInventory.PartId}:");
                Console.WriteLine($"- Старое количество: {randomInventory.Stock}");
                Console.WriteLine($"- Новое количество: {newStock}");
                await _databaseOperations.UpdateInventoryAsync(randomInventory.Id, newStock);
                break;

            case 4:
                // Сценарий: Обновление даты завершения заказа
                var orders = await _databaseOperations.GetOrdersAsync();
                if (orders.Count == 0) return;

                var randomOrder = orders[_random.Next(orders.Count)];
                DateTime newCompletedAt = DateTime.UtcNow.AddDays(_random.Next(1, 30));

                Console.WriteLine($"Обновление даты завершения заказа ID {randomOrder.Id}:");
                Console.WriteLine($"- Старая дата: {randomOrder.CompletedAt}");
                Console.WriteLine($"- Новая дата: {newCompletedAt}");
                await _databaseOperations.UpdateOrderAsync(randomOrder.Id, newCompletedAt);
                break;

            case 5:
                // Сценарий: Обновление цены услуги
                var services = await _databaseOperations.GetServicesAsync();
                if (services.Count == 0) return;

                var randomService = services[_random.Next(services.Count)];
                decimal newServicePrice = _random.Next(100, 1000);

                Console.WriteLine($"Обновление цены услуги ID {randomService.Id}:");
                Console.WriteLine($"- Старая цена: {randomService.Price}");
                Console.WriteLine($"- Новая цена: {newServicePrice}");
                await _databaseOperations.UpdateServiceAsync(randomService.Id, newServicePrice);
                break;
        }

        Console.WriteLine();
    }

    private async Task GenerateDeleteQueryAsync(CarserviceContext context)
    {
        int deleteType = _random.Next(1, 6); // Случайное число для выбора типа удаления

        switch (deleteType)
        {
            case 1:
                // Сценарий: Удаление случайного автомобиля
                var cars = await _databaseOperations.GetCarsAsync();
                if (cars.Count == 0) return;

                var randomCar = cars[_random.Next(cars.Count)];

                Console.WriteLine($"Удаление автомобиля с ID {randomCar.Id}. Был: {randomCar.Brand} {randomCar.Model}.");
                await _databaseOperations.DeleteCarAsync(randomCar.Id);
                break;

            case 2:
                // Сценарий: Удаление случайного клиента
                var clients = await _databaseOperations.GetClientsAsync();
                if (clients.Count == 0) return;

                var randomClient = clients[_random.Next(clients.Count)];

                Console.WriteLine($"Удаление клиента с ID {randomClient.Id}. Имя: \"{randomClient.Name}\".");
                await _databaseOperations.DeleteClientAsync(randomClient.Id);
                break;

            case 3:
                // Сценарий: Удаление случайной запчасти
                var parts = await _databaseOperations.GetPartsAsync();
                if (parts.Count == 0) return;

                var randomPart = parts[_random.Next(parts.Count)];

                Console.WriteLine($"Удаление запчасти с ID {randomPart.Id}. Название: \"{randomPart.Name}\".");
                await _databaseOperations.DeletePartAsync(randomPart.Id);
                break;

            case 4:
                // Сценарий: Удаление случайной услуги
                var services = await _databaseOperations.GetServicesAsync();
                if (services.Count == 0) return;

                var randomService = services[_random.Next(services.Count)];

                Console.WriteLine($"Удаление услуги с ID {randomService.Id}. Название: \"{randomService.Name}\".");
                await _databaseOperations.DeleteServiceAsync(randomService.Id);
                break;

            case 5:
                // Сценарий: Удаление случайного назначения сотрудника
                var assignments = await _databaseOperations.GetOrderAssignmentsByOrderIdAsync(
                    (await _databaseOperations.GetOrdersAsync()).FirstOrDefault()?.Id ?? 0);
                if (assignments.Count == 0) return;

                var randomAssignment = assignments[_random.Next(assignments.Count)];

                Console.WriteLine($"Удаление назначения сотрудника с ID {randomAssignment.EmployeeId} из заказа с ID {randomAssignment.OrderId}.");
                await _databaseOperations.DeleteOrderAssignmentAsync(randomAssignment.Id);
                break;
        }

        Console.WriteLine();
    }










    //private async Task<List<Task>> GenerateClientTasksAsync(int i)
    //{
    //    var clients = await _dbOperations.GetClientsAsync();
    //    var tasks = new List<Task>();

    //    if (clients.Any()) // Если данные в таблице клиентов есть
    //    {
    //        // Создаем нового клиента
    //        var newClient = new Client
    //        {
    //            Name = new Faker().Name.FullName(),
    //            Phone = new Faker().Phone.PhoneNumber("+7 (###) ### ## ##")
    //        };
    //        tasks.Add(_dbOperations.CreateClientAsync(newClient));

    //        // Для обновления используем существующий Id
    //        var clientIdToUpdate = clients.First().Id;
    //        tasks.Add(_dbOperations.UpdateClientAsync(clientIdToUpdate, newClient.Name));

    //        // Удаление случайного существующего клиента
    //        var clientIdToDelete = clients[_random.Next(clients.Count)].Id; // Случайным образом выбираем ID клиента
    //        tasks.Add(_dbOperations.DeleteClientAsync(clientIdToDelete));
    //    }

    //    return tasks;
    //}

    //private async Task<List<Task>> GenerateCarTasksAsync(int i)
    //{
    //    var clients = await _dbOperations.GetClientsAsync();
    //    var clientIds = clients.Select(c => c.Id).ToList();
    //    if (!clientIds.Any()) return new List<Task>();

    //    // Создание новой машины
    //    var newCar = new Faker<Car>()
    //        .RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
    //        .RuleFor(c => c.Model, f => f.Vehicle.Model())
    //        .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
    //        .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Past(20)))
    //        .RuleFor(c => c.ClientId, f => f.PickRandom(clientIds))
    //        .Generate();

    //    var tasks = new List<Task>
    //    {
    //        _dbOperations.CreateCarAsync(newCar)
    //    };

    //    // Обновление случайного автомобиля
    //    var cars = await _dbOperations.GetCarsAsync();
    //    if (cars.Any())
    //    {
    //        var carToUpdate = cars[_random.Next(cars.Count)];
    //        tasks.Add(_dbOperations.UpdateCarAsync(
    //            carToUpdate.Id,
    //            new Faker().Vehicle.Manufacturer(),
    //            new Faker().Vehicle.Model()));
    //    }

    //    // Удаление случайного автомобиля
    //    if (cars.Any())
    //    {
    //        var carToDelete = cars[_random.Next(cars.Count)];
    //        tasks.Add(_dbOperations.DeleteCarAsync(carToDelete.Id));
    //    }

    //    return tasks;
    //}

    //private async Task<List<Task>> GenerateEmployeeTasksAsync(int i)
    //{
    //    var employees = await _dbOperations.GetEmployeesAsync();
    //    if (!employees.Any()) return new List<Task>();

    //    // Создание нового сотрудника
    //    var newEmployee = new Faker<Employee>()
    //        .RuleFor(e => e.Name, f => f.Name.FullName())
    //        .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
    //        .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"))
    //        .Generate();

    //    var tasks = new List<Task>
    //    {
    //        _dbOperations.CreateEmployeeAsync(newEmployee)
    //    };

    //    // Обновление случайного сотрудника
    //    var employeeToUpdate = employees[_random.Next(employees.Count)];
    //    tasks.Add(_dbOperations.UpdateEmployeeAsync(
    //        employeeToUpdate.Id,
    //        new Faker().Name.FullName()));

    //    // Удаление случайного сотрудника
    //    var employeeToDelete = employees[_random.Next(employees.Count)];
    //    tasks.Add(_dbOperations.DeleteEmployeeAsync(employeeToDelete.Id));

    //    return tasks;
    //}

    //private async Task<List<Task>> GenerateOrderTasksAsync(int i)
    //{
    //    var clients = await _dbOperations.GetClientsAsync();
    //    var cars = await _dbOperations.GetCarsAsync();
    //    if (!clients.Any() || !cars.Any()) return new List<Task>();

    //    // Создание нового заказа
    //    var newOrder = new Faker<Order>()
    //        .RuleFor(o => o.CarId, f => f.PickRandom(cars).Id)
    //        .RuleFor(o => o.ClientId, f => f.PickRandom(clients).Id)
    //        .RuleFor(o => o.CreatedAt, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now))
    //        .RuleFor(o => o.CompletedAt, (f, o) => f.Date.Between(o.CreatedAt, o.CreatedAt.AddMonths(6)))
    //        .Generate();

    //    var tasks = new List<Task>
    //    {
    //        _dbOperations.CreateOrderAsync(newOrder)
    //    };

    //    // Обновление случайного заказа
    //    var orders = await _dbOperations.GetOrdersAsync();
    //    if (orders.Any())
    //    {
    //        var orderToUpdate = orders[_random.Next(orders.Count)];
    //        tasks.Add(_dbOperations.UpdateOrderAsync(
    //            orderToUpdate.Id,
    //            DateTime.Now.AddDays(_random.Next(1, 10))));
    //    }

    //    // Удаление случайного заказа
    //    if (orders.Any())
    //    {
    //        var orderToDelete = orders[_random.Next(orders.Count)];
    //        tasks.Add(_dbOperations.DeleteOrderAsync(orderToDelete.Id));
    //    }

    //    return tasks;
    //}

    //private async Task<List<Task>> GeneratePartTasksAsync(int i)
    //{
    //    var parts = await _dbOperations.GetPartsAsync();
    //    if (!parts.Any()) return new List<Task>();

    //    // Создание новой запчасти
    //    var newPart = new Faker<Part>()
    //        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
    //        .RuleFor(p => p.Price, f => f.Random.Number(100, 50000))
    //        .Generate();

    //    var tasks = new List<Task>
    //    {
    //        _dbOperations.CreatePartAsync(newPart)
    //    };

    //    // Обновление случайной запчасти
    //    var partToUpdate = parts[_random.Next(parts.Count)];
    //    tasks.Add(_dbOperations.UpdatePartAsync(
    //        partToUpdate.Id,
    //        _random.Next(100, 1000)));

    //    // Удаление случайной запчасти
    //    var partToDelete = parts[_random.Next(parts.Count)];
    //    tasks.Add(_dbOperations.DeletePartAsync(partToDelete.Id));

    //    return tasks;
    //}

    //private async Task<List<Task>> GenerateServiceTasksAsync(int i)
    //{
    //    var services = await _dbOperations.GetServicesAsync();
    //    if (!services.Any()) return new List<Task>();

    //    // Создание новой услуги
    //    var newService = new Faker<Service>()
    //        .RuleFor(s => s.Name, f => f.Commerce.ProductName())
    //        .RuleFor(s => s.Price, f => f.Random.Number(500, 30000))
    //        .Generate();

    //    var tasks = new List<Task>
    //    {
    //        _dbOperations.CreateServiceAsync(newService)
    //    };

    //    // Обновление случайной услуги
    //    var serviceToUpdate = services[_random.Next(services.Count)];
    //    tasks.Add(_dbOperations.UpdateServiceAsync(
    //        serviceToUpdate.Id,
    //        _random.Next(100, 1000)));

    //    // Удаление случайной услуги
    //    var serviceToDelete = services[_random.Next(services.Count)];
    //    tasks.Add(_dbOperations.DeleteServiceAsync(serviceToDelete.Id));

    //    return tasks;
    //}

}