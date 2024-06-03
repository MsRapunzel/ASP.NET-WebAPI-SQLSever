using System.ComponentModel.DataAnnotations;

namespace testSQLServer.Models;

public class Species
{
    [Key]
    public int Species_id { get; set; }
    public string Common_name { get; set; } = string.Empty;
    public string Scientific_name { get; set; } = string.Empty;
    public string Conservation_status { get; set; } = string.Empty;
    public List<Specimens> Specimens { get; set; } = new List<Specimens>();
}
