using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public bool IsOpened;
    public GameObject OpenedDoor, ClosedDoor;
    
    void Start()
    {
        IsOpened = true;
        OpenedDoor.SetActive(true);
        ClosedDoor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDoor(GameObject gameObject)
    {
        PlayerController player = gameObject.GetComponent<PlayerController>();

        if (!player.getPlayerItemName().Equals(player.itemLock.name))
            return;

        Debug.Log(player.getPlayerItemName().Equals(player.itemLock.name));
        
        if (IsOpened)
        {
            OpenedDoor.SetActive(false);
            ClosedDoor.SetActive(true);
            IsOpened = false;
            player.item.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            SetDoor(collision.gameObject);
    }

}
