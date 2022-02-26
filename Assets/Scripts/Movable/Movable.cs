using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public float MoveSpeed = 5f;
    
    public GridManager Grid;
    
    const float MOVE_THRESHOLD = 0.0001f;

    bool moveTrigger = false;
    Vector2 targetPos;

    AudioSource audioSource;

    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (moveTrigger)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);

            if ((Vector2.Distance(transform.position, targetPos) < MOVE_THRESHOLD)) moveTrigger = false;
        }
    }

    public void MoveObject(Vector2 dir)
    {
        moveTrigger = true;

        Vector3 curPos = transform.position;
        targetPos = (Vector2)curPos + dir;

        Vector2Int curGridIndex = Grid.GetNodeGridIndex(curPos);
        Vector2Int targetGridIndex = Grid.GetNodeGridIndex(targetPos);

        Grid.NodeGrid[curGridIndex.x, curGridIndex.y].nodeType = NodeTypes.Ground;
        Grid.NodeGrid[targetGridIndex.x, targetGridIndex.y].nodeType = NodeTypes.Movable;

        audioSource.Play();
    }

}
