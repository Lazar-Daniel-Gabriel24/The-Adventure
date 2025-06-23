using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeybindSettings : MonoBehaviour
{
    public static KeybindSettings Instance { get; private set; }

    [System.Serializable]
    public class Keybind
    {
        public string actionName;
        public string defaultBinding; // ex: "<Keyboard>/space"
        [HideInInspector] public string currentBinding;
    }

    public List<Keybind> keybinds = new List<Keybind>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadKeybinds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool GetKeyDown(string actionName)
    {
        var binding = GetBindingForAction(actionName);
        if (string.IsNullOrEmpty(binding)) return false;

        var control = InputSystem.FindControl(binding);
        return control is ButtonControl button && button.wasPressedThisFrame;
    }

    public bool GetKey(string actionName)
    {
        var binding = GetBindingForAction(actionName);
        if (string.IsNullOrEmpty(binding)) return false;

        var control = InputSystem.FindControl(binding);
        return control is ButtonControl button && button.isPressed;
    }

    public bool GetKeyUp(string actionName)
    {
        var binding = GetBindingForAction(actionName);
        if (string.IsNullOrEmpty(binding)) return false;

        var control = InputSystem.FindControl(binding);
        return control is ButtonControl button && button.wasReleasedThisFrame;
    }

    public bool SetKeybind(string actionName, InputControl control)
    {
        foreach (var bind in keybinds)
        {
            if (bind.actionName != actionName && bind.currentBinding == control.path)
            {
                Debug.LogWarning($"Tasta {control.displayName} este deja folosită pentru {bind.actionName}!");
                return false;
            }
        }

        foreach (var bind in keybinds)
        {
            if (bind.actionName == actionName)
            {
                bind.currentBinding = control.path;
                SaveKeybinds();
                return true;
            }
        }

        return false;
    }

    public string GetBindingForAction(string actionName)
    {
        foreach (var bind in keybinds)
        {
            if (bind.actionName == actionName)
                return bind.currentBinding;
        }
        return string.Empty;
    }

    private void SaveKeybinds()
    {
        foreach (var bind in keybinds)
        {
            PlayerPrefs.SetString("Keybind_" + bind.actionName, bind.currentBinding);
        }
        PlayerPrefs.Save();
    }

    private void LoadKeybinds()
    {
        foreach (var bind in keybinds)
        {
            string saved = PlayerPrefs.GetString("Keybind_" + bind.actionName, bind.defaultBinding);
            bind.currentBinding = saved;
        }

        // Elimină duplicatele la încărcare
        HashSet<string> usedBindings = new HashSet<string>();

        foreach (var bind in keybinds)
        {
            if (string.IsNullOrEmpty(bind.currentBinding)) continue;

            if (usedBindings.Contains(bind.currentBinding))
            {
                Debug.LogWarning($"Binding duplicat detectat la inițializare: {bind.actionName} folosește deja {bind.currentBinding}. Golește-l temporar.");
                bind.currentBinding = string.Empty;
            }
            else
            {
                usedBindings.Add(bind.currentBinding);
            }
        }
    }
}
