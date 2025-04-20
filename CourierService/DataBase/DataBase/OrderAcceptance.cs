using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class OrderAcceptance
{
    public Guid OrderId { get; set; }

    public Guid CourierId { get; set; }

    public DateOnly Date { get; set; }

    public virtual Courier Courier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
