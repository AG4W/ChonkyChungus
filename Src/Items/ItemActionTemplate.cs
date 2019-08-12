using UnityEngine;

[CreateAssetMenu(menuName = "Templates/Item Action")]
public class ItemActionTemplate : ScriptableObject
{
    [SerializeField]string _header;
    [TextArea(3, 10)][SerializeField]string _description;

    [SerializeField]Sprite _icon;

    [TextArea(3, 10)][SerializeField]string _lua = 
        "--validate must always return a bool\n" +
        "function validate()\n" +
        "   return true\n" +
        "end\n\n" +
        "--execute should return a string upon failure\n" +
        "function execute(executee)\n" +
        "end";

    public ItemAction Instantiate()
    {
        return new ItemAction(_header, _description, _icon, _lua);
    }
}
