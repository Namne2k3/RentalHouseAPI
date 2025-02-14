using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalHouse.Domain.Entities
{
    public class NhaTro
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? DescriptionHtml { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Url { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Price { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? PriceExt { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Area { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? BedRoom { get; set; }

        public DateTime? PostedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Type { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? BedRoomCount { get; set; }


        [Column(TypeName = "nvarchar(255)")]
        public string? Furniture { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

        public float? PriceBil { get; set; }
        public float? PriceMil { get; set; }
        public float? PriceVnd { get; set; }
        public float? AreaM2 { get; set; }
        public float? PricePerM2 { get; set; }

        // Quan hệ 1-N với NhaTroImages
        public List<NhaTroImage> Images { get; set; } = new();
    }
}
