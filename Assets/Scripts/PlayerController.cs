#define DEBUG

using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVE_THRESHOLD = 0.0001f;
    public float moveSpeed = 5f;
    public Transform movePoint;

    public GameEvent MoveLeft;
    public GameEvent MoveRight;
    public GameEvent MoveUp;
    public GameEvent MoveDown;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, movePoint.position) > MOVE_THRESHOLD) return;
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizontalInput) >= 1f)
        {
            float dir = Mathf.Sign(horizontalInput);
            if(dir > 0) MoveLeft.Raise();
            else MoveRight.Raise();
#if DEBUG
            Debug.LogFormat("horizontal input: {0}", dir);
#endif
            movePoint.position += new Vector3(dir, 0f, 0f);
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) >= 1f)
        {
            float dir = Mathf.Sign(verticalInput);
            if(dir > 0) MoveUp.Raise();
            else MoveDown.Raise();
#if DEBUG
            Debug.LogFormat("vertical input: {0}", dir);
#endif
            movePoint.position += new Vector3(0f, dir, 0f);
        }
    }
}
