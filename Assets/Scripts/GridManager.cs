using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    public Tilemap ground;
    public Tilemap entities;
    public Tilemap obstacles;
    public Tilemap interactables;
    
    public Vector3Int[,] spots;
    public Node[,] NodeGrid;

    public Astar PathFinder;
    new Camera camera;
    
    [Header("Tile Info")]
    [SerializeField] private Vector3Int curGridPos;
    [SerializeField] private string curTileType;
    
    [Header("Tilemap Bounds")]
    [SerializeField] private BoundsInt groundBounds;
    [SerializeField] private BoundsInt entityBounds;
    [SerializeField] private BoundsInt obstacleBounds;
    [SerializeField] private BoundsInt interactableBounds;
    
    void Start()
    {
        ground.CompressBounds();
        groundBounds = ground.cellBounds;
        
        entities.CompressBounds();
        entityBounds = entities.cellBounds;
        
        obstacles.CompressBounds();
        obstacleBounds = obstacles.cellBounds;
        
        interactables.CompressBounds();
        interactableBounds = interactables.cellBounds;
        
        camera = Camera.main;
        
        CreateNodeArrayTilemaps();
        PathFinder = new Astar(groundBounds.size.x, groundBounds.size.y);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 world = camera.ScreenToWorldPoint(Input.mousePosition);
            curGridPos = ground.WorldToCell(world);
            if (curGridPos.x < 0 || curGridPos.x >= groundBounds.size.x || curGridPos.y < 0 ||
                curGridPos.y >= groundBounds.size.y) return;
            curTileType = NodeGrid[curGridPos.x, curGridPos.y].nodeType.ToString();
        }
    }

    /// <summary>
    /// Ground 타일맵을 기준으로 장애물, 상호작용가능 타일맵에 있는 오브젝트의 위치를 변환.
    /// 변환된 위치를 하나의 노드 배열에 저장
    /// </summary>
    private void CreateNodeArrayTilemaps()
    {
        NodeGrid = new Node[groundBounds.size.x, groundBounds.size.y];
        Vector3Int nodeGridOrg = new Vector3Int(groundBounds.xMin, groundBounds.yMin, 0);
        
        #region Ground Tilemap
        for (int x = groundBounds.xMin, i = 0; i < groundBounds.size.x; x++, i++)
        {
            for (int y = groundBounds.yMin, j = 0; j < groundBounds.size.y; y++, j++)
            {
                if (ground.HasTile(new Vector3Int(x, y, 0)))
                {
                    NodeGrid[i, j] = new Node(x, y, 0, NodeTypes.Ground);
                }
                else
                {
                    NodeGrid[i, j] = new Node(x, y, 0, NodeTypes.None);
                }
            }
        }
        #endregion

        #region Obstacle Tilemap

        for (int x = obstacleBounds.xMin, i = 0; i < obstacleBounds.size.x; x++, i++)
        {
            for (int y = obstacleBounds.yMin, j = 0; j < obstacleBounds.size.y; y++, j++)
            {
                if (!obstacles.HasTile(new Vector3Int(x, y, 0))) continue;
                
                int p = x - nodeGridOrg.x;
                int q = y - nodeGridOrg.y;
                NodeGrid[p, q] = new Node(x, y, 0, NodeTypes.Obstacle);
            }
        }

        #endregion
    }

    public Vector2Int GetNodeGridIndex(Vector3Int worldPosition)
    {
        Vector3Int curNodeGridPos = ground.WorldToCell(worldPosition);
        Vector2Int curIndex = new Vector2Int(curNodeGridPos.x, curNodeGridPos.y);
        return curIndex;
    }

    public Vector3 GetWorldPositionFromNodeGrid(int x, int y)
    {
        return ground.CellToWorld(new Vector3Int(x, y, 0));
    }

    public NodeTypes GetNodeType(int x, int y)
    {
        if (x < 0 || x >= groundBounds.size.x || y < 0 || y >= groundBounds.size.y) return NodeTypes.None;
        return NodeGrid[x, y].nodeType;
    }
}
