using UnityEngine;

public class LightSourceEntity : MonoBehaviour
{
    Light[] _lights;
    LightFlickerEffect[] _effects;

    public void Initialize(LightSource source)
    {
        source.OnTicked += UpdateLightVisuals;

        _lights = this.GetComponentsInChildren<Light>();
        _effects = this.GetComponentsInChildren<LightFlickerEffect>();
    
        UpdateLightVisuals(source);
    }
    void UpdateLightVisuals(LightSource source)
    {
        for (int i = 0; i < _lights.Length; i++)
            _lights[i].range = source.range;
        for (int i = 0; i < _effects.Length; i++)
            _effects[i].ModifyBaseIntensity(source.EvaluateVisualLuminosity());
    }
}
