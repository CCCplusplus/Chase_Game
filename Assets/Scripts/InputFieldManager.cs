using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InputFieldManager : MonoBehaviour
{
    public PlayerInput playerInput; // Reference to the PlayerInput component

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        // Listen for events when the InputField is selected or deselected
        inputField.onSelect.AddListener(OnInputFieldSelected);
        inputField.onDeselect.AddListener(OnInputFieldDeselected);
    }

    private void OnInputFieldSelected(string text)
    {
        // Disable the PlayerInput when the InputField is selected
        playerInput.enabled = false;
    }

    private void OnInputFieldDeselected(string text)
    {
        // Re-enable the PlayerInput when the InputField is deselected
        playerInput.enabled = true;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        inputField.onSelect.RemoveListener(OnInputFieldSelected);
        inputField.onDeselect.RemoveListener(OnInputFieldDeselected);
    }
}
