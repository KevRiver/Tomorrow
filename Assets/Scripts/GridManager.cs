using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;



[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    public Tilemap GroundTilemap;
    public Tilemap ObstacleTilemap;
    public Transform EntityTilemap;
    public Transform InteractableTilemap;
    public Transform MovableTilemap;
    
    [SerializeField] private List<Transform> _entities;
    [SerializeField] private List<Transform> _interactables;
    [SerializeField] private List<Transform> _movable;
    
    public Node[,] NodeGrid;

    public GameObject Player;
    public Dictionary<int, GameObject> Murderers = new Dictionary<int, GameObject>();
    public List<Switch> Switches = new List<Switch>();

    public Astar PathFinder;
    new Camera camera;
    
    [Header("Tile Info")]
    [SerializeField] private Vector3Int curGridPos;
    [SerializeField] private string curTileType;
    
    [Header("Tilemap Bounds")]
    [SerializeField] private BoundsInt groundBounds;
    [SerializeField] private BoundsInt obstacleBounds;

    void Awake()
    {
        GroundTilemap.CompressBounds();
        groundBounds = GroundTilemap.cellBounds;
        
        ObstacleTilemap.CompressBounds();
        obstacleBounds = ObstacleTilemap.cellBounds;

        _entities = EntityTilemap.GetComponentsInChildren<Transform>().ToList();
        _entities.RemoveAt(0);
        
        _interactables = InteractableTilemap.GetComponentsInChildren<Transform>().ToList();
        _interactables.RemoveAt(0);
        
        _movable = MovableTilemap.GetComponentsInChildren<Transform>().ToList();
        _movable.RemoveAt(0);

        camera = Camera.main;
        
        CreateNodeGrid();
        PathFinder = new Astar(groundBounds.size.x, groundBounds.size.y);
    }

    /// <summary>
    /// Ground 타일맵을 기준으로 장애물, 상호작용가능 타일맵에 있는 오브젝트의 위치를 변환.
    /// 변환된 위치를 하나의 노드 배열에 저장
    /// </summary>
    private void CreateNodeGrid()
    {
        NodeGrid = new Node[groundBounds.size.x, groundBounds.size.y];
        Vector3Int nodeGridOrg = new Vector3Int(groundBounds.xMin, groundBounds.yMin, 0);
        
        #region Ground Tilemap
        for (int x = groundBounds.xMin, i = 0; i < groundBounds.size.x; x++, i++)
        {
            for (int y = groundBounds.yMin, j = 0; j < groundBounds.size.y; y++, j++)
            {
                if (GroundTilemap.HasTile(new Vector3Int(x, y, 0)))
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
                if (!ObstacleTilemap.HasTile(new Vector3Int(x, y, 0))) continue;
                
                int p = x - nodeGridOrg.x;
                int q = y - nodeGridOrg.y;
                NodeGrid[p, q] = new Node(x, y, 0, NodeTypes.Obstacle);
            }
        }

        #endregion
        
        #region Interactable Objects

        for (int i = 0; i < _interactables.Count; i++)
        {
            Switches.Add(_interactables[i].GetComponent<Switch>());
            
            Vector2Int index = GetNodeGridIndex(_interactables[i].position);
            NodeGrid[index.x, index.y].nodeType = NodeTypes.Interactable;
        }

        #endregion

        #region Movable Objects

        for (int i = 0; i < _movable.Count; i++)
        {
            Movable movable = _movable[i].GetComponent<Movable>();
            movable.Grid = this;

            Vector2Int index = GetNodeGridIndex(_movable[i].position);
            NodeGrid[index.x, index.y].nodeType = NodeTypes.Movable;
        }

        #endregion

        #region Entity Store

        for (int i = 0; i < _entities.Count; i++)
        {
            GameObject entity = _entities[i].gameObject;
            if (entity.CompareTag("Murderer"))
            {
                Murderers.Add(i, entity);
            }
            else if (entity.CompareTag("Player"))
            {
                Player = entity;
            }
        }

        #endregion
    }

    public Vector2Int GetNodeGridIndex(Vector3 worldPosition)
    {
        Vector3Int curNodeGridPos = GroundTilemap.WorldToCell(worldPosition);
        Vector2Int curIndex = new Vector2Int(curNodeGridPos.x, curNodeGridPos.y);
        return curIndex;
    }

    public bool UpdateValidDirectionWhenGrabbing(Vector3 ownerPos, Vector3 objPos, Vector2 moveDir, Vector2 faceDir)
    {
        Node nodeForCheck;
        NodeTypes nodeType;
        Vector2Int objIndex = GetNodeGridIndex(objPos);
        Vector2Int ownerIndex = GetNodeGridIndex(ownerPos);
        bool isFacingEqualsToMoveDir;
        
        if (moveDir.Equals(Vector2.up) && objIndex.y < groundBounds.size.y)
        {
            isFacingEqualsToMoveDir = faceDir.Equals(moveDir);
            nodeForCheck = isFacingEqualsToMoveDir
                ? GetNode(objIndex.x, objIndex.y + 1)
                : GetNode(ownerIndex.x, ownerIndex.y + 1);
            if (!(nodeForCheck is null))
            {
                nodeType = nodeForCheck.nodeType;
                if (nodeType.Equals(NodeTypes.None) || nodeType.Equals(NodeTypes.Obstacle)) return false;
            }
            else
            {
                return false;
            }
        }
        else if (moveDir.Equals(Vector2.down) && objIndex.y > 0)
        {
            isFacingEqualsToMoveDir = faceDir.Equals(moveDir);
            nodeForCheck = isFacingEqualsToMoveDir
                ? GetNode(objIndex.x, objIndex.y - 1)
                : GetNode(ownerIndex.x, ownerIndex.y - 1);
            if (!(nodeForCheck is null))
            {
                nodeType = nodeForCheck.nodeType;
                if (nodeType.Equals(NodeTypes.None) || nodeType.Equals(NodeTypes.Obstacle)) return false;
            }
            else
            {
                return false;
            }
        }
        else if (moveDir.Equals(Vector2.right) && objIndex.x < groundBounds.size.x)
        {
            isFacingEqualsToMoveDir = faceDir.Equals(moveDir);
            nodeForCheck = isFacingEqualsToMoveDir
                ? GetNode(objIndex.x + 1, objIndex.y)
                : GetNode(ownerIndex.x + 1, ownerIndex.y);
            if (!(nodeForCheck is null))
            {
                nodeType = nodeForCheck.nodeType;
                if (nodeType.Equals(NodeTypes.None) || nodeType.Equals(NodeTypes.Obstacle)) return false;
            }
            else
            {
                return false;
            }
        }
        else if (moveDir.Equals(Vector2.left) && objIndex.x > 0)
        {
            isFacingEqualsToMoveDir = faceDir.Equals(moveDir);
            nodeForCheck = isFacingEqualsToMoveDir
                ? GetNode(objIndex.x - 1, objIndex.y)
                : GetNode(ownerIndex.x - 1, ownerIndex.y);
            if (!(nodeForCheck is null))
            {
                nodeType = nodeForCheck.nodeType;
                if (nodeType.Equals(NodeTypes.None) || nodeType.Equals(NodeTypes.Obstacle)) return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < groundBounds.size.x && y >= 0 && y < groundBounds.size.y)
            return NodeGrid[x, y];
        return null;
    }

    public Node GetNode(Vector2Int index)
    {
        int x = index.x;
        int y = index.y;
        if (x >= 0 && x < groundBounds.size.x && y >= 0 && y < groundBounds.size.y)
            return NodeGrid[x, y];
        return null;
    }

    public NodeTypes GetNodeType(int x, int y)
    {
        if (x < 0 || x >= groundBounds.size.x || y < 0 || y >= groundBounds.size.y) return NodeTypes.None;
        return NodeGrid[x, y].nodeType;
    }

    public void GlowInteractables()
    {
        foreach (var item in Switches)
        {
            Vector2Int switchIndex = GetNodeGridIndex(item.transform.position);
            Vector2Int playerIndex = GetNodeGridIndex(Player.transform.position);
            float distance = Vector2Int.Distance(switchIndex, playerIndex);
            
            if(distance < 3) item.Focus();
            else item.DeFocus();
        }
    }
}
