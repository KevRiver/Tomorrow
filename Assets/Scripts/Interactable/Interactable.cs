using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Light2D highlight;

    [SerializeField] protected float highlightIntensity;


    protected virtual void Awake()
    {
        highlight = GetComponent<Light2D>();
    }

    protected virtual void Start()
    {
        highlight.intensity = 0;
    }

    public abstract void Interact();
    public virtual void Focous()
    {
        highlight.intensity = highlightIntensity;
    }

    public virtual void DeFocous()
    {
        highlight.intensity = 0;
    }

}
