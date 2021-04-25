using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTwoObjects : MonoBehaviour
{

    public GameObject option1, option2;
    private Toggle toggle;

    void Start()
    {

        toggle = GetComponent<Toggle>();

    }

    void Update()
    {

        if (toggle.isOn)
        {
            option1.SetActive(true);
            option2.SetActive(false);
        } else
        {
            option1.SetActive(false);
            option2.SetActive(true);
        }    

    }
}
