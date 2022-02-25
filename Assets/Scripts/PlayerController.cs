#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVE_THRESHOLD = 0.0001f;
    public float MoveSpeed = 5f;
    public Transform movePoint;

    [Header("Neighbor Tile Detection")] 
    public GridManager Grid;
    [SerializeField] private bool CanMoveLeft = false;
    [SerializeField] private bool CanMoveRight = false;
    [SerializeField] private bool CanMoveUp = false;
    [SerializeField] private bool CanMoveDown = false;
    
    [Header("MoveEvents")]
    public GameEvent FaceLeft;
    public GameEvent FaceRight;
    public GameEvent FaceUp;
    public GameEvent FaceDown;
    public GameEvent Move;
    public GameEvent Drag;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        movePoint.parent = null;
    }
    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, MoveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, movePoint.position) > MOVE_THRESHOLD) return;
        UpdateMovableDirection();
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizontalInput) >= 1f)
        {
            float dir = Mathf.Sign(horizontalInput);

            if(dir < 0) FaceLeft.Raise();
            else FaceRight.Raise();
            
            if (dir < 0 && !CanMoveLeft) return;
            if (dir > 0 && !CanMoveRight) return;
            
            Move.Raise();

            movePoint.position += new Vector3(dir, 0f, 0f);
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) >= 1f)
        {
            float dir = Mathf.Sign(verticalInput);
            
            if(dir > 0) FaceUp.Raise();
            else FaceDown.Raise();
            
            if (dir < 0 && !CanMoveDown) return;
            if (dir > 0 && !CanMoveUp) return;
            
            Move.Raise();

            movePoint.position += new Vector3(0f, dir, 0f);
        }
    }

    void UpdateMovableDirection()
    {
        Vector3 raw = transform.position;
        Vector3Int pos = new Vector3Int((int) raw.x, (int) raw.y, (int) raw.z);
        Vector2Int index = Grid.GetNodeGridIndex(pos);

        CanMoveUp = Grid.GetNodeType(index.x, index.y + 1) == NodeTypes.Ground;
        CanMoveDown = Grid.GetNodeType(index.x, index.y - 1) == NodeTypes.Ground;
        CanMoveLeft = Grid.GetNodeType(index.x - 1, index.y) == NodeTypes.Ground;
        CanMoveRight = Grid.GetNodeType(index.x + 1, index.y) == NodeTypes.Ground;
    }
}
