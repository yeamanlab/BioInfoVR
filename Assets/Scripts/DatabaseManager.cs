using Database;
using Database.Tables;
using DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private DataService _dataService;

    // Start is called before the first frame update
    void Start()
    {
        _dataService = new DataService ("database.db"); //connect to database
    }

    /// <summary>
    /// Add all populations to a list
    /// </summary>
    public List<Populations> GetPopulations()
    {
        var populations = new List<Populations>();
        var populationInDb = _dataService.GetPopulations();
        foreach (var pop in populationInDb)
        {
            populations.Add(pop);
        }

        return populations;
    }

    /// <summary>
    /// Attempts to get the population locations as a Vector 2
    /// with Latitude and Longitude information in Samples table
    /// then add the location to population locations along with population ID
    /// </summary>
    /// <return>
    /// A PopulationLocations which contains all sample locations of a population
    /// </return>
    public PopulationLocations GetPopulationLocations()
    {
        var locations = new PopulationLocations();
        var populationInDb = _dataService.GetPopulations();
        foreach (var pop in populationInDb)
        {
            var samples = _dataService.GetSamplesForPopulation(pop.PopulationId);
            var position = new Vector2();
            var firstSample = true;
            foreach (var sample in samples)
            {
                var nextPosition = new Vector2(sample.Latitude, sample.Longitude);
                if (firstSample)
                {
                    firstSample = false;
                    position = nextPosition;
                    continue;
                }

                position += nextPosition;
                position /= 2;
            }

            locations.Add(pop.PopulationId, position);
        }

        return locations;
    }

    public List<Samples> GetSamplesForPopulation(int populationId){
        return _dataService.GetSamplesForPopulation(populationId);
    }

    internal List<Records> GetRecordListFromPopulation(int samplesId)
    {
        return _dataService.GetRecordListFromPopulation(samplesId);
    }
}
