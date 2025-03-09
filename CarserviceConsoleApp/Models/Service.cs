using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }
    public List<ServicePart> ServiceParts { get; set; } = new();
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
}
