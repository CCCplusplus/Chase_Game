using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPlatform : MonoBehaviour
{
    private float rotZ;
    public float RotSpeed;
    public bool ClockRot;
    void Update()
    {
        if (ClockRot == false)
        {
            rotZ += Time.deltaTime * RotSpeed;
        }
        else
        {
            rotZ += -Time.deltaTime * RotSpeed;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
