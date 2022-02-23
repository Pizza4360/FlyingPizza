using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyingPizza.Shared
{
    public class QueryGen
    {

        // this class will primarily control the connection to the restheart server by generating filtered strings 
        // collName is the name of the table you want to connect to 
        // filtertype is the attribute you want to filter by for example badgenumber
        // filter val is the value of that attribute eg a 1 to search for badge number 1
        // sort is the sort string and direction so 'badgeNumber':1 to get them in ascending order and 'badgenumber':-1 for descending
        public static string connectionString = null;

         
        // This method just grabs the whole collection
        public static string GetColl(string collName)
        {
            connectionString = "http://localhost:8080/" + collName;
            return connectionString;
        }

        // This method grabs the whole collection sorted
        public static string GetCollSort(string collName, string sort)
        {
            connectionString = "http://localhost:8080/" + collName + "?sort={" + sort + "}";
            return connectionString;
        }

        // This method takes in a collection name, filter type, and filter value and an optional sort value
        // and returns a string which can be used by the RestDbSvc class for connections.
        public static string BasicFilter(string collName, string filterType, string filterVal)
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal + "'}";
            return connectionString;
        }
        
        // This is a basic filter but done sorted
        public static string BasicFilterSort(string collName, string filterType, string filterVal, string sort)
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal + "'}&sort={"+ sort +"}";
            return connectionString;
        }


        public static string QueryArray(string collName, string arrayName, string filterType, string filterVal)
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + arrayName + "':{'"+filterType+"':'" + filterVal + "'}";
            return connectionString;
        }


    }
}
