using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public float moveSpeed = 5f;


    const float MOVE_THRESHOLD = 0.0001f;

    bool moveTrigger = false;
    Vector2 tarPos;

    // Update is called once per frame
    void Update()
    {

        if (moveTrigger)
        {
            transform.position = Vector2.MoveTowards(transform.position, tarPos, moveSpeed * Time.deltaTime);

            if ((Vector2.Distance(transform.position, tarPos) < MOVE_THRESHOLD)) moveTrigger = false;
        }
    }

    public void MoveObject(Vector2 dir)
    {
        moveTrigger = true;

        tarPos= (Vector2)transform.position + dir;
    }

}
