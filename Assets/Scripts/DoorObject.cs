using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public bool IsOpened;
    public GameObject OpenedDoor, ClosedDoor;
    public GameEvent OpenDialog;

    AudioSource audioSource;
    
    void Start()
    {
        IsOpened = true;
        OpenedDoor.SetActive(false);
        ClosedDoor.SetActive(true);

        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    void SetDoor(GameObject gameObject)
    {
        PlayerController player = gameObject.GetComponent<PlayerController>();

        if (!player.item.CompareTag("lock"))
            return;
        
        if (IsOpened)
        {
            OpenedDoor.SetActive(true);
            ClosedDoor.SetActive(false);
            
            audioSource.Play();

            IsOpened = true;
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
