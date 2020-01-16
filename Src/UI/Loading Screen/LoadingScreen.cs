using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen getInstance { get; private set; }

    [SerializeField]GameObject _loadingScreen;
    [SerializeField]Image _background;
    [SerializeField]Image _bar;

    [SerializeField]Text _tip;
    [SerializeField]Text _barText;

    [SerializeField]string[] _tips = new string[] 
    { 
        "Running is ...almost always a valid option!",
        "Customize your characters with different weaponry in the armory menu.",
    };


    void Awake()
    {
        if (getInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _loadingScreen.SetActive(false);

        DontDestroyOnLoad(this);

        GlobalEvents.Subscribe(GlobalEvent.OpenLoadingScreen, (object[] args) => FadeIn());
        GlobalEvents.Subscribe(GlobalEvent.SetLoadingBarText, (object[] args) => _barText.text = (string)args[0]);
        GlobalEvents.Subscribe(GlobalEvent.SetLoadingBarProgress, (object[] args) => _bar.fillAmount = (float)args[0]);
        GlobalEvents.Subscribe(GlobalEvent.CloseLoadingScreen, (object[] args) => FadeOut());
    }

    void FadeIn()
    {
        _loadingScreen.SetActive(true);
        _tip.text = _tips.Random();
    }
    void FadeOut()
    {
        _loadingScreen.SetActive(false);
    }
}
