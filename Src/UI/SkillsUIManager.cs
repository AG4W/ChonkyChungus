using UnityEngine;
using UnityEngine.UI;

public class SkillsUIManager : TabBehaviour
{
    [SerializeField]Transform _list;
    [SerializeField]GameObject _listItem;

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        GlobalEvents.Subscribe(GlobalEvent.ActorSpellbookChanged, UpdateSpellbook);
    }

    void UpdateSpellbook(object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        for (int i = 0; i < _list.childCount; i++)
            Destroy(_list.GetChild(i).gameObject);

        for (int i = 0; i < Player.data.spellbook.Count; i++)
        {
            int a = i;

            Spell s = Player.data.spellbook[a];
            GameObject g = Instantiate(_listItem, _list);

            g.transform.Find("name").GetComponent<Text>().text = s.name;
            g.transform.Find("description").GetComponent<Text>().text = s.description;
            g.GetComponent<GenericPointerHandler>().Initialize(
                () => Tooltip.Open(s.ToTooltip()),
                null,
                null,
                null,
                Tooltip.Close);
        }
    }
}
