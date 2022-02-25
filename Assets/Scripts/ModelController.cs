using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class ModelController : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Animator _animator;
    public List<Sprite> Sprites;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _renderer.sprite = Sprites[0];
    }

    public void FaceLeft()
    {
        _renderer.flipX = true;
        _renderer.sprite = Sprites[1];
    }

    public void FaceRight()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[1];
    }

    public void FaceUp()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[2];
    }

    public void FaceDown()
    {
        _renderer.flipX = false;
        _renderer.sprite = Sprites[0];
    }

    public void Hop()
    {
        int id = Animator.StringToHash("Hop");
        _animator.SetTrigger(id);
    }
}
