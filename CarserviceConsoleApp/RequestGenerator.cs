using Bogus;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarserviceConsoleApp
{
    public class RequestGenerator
    {
        private readonly DatabaseOperations _dbOperations;
        private readonly DataGenerator _dataGenerator;
        private readonly Random _random = Random.Shared;

        public RequestGenerator(DatabaseOperations dbOperations, DataGenerator dataGenerator)
        {
            _dbOperations = dbOperations;
            _dataGenerator = dataGenerator;
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
            var clients = await _dbOperations.GetClientsAsync();
            var tasks = new List<Task>();

            if (clients.Any())
            {
                // Создаем нового клиента без Id
                var newClient = new Client
                {
                    Name = new Faker().Name.FullName(),
                    Phone = new Faker().Phone.PhoneNumber("+7 (###) ### ## ##")
                };
                tasks.Add(_dbOperations.CreateClientAsync(newClient));

                // Для обновления используем существующий Id
                var clientIdToUpdate = clients.First().Id;
                tasks.Add(_dbOperations.UpdateClientAsync(clientIdToUpdate, newClient.Name));

                // Удаление случайного существующего клиента
                var clientIdToDelete = clients[_random.Next(clients.Count)].Id;
                tasks.Add(_dbOperations.DeleteClientAsync(clientIdToDelete));
            }

            return tasks;
        }

        private async Task<List<Task>> GenerateCarTasksAsync(int i)
        {
            var clients = await _dbOperations.GetClientsAsync();
            var clientIds = clients.Select(c => c.Id).ToList();
            if (!clientIds.Any()) return new List<Task>();

            // Создание новой машины
            var newCar = new Faker<Car>()
                .RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
                .RuleFor(c => c.Model, f => f.Vehicle.Model())
                .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
                .RuleFor(c => c.Year, f => DateOnly.FromDateTime(f.Date.Past(20)))
                .RuleFor(c => c.ClientId, f => f.PickRandom(clientIds))
                .Generate();

            var tasks = new List<Task>
            {
                _dbOperations.CreateCarAsync(newCar)
            };

            // Обновление случайного автомобиля
            var cars = await _dbOperations.GetCarsAsync();
            if (cars.Any())
            {
                var carToUpdate = cars[_random.Next(cars.Count)];
                tasks.Add(_dbOperations.UpdateCarAsync(
                    carToUpdate.Id,
                    new Faker().Vehicle.Manufacturer(),
                    new Faker().Vehicle.Model()));
            }

            // Удаление случайного автомобиля
            if (cars.Any())
            {
                var carToDelete = cars[_random.Next(cars.Count)];
                tasks.Add(_dbOperations.DeleteCarAsync(carToDelete.Id));
            }

            return tasks;
        }

        private async Task<List<Task>> GenerateEmployeeTasksAsync(int i)
        {
            var employees = await _dbOperations.GetEmployeesAsync();
            if (!employees.Any()) return new List<Task>();

            // Создание нового сотрудника
            var newEmployee = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Position, f => f.PickRandom(new[] { "Механик", "Старший механик", "Менеджер", "Руководитель" }))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("+7 (###) ### ## ##"))
                .Generate();

            var tasks = new List<Task>
            {
                _dbOperations.CreateEmployeeAsync(newEmployee)
            };

            // Обновление случайного сотрудника
            var employeeToUpdate = employees[_random.Next(employees.Count)];
            tasks.Add(_dbOperations.UpdateEmployeeAsync(
                employeeToUpdate.Id,
                new Faker().Name.FullName()));

            // Удаление случайного сотрудника
            var employeeToDelete = employees[_random.Next(employees.Count)];
            tasks.Add(_dbOperations.DeleteEmployeeAsync(employeeToDelete.Id));

            return tasks;
        }

        private async Task<List<Task>> GenerateOrderTasksAsync(int i)
        {
            var clients = await _dbOperations.GetClientsAsync();
            var cars = await _dbOperations.GetCarsAsync();
            if (!clients.Any() || !cars.Any()) return new List<Task>();

            // Создание нового заказа
            var newOrder = new Faker<Order>()
                .RuleFor(o => o.CarId, f => f.PickRandom(cars).Id)
                .RuleFor(o => o.ClientId, f => f.PickRandom(clients).Id)
                .RuleFor(o => o.CreatedAt, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now))
                .RuleFor(o => o.CompletedAt, (f, o) => f.Date.Between(o.CreatedAt, o.CreatedAt.AddMonths(6)))
                .Generate();

            var tasks = new List<Task>
            {
                _dbOperations.CreateOrderAsync(newOrder)
            };

            // Обновление случайного заказа
            var orders = await _dbOperations.GetOrdersAsync();
            if (orders.Any())
            {
                var orderToUpdate = orders[_random.Next(orders.Count)];
                tasks.Add(_dbOperations.UpdateOrderAsync(
                    orderToUpdate.Id,
                    DateTime.Now.AddDays(_random.Next(1, 10))));
            }

            // Удаление случайного заказа
            if (orders.Any())
            {
                var orderToDelete = orders[_random.Next(orders.Count)];
                tasks.Add(_dbOperations.DeleteOrderAsync(orderToDelete.Id));
            }

            return tasks;
        }

        private async Task<List<Task>> GeneratePartTasksAsync(int i)
        {
            var parts = await _dbOperations.GetPartsAsync();
            if (!parts.Any()) return new List<Task>();

            // Создание новой запчасти
            var newPart = new Faker<Part>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Number(100, 50000))
                .Generate();

            var tasks = new List<Task>
            {
                _dbOperations.CreatePartAsync(newPart)
            };

            // Обновление случайной запчасти
            var partToUpdate = parts[_random.Next(parts.Count)];
            tasks.Add(_dbOperations.UpdatePartAsync(
                partToUpdate.Id,
                _random.Next(100, 1000)));

            // Удаление случайной запчасти
            var partToDelete = parts[_random.Next(parts.Count)];
            tasks.Add(_dbOperations.DeletePartAsync(partToDelete.Id));

            return tasks;
        }

        private async Task<List<Task>> GenerateServiceTasksAsync(int i)
        {
            var services = await _dbOperations.GetServicesAsync();
            if (!services.Any()) return new List<Task>();

            // Создание новой услуги
            var newService = new Faker<Service>()
                .RuleFor(s => s.Name, f => f.Commerce.ProductName())
                .RuleFor(s => s.Price, f => f.Random.Number(500, 30000))
                .Generate();

            var tasks = new List<Task>
            {
                _dbOperations.CreateServiceAsync(newService)
            };

            // Обновление случайной услуги
            var serviceToUpdate = services[_random.Next(services.Count)];
            tasks.Add(_dbOperations.UpdateServiceAsync(
                serviceToUpdate.Id,
                _random.Next(100, 1000)));

            // Удаление случайной услуги
            var serviceToDelete = services[_random.Next(services.Count)];
            tasks.Add(_dbOperations.DeleteServiceAsync(serviceToDelete.Id));

            return tasks;
        }
    }
}