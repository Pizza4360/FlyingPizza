using FlyingDrone.Pages.FleetPages;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyingPizza.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {

        public string Greeting = null;
        public string[] block;

        protected override void OnInitialized()
        {
            Greeting = "Hello";
            block = new string[5]{ "drone1", "drone2", "drone3", "drone4", "drone5" };

            //DroneData[] drone = await restDbSvc.Get<DroneData[]>("localhost:4544/Fleet?filter={ }");
            //DroneData drone = await restDvSvc.Post<DroneData>(url, DroneData)

            // Post template
            //Case newCase = new Case() { caseStatus = "uninitialized" };
            //var rval = await restDbSvc.Post<Case>("http://localhost:8080/cases/", newCase);

            //newCaseURL = rval.Headers.Location.AbsoluteUri;

            //caseVar = await restDbSvc.Get<Case>(newCaseURL);
            //caseVar.url = newCaseURL;

        }


    }
}
