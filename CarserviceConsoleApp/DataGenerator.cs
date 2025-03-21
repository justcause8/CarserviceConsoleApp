using Bogus;
using CarserviceConsoleApp;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DataGenerator
{
    private readonly IDbContextFactory<CarserviceContext> _contextFactory; // Поле для хранения фабрики контекста базы данных, которая используется для создания экземпляров CarserviceContext
    private static readonly int batchSize = 1000;

    // Конструктор класса, который принимает фабрику контекста через Dependency Injection и сохраняет её в поле _contextFactory
    public DataGenerator(IDbContextFactory<CarserviceContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Client>> GenerateClients(int count)
    {
        // Создаем временный контекст 
        await using var context = await _contextFactory.CreateDbContextAsync(); 
        var totalClients = 0;
        var allClients = new List<Client>();
        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var clients = new Faker<Client>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"))
                .Generate(currentBatchSize);

            await context.Clients.AddRangeAsync(clients);
            await context.SaveChangesAsync();

            totalClients += currentBatchSize;
            Console.WriteLine($"Добавлено {totalClients} клиентов...");
            allClients.AddRange(clients);
        }
        return allClients;
    }

    public async Task<List<Car>> GenerateCars(int count, List<int> clientIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalCars = 0;
        var allCars = new List<Car>();
        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i); // Если осталось меньше записей, чем размер пачки, берется остаток
            var cars = new Faker<Car>()
                .RuleFor(c => c.Brand, f => f.PickRandom(DataConstants.carData.Keys.ToArray()))
                .RuleFor(c => c.Model, (f, car) =>
                {
                    var models = DataConstants.carData[car.Brand];
                    return f.PickRandom(models);
                })
                .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
                .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now)))
                .RuleFor(c => c.ClientId, f => f.PickRandom(clientIds))
                .Generate(currentBatchSize);

            await context.Cars.AddRangeAsync(cars); // Добавляем все сгенерированные автомобили в контекст базы данных
            await context.SaveChangesAsync();

            totalCars += currentBatchSize; // Увеличиваем на количество добавленных автомобилей в текущей пачке
            Console.WriteLine($"Добавлено {totalCars} автомобилей...");
            allCars.AddRange(cars);
        }
        return allCars;
    }

    public async Task<List<Employee>> GenerateEmployees(int count)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalEmployees = 0;
        var allEmployees = new List<Employee>();
        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var employees = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"))
                .Generate(currentBatchSize);

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            totalEmployees += currentBatchSize;
            Console.WriteLine($"Добавлено {totalEmployees} сотрудников...");
            allEmployees.AddRange(employees);
        }
        return allEmployees;
    }

    public async Task<List<Part>> GenerateParts()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        if (await context.Parts.AnyAsync())
        {
            Console.WriteLine("Запчасти уже существуют в базе данных. Генерация пропущена.");
            return new List<Part>();
        }
        Console.WriteLine("Генерация запчастей...");
        var partNames = DataConstants.ServiceToParts.Values
            .SelectMany(parts => parts)
            .Distinct()
            .ToArray();
        var partsFaker = new Faker<Part>()
            .RuleFor(p => p.Name, f => f.PickRandom(partNames))
            .RuleFor(p => p.Price, (f, part) =>
            {
                if (DataConstants.PartPrices.TryGetValue(part.Name, out var priceRange))
                {
                    return f.Random.Number(priceRange.Min, priceRange.Max);
                }
                return f.Random.Number(100, 50000);
            });

        var parts = partsFaker.Generate(partNames.Length);

        await context.Parts.AddRangeAsync(parts);
        await context.SaveChangesAsync();

        Console.WriteLine($"Добавлено {parts.Count} запчастей...");
        return parts;
    }

    public async Task<List<Service>> GenerateServices(int count)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalServices = 0;
        var allServices = new List<Service>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var services = new Faker<Service>()
                .RuleFor(s => s.Name, f => f.PickRandom(DataConstants.ServiceToParts.Keys.ToArray()))
                .RuleFor(s => s.Price, f => f.Random.Number(500, 30000))
                .Generate(currentBatchSize);

            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();
            totalServices += currentBatchSize;
            Console.WriteLine($"Добавлено {totalServices} услуг...");

            allServices.AddRange(services);
        }

        return allServices;
    }

    public async Task<List<Order>> GenerateOrders(int count, List<int> carIds, List<int> clientIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalOrders = 0;
        var allOrders = new List<Order>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var orders = new Faker<Order>()
                .RuleFor(o => o.CarId, f => f.PickRandom(carIds))
                .RuleFor(o => o.ClientId, f => f.PickRandom(clientIds))
                .RuleFor(o => o.CreatedAt, f =>
                {
                    var startDate = DateTime.Now.AddYears(-20);
                    return f.Date.Between(startDate, DateTime.Now);
                })
                .RuleFor(o => o.CompletedAt, (f, order) =>
                {
                    var endDate = order.CreatedAt.AddMonths(6);
                    return f.Date.Between(order.CreatedAt, endDate);
                })
                .Generate(currentBatchSize);

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            totalOrders += currentBatchSize;
            Console.WriteLine($"Добавлено {totalOrders} заказов...");

            allOrders.AddRange(orders);
        }

        return allOrders;
    }

    public async Task<List<Inventory>> GenerateInventory(int count, List<int> partIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalInventory = 0;
        var allInventories = new List<Inventory>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var inventory = new Faker<Inventory>()
                .RuleFor(i => i.PartId, f => f.PickRandom(partIds))
                .RuleFor(i => i.Stock, f => f.Random.Number(1, 100))
                .Generate(currentBatchSize);

            await context.Inventories.AddRangeAsync(inventory);
            await context.SaveChangesAsync();
            totalInventory += currentBatchSize;
            Console.WriteLine($"Добавлено {totalInventory} записей на складе...");

            allInventories.AddRange(inventory);
        }

        return allInventories;
    }

    public async Task<List<OrderPart>> GenerateOrderParts(int count, List<int> orderIds, List<int> partIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalOrderParts = 0;
        var allOrderParts = new List<OrderPart>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var orderParts = new Faker<OrderPart>()
                .RuleFor(op => op.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(op => op.PartId, f => f.PickRandom(partIds))
                .RuleFor(op => op.Quantity, f => f.Random.Number(1, 10))
                .Generate(currentBatchSize);

            await context.OrderParts.AddRangeAsync(orderParts);
            await context.SaveChangesAsync();
            totalOrderParts += currentBatchSize;
            Console.WriteLine($"Добавлено {totalOrderParts} частей заказов...");

            allOrderParts.AddRange(orderParts);
        }

        return allOrderParts;
    }

    public async Task<List<OrderService>> GenerateOrderServices(int count, List<int> orderIds, List<int> serviceIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalOrderServices = 0;
        var allOrderServices = new List<OrderService>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var orderServices = new Faker<OrderService>()
                .RuleFor(os => os.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(os => os.ServiceId, f => f.PickRandom(serviceIds))
                .Generate(currentBatchSize);

            await context.OrderServices.AddRangeAsync(orderServices);
            await context.SaveChangesAsync();
            totalOrderServices += currentBatchSize;
            Console.WriteLine($"Добавлено {totalOrderServices} услуг заказов...");

            allOrderServices.AddRange(orderServices);
        }

        return allOrderServices;
    }

    public async Task<List<OrderAssignment>> GenerateOrderAssignments(int count, List<int> orderIds, List<int> employeeIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var totalOrderAssignments = 0;
        var allOrderAssignments = new List<OrderAssignment>();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var orderAssignments = new Faker<OrderAssignment>()
                .RuleFor(oa => oa.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(oa => oa.EmployeeId, f => f.PickRandom(employeeIds))
                .Generate(currentBatchSize);

            await context.OrderAssignments.AddRangeAsync(orderAssignments);
            await context.SaveChangesAsync();
            totalOrderAssignments += currentBatchSize;
            Console.WriteLine($"Добавлено {totalOrderAssignments} назначений сотрудников...");

            allOrderAssignments.AddRange(orderAssignments);
        }

        return allOrderAssignments;
    }

    public List<ServicePart> GenerateServiceParts(List<Service> services, List<Part> parts)
    {
        var serviceParts = new List<ServicePart>();
        foreach (var service in services)
        {
            if (DataConstants.ServiceToParts.TryGetValue(service.Name, out var partNames))
            {
                foreach (var partName in partNames)
                {
                    var part = parts.FirstOrDefault(p => p.Name == partName);
                    if (part != null)
                    {
                        serviceParts.Add(new ServicePart
                        {
                            ServiceId = service.Id,
                            PartId = part.Id
                        });
                    }
                }
            }
        }
        return serviceParts;
    }

    public async Task FillDatabaseAsync(
        int clientsCount,
        int carsCount,
        int employeesCount,
        int ordersCount,
        int inventoryCount,
        int orderPartsCount,
        int orderServicesCount,
        int orderAssignmentsCount)
    {
        int servicesCount = DataConstants.ServiceToParts.Keys.Count; // Определяем количество услуг из словаря по их количеству ключей
        
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.BeginTransactionAsync();
        
        try
        {
            var parts = await GenerateParts();
            var services = await GenerateServices(servicesCount);
            var clients = await GenerateClients(clientsCount);
            var employees = await GenerateEmployees(employeesCount);
            var clientIds = clients.Select(c => c.Id).ToList();
            var cars = await GenerateCars(carsCount, clientIds);
            var partIds = parts.Select(p => p.Id).ToList();
            var inventories = await GenerateInventory(inventoryCount, partIds);
            var carIds = cars.Select(c => c.Id).ToList();
            var orders = await GenerateOrders(ordersCount, carIds, clientIds);
            var serviceParts = GenerateServiceParts(services, parts);
            await context.ServiceParts.AddRangeAsync(serviceParts); // AddRangeAsync добавляет сразу все записи из списка serviceParts в контекст (это быстрее чем добавлять по одному)
            await context.SaveChangesAsync();
            var orderIds = orders.Select(o => o.Id).ToList();
            var serviceIds = services.Select(s => s.Id).ToList();
            var employeeIds = employees.Select(e => e.Id).ToList();
            var orderParts = await GenerateOrderParts(orderPartsCount, orderIds, partIds);
            var orderServices = await GenerateOrderServices(orderServicesCount, orderIds, serviceIds);
            var orderAssignments = await GenerateOrderAssignments(orderAssignmentsCount, orderIds, employeeIds);
            
            await context.Database.CommitTransactionAsync(); //Завершаем изменения объектов
        }
        catch (Exception ex)
        {
            await context.Database.RollbackTransactionAsync();
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine($"Детали: {ex.InnerException?.Message}");
            throw;
        }
    }

    public async Task ClearDatabaseAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.OrderParts.ExecuteDeleteAsync();
            await context.OrderServices.ExecuteDeleteAsync();
            await context.OrderAssignments.ExecuteDeleteAsync();
            await context.ServiceParts.ExecuteDeleteAsync();
            await context.Orders.ExecuteDeleteAsync();
            await context.Cars.ExecuteDeleteAsync();
            await context.Clients.ExecuteDeleteAsync();
            await context.Employees.ExecuteDeleteAsync();
            await context.Inventories.ExecuteDeleteAsync();
            await context.Parts.ExecuteDeleteAsync();
            await context.Services.ExecuteDeleteAsync();
            
            var entityTypes = context.Model.GetEntityTypes(); // Возвращаем список всех типов сущностей, определенных в контексте базы данных.
            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName(); // Для каждой сущности получаем имя соответствующей таблице в БД
                var identityColumn = entityType.GetProperties().FirstOrDefault(p => p.ValueGenerated == ValueGenerated.OnAdd); // Перебираем у каждой сущности столбцы и ищем в них столбец (обячно первый) с автоинкрементом
                if (identityColumn != null && !string.IsNullOrEmpty(tableName))
                {
                    await context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('{tableName}', RESEED, 0)"); // Сбрасываем счетчик автоинкремента для таблицы
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при очистке базы данных: {ex.Message}");
        }
    }
}