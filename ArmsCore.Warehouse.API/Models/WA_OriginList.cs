using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("WA_OriginList", Schema = "dbo")]
    public class WA_OriginList
    {
        [Key]
        public Int64 RecordId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
    }
}
