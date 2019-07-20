using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;

public class GenericPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    Action _onEnter;
    Action _onLeftDown;
    Action _onScrollDown;
    Action _onRightDown;
    Action _onExit;

    [SerializeField]bool _playEnterExitSFX;
    [SerializeField]bool _playClickSFX;

    [SerializeField]bool _highlightOnHover = false;

    [SerializeField]Graphic[] _graphics;

    [SerializeField]Color _highlightColor = Color.white;
    Color[] _graphicColors;

    public void Initialize(Action onEnter = null, Action onLeftDown = null, Action onScrollDown = null, Action onRightDown = null, Action onExit = null)
    {
        _onEnter = onEnter;
        _onLeftDown = onLeftDown;
        _onScrollDown = onScrollDown;
        _onRightDown = onRightDown;
        _onExit = onExit;

        if (_highlightOnHover)
        {
            _graphicColors = new Color[_graphics.Length];

            for (int i = 0; i < _graphics.Length; i++)
                _graphicColors[i] = _graphics[i].color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_playEnterExitSFX)
            UIAudioManager.Play(UISoundType.EnterExit);

        if (_highlightOnHover)
            ApplyColor();

        _onEnter?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                _onLeftDown?.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                _onRightDown?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                _onScrollDown?.Invoke();
                break;
            default:
                break;
        }

        if (_playClickSFX)
            UIAudioManager.Play(UISoundType.Click);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_playEnterExitSFX)
            UIAudioManager.Play(UISoundType.EnterExit);
        if (_highlightOnHover)
            ResetColors();

        _onExit?.Invoke();
    }

    void ApplyColor()
    {
        for (int i = 0; i < _graphics.Length; i++)
            _graphics[i].color = _highlightColor;
    }
    void ResetColors()
    {
        for (int i = 0; i < _graphics.Length; i++)
            _graphics[i].color = _graphicColors[i];
    }
}
