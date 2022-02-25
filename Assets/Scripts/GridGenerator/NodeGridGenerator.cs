using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGridGenerator : MonoBehaviour
{

    [SerializeField] Vector2 gridStartPoint;
    public Vector2 GridStartPoint { get { return gridStartPoint; } }

    [SerializeField] Vector2 gridEndPoint;
    public Vector2 GridEndPoint { get { return gridEndPoint; } }

    [SerializeField] float cellSize = 0.5f;
    public float CellSize { get { return cellSize; } }

    [SerializeField] Vector2 collisionCheckSensorSize = new Vector2(1, 1);
    public Vector2 CollisionCheckSensorSize { get { return collisionCheckSensorSize; } }

    [SerializeField] LayerMask layerTocheckCollide;
    public LayerMask LayerToCheckCollide { get { return layerTocheckCollide; } }


    int numCols;
    public int NumCols { get { return numCols; } }
    
    int numRows;
    public int NumRows { get { return numRows; } }

    Node[,] nodeGrid;
    public Node[,] NodeGrid { get { return nodeGrid; } }


    // Start is called before the first frame update
    void Awake()
    {

        numCols = (int)((gridEndPoint.x - gridStartPoint.x) / cellSize + 0.5);
        numRows = (int)((gridEndPoint.y - gridStartPoint.y) / cellSize + 0.5);

        nodeGrid = generateNodeGrid();


    }

    public Node findNodeOnPosition(Vector2 position)
    {
        if (nodeGrid == null) return null;

        if (position.x < gridStartPoint.x || position.y < gridStartPoint.y
            || position.x > gridEndPoint.x || position.y > gridEndPoint.y) return null;


        Vector2 relativePosition = position - gridStartPoint;

        int x = (int)(relativePosition.x / cellSize);
        int y = (int)(relativePosition.y / cellSize);

        return nodeGrid[x, y];
    }


    Node[,] generateNodeGrid()
    {



        Node[,] nodes = new Node[numCols, numRows];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                Vector2 nodeCenter = new Vector2(
                    gridStartPoint.x + cellSize / 2 + x * cellSize,
                    gridStartPoint.y + cellSize / 2 + y * cellSize);

                bool isWall =
                    null != Physics2D.OverlapBox(nodeCenter, collisionCheckSensorSize, 0, layerTocheckCollide);


                nodes[x, y] = new Node(x, y, nodeCenter, isWall);



            }
        }
        return nodes;
    }


    #region Draw On Gizmos for Debug
#if DEBUG
    [SerializeField] bool GizmoDraw;
    void OnDrawGizmos()
    {
        if (!GizmoDraw) return;

        //drawObjectPositionNode();
        //drawNodeOnGizmo(goalNode, goalGizmoColor);
        drawGridLine();
        drawObstacles();
        //drawShortestPath();
        //drawFinalWaypoints();
        //drawFinalNodes();
        //drawClosedNodes();


    }


    void drawNodeOnGizmo(Node node, Color gizmoColor)
    {
        if (node == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(node.nodeCenter, new Vector2(cellSize, cellSize));
    }

    void drawGridLine()
    {
        Gizmos.color = Color.gray;

        Vector2 lineStartPoint = new Vector2();
        Vector2 lineEndPoint = new Vector2();

        //drawVerticalLines
        lineStartPoint.x = gridStartPoint.x;
        lineStartPoint.y = gridStartPoint.y;
        lineEndPoint.x = gridStartPoint.x;
        lineEndPoint.y = gridEndPoint.y; // line end point
        for (int i = 0; i <= numCols; i++)
        {
            Gizmos.DrawLine(lineStartPoint, lineEndPoint);
            lineStartPoint.x += cellSize;
            lineEndPoint.x += cellSize;
        }


        //drawHorizontalLines
        lineStartPoint.x = gridStartPoint.x;
        lineStartPoint.y = gridStartPoint.y;
        lineEndPoint.y = gridStartPoint.y;
        lineEndPoint.x = gridEndPoint.x; //line end point
        for (int i = 0; i <= numRows; i++)
        {
            Gizmos.DrawLine(lineStartPoint, lineEndPoint);
            lineStartPoint.y += cellSize;
            lineEndPoint.y += cellSize;
        }

    }

    void drawObstacles()
    {
        Color red = new Color(1, 0, 0, 0.5f);
        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                if (nodeGrid[x, y].isWall)
                    drawNodeOnGizmo(nodeGrid[x, y], red);

            }
        }
    }

#endif
    #endregion
}
