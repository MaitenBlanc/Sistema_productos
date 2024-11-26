using System.ComponentModel.DataAnnotations;

namespace prueba_addaccion.Models
{
    public class ProductCategory
    {
        [Key]
        [Range(0, 99999)]
        [Required]
        public int CategoryProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryDescription { get; set; } = string.Empty;

        // Relación con Product
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
