#define DEBUG

using System;
using UnityEngine;

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
    public GameEvent PlayerMove;
    
    float _grabRadius = 0.5f;
    Movable _grabbedMovableObj;

    private bool _horizontalInputLock;
    private bool _verticalInputLock;

    private float _horizontalInput;
    private float _verticalInput;

    private bool hasPlayerMoved = false;

    [HideInInspector] public bool StoredInGridManger = false;

    private void Start()
    {
        transform.parent = null;
        movePoint.parent = null;
    }

    private void Update()
    {
        // Move
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) > MOVE_THRESHOLD) return;
        
        if (hasPlayerMoved)
        {
            PlayerMove.Raise();
            hasPlayerMoved = false;
        }

        UpdateValidDirection();
        
        HandleGrabInput();

        //CheckPreventedInputAxis();
        
        HandleInput();
    }

    private void UpdateValidDirection()
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
    
    void HandleGrabInput()
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

        if (!(_grabbedMovableObj is null))
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
        
        _horizontalInputLock = IsGrabbing &&
                               (PlayerFaceDirection.Equals(Vector2.up) || PlayerFaceDirection.Equals(Vector2.down));
        _verticalInputLock = IsGrabbing &&
                             (PlayerFaceDirection.Equals(Vector2.left) || PlayerFaceDirection.Equals(Vector2.right));

        _horizontalInput = !_horizontalInputLock ? Input.GetAxisRaw("Horizontal") : 0f;
        _verticalInput = !_verticalInputLock ? Input.GetAxisRaw("Vertical") : 0f;
    }

    private void HandleInput()
    {
        float dir = 0;
        if (Mathf.Abs(_horizontalInput) >= 1f)
        {
            dir = Mathf.Sign(_horizontalInput);
            
            if (!IsGrabbing)
            {
                PlayerFaceDirection = new Vector2(dir, 0);
                if (dir < 0) FaceLeft.Raise();
                else FaceRight.Raise();
            }
            else
            {
                if(dir < 0) {
                    CanMoveLeft = Grid.UpdateValidDirectionWhenGrabbing(transform.position, _grabbedMovableObj.transform.position, Vector2.left, PlayerFaceDirection);
                }
                else if(dir > 0)
                {
                    CanMoveRight = Grid.UpdateValidDirectionWhenGrabbing(transform.position, _grabbedMovableObj.transform.position, Vector2.right, PlayerFaceDirection);
                }
            }

            if (dir < 0 && !CanMoveLeft) return;
            if (dir > 0 && !CanMoveRight) return;
            
            movePoint.position += new Vector3(dir, 0f, 0f);
            
            if (!(_grabbedMovableObj is null)) _grabbedMovableObj.MoveObject(new Vector2(dir, 0f));
            
            hasPlayerMoved = true;
            
            // item.GetComponent<SpriteRenderer>().flipX = (horizontalInput >= 1f);
            //
            // item.transform.position = (horizontalInput >= 1)? RightItem.transform.position : LeftItem.transform.position;
            //
            // item.GetComponent<SpriteRenderer>().sortingOrder = 1;
            
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) >= 1f)
        {
            dir = Mathf.Sign(_verticalInput);

            if (!IsGrabbing)
            {
                PlayerFaceDirection = new Vector2(0, dir);

                if (dir > 0) FaceUp.Raise();
                else FaceDown.Raise();
            }
            else
            {
                if(dir < 0)
                {
                    CanMoveDown = Grid.UpdateValidDirectionWhenGrabbing(transform.position,
                        _grabbedMovableObj.transform.position, Vector2.down, PlayerFaceDirection);
                }
                else if(dir > 0)
                {
                    CanMoveUp = Grid.UpdateValidDirectionWhenGrabbing(transform.position,
                        _grabbedMovableObj.transform.position, Vector2.up, PlayerFaceDirection);
                }
            }

            if (dir < 0 && !CanMoveDown) return;
            if (dir > 0 && !CanMoveUp) return;
            
            movePoint.position += new Vector3(0f, dir, 0f);
            
            if (!(_grabbedMovableObj is null)) _grabbedMovableObj.MoveObject(new Vector2(0f, dir));

            hasPlayerMoved = true;

            // item.GetComponent<SpriteRenderer>().flipX = isVerticalInput;
            //
            // item.transform.position = (verticalInput >= 1) ? RightItem.transform.position : LeftItem.transform.position;
            //
            // item.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(!isVerticalInput);
        }
    }

    

    Movable GetMovableObj()
    {
        Collider2D obj = Physics2D.OverlapCircle((Vector2)transform.position + PlayerFaceDirection, _grabRadius);
        Movable m = null;
        if (!(obj is null)) m = obj.CompareTag("Movable") ? obj.GetComponent<Movable>() : null;
        return m;
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

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position + (Vector3)PlayerFaceDirection, _grabRadius);

    }
}
