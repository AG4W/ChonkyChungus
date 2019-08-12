using UnityEngine;

using System;

public class Interaction
{
    Func<bool> _condition;

    public string title { get; private set; }
    public string description { get; private set; }

    public Sprite icon { get; private set; }

    public bool EvaluateCondition()
    {
        return _condition();
    }
}
