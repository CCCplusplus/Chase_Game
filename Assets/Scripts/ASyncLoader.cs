using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;



public class ASyncLoader : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cSettings;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;


    private float timer = 5;
    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu.SetActive(false);
        cSettings.SetActive(false);
        loadingScreen.SetActive(true);
        //empezar la operacion Async
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;


        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            if (loadOperation.progress >= 0.9f && timer <=0)
            {
                loadOperation.allowSceneActivation = true;
            }
            timer -= Time.deltaTime;
            yield return null;

        }


    }
}
