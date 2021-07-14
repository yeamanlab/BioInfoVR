using SQLite4Unity3d;
using UnityEngine;
using Database.Tables;
using System.Linq;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

namespace Database
{
    public class DataService
    {
        private SQLiteConnection _connection;

        const string _getSamplesForPopulation = "SELECT * FROM Samples INNER JOIN SamplesToPopulations ON Samples.SampleId=SamplesToPopulations.SampleId WHERE SamplesToPopulations.PopulationId = \'?\'";

        public DataService(string DatabaseName)
        {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Debug.Log("Final PATH: " + dbPath);

        }


        public IEnumerable<Samples> GetSamples()
        {
            return _connection.Table<Samples>();
        }

        public IEnumerable<Populations> GetPopulations()
        {
            return _connection.Table<Populations>();
        }

        public IEnumerable<SamplesToPopulations> GetSamplesToPopulations()
        {
            return _connection.Table<SamplesToPopulations>();
        }


        public List<Samples> GetSamplesForPopulation(Populations populations)
        {
            var returnSamples = new List<Samples>();
            var populations2Samples = GetSamplesToPopulations();
            var samples = GetSamples();
            foreach (var pop2sam in populations2Samples)
            {
                if (populations.PopulationId.Equals(pop2sam.PopulationId))
                {
                    var sample = samples.Where(t => t.SampleId.Equals(pop2sam.SampleId));
                    if (sample.Any())
                    {
                        returnSamples.Add(sample.First());
                    }
                }
            }


            return returnSamples;
        }
        

    }
}