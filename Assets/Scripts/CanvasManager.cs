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
    // private bool drawing;
    // private List<Records> recordList;

    // private int type;
    private int posX;
    private DataService _dataService;
    public List<NativeArray<decimal>> positionXArrays = new List<NativeArray<decimal>>();
    public List<NativeArray<decimal>> sizeXArrays = new List<NativeArray<decimal>>();
    public List<NativeArray<decimal>> genotypeArrays = new List<NativeArray<decimal>>();

    /// Setup canvas
    private async void Awake()
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
    /// if a new tree object has been clicked, destroys the current graph,
    /// then, reads data of the new population using job system
    /// and starts drawing new graph
    /// </summary>
    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (_populationId != _prevPopulationId)
        {
            _sampleList = _databaseManager.GetSamplesForPopulation(_populationId);
            
            /// Create Job System
            /// Each job reads data from one sample
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            Debug.Log("start");

            for (int i = 0; i< _sampleList.Count; i++){
                NativeArray<decimal> positionXArray = new NativeArray<decimal>(75000, Allocator.TempJob);
                NativeArray<decimal> sizeXArray = new NativeArray<decimal>(75000, Allocator.TempJob);
                NativeArray<decimal> genotypeArray = new NativeArray<decimal>(75000, Allocator.TempJob);

                positionXArrays.Add(positionXArray);
                sizeXArrays.Add(sizeXArray);
                genotypeArrays.Add(genotypeArray);
                
                JobHandle jobHandle = ReadDataJob(i);
                jobHandleList.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandleList);
            jobHandleList.Dispose();


            ///Draw graph
            Draw(0, 0, 1, 1, 0);
            int rowCount = _sampleList.Count;
            float sizeY = 1.0f / rowCount;
            float posY = 0;
            for (int j = 0; j< _sampleList.Count; j++){
                Debug.Log(_sampleList[j].SampleName);            
                for (int i = 0; i < genotypeArrays[j].Length ; i++){
                    if (genotypeArrays[j][i] == 0) break;
                    Draw((decimal) positionXArrays[j][i], (decimal)posY, (decimal) sizeXArrays[j][i], (decimal) sizeY,(int) genotypeArrays[j][i]);
                }
                posY += sizeY;
            }

            for (int i = 0; i< _sampleList.Count; i++){
                positionXArrays[i].Dispose();
                sizeXArrays[i].Dispose();
                genotypeArrays[i].Dispose();
            }
            
            Debug.Log("end");
        }
        _prevPopulationId = _populationId;
    }

    /// Job Handler
    private JobHandle ReadDataJob(int index)
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

    /// draws blocks of genotypes with different colors corresponding to different genotypes
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


/// <summary>
/// The job reads all records from a sample
/// then calculates the size of each genotype block
/// </summary>
[BurstCompile]
public struct ReallyToughJob : IJob {

    public NativeArray<decimal> positionXArray;
    public NativeArray<decimal> sizeXArray;
    public NativeArray<decimal> genotypeArray;
    public int sampleId;
    public void Execute() {
        DataService ds = new DataService ("database.db");
        List<int> recordList = ds.GetRecordGenoFromPopulation(sampleId);
        int index = 0;
        int logIndex = 0;
        float posX = (float) index/ recordList.Count;
        float sizeX = 1.0f / recordList.Count;

        int type = recordList[0];
        while(++index < recordList.Count){
            if (recordList[index] != type)
                {
                    float newPosX = (float) index/ recordList.Count;
                    if(type != 0)
                    {
                        positionXArray[logIndex] = (decimal) posX;
                        sizeXArray[logIndex] = (decimal) (newPosX - posX);
                        genotypeArray[logIndex++] = (decimal) type;

                    }
                    type = recordList[index];
                    posX = newPosX;
                }
        }

        genotypeArray[logIndex] = 0;
    }

}