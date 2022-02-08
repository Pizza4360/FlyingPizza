using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyingPizza.Drone;


namespace FlyingPizza.Pages
{
    partial class Index: ComponentBase
    {

        public DroneModel[] Fleet = null;
        public string str = null;
        protected override async Task OnInitializedAsync()
        {
            Fleet = await restPoint.Get<DroneModel[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
            
        }
    }
}
