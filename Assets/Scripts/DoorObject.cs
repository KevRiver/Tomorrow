using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public bool IsOpened;
    public GameObject OpenedDoor, ClosedDoor;
    public GameEvent OpenDialog;
    
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
        Debug.Log("setdoor called");

        PlayerController player = gameObject.GetComponent<PlayerController>();

        if (!player.item.CompareTag("lock"))
            return;
        
        if (IsOpened)
        {
            OpenedDoor.SetActive(false);
            ClosedDoor.SetActive(true);
            IsOpened = false;
            player.item.GetComponent<SpriteRenderer>().sprite = null;

            OpenDialog.Raise();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            SetDoor(collision.gameObject);
    }

}
