#define DEBUG

using System;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVE_THRESHOLD = 0.0001f;
    public float MoveSpeed = 5f;
    public Vector2 PlayerFaceDirection = Vector2.down;
    
    public Transform movePoint;

    public ModelController PlayerModelController;

    public GameObject item, LeftItem, RightItem;
    // player.item.tag => free_hand / knife / lock
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
    public GameEvent PlayerMurdered;
    
    float _grabRadius = 0.5f;
    Movable _grabbedMovableObj;
    private Switch _switch;

    private bool _horizontalInputLock;
    private bool _verticalInputLock;

    private float _horizontalInput;
    private float _verticalInput;

    private bool hasPlayerMoved = false;

    [HideInInspector] public bool StoredInGridManger = false;

    AudioSource audioSource;

    private void Start()
    {
        transform.parent = null;
        movePoint.parent = null;

        audioSource = this.gameObject.GetComponent<AudioSource>();
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
            _switch = GetInteractableObj();
            if (!(_switch is null))
            {
                _switch.Interact();
                _switch = null;
            }
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

    private Switch GetInteractableObj()
    {
        Collider2D obj = Physics2D.OverlapCircle((Vector2)transform.position + PlayerFaceDirection, _grabRadius);
        Switch s = null;
        if (!(obj is null)) s = obj.CompareTag("Interactable") ? obj.GetComponent<Switch>() : null;
        return s;
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

            PlayerModelController.Hop();
            
            hasPlayerMoved = true;

            item.GetComponent<SpriteRenderer>().flipX = (dir > 0);

            item.transform.position = (dir > 0) ? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = 5;

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

            PlayerModelController.Hop();
            
            hasPlayerMoved = true;

            // vertical input이면 아이템의 order layer를 0으로, 아니면 1로 지정
            item.GetComponent<SpriteRenderer>().flipX = (dir > 0);

            item.transform.position = (dir > 0) ? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = dir > 0 ? 1 : 5;
        }
    }

    

    Movable GetMovableObj()
    {
        Collider2D obj = Physics2D.OverlapCircle((Vector2)transform.position + PlayerFaceDirection, _grabRadius);
        Movable m = null;
        if (!(obj is null)) m = obj.CompareTag("Movable") ? obj.GetComponent<Movable>() : null;
        return m;
    }

    void GetItem(Collider2D collision)
    {
        // 플레이어의 아이템 활성화
        bool isItemEnable = collision.tag == "knife" || collision.tag == "lock";
        item.SetActive(isItemEnable);

        Debug.Log(isItemEnable);

        //  아이템을 먹은게 아니면 아무 작업도 하지 않음
        if (!isItemEnable)
        {
            item.tag = "free_hand";
            return;
        }

        // 맵에 있는 아이템 사라지게 처리
        collision.gameObject.SetActive(false);

        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        switch (collision.tag)
        {
            case "knife":
                spriteRenderer.sprite = itemKnife;
                item.tag = "knife";
                // 칼에 대한 정보로 갱신
                break;
            case "lock":
                spriteRenderer.sprite = itemLock;
                item.tag = "lock";
                // 자물쇠에 대한 정보로 갱신
                break;
        }
    }

    void AttackToEnemy(Collider2D collision)
    {
        if (item.activeSelf && collision.tag == "Murderer")
        {
            Debug.Log("TESTtest");
            Destroy(collision.gameObject);
            item.tag = "free_hand";
            item.SetActive(false);
            audioSource.Play();
        }
    }

    void LockTheDoor(Collider2D collision)
    {
        if (item.activeSelf && collision.tag == "door")
        {
            // lock the door
            item.tag = "free_hand";
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("PlayerController OnTriggerEnter2D");

        GameObject collideWith = collision.gameObject;

        bool collideWithItem = collision.tag.Equals("knife") || collision.tag.Equals("lock");
        bool collideWithMurderer = collideWith.tag.Equals("Murderer");
        bool collideWithDoor = collideWith.tag.Equals("door");
        
        if (collideWithMurderer)
        {
            bool playerHasKnife = item.activeInHierarchy && item.tag.Equals("knife");
            if (!playerHasKnife)
            {
                PlayerMurdered.Raise();
                return;
            }

            Destroy(collideWith);
            item.tag = "free_hand";
            item.SetActive(false);
            audioSource.Play();
        }
        else if (collideWithItem)
        {
            item.SetActive(true);
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            switch (collision.tag)
            {
                case "knife":
                    spriteRenderer.sprite = itemKnife;
                    item.tag = "knife";
                    
                    break;
                case "lock":
                    spriteRenderer.sprite = itemLock;
                    item.tag = "lock";
                    
                    break;
            }
            collision.gameObject.SetActive(false);
        }
        else if(collideWithDoor)
        {
            if (item.activeSelf && item.tag.Equals("lock"))
            {
                item.tag = "free_hand";
                item.SetActive(false);
            }
        }

        LockTheDoor(collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position + (Vector3)PlayerFaceDirection, _grabRadius);

    }
}
