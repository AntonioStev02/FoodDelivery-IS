namespace FoodDeliveryAdminApplication.Models
{
    public class DishInOrder
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid DishId { get; set; }
        public Dish? OrderedDish { get; set; }

        public int Quantity { get; set; }
    }
}
