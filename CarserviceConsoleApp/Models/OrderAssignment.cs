using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class OrderAssignment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
