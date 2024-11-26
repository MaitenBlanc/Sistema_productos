using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prueba_addaccion.Models
{
    public class Product
    {
        [Key]
        [StringLength(30)]
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(0, 99999)]
        public int? CategoryProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductDescription { get; set; } = string.Empty;

        [Range(0, 99999)]
        public int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(1)]
        public string HaveECDiscount { get; set; } = "N";

        [Required]
        [StringLength(1)]
        public string IsActive { get; set; } = "S";

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Propiedades auxiliares para los checkbox
        [NotMapped]
        public bool HaveECDiscountCheckbox
        {
            get => HaveECDiscount == "S";
            set => HaveECDiscount = value ? "S" : "N";
        }

        [NotMapped]
        public bool IsActiveCheckbox
        {
            get => IsActive == "S";
            set => IsActive = value ? "S" : "N";
        }

        // Relación con la tabla ProductCategory (FK)
        [ForeignKey("CategoryProductId")]
        public virtual ProductCategory Category { get; set; } = null!;
    }
}
