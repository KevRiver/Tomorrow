using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ModelController : MonoBehaviour
{
    private SpriteRenderer _renderer;
    public Animator AnimController;
    public List<Sprite> Sprites;
    
    private const int DOWN = 0;
    private const int RIGHT = 1;
    private const int UP = 2;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        AnimController = GetComponent<Animator>();
        _renderer.sprite = Sprites[DOWN];
    }

    public void FaceLeft()
    {
        _renderer.flipX = true;
        _renderer.sprite = Sprites[RIGHT];
    }

    public void FaceRight()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[RIGHT];
    }

    public void FaceUp()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[UP];
    }

    public void FaceDown()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[DOWN];
    }

    public void Hop()
    {
        if (AnimController == null) return;
        int id = Animator.StringToHash("Hop");
        AnimController.SetTrigger(id);
    }
}
