using SQLite4Unity3d;

namespace Database.Tables
{
    public class Samples
    {
        [PrimaryKey]
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public float Altitude { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public override string ToString()
        {
            return $"[SampleId={SampleId}, SampleName={SampleName}, Altitude={Altitude}, Latitude={Latitude}, Longitude={Longitude}]";
        }
    }
}
