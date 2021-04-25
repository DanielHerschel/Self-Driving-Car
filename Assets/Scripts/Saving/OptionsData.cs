using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsData
{

    public int qualityIndex;

    public OptionsData(MainMenuUIController mc)
    {

        qualityIndex = mc.qualityIndex;

    }

    public OptionsData()
    {

        qualityIndex = 2;

    }

}
