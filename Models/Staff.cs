using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace testSQLServer.Models;

[Table("Staff")]
public class Staff
{
    [Key]
    [Column("staff_id")]
    public int Staff_id { get; set; }

    [Column("department_id")]
    public int Department_id { get; set; }

    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("position")]
    public string Position { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("contact_email")]
    public string Contact_email { get; set; } = string.Empty;

    [MaxLength(15)]
    [Column("contact_number")]
    public string Contact_number { get; set; } = string.Empty;
}

