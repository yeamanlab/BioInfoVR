using SQLite4Unity3d;

namespace Database.Tables
{
    public class Records
    {
        public int Position { get; set; }
        public int SampleId { get; set; }
        public int GenotypeId { get; set; }

        public override string ToString()
        {
            return $"[Position={Position}, SampleId={SampleId}, GenotypeId={GenotypeId}]";
        }
    }
}