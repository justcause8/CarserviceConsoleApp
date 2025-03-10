using Bogus;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

public class DataGenerator
{
    private readonly IDbContextFactory<CarserviceContext> _contextFactory;

    public DataGenerator(IDbContextFactory<CarserviceContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private static readonly Dictionary<string, string[]> serviceToParts = new()
    {
        { "Замена масла", new[] { "Масляный фильтр" } },
        { "Замена тормозных колодок", new[] { "Тормозные колодки" } },
        { "Ремонт двигателя", new[] { "Свечи зажигания", "Топливный фильтр", "Лямбда-зонд" } },
        { "Ремонт ходовой части", new[] { "Амортизаторы", "Шаровые опоры", "Подшипники ступицы" } },
        { "Диагностика автомобиля", new[] { "Датчик давления масла", "Лямбда-зонд" } },
        { "Шиномонтаж", new[] { "Тормозные диски", "Тормозные колодки" } },
        { "Замена воздушного фильтра", new[] { "Воздушный фильтр" } },
        { "Ремонт электроники", new[] { "Лямбда-зонд", "Датчик давления масла" } },
        { "Покраска кузова", new[] { "Лобовое стекло" } },
        { "Ремонт топливной системы", new[] { "Топливный фильтр" } },
        { "Замена свечей зажигания", new[] { "Свечи зажигания" } },
        { "Ремонт коробки передач", new[] { "Выжимной подшипник", "Колодки сцепления" } },
        { "Установка сигнализации", new[] { "Аккумулятор" } },
        { "Ремонт кондиционера", new[] { "Насос гидроусилителя" } },
        { "Замена лобового стекла", new[] { "Лобовое стекло" } },
        { "Чистка радиатора", new[] { "Радиатор охлаждения" } },
        { "Замена ремня ГРМ", new[] { "Ремень ГРМ" } },
        { "Ремонт подвески", new[] { "Амортизаторы", "Стойки стабилизатора" } },
        { "Замена аккумулятора", new[] { "Аккумулятор" } },
        { "Техническое обслуживание", new[] { "Масляный фильтр", "Воздушный фильтр", "Свечи зажигания" } }
    };

    private static readonly Dictionary<string, (int Min, int Max)> partPrices = new()
    {
        { "Тормозные колодки", (1000, 3000) },
        { "Масляный фильтр", (200, 800) },
        { "Воздушный фильтр", (300, 1000) },
        { "Свечи зажигания", (500, 2000) },
        { "Аккумулятор", (4000, 15000) },
        { "Ремень ГРМ", (1500, 5000) },
        { "Топливный фильтр", (400, 1200) },
        { "Амортизаторы", (2000, 8000) },
        { "Стойки стабилизатора", (1000, 4000) },
        { "Радиатор охлаждения", (3000, 12000) },
        { "Термостат", (800, 3000) },
        { "Насос гидроусилителя", (2500, 10000) },
        { "Шаровые опоры", (1500, 6000) },
        { "Рулевые тяги", (1000, 5000) },
        { "Тормозные диски", (2000, 8000) },
        { "Подшипники ступицы", (1000, 4000) },
        { "Колодки сцепления", (1500, 6000) },
        { "Выжимной подшипник", (2000, 8000) },
        { "Лямбда-зонд", (1500, 7000) },
        { "Датчик давления масла", (500, 2500) },
        { "Лобовое стекло", (5000, 20000) }
    };

    public async Task GenerateDataAsync()
    {
        var tasks = new List<Task>();

        tasks.Add(GeneratePartsAsync());
        tasks.Add(GenerateServicesAsync());
        tasks.Add(GenerateClientsAsync());
        tasks.Add(GenerateEmployeesAsync());
        tasks.Add(GenerateCarsAsync());
        tasks.Add(GenerateInventoryAsync());
        tasks.Add(GenerateOrdersAsync());
        tasks.Add(GenerateOrderPartsAsync());
        tasks.Add(GenerateOrderAssignmentsAsync());
        tasks.Add(GenerateOrderServicesAsync());

        await Task.WhenAll(tasks);

        Console.WriteLine("Данные успешно сгенерированы!");
    }


    private async Task GeneratePartsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Parts.AnyAsync())
        {
            Console.WriteLine("Генерация запчастей...");
            var partNames = serviceToParts.Values.SelectMany(parts => parts).Distinct().ToArray();
            var partsFaker = new Faker<Part>()
                .RuleFor(i => i.Name, f => f.PickRandom(partNames))
                .RuleFor(i => i.Price, (f, part) =>
                {
                    // Получаем диапазон цен для текущей запчасти
                    if (partPrices.TryGetValue(part.Name, out var priceRange))
                    {
                        return f.Random.Number(priceRange.Min, priceRange.Max);
                    }
                    return f.Random.Number(100, 50000); // Если диапазон не найден, используем общий диапазон
                });

            var parts = partsFaker.Generate(partNames.Length);
            await context.Parts.AddRangeAsync(parts);
            await context.SaveChangesAsync();
            
            Console.WriteLine("Запчасти успешно добавлены!");
        }
        
    }

    private async Task GenerateServicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Services.AnyAsync())
        {
            Console.WriteLine("Генерация услуг...");

            var servicesFaker = new Faker<Service>()
                .RuleFor(s => s.Name, f => f.PickRandom(serviceToParts.Keys.ToArray()))
                .RuleFor(s => s.Price, f => f.Random.Number(500, 30000));

            var services = servicesFaker.Generate(serviceToParts.Count);
            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();

            // Повторно загружаем услуги и запчасти после сохранения
            var savedServices = await context.Services.ToListAsync();
            var parts = await context.Parts.ToListAsync();

            var serviceParts = new List<ServicePart>();

            foreach (var service in savedServices)
            {
                if (serviceToParts.TryGetValue(service.Name, out var partNames))
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

            if (serviceParts.Count > 0)
            {
                await context.ServiceParts.AddRangeAsync(serviceParts);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {serviceParts.Count} связей ServicePart");
            }
            else
            {
                Console.WriteLine("Не удалось создать связи ServicePart – проверьте генерацию запчастей.");
            }

            Console.WriteLine("Услуги успешно добавлены!");
        }
    }


    private async Task GenerateEmployeesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Employees.AnyAsync())
        {
            Console.WriteLine("Генерация сотрудников...");
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            var employees = employeeFaker.Generate(300);
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
            
            Console.WriteLine("Сотрудники успешно добавлены!");
        }
        
    }

    private async Task GenerateClientsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Clients.AnyAsync())
        {
            Console.WriteLine("Генерация клиентов...");

            var batchSize = 1000;
            var totalClients = 10000;    

            var clientsFaker = new Faker<Client>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            for (int i = 0; i < totalClients; i += batchSize)
            {
                var clients = clientsFaker.Generate(batchSize);
                await context.Clients.AddRangeAsync(clients);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} клиентов...");
            }

            Console.WriteLine("Клиенты успешно добавлены!");
        }
    }

    private async Task GenerateCarsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Cars.AnyAsync())
        {
            Console.WriteLine("Генерация автомобилей...");

            var clientIds = await context.Clients.Select(c => c.Id).ToListAsync();

            if (!clientIds.Any()) return;

            var carData = new Dictionary<string, string[]>
            {
                { "Toyota", new[] { "Corolla", "Camry", "RAV4", "Land Cruiser", "Prius", "Highlander", "Yaris", "Tacoma", "Sienna", "Tundra" } },
                { "Porsche", new[] { "911", "Cayenne", "Panamera", "Macan", "Boxster", "Taycan", "718 Cayman" } },
                { "Volvo", new[] { "XC90", "XC60", "S60", "V90", "S90", "XC40", "V60", "C40" } },
                { "BMW", new[] { "X5", "3 Series", "5 Series", "7 Series", "X3", "X7", "M3", "M5", "Z4", "iX" } },
                { "Mercedes-Benz", new[] { "C-Class", "E-Class", "S-Class", "GLC", "GLE", "GLA", "G-Class", "CLA", "A-Class", "EQS" } },
                { "Ford", new[] { "F-150", "Mustang", "Explorer", "Escape", "Focus", "Fiesta", "Bronco", "Ranger", "Edge", "Transit" } },
                { "Chevrolet", new[] { "Silverado", "Equinox", "Malibu", "Tahoe", "Suburban", "Corvette", "Camaro", "Trailblazer", "Spark", "Blazer" } },
                { "Honda", new[] { "Civic", "Accord", "CR-V", "Pilot", "Odyssey", "HR-V", "Ridgeline", "Fit", "Passport", "Insight" } },
                { "Nissan", new[] { "Altima", "Maxima", "Sentra", "Rogue", "Pathfinder", "Murano", "Titan", "Frontier", "Leaf", "GT-R" } },
                { "Hyundai", new[] { "Elantra", "Sonata", "Tucson", "Santa Fe", "Kona", "Palisade", "Venue", "Ioniq", "Nexo", "Veloster" } },
                { "Audi", new[] { "A3", "A4", "A6", "A8", "Q3", "Q5", "Q7", "Q8", "TT", "R8" } },
                { "Volkswagen", new[] { "Golf", "Passat", "Jetta", "Tiguan", "Atlas", "Arteon", "Beetle", "ID.4", "Polo", "Up!" } },
                { "Lexus", new[] { "ES", "IS", "RX", "NX", "GX", "LX", "UX", "LC", "LS", "LM" } },
                { "Subaru", new[] { "Outback", "Forester", "Impreza", "Legacy", "Crosstrek", "WRX", "BRZ", "Ascent", "Levorg", "Exiga" } },
                { "Mazda", new[] { "CX-5", "CX-9", "Mazda3", "Mazda6", "MX-5 Miata", "CX-30", "CX-3", "RX-7", "RX-8", "BT-50" } },
                { "Kia", new[] { "Optima", "Sorento", "Sportage", "Soul", "Telluride", "Stinger", "Rio", "Niro", "Carnival", "EV6" } },
            };

            var batchSize = 1000;
            var totalCars = 30000;

            var carFaker = new Faker<Car>()
                .RuleFor(c => c.Brand, f => f.PickRandom(carData.Keys.ToArray())) // Выбираем случайный бренд
                .RuleFor(c => c.Model, (f, car) =>
                {
                    // Получаем список моделей для выбранного бренда
                    var models = carData[car.Brand];
                    // Выбираем случайную модель из списка
                    return f.PickRandom(models);
                })
                .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
                .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now)))
                .RuleFor(c => c.ClientId, f => f.PickRandom(clientIds)); // Теперь `clientIds` содержит список

            //var cars = carFaker.Generate(30000);
            //await context.Cars.AddRangeAsync(cars);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalCars; i += batchSize)
            {
                var cars = carFaker.Generate(batchSize);
                await context.Cars.AddRangeAsync(cars);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} автомобилей...");
            }

            Console.WriteLine("Автомобили успешно добавлены!");
        }
    }

    private async Task GenerateInventoryAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Inventories.AnyAsync())
        {
            Console.WriteLine("Генерация склада...");
            var partIds = await context.Parts.Select(p => p.Id).ToListAsync();

            if (!partIds.Any()) return;

            var batchSize = 1000;
            var totalInventories = 5000;

            var inventoryFaker = new Faker<Inventory>()
                .RuleFor(i => i.PartId, f => f.PickRandom(partIds))
                .RuleFor(i => i.Stock, f => f.Random.Number(1, 100));

            //var inventories = inventoryFaker.Generate(5000);
            //await context.Inventories.AddRangeAsync(inventories);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalInventories; i += batchSize)
            {
                var inventories = inventoryFaker.Generate(batchSize);
                await context.Inventories.AddRangeAsync(inventories);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} запчастей в складе...");
            }

            Console.WriteLine("Склад успешно добавлен!");
        }
    }

    private async Task GenerateOrdersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.Orders.AnyAsync())
        {
            Console.WriteLine("Генерация заказов...");
            var carIds = await context.Cars.Select(c => c.Id).ToListAsync();
            var clientIds = await context.Clients.Select(c => c.Id).ToListAsync();

            var batchSize = 1000;
            var totalOrders = 40000;

            var orderFaker = new Faker<Order>()
               .RuleFor(o => o.CarId, f => f.PickRandom(carIds))
               .RuleFor(o => o.ClientId, f => f.PickRandom(clientIds))
               .RuleFor(o => o.CreatedAt, f =>
               {
                   // Генерируем случайную дату в диапазоне от "сегодня минус 20 лет" до "сегодня"
                   var startDate = DateTime.Now.AddYears(-20);
                   return f.Date.Between(startDate, DateTime.Now);
               })
               .RuleFor(o => o.CompletedAt, (f, order) =>
               {
                   // Генерируем случайную дату в диапазоне от CreatedAt до CreatedAt + 6 месяцев
                   var endDate = order.CreatedAt.AddMonths(6);
                   return f.Date.Between(order.CreatedAt, endDate);
               });

            //var orders = orderFaker.Generate(40000);
            //await context.Orders.AddRangeAsync(orders);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalOrders; i += batchSize)
            {
                var orders = orderFaker.Generate(batchSize);
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} заказов...");
            }

            Console.WriteLine("Заказы успешно добавлены!");
        }
    }

    private async Task GenerateOrderPartsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.OrderParts.AnyAsync())
        {
            Console.WriteLine("Генерация запчастей в заказах...");
            var orderIds = await context.Orders.Select(o => o.Id).ToListAsync();
            var partIds = await context.Parts.Select(p => p.Id).ToListAsync();

            var batchSize = 1000;
            var totalOrderParts = 50000;

            var orderPartsFaker = new Faker<OrderPart>()
                .RuleFor(op => op.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(op => op.PartId, f => f.PickRandom(partIds))
                .RuleFor(op => op.Quantity, f => f.Random.Number(1, 10));

            //var orderParts = orderPartsFaker.Generate(50000);
            //await context.OrderParts.AddRangeAsync(orderParts);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalOrderParts; i += batchSize)
            {
                var orderParts = orderPartsFaker.Generate(batchSize);
                await context.OrderParts.AddRangeAsync(orderParts);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} запчастей в заказах...");
            }

            Console.WriteLine("Запчасти в заказах успешно добавлены!");
        }
    }

    private async Task GenerateOrderServicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.OrderServices.AnyAsync())
        {
            Console.WriteLine("Генерация услуг в заказах...");
            var orderIds = await context.Orders.Select(o => o.Id).ToListAsync();
            var serviceIds = await context.Services.Select(s => s.Id).ToListAsync();

            //if (!orderIds.Any() || !serviceIds.Any())
            //{
            //    Console.WriteLine("Не найдено заказов или услуг. Пропуск генерации OrderServices.");
            //    return;
            //}

            var batchSize = 1000;
            var totalOrderServices = 50000;

            var orderServicesFaker = new Faker<OrderService>()
                .RuleFor(os => os.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(os => os.ServiceId, f => f.PickRandom(serviceIds));

            //var orderServices = orderServicesFaker.Generate(50000);
            //await context.OrderServices.AddRangeAsync(orderServices);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalOrderServices; i += batchSize)
            {
                var orderServices = orderServicesFaker.Generate(batchSize);
                await context.OrderServices.AddRangeAsync(orderServices);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} услуг в заказах...");
            }

            Console.WriteLine("Услуги в заказах успешно добавлены!");
        }
    }

    private async Task GenerateOrderAssignmentsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await context.OrderAssignments.AnyAsync())
        {
            Console.WriteLine("Генерация назначений сотрудников на заказы...");
            var orderIds = await context.Orders.Select(o => o.Id).ToListAsync();
            var employeeIds = await context.Employees.Select(e => e.Id).ToListAsync();

            //if (!orderIds.Any() || !employeeIds.Any())
            //{
            //    Console.WriteLine("Не найдено заказов или сотрудников. Пропуск генерации OrderAssignments.");
            //    return;
            //}

            var batchSize = 1000;
            var totalOrderAssignments = 50000;

            var orderAssignmentsFaker = new Faker<OrderAssignment>()
                .RuleFor(oa => oa.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(oa => oa.EmployeeId, f => f.PickRandom(employeeIds));

            //var orderAssignments = orderAssignmentsFaker.Generate(50000);
            //await context.OrderAssignments.AddRangeAsync(orderAssignments);
            //await context.SaveChangesAsync();

            for (int i = 0; i < totalOrderAssignments; i += batchSize)
            {
                var orderAssignments = orderAssignmentsFaker.Generate(batchSize);
                await context.OrderAssignments.AddRangeAsync(orderAssignments);
                await context.SaveChangesAsync();
                Console.WriteLine($"Добавлено {i + batchSize} сотрудников в заказах...");
            }

            Console.WriteLine("Сотрудники в заказах успешно добавлены!");
        }
    }

    public async Task ClearDatabaseAsync()
    {
        try
        {
            Console.WriteLine("Очистка базы данных...");

            using var context = await _contextFactory.CreateDbContextAsync();

            // Удаляем записи из таблиц в правильном порядке, учитывая связи
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

            // Сбрасываем счетчики идентификаторов для таблиц с автоинкрементом
            var tablesToReset = new[] { "Clients", "Cars", "Employees", "Orders", "Parts", "Services", "Inventories" };

            foreach (var table in tablesToReset)
            {
                await context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('{table}', RESEED, 0)");
            }

            Console.WriteLine("База данных успешно очищена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при очистке базы данных: {ex.Message}");
        }
    }
}
