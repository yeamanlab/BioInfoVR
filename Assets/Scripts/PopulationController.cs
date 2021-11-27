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
    
    public void setDatabaseManager(DatabaseManager dm){
        _databaseManager = dm;
    }

    public void setCanvasManager(CanvasManager cm){
        _canvasManager = cm;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // under assumption name is popylation id
        var sampleList =  _databaseManager.GetSamplesForPopulation(Int32.Parse(name));
        _canvasManager.setSampleList(sampleList);
        _canvasManager.setPopulationId(Int32.Parse(name));
        _canvasManager.show();
    }
}
