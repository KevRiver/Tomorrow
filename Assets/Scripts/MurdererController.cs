using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererController : MonoBehaviour
{
    private const float MOVE_THRESHOLD = 0.0001f;
    public float MoveSpeed = 5f;
    public float JumpSpeed = 2.5f;

    [Header("Grid Info")] 
    public GridManager Grid;

    [Header("Move Info")] public bool IsMovedThisTurn;

    [Header("Model Controller")] public Transform Model;
    public ModelController modelcontroller;

    [Header("Target")] public Transform Target;

    [Header("Murderer Events")] public GameEvent MurdererMove;
    
    private Astar _pathFinder;
    
    private Vector2Int _start;
    private Vector2Int _end;
    private Vector2Int _nextStep;
    private Vector3 _nextPosition;

    private void Start()
    {
        transform.parent = null;
    }

    public void Move()
    {
        _pathFinder = Grid.PathFinder;
        // 타겟 오브젝트 또는 위치로 이동
        SetStart();
        SetEnd();

        List<Node> path = _pathFinder.CreatePath(Grid.NodeGrid, _start, _end, 1);

        if (path is null)
        {
            Grid.MurderersMoveCount += 1;
            return;
        }

        _nextStep = new Vector2Int(path[0].X, path[0].Y);
        int dx = (int)_nextStep.x - _start.x;
        int dy = (int)_nextStep.y - _start.y;

        _nextPosition = new Vector3(_nextStep.x + 0.5f, _nextStep.y + 0.5f, 0);

        if (dx < 0) modelcontroller.FaceLeft();
        if (dx > 0) modelcontroller.FaceRight();
        if (dy < 0) modelcontroller.FaceDown();
        if (dy > 0) modelcontroller.FaceUp();
        
        StartCoroutine(IMove());
    }

    private void SetStart()
    {
        Vector3 raw = transform.position;
        Vector3Int pos = new Vector3Int((int) raw.x, (int) raw.y, (int) raw.z);
        _start = Grid.GetNodeGridIndex(pos);
    }

    private void SetEnd()
    {
        Vector3 raw = Target.position;
        Vector3Int pos = new Vector3Int((int) raw.x, (int) raw.y, (int) raw.z);
        _end = Grid.GetNodeGridIndex(pos);
    }
    
    private IEnumerator IMove()
    {
        float timeElapsed = 0;
        float timeLimit = 0.2f;
        
        while (Vector3.Distance(transform.position, _nextPosition) > MOVE_THRESHOLD)
        {
            Vector3 org = Model.position;
            Model.position = timeElapsed < timeLimit / 2
                ? new Vector3(org.x, org.y + JumpSpeed * Time.deltaTime, org.z)
                : new Vector3(org.x, org.y - JumpSpeed * Time.deltaTime, org.z);
            
            transform.position = Vector3.MoveTowards(transform.position, _nextPosition, MoveSpeed * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            
            yield return null;
        }
    }

    
}
