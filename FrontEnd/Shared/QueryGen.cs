namespace FrontEnd.Shared;

public class QueryGen
{
    // this class will primarily control the connection to the restheart server by generating filtered strings 
    // collName is the name of the table you want to connect to 
    // filtertype is the attribute you want to filter by for example badgenumber
    // filter val is the value of that attribute eg a 1 to search for badge number 1
    // sort is the sort string and direction so 'badgeNumber':1 to get them in ascending order and 'badgenumber':-1 for descending
    public static string connectionString;

    public static string Generate(string collName, string arrayName = "", string filterType = "", string filterVal = "",
        string sort = "")
    {
        // basic get all other things except collection are blank
        if (arrayName == "" && filterType == "" && filterVal == "" && sort == "")
        {
            connectionString = "http://localhost:8080/" + collName;
            return connectionString;
        }
        // collection and sort arent blank

        if (sort != "" && arrayName == "" && filterType == "" && filterVal == "")
        {
            connectionString = "http://localhost:8080/" + collName + "?sort={" + sort + "}";
            return connectionString;
        }
        // collection, filtertype, and filterval not blank

        if (filterType != "" && filterVal != "" && sort == "" && arrayName == "")
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal +
                               "'}";
            return connectionString;
        }
        // collection filter type, filterVal, and sort are not blank

        if (filterType != "" && filterVal != "" && sort != "" && arrayName == "")
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal +
                               "'}&sort={" + sort + "}";
        }
        // collection array name, filterType, filterVal
        else if (filterType != "" && filterVal != "" && arrayName != "" && sort == "")
        {
            connectionString = "http://localhost:8080/" + collName + "?filter={'" + arrayName + "':{'" + filterType +
                               "':'" + filterVal + "'}";
        }
        // collection, array name, filterType, FilterVal, sort are not blank
        //else if (arrayName != "" && filterType != "" && filterVal != "" && sort != "")
        //{
        //    connectionString = "http://localhost:8080/" + collName + "?filter={'" + arrayName + "':{'" + filterType + "':'" + filterVal + "'}";
        //}


        return connectionString;
    }

    // this class will primarily control the connection to the restheart server by generating filtered strings 
    // collName is the name of the table you want to connect to 
    // filtertype is the attribute you want to filter by for example badgenumber
    // filter val is the value of that attribute eg a 1 to search for badge number 1
    // sort is the sort string and direction so 'badgeNumber':1 to get them in ascending order and 'badgenumber':-1 for descending
}