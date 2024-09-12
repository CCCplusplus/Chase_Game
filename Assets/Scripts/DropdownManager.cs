using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownManager : MonoBehaviour
{
    public TMPro.TMP_Dropdown HostClientDropdown;
    public GameObject text;
    public int indexToActivate = 1;

    void Start()
    {
        text.SetActive(false);

        HostClientDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(HostClientDropdown);
        });
    }

    void DropdownValueChanged(TMPro.TMP_Dropdown dropdown)
    {
        if (dropdown.value == indexToActivate)
        {
            text.SetActive(true);
        }
        else
        {
            text.SetActive(false);
        }
    }
}
