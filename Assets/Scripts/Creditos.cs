using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class Creditos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("WaitToEnd", 156);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
    public void ReturnRoMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void WaitToEnd()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
