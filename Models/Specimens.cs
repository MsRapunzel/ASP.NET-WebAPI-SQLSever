using System.ComponentModel.DataAnnotations;

namespace testSQLServer.Models;

public class Specimens
{
    [Key]
    public int Specimen_id { get; set; }
    public int Species_id { get; set; }
    public int Exhibit_id { get; set; }
    public DateTime Date_of_birth { get; set; }
    public char Gender { get; set; }
}
