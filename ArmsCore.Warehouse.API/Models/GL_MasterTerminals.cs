using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("GL_MasterTerminals", Schema ="dbo")]
    public class GL_MasterTerminals
    {
        [Key]
        public Int64? RecordId { get; set; }
        public Int64? ModelId { get; set; }
        public string Type { get; set; } = string.Empty;
        
        public string? SerialNumber { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public Int64? ParameterGroupId { get; set; }
        public TimeSpan PrimaryConnTime { get; set; }
        public TimeSpan SecondaryConnTime { get; set; }
        public string? PrimaryAppId { get; set; } = string.Empty;
        public string? Profile { get; set; } = string.Empty;
        public string? Owner { get; set; } = string.Empty;
        public string? Group { get; set; } = string.Empty;
        public Int64? SoftwareGroupId { get; set; }
        public Int64? TerminalStateId { get; set; }
        public Int64? ProductionStateId { get; set; }
        public Int64 AssetPoId { get; set; }
        public DateTime Inserted { get; set; }
        public DateTime Updated { get; set; }




    }
}
