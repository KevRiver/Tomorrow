using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Astar
{
    public Node[,] Nodes;
    public Astar(int columns, int rows)
    {
        Nodes = new Node[columns, rows];
    }
    private bool IsValidPath(Node start, Node end)
    {
        if (end == null)
            return false;
        if (start == null)
            return false;
        if (end.Height >= 1)
            return false;
        return true;
    }
    public List<Node> CreatePath(Node[,] org, Vector2Int start, Vector2Int end, int length)
    {
        Node End = null;
        Node Start = null;
        var columns = Nodes.GetUpperBound(0) + 1;
        var rows = Nodes.GetUpperBound(1) + 1;
        Nodes = new Node[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Nodes[i, j] = new Node(org[i, j].X, org[i, j].Y, org[i, j].Height, org[i, j].nodeType);
            }
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Nodes[i, j].AddNeighboors(Nodes, i, j);
                if (Nodes[i, j].X == start.x && Nodes[i, j].Y == start.y)
                    Start = Nodes[i, j];
                else if (Nodes[i, j].X == end.x && Nodes[i, j].Y == end.y)
                    End = Nodes[i, j];
            }
        }
        if (!IsValidPath(Start, End))
            return null;
        
        List<Node> OpenSet = new List<Node>();
        List<Node> ClosedSet = new List<Node>();

        OpenSet.Add(Start);

        while (OpenSet.Count > 0)
        {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < OpenSet.Count; i++)
                if (OpenSet[i].F < OpenSet[winner].F)
                    winner = i;
                else if (OpenSet[i].F == OpenSet[winner].F)//tie breaking for faster routing
                    if (OpenSet[i].H < OpenSet[winner].H)
                        winner = i;

            var current = OpenSet[winner];

            //Found the path, creates and returns the path
            if (End != null && OpenSet[winner] == End)
            {
                List<Node> Path = new List<Node>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != null)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count - 1) < 0)
                {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }

            OpenSet.Remove(current);
            ClosedSet.Add(current);


            //Finds the next closest step on the grid
            var neighboors = current.Neighboors;
            for (int i = 0; i < neighboors.Count; i++)//look threw our current spots neighboors (current spot is the shortest F distance in openSet
            {
                var n = neighboors[i];
                if (!ClosedSet.Contains(n) && n.Height < 1)//Checks to make sure the neighboor of our current tile is not within closed set, and has a height of less than 1
                {
                    var tempG = current.G + 1;//gets a temp comparison integer for seeing if a route is shorter than our current path

                    bool newPath = false;
                    if (OpenSet.Contains(n)) //Checks if the neighboor we are checking is within the openset
                    {
                        if (tempG < n.G)//The distance to the end goal from this neighboor is shorter so we need a new path
                        {
                            n.G = tempG;
                            newPath = true;
                        }
                    }
                    else//if its not in openSet or closed set, then it IS a new path and we should add it too openset
                    {
                        n.G = tempG;
                        newPath = true;
                        OpenSet.Add(n);
                    }
                    if (newPath)//if it is a newPath caclulate the H and F and set current to the neighboors previous
                    {
                        n.H = Heuristic(n, End);
                        n.F = n.G + n.H;
                        n.previous = current;
                    }
                }
            }

        }
        return null;
    }

    private int Heuristic(Node a, Node b)
    {
        //manhattan
        var dx = Math.Abs(a.X - b.X);
        var dy = Math.Abs(a.Y - b.Y);
        return 1 * (dx + dy);

        #region diagonal
        //diagonal
        // Chebyshev distance
        //var D = 1;
        // var D2 = 1;
        //octile distance
        //var D = 1;
        //var D2 = 1;
        //var dx = Math.Abs(a.X - b.X);
        //var dy = Math.Abs(a.Y - b.Y);
        //var result = (int)(1 * (dx + dy) + (D2 - 2 * D));
        //return result;// *= (1 + (1 / 1000));
        //return (int)Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        #endregion
    }
}

public enum NodeTypes
{
    None,
    Ground,
    Obstacle,
    Movable,
    Interactable
}
public class Node
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public int Height = 0;
    public List<Node> Neighboors;
    public Node previous = null;
    public NodeTypes nodeType = NodeTypes.None;
    public Node(int x, int y, int height, NodeTypes type)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        Neighboors = new List<Node>();
        Height = height;
        nodeType = type;
    }
    public void AddNeighboors(Node[,] map, int x, int y)
    {
        if (x < map.GetUpperBound(0) && map[x + 1, y].nodeType.Equals(NodeTypes.Ground))
            Neighboors.Add(map[x + 1, y]);

        if (x > 0 && map[x - 1, y].nodeType.Equals(NodeTypes.Ground))
            Neighboors.Add(map[x - 1, y]);
        
        if (y < map.GetUpperBound(1) && map[x, y + 1].nodeType.Equals(NodeTypes.Ground))
            Neighboors.Add(map[x, y + 1]);
        
        if (y > 0 && map[x, y - 1].nodeType.Equals(NodeTypes.Ground))
            Neighboors.Add(map[x, y - 1]);
        #region diagonal
        //if (X > 0 && Y > 0)
        //    Neighboors.Add(grid[X - 1, Y - 1]);
        //if (X < Utils.Columns - 1 && Y > 0)
        //    Neighboors.Add(grid[X + 1, Y - 1]);
        //if (X > 0 && Y < Utils.Rows - 1)
        //    Neighboors.Add(grid[X - 1, Y + 1]);
        //if (X < Utils.Columns - 1 && Y < Utils.Rows - 1)
        //    Neighboors.Add(grid[X + 1, Y + 1]);
        #endregion
    }
}