using Bogus;
using CarserviceConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarserviceConsoleApp
{
    public class RequestGenerator
    {
        private readonly DatabaseOperations _dbOperations;
        private readonly Random _random = Random.Shared;

        public RequestGenerator(DatabaseOperations dbOperations)
        {
            _dbOperations = dbOperations;
        }

        public async Task GenerateRequestsAsync()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 3; i++)
            {
                tasks.AddRange(await GenerateClientTasksAsync(i));
                tasks.AddRange(await GenerateCarTasksAsync(i));
                tasks.AddRange(await GenerateEmployeeTasksAsync(i));
                tasks.AddRange(await GenerateOrderTasksAsync(i));
                tasks.AddRange(await GeneratePartTasksAsync(i));
                tasks.AddRange(await GenerateServiceTasksAsync(i));
            }

            // Дожидаемся завершения всех задач
            await Task.WhenAll(tasks);
        }

        private async Task<List<Task>> GenerateClientTasksAsync(int i)
        {
            var clientFaker = new Faker<Client>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            var tasks = new List<Task>();
            var client = clientFaker.Generate();
            tasks.Add(_dbOperations.CreateClientAsync(client));
            tasks.Add(_dbOperations.GetClientsAsync());

            // Генерация случайного имени для обновления клиента
            var clientIdToUpdate = _random.Next(1, 100);
            var updatedClientName = new Faker().Name.FullName(); // новое случайное имя
            tasks.Add(_dbOperations.UpdateClientAsync(clientIdToUpdate, updatedClientName));

            var clientIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeleteClientAsync(clientIdToDelete));

            return tasks;
        }

        private async Task<List<Task>> GenerateCarTasksAsync(int i)
        {
            var carFaker = new Faker<Car>()
                .RuleFor(c => c.Brand, f => f.PickRandom(DataConstants.ServiceToParts.Keys.ToArray()))
                .RuleFor(c => c.Model, (f, car) =>
                {
                    var models = DataConstants.ServiceToParts[car.Brand];
                    return f.PickRandom(models);
                })
                .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
                .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now)))
                .RuleFor(c => c.ClientId, f => f.Random.Number(1, 100));

            var tasks = new List<Task>();
            var car = carFaker.Generate();
            tasks.Add(_dbOperations.CreateCarAsync(car));

            // Генерация случайных данных для обновления автомобиля
            var carIdToUpdate = _random.Next(1, 100);
            var updatedBrand = new Faker().Vehicle.Manufacturer(); // новое случайное имя бренда
            var updatedModel = new Faker().Vehicle.Model(); // новое случайное имя модели
            tasks.Add(_dbOperations.UpdateCarAsync(carIdToUpdate, updatedBrand, updatedModel));

            var carIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeleteCarAsync(carIdToDelete));

            return tasks;
        }

        private async Task<List<Task>> GenerateEmployeeTasksAsync(int i)
        {
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"));

            var tasks = new List<Task>();
            var employee = employeeFaker.Generate();
            tasks.Add(_dbOperations.CreateEmployeeAsync(employee));
            tasks.Add(_dbOperations.GetClientsAsync());

            // Генерация случайного имени для обновления сотрудника
            var employeeIdToUpdate = _random.Next(1, 100);
            var updatedEmployeeName = new Faker().Name.FullName(); // новое случайное имя
            tasks.Add(_dbOperations.UpdateEmployeeAsync(employeeIdToUpdate, updatedEmployeeName));

            var employeeIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeleteEmployeeAsync(employeeIdToDelete));

            return tasks;
        }

        private async Task<List<Task>> GenerateOrderTasksAsync(int i)
        {
            var tasks = new List<Task>();
            var order = new Order
            {
                CarId = _random.Next(1, 100),
                ClientId = _random.Next(1, 100),
                CreatedAt = DateTime.Now,
                CompletedAt = DateTime.Now.AddDays(1)
            };
            tasks.Add(_dbOperations.CreateOrderAsync(order));

            var orderIdToUpdate = _random.Next(1, 100);
            tasks.Add(_dbOperations.UpdateOrderAsync(orderIdToUpdate, DateTime.Now.AddDays(2)));

            var orderIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeleteOrderAsync(orderIdToDelete));

            return tasks;
        }

        private async Task<List<Task>> GeneratePartTasksAsync(int i)
        {
            var partFaker = new Faker<Part>()
                .RuleFor(p => p.Name, f => f.PickRandom(DataConstants.PartPrices.Keys.ToArray()))
                .RuleFor(p => p.Price, (f, part) =>
                {
                    if (DataConstants.PartPrices.TryGetValue(part.Name, out var priceRange))
                    {
                        return f.Random.Number(priceRange.Min, priceRange.Max);
                    }
                    return f.Random.Number(100, 50000);
                });

            var tasks = new List<Task>();
            var part = partFaker.Generate();
            tasks.Add(_dbOperations.CreatePartAsync(part));

            var partIdToUpdate = _random.Next(1, 100);
            tasks.Add(_dbOperations.UpdatePartAsync(partIdToUpdate, _random.Next(100, 1000)));

            var partIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeletePartAsync(partIdToDelete));

            return tasks;
        }

        private async Task<List<Task>> GenerateServiceTasksAsync(int i)
        {
            var serviceFaker = new Faker<Service>()
                .RuleFor(s => s.Name, f => f.PickRandom(DataConstants.ServiceToParts.Keys.ToArray()))
                .RuleFor(s => s.Price, f => f.Random.Number(500, 30000));

            var tasks = new List<Task>();
            var service = serviceFaker.Generate();
            tasks.Add(_dbOperations.CreateServiceAsync(service));

            var serviceIdToUpdate = _random.Next(1, 100);
            tasks.Add(_dbOperations.UpdateServiceAsync(serviceIdToUpdate, _random.Next(100, 1000)));

            var serviceIdToDelete = _random.Next(1, 100);
            tasks.Add(_dbOperations.DeleteServiceAsync(serviceIdToDelete));

            return tasks;
        }
    }

}
