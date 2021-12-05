using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Database.Tables;
using UnityEngine.UI;



public class CanvasManager : MonoBehaviour
{
    private List<Samples> _sampleList;
    private Canvas _canvas;
    public DatabaseManager _databaseManager;

    private int _populationId;
    private int _prevPopulationId;
    private RectTransform _graphContainer;
    float maxX;
    float maxY;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _sampleList = new List<Samples>();
        _populationId = -1;
        _prevPopulationId = -1;
        _graphContainer = transform.Find("GraphWrapper").Find("GraphContainer").GetComponent<RectTransform>();
        maxX = _graphContainer.sizeDelta.x;
        maxY = _graphContainer.sizeDelta.y;
    }

    private void Update()
    {
        if (_populationId != _prevPopulationId)
        {
            foreach (Transform child in _graphContainer.transform) {
                GameObject.Destroy(child.gameObject);
            }
            Debug.Log(_populationId);
            DrawFromPopulation();
        }
        _prevPopulationId = _populationId;
    }

    public bool Showing()
    {
        return _canvas.enabled;
    }

    public void Hide()
    {
        _canvas.enabled = false;
    }

    public void Show()
    {
        _canvas.enabled = true;
    }

    public void SetSampleList(List<Samples> sampleList)
    {
        _sampleList = sampleList;
    }

    public void SetPopulationId(int populationId)
    {
        _populationId = populationId;
    }

    public void DrawFromPopulation()
    {
        Debug.Log("Drawing graph...");
        int rowCount = _sampleList.Count;
        float sizeY = 1.0f / rowCount;
        float posY = 0;
        for (int i = 0; i < rowCount; i++)
        {
            List<Records> recordList = _databaseManager.GetRecordListFromPopulation(_sampleList[i]);
            int j = 0;
            float posX = (float) j/ recordList.Count;
            float sizeX = 1.0f / recordList.Count;
            int type = recordList[0].GenotypeId;
            while (++j < recordList.Count)
            {
                if (recordList[j].GenotypeId == type)
                {
                    sizeX += 1.0f / recordList.Count;
                }
                else
                {
                    Draw(posX, posY, sizeX, sizeY, type);
                    posX = (float) j/ recordList.Count;
                    sizeX = 1.0f / recordList.Count;
                    type = recordList[j].GenotypeId;
                }
            }
            posY += sizeY;
            }
        Debug.Log("Finished drawing graph");
    }
    public void Draw(float posX, float posY, float sizeX, float sizeY, int type)
    {
        GameObject gameObject = new GameObject("block", typeof(Image));
        gameObject.transform.SetParent(_graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        Image gameImage = gameObject.GetComponent<Image>();
        gameImage.color = _colorMap[type];

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);

        rectTransform.sizeDelta = new Vector2(sizeX * maxX, sizeY * maxY);
        float x = posX * maxX - maxX / 2;
        float y = posY * maxY - maxY / 2;
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    private IReadOnlyDictionary<int, Color32> _colorMap = new Dictionary<int, Color32>
    {
        {-1, new Color32(255, 255, 255, 255)},
        {0  , new Color32(232, 178, 14, 255)},
        {1, new Color32(5, 33, 94, 255)},
        {2, new Color32(179, 0, 0, 255)},
    };

}