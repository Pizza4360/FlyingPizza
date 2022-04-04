using System;
using System.Collections.Generic;
using FlyingPizza.Domain.Entities;

namespace FlyingPizza.Services
{
    public class GlobalDataSvc
    {
        public DroneModel currDrone { get; set; }
        public Order currOrder { get; set; }
        public List<Object> cart { get; set; }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

    }
}
