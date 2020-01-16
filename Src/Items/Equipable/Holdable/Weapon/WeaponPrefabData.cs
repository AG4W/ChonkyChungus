using UnityEngine;

[System.Serializable]
public class WeaponPrefabData
{
    [SerializeField]GameObject _prefab;
    [SerializeField]AnimationSet[] _animationSet;

    [SerializeField]bool _requiresBothHands;
    [SerializeField]bool _useLeftHandIK;

    public GameObject prefab { get { return _prefab; } }
    public AnimationSet[] animationSet { get { return _animationSet; } }

    public bool requiresBothHands { get { return _requiresBothHands; } }
    public bool useLeftHandIK { get { return _useLeftHandIK; } }
}
