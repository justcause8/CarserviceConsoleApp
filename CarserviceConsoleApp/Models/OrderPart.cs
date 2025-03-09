﻿using System;
using System.Collections.Generic;

namespace CarserviceConsoleApp.Models;

public partial class OrderPart
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int PartId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Part Part { get; set; } = null!;
}
