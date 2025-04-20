using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class Courier
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string TelephoneNumber { get; set; } = null!;

    public decimal OrderPercentage { get; set; }

    public virtual ICollection<OrderAcceptance> OrderAcceptances { get; set; } = new List<OrderAcceptance>();
}
