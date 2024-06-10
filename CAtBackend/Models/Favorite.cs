using System.ComponentModel.DataAnnotations;

namespace CatApp.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string CatImageUrl { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}
