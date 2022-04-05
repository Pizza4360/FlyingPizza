using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Interfaces;

namespace FrontEnd.Services
{
    public class GlobalDataSvc
    {
        public DroneRecord currDrone { get; set; }
        public Order currOrder { get; set; }
        public List<Object> cart { get; set; }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

    }
}
