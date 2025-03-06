using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ToggleScore : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            ToggleUIScore();
    }

    private void ToggleUIScore()
    {
        _canvas.enabled = !_canvas.enabled;
    }
}
