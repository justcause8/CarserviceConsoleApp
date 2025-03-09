using Bogus;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

public class DataGenerator
{
    private readonly CarserviceContext _context; // Ссылка на контекст БД

    public DataGenerator(CarserviceContext context)
    {
        _context = context;
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
        if (!await _context.Parts.AnyAsync())
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
            await _context.Parts.AddRangeAsync(parts);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateServicesAsync()
    {
        if (!await _context.Services.AnyAsync())
        {
            Console.WriteLine("Генерация услуг...");

            var servicesFaker = new Faker<Service>()
                .RuleFor(s => s.Name, f => f.PickRandom(serviceToParts.Keys.ToArray()))
                .RuleFor(s => s.Price, f => f.Random.Number(500, 30000));

            var services = servicesFaker.Generate(serviceToParts.Count);
            await _context.Services.AddRangeAsync(services);
            await _context.SaveChangesAsync();

            // После генерации услуг добавляем связи с запчастями
            foreach (var service in _context.Services.ToList())
            {
                if (serviceToParts.TryGetValue(service.Name, out var partNames))
                {
                    foreach (var partName in partNames)
                    {
                        var part = _context.Parts.FirstOrDefault(p => p.Name == partName);
                        if (part != null)
                        {
                            _context.ServiceParts.Add(new ServicePart
                            {
                                ServiceId = service.Id,
                                PartId = part.Id
                            });
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateClientsAsync()
    {
        if (!await _context.Clients.AnyAsync())
        {
            Console.WriteLine("Генерация клиентов...");
            var clientsFaker = new Faker<Client>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            var clients = clientsFaker.Generate(100);
            await _context.Clients.AddRangeAsync(clients);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateEmployeesAsync()
    {
        if (!await _context.Employees.AnyAsync())
        {
            Console.WriteLine("Генерация сотрудников...");
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            var employees = employeeFaker.Generate(100);
            await _context.Employees.AddRangeAsync(employees);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateCarsAsync()
    {
        if (!await _context.Cars.AnyAsync())
        {
            Console.WriteLine("Генерация автомобилей...");

            var clientIds = await _context.Clients.Select(c => c.Id).ToListAsync(); // Ожидаем результат

            if (!clientIds.Any()) return; // Проверяем, есть ли клиенты в БД

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
                .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Past(20, DateTime.Now)))
                .RuleFor(c => c.ClientId, f => f.PickRandom(clientIds)); // Теперь `clientIds` содержит список

            var cars = carFaker.Generate(100);
            await _context.Cars.AddRangeAsync(cars);
            await _context.SaveChangesAsync();

            Console.WriteLine("Автомобили успешно добавлены!");
        }
    }

    private async Task GenerateInventoryAsync()
    {
        if (!await _context.Inventories.AnyAsync())
        {
            Console.WriteLine("Генерация склада...");
            var partIds = await _context.Parts.Select(p => p.Id).ToListAsync();

            if (!partIds.Any()) return;

            var inventoryFaker = new Faker<Inventory>()
                .RuleFor(i => i.PartId, f => f.PickRandom(partIds))
                .RuleFor(i => i.Stock, f => f.Random.Number(1, 100));

            var inventories = inventoryFaker.Generate(100);
            await _context.Inventories.AddRangeAsync(inventories);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateOrdersAsync()
    {
        if (!await _context.Orders.AnyAsync())
        {
            Console.WriteLine("Генерация заказов...");
            var carIds = await _context.Cars.Select(c => c.Id).ToListAsync();
            var clientIds = await _context.Clients.Select(c => c.Id).ToListAsync();
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

            var orders = orderFaker.Generate(100);
            await _context.Orders.AddRangeAsync(orders);
            //var batchSize = 1000; // Количество записей в одном пакете
            //for (int i = 0; i < 100000; i += batchSize)
            //{
            //    var orders = orderFaker.Generate(batchSize);
            //    await _context.Orders.AddRangeAsync(orders);
            //    if (i % 5000 == 0) // После каждых 5000 записей сохраняем
            //    {
            //        await _context.SaveChangesAsync();
            //        Console.WriteLine($"Добавлено {i} заказов...");
            //    }
            //}
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateOrderPartsAsync()
    {
        if (!await _context.OrderParts.AnyAsync())
        {
            Console.WriteLine("Генерация запчастей в заказах...");
            var orderIds = await _context.Orders.Select(o => o.Id).ToListAsync();
            var partIds = await _context.Parts.Select(p => p.Id).ToListAsync();
            var orderPartsFaker = new Faker<OrderPart>()
                .RuleFor(op => op.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(op => op.PartId, f => f.PickRandom(partIds))
                .RuleFor(op => op.Quantity, f => f.Random.Number(1, 10));

            var orderParts = orderPartsFaker.Generate(100);
            await _context.OrderParts.AddRangeAsync(orderParts);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateOrderAssignmentsAsync()
    {
        if (!await _context.OrderAssignments.AnyAsync())
        {
            Console.WriteLine("Генерация назначений сотрудников на заказы...");
            var orderIds = await _context.Orders.Select(o => o.Id).ToListAsync();
            var employeeIds = await _context.Employees.Select(e => e.Id).ToListAsync();
            var orderAssignmentsFaker = new Faker<OrderAssignment>()
                .RuleFor(oa => oa.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(oa => oa.EmployeeId, f => f.PickRandom(employeeIds));

            var orderAssignments = orderAssignmentsFaker.Generate(100);
            await _context.OrderAssignments.AddRangeAsync(orderAssignments);
            await _context.SaveChangesAsync();
        }
    }

    private async Task GenerateOrderServicesAsync()
    {
        if (!await _context.OrderServices.AnyAsync())
        {
            Console.WriteLine("Генерация услуг в заказах...");
            var orderIds = await _context.Orders.Select(o => o.Id).ToListAsync();
            var serviceIds = await _context.Services.Select(s => s.Id).ToListAsync();
            var orderServicesFaker = new Faker<OrderService>()
                .RuleFor(os => os.OrderId, f => f.PickRandom(orderIds))
                .RuleFor(os => os.ServiceId, f => f.PickRandom(serviceIds));

            var orderServices = orderServicesFaker.Generate(100);
            await _context.OrderServices.AddRangeAsync(orderServices);
            await _context.SaveChangesAsync();
        }
    }


    //public void ClearDatabase()
    //{
    //    try
    //    {
    //        Console.WriteLine("Очистка базы данных...");

    //        // Удаляем записи из таблиц в правильном порядке, учитывая связи
    //        _context.OrderParts.ExecuteDelete();
    //        _context.OrderServices.ExecuteDelete();
    //        _context.OrderAssignments.ExecuteDelete();
    //        _context.ServiceParts.ExecuteDelete();
    //        _context.Orders.ExecuteDelete();
    //        _context.Cars.ExecuteDelete();
    //        _context.Clients.ExecuteDelete();
    //        _context.Employees.ExecuteDelete();
    //        _context.Inventories.ExecuteDelete();
    //        _context.Parts.ExecuteDelete();
    //        _context.Services.ExecuteDelete();

    //        // Сбрасываем счетчики идентификаторов
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Clients', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Cars', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Employees', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Orders', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Parts', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Services', RESEED, 0)");
    //        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Inventories', RESEED, 0)");

    //        _context.SaveChanges();
    //        Console.WriteLine("База данных успешно очищена.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Ошибка при очистке базы данных: {ex.Message}");
    //    }
    //}
}
