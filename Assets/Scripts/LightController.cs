using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class LightController : MonoBehaviour
{
    // Start is called before the first frame update
    float lightIntensity;
    Light2D light;
    bool lightOn;


    void Awake()
    {
        light = GetComponent<Light2D>();
    }
    void Start()
    {
        lightIntensity = light.intensity;
        light.intensity = 0;
    }

    public void toggleLight()
    {
        if (lightOn)
        {
            light.intensity = 0;
            lightOn = false;
        }
        else
        {
            light.intensity = lightIntensity;
            lightOn = true;
        }
    }
}
