using System.ComponentModel.DataAnnotations;

namespace testSQLServer.Models;

public class Exhibits
{
    [Key]
    public int Exhibit_id;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Water_type { get; set; } = string.Empty;
}
