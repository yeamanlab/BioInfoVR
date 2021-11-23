using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Database.Tables;
using DataStructures;


public class PopulationController : MonoBehaviour, IPointerClickHandler
{    
    public DatabaseManager _databaseManager;
    private Canvas _canvas;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake() 
    {
        _canvas = transform.Find("Canvas").GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    private bool Showing()
    {
        return _canvas.enabled;
    }

    private void Hide()
    {
        _canvas.enabled = false;
    }

    private void Show()
    {
        _canvas.enabled = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (Showing())
        {
            Hide();
        }
        else 
        {
            Show();
        }
        Debug.Log(name);
        var sampleList =  _databaseManager.GetSamplesForPopulation(Int32.Parse(name));
        Debug.Log("Samples: ");
        foreach (var i in sampleList){
            Debug.Log(i.SampleId +" " + i.SampleName);
        }
    }
}
