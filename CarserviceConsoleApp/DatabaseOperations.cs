using Bogus;
using Bogus.DataSets;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarserviceConsoleApp
{
    public class DatabaseOperations
    {
        private readonly IDbContextFactory<CarserviceContext> _contextFactory;

        public DatabaseOperations(IDbContextFactory<CarserviceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Логирование
        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        // Clients
        public async Task CreateClientAsync(Client client)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Clients.Add(client);
            await context.SaveChangesAsync();
            Log($"[CREATE]Создан клиент: \"{client.Name}\", ID: {client.Id}.");
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Clients.ToListAsync();
        }

        public async Task UpdateClientAsync(int clientId, string newName)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var client = await context.Clients.FindAsync(clientId);
            if (client != null)
            {
                string oldName = client.Name;
                client.Name = newName;
                await context.SaveChangesAsync();
                Log($"[UPDATE]Обновлен клиент с ID {clientId}. Старое имя: \"{oldName}\", новое имя: \"{newName}\".");
            }
        }

        public async Task DeleteClientAsync(int clientId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var client = await context.Clients
                .Include(c => c.Cars) // Включаем автомобили клиента
                .Include(c => c.Orders) // Включаем заказы клиента
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client != null)
            {
                string clientName = client.Name;

                // Удаляем все связанные автомобили
                context.Cars.RemoveRange(client.Cars);

                // Удаляем все связанные заказы
                foreach (var order in client.Orders)
                {
                    // Удаляем связанные записи из таблицы OrderParts
                    var relatedOrderParts = await context.OrderParts.Where(op => op.OrderId == order.Id).ToListAsync();
                    context.OrderParts.RemoveRange(relatedOrderParts);

                    // Удаляем связанные записи из таблицы OrderServices
                    var relatedOrderServices = await context.OrderServices.Where(os => os.OrderId == order.Id).ToListAsync();
                    context.OrderServices.RemoveRange(relatedOrderServices);

                    // Удаляем связанные записи из таблицы OrderAssignments
                    var relatedOrderAssignments = await context.OrderAssignments.Where(oa => oa.OrderId == order.Id).ToListAsync();
                    context.OrderAssignments.RemoveRange(relatedOrderAssignments);
                }

                // Удаляем все заказы клиента
                context.Orders.RemoveRange(client.Orders);

                // Удаляем самого клиента
                context.Clients.Remove(client);

                await context.SaveChangesAsync();
                Log($"[DELETE]Удален клиент с ID {clientId}. Имя: \"{clientName}\".");
            }
        }

        // Cars
        public async Task CreateCarAsync(Car car)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Cars.Add(car);
            await context.SaveChangesAsync();
            Log($"[CREATE]Создан автомобиль: {car.Brand} {car.Model}, ID: {car.Id}.");
        }

        public async Task<List<Car>> GetCarsByClientIdAsync(int clientId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Cars.Where(c => c.ClientId == clientId).ToListAsync();
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Cars.ToListAsync();
        }

        public async Task UpdateCarAsync(int carId, string newBrand, string newModel)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var car = await context.Cars.FindAsync(carId);
            if (car != null)
            {
                string oldBrand = car.Brand, oldModel = car.Model;
                car.Brand = newBrand;
                car.Model = newModel;
                await context.SaveChangesAsync();
                Log($"[UPDATE]Обновлен автомобиль с ID {carId}. Было: {oldBrand} {oldModel}, стало: {newBrand} {newModel}.");
            }
        }

        public async Task DeleteCarAsync(int carId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var car = await context.Cars.FindAsync(carId);
            if (car != null)
            {
                // Находим связанные заказы
                var relatedOrders = await context.Orders.Where(o => o.CarId == carId).ToListAsync();

                // Удаляем связанные записи из таблицы OrderParts
                foreach (var order in relatedOrders)
                {
                    var relatedOrderParts = await context.OrderParts.Where(op => op.OrderId == order.Id).ToListAsync();
                    context.OrderParts.RemoveRange(relatedOrderParts);
                }

                // Удаляем связанные заказы
                context.Orders.RemoveRange(relatedOrders);

                // Удаляем автомобиль
                context.Cars.Remove(car);
                await context.SaveChangesAsync();
                Log($"[DELETE]Удален автомобиль с ID {carId}. Был: {car.Brand} {car.Model}.");
            }
        }

        // Employees
        public async Task CreateEmployeeAsync(Employee employee)
        {

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            Log($"[CREATE]Создан сотрудник: \"{employee.Name}\", Телефон: {employee.Phone}, ID: {employee.Id}.");
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Employees.ToListAsync();
        }

        public async Task UpdateEmployeeAsync(int employeeId, string newName)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                string oldName = employee.Name;
                employee.Name = newName;
                await context.SaveChangesAsync();
                Log($"[UPDATE]Обновлен сотрудник с ID {employeeId}. Старое имя: \"{oldName}\", новое имя: \"{newName}\".");
            }
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                string employeeName = employee.Name;
                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
                Log($"[DELETE]Удален сотрудник с ID {employeeId}. Имя: \"{employeeName}\".");
            }
        }

        // Orders
        public async Task CreateOrderAsync(Order order)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Создан Заказ ID: {order.Id}, Клиент ID: {order.ClientId}.");
        }


        public async Task<List<Order>> GetOrdersByClientIdAsync(int clientId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Orders.Where(o => o.ClientId == clientId).ToListAsync();
        }

        public async Task UpdateOrderAsync(int orderId, DateTime newCompletedAt)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var order = await context.Orders.FindAsync(orderId);
            if (order != null)
            {
                var oldDate = order.CompletedAt;
                order.CompletedAt = newCompletedAt;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлён Заказ ID: {orderId}, Старая дата завершения: {oldDate}, Новая дата: {newCompletedAt}.");
            }
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var order = await context.Orders.FindAsync(orderId);
            if (order != null)
            {
                // Удаляем связанные записи из таблицы OrderParts
                var relatedOrderParts = await context.OrderParts.Where(op => op.OrderId == orderId).ToListAsync();
                context.OrderParts.RemoveRange(relatedOrderParts);

                // Удаляем заказ
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалён Заказ ID: {orderId}.");
            }
        }

        // Parts
        public async Task CreatePartAsync(Part part)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Parts.Add(part);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Создана Запчасть ID: {part.Id}, Название: \"{part.Name}\".");
        }

        public async Task<List<Part>> GetPartsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Parts.ToListAsync();
        }

        public async Task UpdatePartAsync(int partId, decimal newPrice)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var part = await context.Parts.FindAsync(partId);
            if (part != null)
            {
                decimal oldPrice = part.Price;
                part.Price = newPrice;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлена Запчасть ID: {partId}, Старая цена: {oldPrice}, Новая цена: {newPrice}.");
            }
        }

        public async Task DeletePartAsync(int partId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var part = await context.Parts.FindAsync(partId);

            if (part != null)
            {
                // Находим связанные записи в таблице Inventory
                var relatedInventories = await context.Inventories.Where(i => i.PartId == partId).ToListAsync();

                // Удаляем связанные записи из таблицы Inventory
                context.Inventories.RemoveRange(relatedInventories);

                // Удаляем саму деталь
                context.Parts.Remove(part);

                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалена запчасть с ID {partId}, Название: \"{part.Name}\".");
            }
        }

        // Services
        public async Task CreateServiceAsync(Service service)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Services.Add(service);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Создана услуга: \"{service.Name}\", цена: {service.Price}, ID: {service.Id}.");
        }

        public async Task<List<Service>> GetServicesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Services.ToListAsync();
        }

        public async Task UpdateServiceAsync(int serviceId, decimal newPrice)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var service = await context.Services.FindAsync(serviceId);
            if (service != null)
            {
                decimal oldPrice = service.Price;
                service.Price = newPrice;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлена услуга с ID {serviceId}. Старая цена: {oldPrice}, новая цена: {newPrice}.");
            }
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var service = await context.Services.FindAsync(serviceId);
            if (service != null)
            {
                string serviceName = service.Name;
                context.Services.Remove(service);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалена услуга с ID {serviceId}. Название: \"{serviceName}\".");
            }
        }

        // Inventories
        public async Task CreateInventoryAsync(Inventory inventory)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Inventories.Add(inventory);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Добавлена деталь в склад: \"{inventory.PartId}\", количество: {inventory.Stock}, ID: {inventory.Id}.");
        }

        public async Task<List<Inventory>> GetInventoriesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Inventories.ToListAsync();
        }

        public async Task UpdateInventoryAsync(int inventoryId, int newStock)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var inventory = await context.Inventories.FindAsync(inventoryId);
            if (inventory != null)
            {
                int oldStock = inventory.Stock;
                inventory.Stock = newStock;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлёна деталь в складе с ID {inventoryId}. Старое количество: {oldStock}, новое количество: {newStock}.");
            }
        }

        public async Task DeleteInventoryAsync(int inventoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var inventory = await context.Inventories.FindAsync(inventoryId);
            if (inventory != null)
            {
                int partId = inventory.PartId;
                context.Inventories.Remove(inventory);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалёна деталь из склада с ID {inventoryId} для детали с ID {partId}.");
            }
        }

        // OrderParts
        public async Task CreateOrderPartAsync(OrderPart orderPart)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.OrderParts.Add(orderPart);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Добавлена деталь к заказу. OrderID: {orderPart.OrderId}, PartID: {orderPart.PartId}, Количество: {orderPart.Quantity}.");
        }

        public async Task<List<OrderPart>> GetOrderPartsByOrderIdAsync(int orderId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.OrderParts.Where(op => op.OrderId == orderId).ToListAsync();
        }

        public async Task UpdateOrderPartAsync(int orderPartId, int newQuantity)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderPart = await context.OrderParts.FindAsync(orderPartId);
            if (orderPart != null)
            {
                int oldQuantity = orderPart.Quantity;
                orderPart.Quantity = newQuantity;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлена деталь в заказе с ID {orderPartId}. Старое количество: {oldQuantity}, новое количество: {newQuantity}.");
            }
        }

        public async Task DeleteOrderPartAsync(int orderPartId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderPart = await context.OrderParts.FindAsync(orderPartId);
            if (orderPart != null)
            {
                int partId = orderPart.PartId;
                context.OrderParts.Remove(orderPart);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалена деталь с ID {partId} из заказа с ID {orderPart.OrderId}.");
            }
        }

        // OrderServices
        public async Task CreateOrderServiceAsync(OrderService orderService)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.OrderServices.Add(orderService);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Добавлена услуга к заказу. OrderID: {orderService.OrderId}, ServiceID: {orderService.ServiceId}.");
        }

        public async Task<List<OrderService>> GetOrderServicesByOrderIdAsync(int orderId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.OrderServices.Where(os => os.OrderId == orderId).ToListAsync();
        }

        public async Task UpdateOrderServiceAsync(int orderServiceId, int newServiceId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderService = await context.OrderServices.FindAsync(orderServiceId);
            if (orderService != null)
            {
                int oldServiceId = orderService.ServiceId;
                orderService.ServiceId = newServiceId;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлена услуга в заказе с ID {orderServiceId}. Старый ServiceID: {oldServiceId}, новый ServiceID: {newServiceId}.");
            }
        }

        public async Task DeleteOrderServiceAsync(int orderServiceId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderService = await context.OrderServices.FindAsync(orderServiceId);
            if (orderService != null)
            {
                int serviceId = orderService.ServiceId;
                context.OrderServices.Remove(orderService);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалена услуга с ID {serviceId} из заказа с ID {orderService.OrderId}.");
            }
        }

        // OrderAssignments
        public async Task CreateOrderAssignmentAsync(OrderAssignment orderAssignment)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.OrderAssignments.Add(orderAssignment);
            await context.SaveChangesAsync();
            Console.WriteLine($"[CREATE]Назначен сотрудник с ID {orderAssignment.EmployeeId} на заказ с ID {orderAssignment.OrderId}.");
        }

        public async Task<List<OrderAssignment>> GetOrderAssignmentsByOrderIdAsync(int orderId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.OrderAssignments.Where(oa => oa.OrderId == orderId).ToListAsync();
        }

        public async Task UpdateOrderAssignmentAsync(int orderAssignmentId, int newEmployeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderAssignment = await context.OrderAssignments.FindAsync(orderAssignmentId);
            if (orderAssignment != null)
            {
                int oldEmployeeId = orderAssignment.EmployeeId;
                orderAssignment.EmployeeId = newEmployeeId;
                await context.SaveChangesAsync();
                Console.WriteLine($"[UPDATE]Обновлено назначение в заказе с ID {orderAssignment.OrderId}. Старый EmployeeID: {oldEmployeeId}, новый EmployeeID: {newEmployeeId}.");
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Orders.ToListAsync();
        }

        public async Task DeleteOrderAssignmentAsync(int orderAssignmentId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderAssignment = await context.OrderAssignments.FindAsync(orderAssignmentId);
            if (orderAssignment != null)
            {
                int employeeId = orderAssignment.EmployeeId;
                context.OrderAssignments.Remove(orderAssignment);
                await context.SaveChangesAsync();
                Console.WriteLine($"[DELETE]Удалено назначение сотрудника с ID {employeeId} из заказа с ID {orderAssignment.OrderId}.");
            }
        }
    }
}