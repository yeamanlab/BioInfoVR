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
    private decimal maxX;
    private decimal maxY;

    private int countX;
    private int countY;
    private bool drawing;
    private List<Records> recordList;

    /// Setup canvas
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _sampleList = new List<Samples>();
        _populationId = -1;
        _prevPopulationId = -1;

        /// Get _graphContainer from MainScence
        _graphContainer = transform.Find("GraphWrapper").Find("GraphContainer").Find("BlockContainer").GetComponent<RectTransform>();
        maxX = (decimal) _graphContainer.sizeDelta.x;
        maxY = (decimal) _graphContainer.sizeDelta.y;
    }

    /// <summary>
    /// Check every frame if a new tree object has been clicked
    /// If so, it destroys the current graph,
    /// reads records of the new population from database
    /// and starts drawing
    /// </summary>
    private void Update()
    {
        if(_populationId != _prevPopulationId){
            _sampleList = _databaseManager.GetSamplesForPopulation(_populationId);
            DrawFromPopulation();
        }

        _prevPopulationId = _populationId;
    }


    private int type;
    private int posX;

    private void DrawFromPopulation()
    {
        Debug.Log("Drawing graph...");
        Draw(0, 0, 1, 1, 0);
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
                if (recordList[j].GenotypeId != type)
                {
                    float newPosX = (float) j/ recordList.Count;
                    if(type != 0)
                    Draw((decimal) posX, (decimal)posY, (decimal) (newPosX - posX), (decimal) sizeY, type);
                    // break;
                    type = recordList[j].GenotypeId;
                    posX = newPosX;
                }
            }
            posY += sizeY;
            }
        Debug.Log("Finished drawing graph");
    }
    private void Draw(decimal posX, decimal posY, decimal sizeX, decimal sizeY, int type)
    {
        GameObject block = new GameObject("block", typeof(Image));
        block.transform.SetParent(_graphContainer, false);
        RectTransform rectTransform = block.GetComponent<RectTransform>();

        Image gameImage = block.GetComponent<Image>();
        gameImage.color = _colorMap[type];

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);

        rectTransform.sizeDelta = new Vector2((float) (sizeX * maxX),(float) (sizeY * maxY));
        decimal x = posX * maxX - maxX / 2;
        decimal y = posY * maxY - maxY / 2;
        rectTransform.anchoredPosition = new Vector2((float)x,(float) y);
    }

    /// <summary>
    /// Color mapping for different genotype
    /// </summary>
    private IReadOnlyDictionary<int, Color32> _colorMap = new Dictionary<int, Color32>
    {
        {-1, new Color32(255, 255, 255, 255)},
        {0  , new Color32(232, 178, 14, 255)},
        {1, new Color32(5, 33, 94, 255)},
        {2, new Color32(179, 0, 0, 255)},
    };

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
}