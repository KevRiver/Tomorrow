#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    const float MOVE_THRESHOLD = 0.0001f;
    public float MoveSpeed = 5f;
    public Vector2 PlayerFaceDirection = Vector2.down;
    
    public Transform movePoint;

    public GameObject item, LeftItem, RightItem;
    public Sprite itemKnife, itemLock;

    [Header("Neighbor Tile Detection")] 
    public GridManager Grid;
    [SerializeField] private bool CanMoveLeft = false;
    [SerializeField] private bool CanMoveRight = false;
    [SerializeField] private bool CanMoveUp = false;
    [SerializeField] private bool CanMoveDown = false;
    [SerializeField] private bool IsGrabbing = false;
    
    [Header("MoveEvents")]
    public GameEvent FaceLeft;
    public GameEvent FaceRight;
    public GameEvent FaceUp;
    public GameEvent FaceDown;
    public GameEvent Move;
    public GameEvent Drag;
    
    
    #region Movable object
    
    float _grabRadius = 0.5f;
    Movable _grabbedMovableObj;

    #endregion

    void Start()
    {
        transform.parent = null;
        movePoint.parent = null;
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) > MOVE_THRESHOLD) return;
        
        UpdateValidDirection();

        bool horizontalInputLock = IsGrabbing &&
                                   (PlayerFaceDirection.Equals(Vector2.up) || PlayerFaceDirection.Equals(Vector2.down));
        bool verticalInputLock = IsGrabbing &&
                                   (PlayerFaceDirection.Equals(Vector2.left) || PlayerFaceDirection.Equals(Vector2.right));

        float horizontalInput = !horizontalInputLock ? Input.GetAxisRaw("Horizontal") : 0f;
        float verticalInput = !verticalInputLock ? Input.GetAxisRaw("Vertical") : 0f;
        
        if (Mathf.Abs(horizontalInput) >= 1f)
        {
            float dir = Mathf.Sign(horizontalInput);
            
            if (!IsGrabbing)
            {
                PlayerFaceDirection = new Vector2(dir, 0);
                if (dir < 0) FaceLeft.Raise();
                else FaceRight.Raise();
            }
            else
            {
                if(dir < 0 && CanMoveLeft) {
                    CanMoveLeft = Grid.CheckMovementValid(_grabbedMovableObj.transform.position, Vector2.left);
                }
                else if(dir > 0 && CanMoveRight)
                {
                    CanMoveRight = Grid.CheckMovementValid(_grabbedMovableObj.transform.position, Vector2.right);
                }
            }

            if (dir < 0 && !CanMoveLeft) return;
            if (dir > 0 && !CanMoveRight) return;

            Move.Raise();

            movePoint.position += new Vector3(dir, 0f, 0f);
            
            // item.GetComponent<SpriteRenderer>().flipX = (horizontalInput >= 1f);
            //
            // item.transform.position = (horizontalInput >= 1)? RightItem.transform.position : LeftItem.transform.position;
            //
            // item.GetComponent<SpriteRenderer>().sortingOrder = 1;

            if (_grabbedMovableObj != null) _grabbedMovableObj.MoveObject(new Vector2(Input.GetAxisRaw("Horizontal"), 0f));



        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) >= 1f)
        {
            float dir = Mathf.Sign(verticalInput);

            if (!IsGrabbing)
            {
                PlayerFaceDirection = new Vector2(0, dir);

                if (dir > 0) FaceUp.Raise();
                else FaceDown.Raise();
            }
            else
            {
                if(dir < 0 && CanMoveDown) {
                    CanMoveDown = Grid.CheckMovementValid(_grabbedMovableObj.transform.position, Vector2.down);
                }
                else if(dir > 0 && CanMoveUp)
                {
                    CanMoveUp = Grid.CheckMovementValid(_grabbedMovableObj.transform.position, Vector2.up);
                }
            }

            if (dir < 0 && !CanMoveDown) return;
            if (dir > 0 && !CanMoveUp) return;
            
            Move.Raise();

            movePoint.position += new Vector3(0f, dir, 0f);
            
            // vertical input�̸� �������� order layer�� 0����, �ƴϸ� 1�� ����
            bool isVerticalInput = (verticalInput >= 1f);

            // item.GetComponent<SpriteRenderer>().flipX = isVerticalInput;
            //
            // item.transform.position = (verticalInput >= 1) ? RightItem.transform.position : LeftItem.transform.position;
            //
            // item.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(!isVerticalInput);

            if (_grabbedMovableObj != null) _grabbedMovableObj.MoveObject(new Vector2(0f, Input.GetAxisRaw("Vertical")));

        }
        
        InteractionUpdate();
    }

    void InteractionUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _grabbedMovableObj = GetMovableObj();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            _grabbedMovableObj = null;
        }

        if (!Input.GetButton("Fire1"))
        {
            _grabbedMovableObj = null;
        }

        if (_grabbedMovableObj != null)
        {
            IsGrabbing = true;
            if (PlayerFaceDirection.Equals(Vector2.left) || PlayerFaceDirection.Equals(Vector2.right))
            {
                CanMoveUp = false;
                CanMoveDown = false;
            }
            else
            {
                CanMoveLeft = false;
                CanMoveRight = false;
            }
        }
        else
        {
            IsGrabbing = false;
        }
    }

    Movable GetMovableObj()
    {
        Collider2D obj = Physics2D.OverlapCircle((Vector2)transform.position + PlayerFaceDirection, _grabRadius);
        if (obj != null && obj.CompareTag("Movable"))
            return obj.GetComponent<Movable>();
        
        return null;
    }
    
    
    void SetItemPosition(bool isVertical)
    {
        /*
         horizontal >= 1�̸�
            flipX = true
            rightItem
         else
            flipX = false
            leftItem

         vertical >= 1�̸�
            flipX = true
            rightItem
         else
            leftItem
         */
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

    void UpdateValidDirection()
    {
        Vector3 raw = transform.position;
        Vector3Int pos = new Vector3Int((int) raw.x, (int) raw.y, (int) raw.z);
        Vector2Int index = Grid.GetNodeGridIndex(pos);

        if (IsGrabbing)
        {
            if (PlayerFaceDirection.Equals(Vector2.left) || PlayerFaceDirection.Equals(Vector2.right))
            {
                CanMoveLeft = Grid.GetNodeType(index.x - 1, index.y) == NodeTypes.Ground;
                CanMoveRight = Grid.GetNodeType(index.x + 1, index.y) == NodeTypes.Ground;
            }
            else
            {
                CanMoveUp = Grid.GetNodeType(index.x, index.y + 1) == NodeTypes.Ground;
                CanMoveDown = Grid.GetNodeType(index.x, index.y - 1) == NodeTypes.Ground;
            }
        }
        else
        {
            CanMoveUp = Grid.GetNodeType(index.x, index.y + 1) == NodeTypes.Ground;
            CanMoveDown = Grid.GetNodeType(index.x, index.y - 1) == NodeTypes.Ground;
            CanMoveLeft = Grid.GetNodeType(index.x - 1, index.y) == NodeTypes.Ground;
            CanMoveRight = Grid.GetNodeType(index.x + 1, index.y) == NodeTypes.Ground;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position + (Vector3)PlayerFaceDirection, _grabRadius);

    }
}
