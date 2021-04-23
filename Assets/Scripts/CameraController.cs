using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera[] cameras;
    public int currentCamera = 0;

    public void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SwitchCamera();
        }

    }

    /// <summary>
    /// Switch to the next camera in the array.
    /// </summary>
    public void SwitchCamera()
    {

        cameras[currentCamera].gameObject.SetActive(false);

        if (currentCamera == cameras.Length - 1)
            currentCamera = 0;
        else
            currentCamera++;

        cameras[currentCamera].gameObject.SetActive(true);

    }

}
