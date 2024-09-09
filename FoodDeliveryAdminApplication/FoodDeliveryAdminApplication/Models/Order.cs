using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAdminApplication.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? OwnerId { get; set; }
        public FoodDeliveryApplicationUser? Owner { get; set; }
        
        public string? address { get; set; }
        
        public string? contactPhone { get; set; }
        public int? totalPrice { get; set; }


        public virtual ICollection<DishInOrder>? DishInOrders { get; set; }
    }
}
