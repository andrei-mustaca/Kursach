using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class Payment
{
    public Guid OrderId { get; set; }

    public int Status { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public virtual Order Order { get; set; } = null!;
}
