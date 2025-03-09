using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class OrderService
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ServiceId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
