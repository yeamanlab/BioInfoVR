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

    
    // public void draw(){

    //     GameObject gameObject = new GameObject("dotConnection" , typeof(Image));
    //     gameObject.transform.SetParent(_graphContainer, false);
    //     RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    //     rectTransform.anchorMin = new Vector2(0,0);
    //     rectTransform.anchorMax = new Vector2(0,0);
    //     rectTransform.pivot = new Vector2(0, 0);
    //     float max = 100.0f;
    //     float maxX = _graphContainer.sizeDelta.x;
    //     float maxY = _graphContainer.sizeDelta.y;

    //     Debug.Log(maxX);
    //     Debug.Log(maxY);
    //     rectTransform.sizeDelta = new Vector2(maxX,1.0f);
    //     float x = xPos/max * maxX - maxX;
    //     float y = yPos/max * maxY - maxY;
    //     rectTransform.anchoredPosition = new Vector2(x,y);

    // }

    // private IReadOnlyDictionary<int, Color32> _colorMap = new Dictionary<int, Color32> 
    // {
    //     {-1, new Color32(255, 255, 255, 100)},
    //     {1, new Color32(0, 255, 255, 100)},
    //     {2, new Color32(255, 0,  255, 100)},

    // };

}