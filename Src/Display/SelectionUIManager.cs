using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionUIManager : MonoBehaviour
{
    public static SelectionUIManager getInstance { get; private set; }

    [SerializeField]GameObject _characters;
    [SerializeField]Transform _characterList;

    [SerializeField]Transform _troopList;
    [SerializeField]GameObject _troopListItem;

    [SerializeField]Transform[] _spawnPoints;

    [SerializeField]GameObject _displayModel;

    [SerializeField]GameObject _toCustomization;
    [SerializeField]GameObject _autofill;
    [SerializeField]GameObject _embark;

    DisplayModelController[] _models;
    GameObject[] _troopListItems;

    void Awake()
    {
        getInstance = this;
        getInstance.gameObject.SetActive(false);

        _models = new DisplayModelController[Player.party.Length];
        _troopListItems = new GameObject[Player.party.Length];

        _characters.SetActive(false);

        for (int i = 0; i < Player.party.Length; i++)
        {
            int a = i;
        
            _models[i] = Instantiate(_displayModel, _spawnPoints[i].position, _spawnPoints[i].rotation, _spawnPoints[i]).GetComponent<DisplayModelController>();
            _models[i].gameObject.SetActive(false);

            _troopListItems[i] = Instantiate(_troopListItem, _troopList);
            _troopListItems[i].transform.Find("name").GetComponent<Text>().text = "EMPTY TROOP SLOT";
            _troopListItems[i].GetComponent<GenericPointerHandler>().Initialize(
                null,
                () => OpenCharacterList(a),
                null,
                () => UpdateModel(null, a),
                null);
        }

        _toCustomization.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("To customization!"),
            () =>
            {
                DisplayUIManager.getInstance.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                Tooltip.Close();

                HubCameraManager.getInstance.GoTo(HubCameraMode.Customization);
            },
            null,
            null,
            () => Tooltip.Close());
        _autofill.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("Autofill any open slots with available characters."),
            () => 
            {
                for (int i = 0; i < Player.party.Length; i++)
                {
                    if (Player.party[i] == null && Player.characters.Any(c => Player.party.IndexOf(c) == -1))
                        UpdateModel(Player.characters.First(c => Player.party.IndexOf(c) == -1), i);
                }

                Tooltip.Close();
            },
            null,
            null,
            () => Tooltip.Close());
        _embark.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("Embark on your quest!"),
            () => {
                GlobalEvents.Raise(GlobalEvent.OpenLoadingScreen);
                SceneManager.LoadScene("Map");
            },
            null,
            null,
            () => Tooltip.Close());
    }

    void OpenCharacterList(int index)
    {
        _characters.SetActive(true);

        for (int i = 0; i < _characterList.childCount; i++)
            Destroy(_characterList.GetChild(i).gameObject);

        for (int i = 0; i < Player.characters.Length; i++)
        {
            if (Player.party.IndexOf(Player.characters[i]) != -1)
                continue;
            else
            {
                int a = i;
                GameObject g = Instantiate(_troopListItem, _characterList);

                g.transform.Find("name").GetComponent<Text>().text = Player.characters[a].name;
                g.GetComponent<GenericPointerHandler>().Initialize(
                    null,
                    () => 
                    {
                        UpdateModel(Player.characters[a], index);

                        _characters.SetActive(false);
                    },
                    null,
                    null,
                    null);
            }
        }
    }

    void UpdateModel(ActorData data, int index)
    {
        if (data == null)
        {
            Player.party[index] = null;

            _models[index].gameObject.SetActive(false);
            _troopListItems[index].transform.Find("name").GetComponent<Text>().text = "EMPTY TROOP SLOT";
        }
        else
        {
            Player.party[index] = data;

            _models[index].gameObject.SetActive(true);
            _models[index].SetActorData(data);
            _models[index].GetComponentInChildren<Animator>().SetFloat("idle", int.Parse(_models[index].transform.parent.tag));
            _models[index].GetComponentInChildren<Animator>().SetFloat("random", Synched.Next(0, 2));
            _troopListItems[index].transform.Find("name").GetComponent<Text>().text = data.name;
        }
    }
}
