using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuNavigation : MonoBehaviour
{
    public Selectable[] selectables; // Usar Selectable en lugar de Button para incluir Dropdowns
    private int currentIndex = 0;
    private bool canNavigate = true;
    private TMP_Dropdown activeDropdown;
    private bool isSubmitPressed = false;

    private void Start()
    {
        SelectButton();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!canNavigate) return;

        Vector2 navigation = context.ReadValue<Vector2>();

        if (activeDropdown == null)
        {
            if (navigation.y > 0)
            {
                currentIndex--;
                if (currentIndex < 0) currentIndex = selectables.Length - 1;
                StartCoroutine(ResetNavigation());
            }
            else if (navigation.y < 0)
            {
                currentIndex++;
                if (currentIndex >= selectables.Length) currentIndex = 0;
                StartCoroutine(ResetNavigation());
            }

            SelectButton();
        }
        else
        {
            if (navigation.y > 0)
            {
                activeDropdown.value = Mathf.Max(activeDropdown.value - 1, 0);
            }
            else if (navigation.y < 0)
            {
                activeDropdown.value = Mathf.Min(activeDropdown.value + 1, activeDropdown.options.Count - 1);
            }
            activeDropdown.RefreshShownValue();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed && !isSubmitPressed)
        {
            isSubmitPressed = true;
            Selectable currentSelectable = selectables[currentIndex];

            if (currentSelectable is Button)
            {
                ((Button)currentSelectable).onClick.Invoke();
            }
            else if (currentSelectable is TMP_Dropdown)
            {
                TMP_Dropdown dropdown = (TMP_Dropdown)currentSelectable;
                if (activeDropdown == null)
                {
                    dropdown.Show();
                    activeDropdown = dropdown;
                }
                else
                {
                    dropdown.Hide();
                    activeDropdown = null;
                }
            }
            else if (currentSelectable is Toggle)
            {
                Toggle toggle = (Toggle)currentSelectable;
                toggle.isOn = !toggle.isOn;
            }

            StartCoroutine(ResetSubmit());
        }
    }

    private IEnumerator ResetSubmit()
    {
        yield return new WaitForSeconds(0.2f);
        isSubmitPressed = false;
    }

    private void SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
    }

    private IEnumerator ResetNavigation()
    {
        canNavigate = false;
        yield return new WaitForSeconds(0.2f);
        canNavigate = true;
    }

    private void Update()
    {
        // Check if dropdown is collapsed to reset its state
        if (activeDropdown != null && activeDropdown.transform.Find("Dropdown List") == null)
        {
            activeDropdown = null;
        }
    }
}
