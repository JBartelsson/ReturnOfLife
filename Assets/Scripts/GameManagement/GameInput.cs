using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    private PlayerInputActions playerInputActions;
    

    public enum Binding
    {
        Interact, Cancel
    }

    public static GameInput Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.GameInputs.Enable();

        playerInputActions.GameInputs.Interact.performed += Interact_Performed;
        playerInputActions.GameInputs.Cancel.performed += Cancel_performed;
        playerInputActions.GameInputs.Pause.performed += PauseOnperformed;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
    }

    private void PauseOnperformed(InputAction.CallbackContext obj)
    {
        EventManager.Game.Input.OnPause?.Invoke();
    }

    private void OnDestroy()
    {
        playerInputActions.GameInputs.Interact.performed -= Interact_Performed;
        playerInputActions.GameInputs.Cancel.performed -= Cancel_performed;
        playerInputActions.GameInputs.Pause.performed -= PauseOnperformed;


        playerInputActions.Dispose();
    }
    private void Cancel_performed(InputAction.CallbackContext obj)
    {
        EventManager.Game.Input.OnCancel?.Invoke();
    }

    private void Interact_Performed(InputAction.CallbackContext obj)
    {
        EventManager.Game.Input.OnInteract?.Invoke();

    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Interact:
                return playerInputActions.GameInputs.Interact.bindings[0].ToDisplayString();
            case Binding.Cancel:
                return playerInputActions.GameInputs.Cancel.bindings[0].ToDisplayString();
        }
        return "";
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.Interact:
                inputAction = playerInputActions.GameInputs.Interact;
                bindingIndex = 0;
                break;
            case Binding.Cancel:
                inputAction = playerInputActions.GameInputs.Cancel;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Enable();
                onActionRebound();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}