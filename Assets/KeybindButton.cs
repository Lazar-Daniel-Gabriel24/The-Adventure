using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeybindButton : MonoBehaviour
{
    public string actionName;
    public TextMeshProUGUI keyText;
    private Button button;
    private KeybindSettings keybindSettings;
    private bool isListening = false;

    private void Start()
    {
        button = GetComponent<Button>();
        keybindSettings = KeybindSettings.Instance;

        if (keyText == null)
            keyText = GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(StartListening);
        UpdateUI();
    }

    private void Update()
    {
        if (!isListening) return;

        // Tastatură
        foreach (KeyControl keyControl in Keyboard.current.allKeys)
        {
            if (keyControl.wasPressedThisFrame)
            {
                ApplyBinding(keyControl);
                return;
            }
        }

        // Mouse
        if (Mouse.current.leftButton.wasPressedThisFrame) ApplyBinding(Mouse.current.leftButton);
        else if (Mouse.current.rightButton.wasPressedThisFrame) ApplyBinding(Mouse.current.rightButton);
        else if (Mouse.current.middleButton.wasPressedThisFrame) ApplyBinding(Mouse.current.middleButton);
    }

    private void ApplyBinding(InputControl control)
    {
        bool success = keybindSettings.SetKeybind(actionName, control);
        isListening = false;

        if (!success)
        {
            keyText.text = "Tastă ocupată!";
        }
        else
        {
            UpdateUI();
        }
    }

    private void StartListening()
    {
        keyText.text = "...";
        isListening = true;
    }

    private void UpdateUI()
    {
        string bindingPath = keybindSettings.GetBindingForAction(actionName);
        var control = InputSystem.FindControl(bindingPath);
        keyText.text = control != null ? control.displayName : "Setează!";
    }
}
