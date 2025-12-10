using System.ComponentModel.DataAnnotations;
using TourAgencyClient.DTOs;

namespace TourAgencyClient.Models
{
    public class TourFormViewModel: TourCreateUpdate
    {
        public int? Id { get; set; }

        [Display(Name = "ID Партнеров (через запятую)")]
        public string PartnerIdsString { get; set; }
    }
}
