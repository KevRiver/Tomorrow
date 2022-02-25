#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVE_THRESHOLD = 0.0001f;
    public float MoveSpeed = 5f;
    public Transform movePoint;
    
    public GameEvent MoveLeft;
    public GameEvent MoveRight;
    public GameEvent MoveUp;
    public GameEvent MoveDown;

    public GameObject item, LeftItem, RightItem;
    public Sprite itemKnife, itemLock;
    
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
            
            item.GetComponent<SpriteRenderer>().flipX = (horizontalInput >= 1f);

            item.transform.position = (horizontalInput >= 1)? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
            
            // vertical input�̸� �������� order layer�� 0����, �ƴϸ� 1�� ����
            bool isVerticalInput = (verticalInput >= 1f);

            item.GetComponent<SpriteRenderer>().flipX = isVerticalInput;

            item.transform.position = (verticalInput >= 1) ? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(!isVerticalInput);
        }
    }
    
    void GetItem(Collider2D collision)
    {
        // �÷��̾��� ������ Ȱ��ȭ
        bool isItemEnable = collision.tag == "knife" || collision.tag == "lock";
        item.SetActive(isItemEnable);

        Debug.Log(isItemEnable);

        //  �������� ������ �ƴϸ� �ƹ� �۾��� ���� ����
        if (!isItemEnable) return;

        // �ʿ� �ִ� ������ ������� ó��
        collision.gameObject.SetActive(false);

        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        switch(collision.tag)
        {
            case "knife":
                spriteRenderer.sprite = itemKnife;
                // Į�� ���� ������ ����
                break;
            case "lock":
                spriteRenderer.sprite = itemLock;
                // �ڹ��迡 ���� ������ ����
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
