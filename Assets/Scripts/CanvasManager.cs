using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Database.Tables;
using UnityEngine.UI;



public class CanvasManager : MonoBehaviour {
    private  List<Samples> _sampleList;
    private Canvas _canvas;
    private int _populationId;
    private int _prevPopulationId;
    private RectTransform _graphContainer;

    [SerializeField] float xPos = 0;
    [SerializeField] float yPos = 0;

    private void Awake(){
        _canvas = GetComponent<Canvas>();
        _sampleList = new List<Samples>();
        _populationId = -1;
        _prevPopulationId = -1;
        _graphContainer = transform.Find("GraphWrapper").Find("GraphContainer").GetComponent<RectTransform>();
    }

    private void Update(){
        if (_populationId != _prevPopulationId){
            Debug.Log(_populationId);
        }
        _prevPopulationId = _populationId;
    }

    public bool showing(){
        return _canvas.enabled;
    }

    public void hide(){
        _canvas.enabled = false;
    }

    public void show(){
        _canvas.enabled = true;
    }

    public void setSampleList(List<Samples> sampleList){
        _sampleList = sampleList;
    }

    public void setPopulationId(int populationId){
        _populationId = populationId;
    }

}