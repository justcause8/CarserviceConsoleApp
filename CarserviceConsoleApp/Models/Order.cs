using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Order
{
    public int Id { get; set; }

    public int CarId { get; set; }

    public int ClientId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderAssignment> OrderAssignments { get; set; } = new List<OrderAssignment>();

    public virtual ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
}
