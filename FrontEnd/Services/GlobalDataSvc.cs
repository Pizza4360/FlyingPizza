using System;
using System.Collections.Generic;
using Domain.Entities;
using Order = FlyingPizza.Domain.Entities.Order;

namespace FlyingPizza.Services
{
    public class GlobalDataSvc
    {
        public DroneFields currDrone { get; set; }
        public Order currOrder { get; set; }
        public List<Object> cart { get; set; }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

    }
}
