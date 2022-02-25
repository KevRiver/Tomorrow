using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;






public class Node 
{
    public float hCost, gCost;
    public float fCost { get { return hCost + gCost; } }

    public bool isWall;
    
    public int XIndex { get; }
    public int YIndex { get; }
    int _XIndex, _YIndex;

    public Node parent;
    public Vector2 nodeCenter { get; }

    public bool onCloseList;
    public bool onOpenList;


    public Node(int XIndex, int YIndex, Vector2 nodeCenter, bool isWall)
    {
        this.hCost = 0f;
        this.gCost = 0f;
        this.isWall = isWall;
        this.parent = null;
        this.nodeCenter = nodeCenter;
        this.XIndex = XIndex;
        this.YIndex = YIndex;
        this.onCloseList = false;
        this.onOpenList = false;


    }



    
     
}
