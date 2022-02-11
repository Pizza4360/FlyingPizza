using FlyingPizza.Drone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyingPizza.Services
{
    public class GlobalDataSvc
    {
        public DroneModel currDrone { get; set; }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

    }
}
