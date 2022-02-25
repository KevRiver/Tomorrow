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

    public GameObject item, LeftItem, RightItem;
    public Sprite itemKnife, itemLock;
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
            
            item.GetComponent<SpriteRenderer>().flipX = (horizontalInput >= 1f);

            item.transform.position = (horizontalInput >= 1)? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
            
            // vertical input�̸� �������� order layer�� 0����, �ƴϸ� 1�� ����
            bool isVerticalInput = (verticalInput >= 1f);

            item.GetComponent<SpriteRenderer>().flipX = isVerticalInput;

            item.transform.position = (verticalInput >= 1) ? RightItem.transform.position : LeftItem.transform.position;

            item.GetComponent<SpriteRenderer>().sortingOrder = Convert.ToInt32(!isVerticalInput);
        }
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

    public string getPlayerItemName()
    {
        Sprite sprite = item.GetComponent<SpriteRenderer>().sprite;
        if (sprite == null) return "";
        else return sprite.name;
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
}
