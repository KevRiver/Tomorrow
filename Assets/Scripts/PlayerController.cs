#define DEBUG

using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    const float MOVE_THRESHOLD = 0.0001f;
    public float moveSpeed = 5f;
    public Transform movePoint;

    public GameEvent MoveLeft;
    public GameEvent MoveRight;
    public GameEvent MoveUp;
    public GameEvent MoveDown;

    public GameObject item, LeftItem, RightItem;
    public Sprite itemKnife, itemLock;


    #region Movable object와 interact하기 위한 variables
    float adjSensorSize = 0.5f;   //근처 Grid에 물건이 있는지 체크하는데 사용되는 센서 사이즈.
    Movable[] adjMovables; // 차례대로 상, 하, 좌, 우

    Movable grappbedMovable;
    int movableSelector = 0; // 플레어의 위치에 여러개의 movable이 있을 수 있으니까. 여러개의 movable중 하나를 고를 수 있게 하는 counter.

    #endregion
    private void Awake()
    {
        adjMovables = new Movable[4];
    }

    void Start()
    {
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
            if(dir > 0) MoveLeft.Raise();
            else MoveRight.Raise();
#if DEBUG
            Debug.LogFormat("horizontal input: {0}", dir);
#endif
            movePoint.position += new Vector3(dir, 0f, 0f);
            
            item.GetComponent<SpriteRenderer>().flipX = (horizontalInput >= 1f);

            item.transform.position = (horizontalInput >= 1)? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = 1;

            if (grappbedMovable != null) grappbedMovable.MoveObject(new Vector2(Input.GetAxisRaw("Horizontal"), 0f));



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
            
            // vertical input이면 아이템의 order layer를 0으로, 아니면 1로 지정
            bool isVerticalInput = (verticalInput >= 1f);

            item.GetComponent<SpriteRenderer>().flipX = isVerticalInput;

            item.transform.position = (verticalInput >= 1) ? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(!isVerticalInput);

            if (grappbedMovable != null) grappbedMovable.MoveObject(new Vector2(0f, Input.GetAxisRaw("Vertical")));

        }

        updateAdjMovable();
        InteractionUpdate();


        
    }

    void InteractionUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            grappbedMovable = getAdjMovable();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            grappbedMovable = null;
        }

        if (!Input.GetButton("Fire1"))
        {
            grappbedMovable = null;
        }




    }

    Movable getAdjMovable()
    {
        
        for (int i = 0; i < 4; i++)
        {
            movableSelector = (movableSelector + 1) % 4;

            if (adjMovables[movableSelector] != null)
            {
                Debug.Log(adjMovables[movableSelector]);
                return adjMovables[movableSelector];
            }

        }
        return null;
    }

    void updateAdjMovable()
    {

        Collider2D obj = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.up, adjSensorSize);
        if (obj != null && obj.CompareTag("Movable"))
            adjMovables[0] = obj.GetComponent<Movable>();
        else adjMovables[0] = null;

        obj = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.down, adjSensorSize);
        if (obj != null && obj.CompareTag("Movable"))
            adjMovables[1] = obj.GetComponent<Movable>();
        else adjMovables[1] = null;

        
        obj = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.left, adjSensorSize);
        if (obj != null && obj.CompareTag("Movable"))
            adjMovables[2] = obj.GetComponent<Movable>();
        else adjMovables[2] = null;

        obj = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.right, adjSensorSize);
        if (obj != null && obj.CompareTag("Movable"))
            adjMovables[3] = obj.GetComponent<Movable>();
        else adjMovables[3] = null;

    }

    bool areThereAdjMovables()
    {
        if (adjMovables[0] != null || adjMovables[1] != null ||
            adjMovables[2] != null || adjMovables[3] != null) return true;

        return false;
    }




    void SetItemPosition(bool isVertical)
    {
        /*
         horizontal >= 1이면
            flipX = true
            rightItem
         else
            flipX = false
            leftItem

         vertical >= 1이면
            flipX = true
            rightItem
         else
            leftItem
         */
    }

    void GetItem(Collider2D collision)
    {
        // 플레이어의 아이템 활성화
        bool isItemEnable = collision.tag == "knife" || collision.tag == "lock";
        item.SetActive(isItemEnable);

        Debug.Log(isItemEnable);

        //  아이템을 먹은게 아니면 아무 작업도 하지 않음
        if (!isItemEnable) return;

        // 맵에 있는 아이템 사라지게 처리
        collision.gameObject.SetActive(false);

        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        switch(collision.tag)
        {
            case "knife":
                spriteRenderer.sprite = itemKnife;
                // 칼에 대한 정보로 갱신
                break;
            case "lock":
                spriteRenderer.sprite = itemLock;
                // 자물쇠에 대한 정보로 갱신
                break;
        }
    }

    void AttackToEnemy(Collider2D collision)
    {
        if (item.activeSelf && collision.tag == "enemy")
        {
            Destroy(collision.gameObject);
            item.SetActive(false);
        }
    }

    void LockTheDoor(Collider2D collision)
    {
        if (item.activeSelf && collision.tag == "door")
        {
            // lock the door
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetItem(collision);
        AttackToEnemy(collision);
        LockTheDoor(collision);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position + Vector3.up, adjSensorSize);
        Gizmos.DrawWireSphere(transform.position + Vector3.down, adjSensorSize);
        Gizmos.DrawWireSphere(transform.position + Vector3.left, adjSensorSize);
        Gizmos.DrawWireSphere(transform.position + Vector3.right, adjSensorSize);


    }
}
