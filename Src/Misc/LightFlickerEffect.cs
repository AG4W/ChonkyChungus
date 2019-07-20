using UnityEngine;

using System.Collections.Generic;

public class LightFlickerEffect : MonoBehaviour
{
    float _baseIntensity;

    [Tooltip("Lower = sparks, higher = Lantern")][Range(1, 50)][SerializeField]int _smoothing = 5;
    [Range(-99f, 0f)][SerializeField]float _maxNegativeChange = -.125f;
    [Range(0f, 99f)][SerializeField]float _maxPositiveChange = .125f;

    float _lastSum = 0;
    Queue<float> _smoothQueue;

    [SerializeField]Light _light;

    void Start()
    {
        _smoothQueue = new Queue<float>(_smoothing);

        if (_light == null)
            _light = GetComponent<Light>();

        _baseIntensity = _light.intensity;
    }
    void Update()
    {
        if (_light == null)
            return;
    
        // pop off an item if too big
        while (_smoothQueue.Count >= _smoothing)
            _lastSum -= _smoothQueue.Dequeue();

        // Generate random new item, calculate new average
        float nv = Random.Range(0, 2) == 1 ? Random.Range(_maxNegativeChange, 0) : Random.Range(0, _maxPositiveChange);
        nv = _baseIntensity + nv;

        _smoothQueue.Enqueue(nv);
        _lastSum += nv;

        // Calculate new smoothed average
        _light.intensity = _lastSum / (float)_smoothQueue.Count;
    }

    public void ModifyBaseIntensity(float modifier)
    {
        _baseIntensity *= modifier;
        _maxNegativeChange *= modifier;
        _maxPositiveChange *= modifier;
    }
}