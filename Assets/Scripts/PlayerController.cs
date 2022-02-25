#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVE_THRESHOLD = 0.0001f;
    public float moveSpeed = 5f;
    public Transform movePoint;

    [Header("Neighbor Tile Detection")]
    [SerializeField] private bool CanMoveLeft = false;
    [SerializeField] private bool CanMoveRight = false;
    [SerializeField] private bool CanMoveUp = false;
    [SerializeField] private bool CanMoveDown = false;
    
    [Header("MoveEvents")]
    public GameEvent MoveLeft;
    public GameEvent MoveRight;
    public GameEvent MoveUp;
    public GameEvent MoveDown;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        movePoint.parent = null;
    }
    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, movePoint.position) > MOVE_THRESHOLD) return;
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizontalInput) >= 1f)
        {
            float dir = Mathf.Sign(horizontalInput);
            if(dir < 0) MoveLeft.Raise();
            else MoveRight.Raise();

            movePoint.position += new Vector3(dir, 0f, 0f);
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) >= 1f)
        {
            float dir = Mathf.Sign(verticalInput);
            if(dir > 0) MoveUp.Raise();
            else MoveDown.Raise();

            movePoint.position += new Vector3(0f, dir, 0f);
        }
    }
    
    
}
