using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class OrderHistory
{
    public Guid OrderId { get; set; }

    public int Status { get; set; }

    public DateOnly Date { get; set; }

    public virtual Order Order { get; set; } = null!;
}