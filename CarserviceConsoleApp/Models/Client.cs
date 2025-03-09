using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
