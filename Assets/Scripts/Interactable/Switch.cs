using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class Switch : Interactable
{

    [SerializeField] LightController remoteLight;

    bool used = false;

    
    public override void Interact()
    {
        if (!used)
        {
            remoteLight.toggleLight();
            used = true;
            DeFocous();
        }
    }

    public override void Focous()
    {
        if (!used)
        {
            highlight.intensity = highlightIntensity;
        }
    }


}
