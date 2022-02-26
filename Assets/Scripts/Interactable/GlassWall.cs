using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class GlassWall : Interactable
{
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    public override void Focus()
    {
        highlight.intensity = highlightIntensity;
    }

    public void Appear()
    {
        spriteRenderer.enabled = true;
    }

    public void Disappear()
    {
        spriteRenderer.enabled = false;
    }
}
