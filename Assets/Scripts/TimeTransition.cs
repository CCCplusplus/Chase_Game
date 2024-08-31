using Mirror;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTransition : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI errortexto;
    private void Start()
    {
        errortexto.text = PlayerPrefs.GetString("Error");
        LogOut();
    }
    public async void LogOut()
    {
        if (NetworkManager.singleton != null)
        {
            if (NetworkManager.singleton.isNetworkActive)
            {
                if (NetworkServer.active && NetworkClient.isConnected)
                    NetworkManager.singleton.StopHost();
                else if (NetworkClient.isConnected)
                    NetworkManager.singleton.StopClient();
            }
        }

        
        if (NetworkManager.singleton != null)
            Destroy(NetworkManager.singleton.gameObject);


        await Task.Delay(10000);
        SceneManager.LoadSceneAsync("Main Menu");

        PlayerPrefs.SetString("Error", "");
    }
}
