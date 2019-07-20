using UnityEngine;

[System.Serializable]
public class DamageType
{
    [SerializeField]public string id;
    [SerializeField]public int damage;

    [SerializeField]public AnimationSet animationSet;
    [SerializeField]public bool requiresBothHands;

    public override string ToString()
    {
        return id + ", animation set:" + animationSet + ", d" + damage + ", requires both hands: " + requiresBothHands;
    }
}