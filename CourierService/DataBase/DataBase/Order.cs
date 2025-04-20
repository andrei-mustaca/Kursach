using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid DeparturePoint { get; set; }

    public Guid DestinationPoint { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Route DeparturePointNavigation { get; set; } = null!;

    public virtual Route DestinationPointNavigation { get; set; } = null!;

    public virtual OrderAcceptance? OrderAcceptance { get; set; }

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    public virtual Payment? Payment { get; set; }
}
