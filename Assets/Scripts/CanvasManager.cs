using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Database.Tables;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Database;



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

    private int type;
    private int posX;
    private DataService _dataService;
    public List<NativeArray<decimal>> positionXArrays = new List<NativeArray<decimal>>();
    public List<NativeArray<decimal>> sizeXArrays = new List<NativeArray<decimal>>();
    public List<NativeArray<decimal>> genotypeArrays = new List<NativeArray<decimal>>();

    /// Setup canvas
    private void Awake()
    {
        this._canvas = GetComponent<Canvas>();
        _sampleList = new List<Samples>();
        _populationId = -1;
        _prevPopulationId = -1;

        /// Get _graphContainer from MainScence
        _graphContainer = transform.Find("GraphWrapper").Find("GraphContainer").Find("BlockContainer").GetComponent<RectTransform>();
        maxX = (decimal)_graphContainer.sizeDelta.x;
        maxY = (decimal)_graphContainer.sizeDelta.y;
        _dataService = new DataService ("database.db");
    }

    /// <summary>
    /// Check every frame if a new tree object has been clicked
    /// If so, it destroys the current graph,
    /// reads records of the new population from database
    /// and starts drawing
    /// </summary>
    // private void Update()
    // {
    //     if (_populationId != _prevPopulationId)
    //     {
    //         _sampleList = _databaseManager.GetSamplesForPopulation(_populationId);
    //         DrawFromPopulation();
    //     }

    //     _prevPopulationId = _populationId;
    // }
    private async void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (_populationId != _prevPopulationId)
        {
            _sampleList = _databaseManager.GetSamplesForPopulation(_populationId);
            
            //Setup canvas for Graphing
            // Draw(0, 0, 1, 1, 0);
            int rowCount = _sampleList.Count;
            float sizeY = 1.0f / rowCount;
            // float posY = 0;
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            Debug.Log("start");
            // Debug.Log(_populationId);

            // List<NativeArray<decimal>> positionXArrays = new List<NativeArray<decimal>>();
            // List<NativeArray<decimal>> sizeXArrays = new List<NativeArray<decimal>>();
            // List<NativeArray<decimal>> genotypeArrays = new List<NativeArray<decimal>>();

            for (int i = 0; i< _sampleList.Count; i++){
                NativeArray<decimal> positionXArray = new NativeArray<decimal>(75000, Allocator.TempJob);
                NativeArray<decimal> sizeXArray = new NativeArray<decimal>(75000, Allocator.TempJob);
                NativeArray<decimal> genotypeArray = new NativeArray<decimal>(75000, Allocator.TempJob);

                positionXArrays.Add(positionXArray);
                sizeXArrays.Add(sizeXArray);
                genotypeArrays.Add(genotypeArray);
                
                JobHandle jobHandle = DrawJobs(i);
                jobHandleList.Add(jobHandle);

                // List<int> recordList = _dataService.GetRecordGenoFromPopulation(_sampleList[i].SampleId);
                // Debug.Log("Time taken: " + (Time.realtimeSinceStartup - startTime) * 1000f + "ms ; Sample: " + _sampleList[i].SampleId);
                // startTime = Time.realtimeSinceStartup;
                // return;
                // posY += sizeY;
            }
            JobHandle.CompleteAll(jobHandleList);
            jobHandleList.Dispose();
        }
        _prevPopulationId = _populationId;
        Debug.Log("end");
    }

    private JobHandle DrawJobs(int index)
    {
        ReallyToughJob job = new ReallyToughJob{
            positionXArray = positionXArrays[index],
            sizeXArray = sizeXArrays[index],
            genotypeArray = genotypeArrays[index],
            sampleId = _sampleList[index].SampleId
        };
        return job.Schedule();
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

    void Draw(decimal posX, decimal posY, decimal sizeX, decimal sizeY, int type)
    {
        GameObject block = new GameObject("block", typeof(Image));
        block.transform.SetParent(_graphContainer, false);
        RectTransform rectTransform = block.GetComponent<RectTransform>();

        Image gameImage = block.GetComponent<Image>();
        gameImage.color = _colorMap[type];

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);

        rectTransform.sizeDelta = new Vector2((float)(sizeX * maxX), (float)(sizeY * maxY));
        decimal x = posX * maxX - maxX / 2;
        decimal y = posY * maxY - maxY / 2;
        rectTransform.anchoredPosition = new Vector2((float)x, (float)y);
    }

    private IReadOnlyDictionary<int, Color32> _colorMap = new Dictionary<int, Color32>
    {
        {-1, new Color32(255, 255, 255, 255)},
        {0  , new Color32(232, 178, 14, 255)},
        {1, new Color32(5, 33, 94, 255)},
        {2, new Color32(179, 0, 0, 255)},
    };
}

[BurstCompile]
public struct ReallyToughJob : IJob {

