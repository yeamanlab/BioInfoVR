using SQLite4Unity3d;

namespace Database.Tables
{
    public class Geno2{
        public int GenotypeId {get; set;}
        public override string ToString()
        {
            return $"[GenotypeId]={GenotypeId}]";
        }
    }
}
