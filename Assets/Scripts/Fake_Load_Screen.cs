using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake_Load_Screen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject loadScreen;
    private float timer = 5;

    void Start()
    {

    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            loadScreen.SetActive(false);
        }
    }

}
