using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MicroserviceTour.DTOs
{
    public class TourCreateUpdate
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }
        public List<int> PartnerIds { get; set; } = new();
    }
}