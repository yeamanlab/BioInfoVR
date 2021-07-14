
using SQLite4Unity3d;

namespace Database.Tables
{
    public class Populations
    {
        [PrimaryKey]
        public int PopulationId { get; set; }
        public string PopulationName { get; set; }
        public override string ToString()
        {
            return $"[PopulationId={PopulationId}, PopulationName={PopulationName}]";
        }
    }
}