    public NativeArray<decimal> positionXArray;
    public NativeArray<decimal> sizeXArray;
    public NativeArray<decimal> genotypeArray;
    public int sampleId;
    public void Execute() {
        // Represents a tough task like some pathfinding or a really complex calculation
        // Debug.Log(index);
        
    }

}

[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor {

    public NativeArray<NativeArray<decimal>> positionXArray;
    public NativeArray<NativeArray<decimal>> sizeXArray;
    public NativeArray<NativeArray<decimal>> genotypeArray;
    [ReadOnly] public float deltaTime;

    public void Execute(int index) {
        
    }

}


// public struct DrawEachPopulation : IJob
// {
//     private int _sampleId;
//     private RectTransform _graphContainer;
//     private decimal maxX;
//     private decimal maxY;
//     private float posY;
//     private float sizeY;
//     // 
//     public DatabaseManager _databaseManager; //global

//     public DrawEachPopulation(int sId, RectTransform gContainter, decimal mx, decimal my, float py, float sy, DatabaseManager db){
//         _databaseManager = db;
//         _sampleId = sId;
//         _graphContainer = gContainter;
//         maxX = mx;
//         maxY = my;
//         posY = py;
//         sizeY = sy;
//          _colorMap = new Dictionary<int, Color32>
//         {
//             {-1, new Color32(255, 255, 255, 255)},
//             {0  , new Color32(232, 178, 14, 255)},
//             {1, new Color32(5, 33, 94, 255)},
//             {2, new Color32(179, 0, 0, 255)},
//         };
//     }
    
//     public void Execute()
//     {
//         List<Records> recordList = _databaseManager.GetRecordListFromPopulation(_sampleId);
//         int j = 0;
//         float posX = (float)j / recordList.Count;
//         float sizeX = 1.0f / recordList.Count;
//         int type = recordList[0].GenotypeId;
//         while (++j < recordList.Count)
//         {
//             if (recordList[j].GenotypeId != type)
//             {
//                 float newPosX = (float)j / recordList.Count;
//                 if (type != 0)
//                     Draw((decimal)posX, (decimal)posY, (decimal)(newPosX - posX), (decimal)sizeY, type);
//                 type = recordList[j].GenotypeId;
//                 posX = newPosX;
//             }
//         }
//     }
//     void Draw(decimal posX, decimal posY, decimal sizeX, decimal sizeY, int type)
//     {
//         GameObject block = new GameObject("block", typeof(Image));
//         block.transform.SetParent(_graphContainer, false);
//         RectTransform rectTransform = block.GetComponent<RectTransform>();

//         Image gameImage = block.GetComponent<Image>();
//         gameImage.color = _colorMap[type];

//         rectTransform.anchorMin = new Vector2(0, 0);
//         rectTransform.anchorMax = new Vector2(0, 0);
//         rectTransform.pivot = new Vector2(0, 0);

//         rectTransform.sizeDelta = new Vector2((float)(sizeX * maxX), (float)(sizeY * maxY));
//         decimal x = posX * maxX - maxX / 2;
//         decimal y = posY * maxY - maxY / 2;
//         rectTransform.anchoredPosition = new Vector2((float)x, (float)y);
//     }

//     private IReadOnlyDictionary<int, Color32> _colorMap;
// }
public struct DrawBySample : IJobParallelFor
{
    private int _sampleId;
    private RectTransform _graphContainer;
    private decimal maxX;
    private decimal maxY;
    private float posY;
    private float sizeY;
    // 
    public DatabaseManager _databaseManager; //global
 
    
    public void Execute(int SampleId)
    {
        List<Records> recordList = _databaseManager.GetRecordListFromPopulation(_sampleId);
        int j = 0;
        float posX = (float)j / recordList.Count;
        float sizeX = 1.0f / recordList.Count;
        int type = recordList[0].GenotypeId;
        while (++j < recordList.Count)
        {
            if (recordList[j].GenotypeId != type)
            {
                float newPosX = (float)j / recordList.Count;
                if (type != 0)
                    Draw((decimal)posX, (decimal)posY, (decimal)(newPosX - posX), (decimal)sizeY, type);
                type = recordList[j].GenotypeId;
                posX = newPosX;
            }
        }
    }
    void Draw(decimal posX, decimal posY, decimal sizeX, decimal sizeY, int type)
    {
        GameObject block = new GameObject("block", typeof(Image));
        block.transform.SetParent(_graphContainer, false);
        RectTransform rectTransform = block.GetComponent<RectTransform>();

        Image gameImage = block.GetComponent<Image>();
        gameImage.color = _colorMap[type];

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);

        rectTransform.sizeDelta = new Vector2((float)(sizeX * maxX), (float)(sizeY * maxY));
        decimal x = posX * maxX - maxX / 2;
        decimal y = posY * maxY - maxY / 2;
        rectTransform.anchoredPosition = new Vector2((float)x, (float)y);
    }

    private IReadOnlyDictionary<int, Color32> _colorMap;
}