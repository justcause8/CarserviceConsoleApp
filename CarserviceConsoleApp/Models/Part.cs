using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Part
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }
    public List<ServicePart> ServiceParts { get; set; } = new();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();
}
