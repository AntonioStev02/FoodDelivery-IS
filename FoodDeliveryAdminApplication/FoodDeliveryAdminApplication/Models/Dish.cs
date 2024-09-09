using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAdminApplication.Models
{
    public class Dish
    {
        [Required(ErrorMessage = "The field is required.")]
        public Category? DishCategory { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public string? DishName { get; set; }


        [Required(ErrorMessage = "The field is required.")]
        public string? DishImage { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public string? DishIngredients { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public int? Price { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
