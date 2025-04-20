using System;
using System.Collections.Generic;

namespace DataBase.DataBase;

public partial class Route
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public virtual ICollection<Route> InverseParent { get; set; } = new List<Route>();

    public virtual ICollection<Order> OrderDeparturePointNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderDestinationPointNavigations { get; set; } = new List<Order>();

    public virtual Route? Parent { get; set; }
}
