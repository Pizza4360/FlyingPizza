namespace FlyingPizza.Drone
{
    public class Order
    {
        // The place to drop off the delivery
        public Point Destination;
        // The id to fetch the information from the database
        public string Id;

        public Order(Point destination, string id)
        {
            destination = Destination;
            Id = id;
        }
    }
}
