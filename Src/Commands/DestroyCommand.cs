using UnityEngine;

public class DestroyCommand : Command
{
    GameObject _root;

    public DestroyCommand(GameObject root)
    {
        _root = root;
    }

    public override void Execute()
    {
        Object.Destroy(_root);
    }
}