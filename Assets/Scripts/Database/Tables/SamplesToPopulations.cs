using SQLite4Unity3d;
namespace Database.Tables
{
    public class SamplesToPopulations
    {
        public int PopulationId { get; set; }
        public int SampleId { get; set; }

        public override string ToString()
        {
            return $"[PopulationId={PopulationId}, SampleId={SampleId}]";
        }
    }
}