using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("GL_MasterSimcards", Schema = "dbo")]
    public class GL_MasterSimcards
    {
        [Key]
        public Int64 RecordId { get; set; }
        public Int64 MasterModelId { get; set; }
        public string ICCID { get; set; } = string.Empty;
        public Int64 SimcardStateId { get; set; }
        public DateTime Inserted { get; set; }
        public DateTime Updated { get; set; }

    }
}
