using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalHouse.Domain.Entities
{
    public class NhaTroImage
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [ForeignKey("NhaTro")]
        public int NhaTroID { get; set; }
        public NhaTro NhaTro { get; set; }
    }
}
