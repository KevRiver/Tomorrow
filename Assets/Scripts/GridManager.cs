using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap ground;
    public Vector3Int[,] spots;
    Astar astar;
    List<Spot> roadPath = new List<Spot>();
    new Camera camera;
    
    [SerializeField] private BoundsInt bounds;

    [SerializeField] private Vector3 curGridPos;
    
    void Start()
    {
        ground.CompressBounds();
        bounds = ground.cellBounds;
        camera = Camera.main;


        CreateGrid();
        astar = new Astar(spots, bounds.size.x, bounds.size.y);
    }
    public void CreateGrid()
    {
        spots = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (ground.HasTile(new Vector3Int(x, y, 0)))
                {
                    spots[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    spots[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }
}
