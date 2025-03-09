using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<OrderAssignment> OrderAssignments { get; set; } = new List<OrderAssignment>();
}
