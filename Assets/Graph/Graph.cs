using System;
using System.Collections;
using UnityEngine;
using Database;
using UnityEngine.UI;
using Database.Tables;
using System.Collections.Generic;
using System.Linq;




public class Graph : MonoBehaviour
{
    public Plane plane;
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(4096 , 4096 );
    public Renderer renderer;

    public int populationId;
    public int prevPopulationId;
    public DataService _dataService = new DataService("database.db");

    private void Awake()
    {
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;
        renderer.enabled = false;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = ((x & y) != 0 ? Color.white : Color.gray);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    private void Draw(int id)
    {
        Debug.Log("reading");
        for (int i = 0; i <= 10; i++)
        {
            List<int> genoList = _dataService.GetGeno2().Select(e => e.GenotypeId).ToList();
            int k = 1;
            foreach (var j in genoList)
            {
                texture.SetPixel(k, i, getColor(j));
                k++;
            }
        }
        texture.Apply();
        Debug.Log("done");
    }                                                                                                                       

    private Color getColor(int genoCode)
    {
        switch (genoCode)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.yellow;
            case 2:
                return Color.blue;
            default:
                return Color.white;
                break;
        }
    }
    
    private void Update()
    {
        if (populationId != prevPopulationId)
        {
            Draw(populationId);
            Show();
        }

        prevPopulationId = populationId;
    }



    public void Show()
    {
        renderer.enabled = true;
    }

    public void Hide()
    {
        renderer.enabled = false;
    }


}