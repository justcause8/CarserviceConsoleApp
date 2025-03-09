using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Car
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public DateOnly Year { get; set; }

    public string Vin { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
