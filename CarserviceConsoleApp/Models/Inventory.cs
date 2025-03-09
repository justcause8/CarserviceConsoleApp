using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public int PartId { get; set; }

    public int Stock { get; set; }

    public virtual Part Part { get; set; } = null!;
}
