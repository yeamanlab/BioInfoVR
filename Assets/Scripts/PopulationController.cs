using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Database.Tables;


public class PopulationController : MonoBehaviour, IPointerClickHandler
{    

    private DatabaseManager _databaseManager;   
    private CanvasManager _canvasManager;
    
    /// Setter for database manager
    public void setDatabaseManager(DatabaseManager dm){
        _databaseManager = dm;
    }

    /// Setter for canvas manager
    public void setCanvasManager(CanvasManager cm){
        _canvasManager = cm;
    }

     private void Awake() {
    }

    /// This method will be called when clicking on a tree object
    /// It then passes the property 'name' of the object as population id
    /// to GetSamplesForPopulation to get a list of samples belongs to the population
    /// The sample list is passed to Canvas Manager for drawing
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // under assumption name is population id
        var sampleList =  _databaseManager.GetSamplesForPopulation(Int32.Parse(name));
        _canvasManager.SetSampleList(sampleList);
        _canvasManager.SetPopulationId(Int32.Parse(name));
        _canvasManager.Show();
    }
}
