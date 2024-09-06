using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSource;

    public GoalTrigger goal;

    private ChaserTrigger chaserTrigger;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        chaserTrigger = Object.FindObjectOfType<ChaserTrigger>();
    }

    private void Update()
    {
        if (goal == null && chaserTrigger == null) return;

        if (goal.isover ^ chaserTrigger.isover)
            musicSource.Stop();
    }
}
