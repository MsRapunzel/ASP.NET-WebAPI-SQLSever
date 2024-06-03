using Dapper;
using testSQLServer.Models;
using testSQLServer.Interfaces;

namespace testSQLServer.Services;

public class SpecimensService : ISpecimensService
{
    private readonly IDapperDbContext _connection;

    public SpecimensService(IDapperDbContext connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Species>> GetAllSpecimensData()
    {
        using var connection = _connection.CreateConnection();
        string sql = @"
        SELECT 
            s.species_id, s.common_name, s.scientific_name, s.conservation_status,
            sp.specimen_id, sp.species_id AS sp_species_id, sp.exhibit_id, sp.date_of_birth, sp.gender,
            COUNT(sp.specimen_id) OVER(PARTITION BY sp.species_id) AS specimen_count
        FROM dbo.Species s
        LEFT JOIN dbo.Specimens sp ON s.species_id = sp.species_id;";

        var speciesDict = new Dictionary<int, Species>();

        var specimensList = await connection.QueryAsync<Species, Specimens, Species>(sql, map:
            (species, specimens) =>
            {
                if (!speciesDict.TryGetValue(species.Species_id, out var currentSpecies))
                {
                    currentSpecies = species;
                    speciesDict.Add(currentSpecies.Species_id, currentSpecies);
                }

                if (specimens != null)
                {
                    currentSpecies.Specimens.Add(specimens);
                }

                return currentSpecies;
            }, splitOn: "specimen_id");

        return speciesDict.Values;
    }
}
