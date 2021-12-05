using Database.Tables;
using DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField]
    private DatabaseManager _databaseManager;

    [SerializeField]
    private GameObject _populationPrefab;

    [SerializeField]
    private float _scalingFactor;
    
    [SerializeField]
    private Canvas _graphCanvas;

    private float _oldScalingFactor;
    private PopulationLocations _locations;
    private List<GameObject> _populations;

    // Start is called before the first frame update
    private void Start()
    {
        _populations = new List<GameObject>();
        Debug.Log("Spawning populations...");
        Task.Run(() => InitializePopulations());
        StartCoroutine(SpawnPopulationsWhenReady());
        _graphCanvas.enabled = false;
        _graphCanvas.GetComponent<CanvasManager>()._databaseManager = _databaseManager;
    }

    private async void InitializePopulations()
    {
        _oldScalingFactor = _scalingFactor;
        _locations = await Task.Run(() => _databaseManager.GetPopulationLocations());
        Debug.Log("Locations ready");
    }


    private IEnumerator SpawnPopulationsWhenReady()
    {
        yield return new WaitWhile(() => _locations == null);

        var scaler = new Vector2(_locations.Max.x - _locations.Min.x, _locations.Max.y - _locations.Min.y) / 10.0f;

        foreach (var location in _locations)
        {
            var normalizedLocation = (location.Value - _locations.Min) / _scalingFactor;

            var population = Instantiate<GameObject>(
                _populationPrefab
                , new Vector3(normalizedLocation.x, transform.position.y, normalizedLocation.y)
                , Quaternion.identity
                );
            
            PopulationController pc = population.AddComponent(typeof(PopulationController)) as PopulationController;    
            pc.setDatabaseManager(_databaseManager);
            pc.setCanvasManager(_graphCanvas.GetComponent<CanvasManager>());
            population.transform.parent = transform;
            population.name = location.Key.ToString();
            _populations.Add(population);
        }

        Debug.Log("Population spawning complete");
    }

    private void Update()
    {
        if (_locations == null || _oldScalingFactor.Equals(_scalingFactor))
        {
            return;
        }

        foreach (var population in _populations)
        {
            population.transform.position *= _scalingFactor / _oldScalingFactor;
        }

        _oldScalingFactor = _scalingFactor;
    }
}
