using UnityEngine;

using System.Linq;

[CreateAssetMenu(menuName = "Vital Modifier Collection")]
public class VitalModifierCollection : ScriptableObject
{
    [SerializeField]VitalModifierCollectionEntry[] _entries;

    public VitalModifierCollectionEntry Get(string suffix)
    {
        return _entries.FirstOrDefault(vmce => vmce.suffix == suffix);
    }
    public VitalModifierCollectionEntry GetRandom()
    {
        return _entries[Synched.Next(0, _entries.Length)];
    }
}
[System.Serializable]
public class VitalModifierCollectionEntry
{
    [SerializeField]string _suffix;
    [SerializeField]VitalModifier _modifiers;

    public string suffix { get { return _suffix; } }
    public VitalModifier modifiers { get { return _modifiers; } }
}
