using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{

    [Header("Main Menu Items")]
    public GameObject mainItems;
    public GameObject startItems;
    public GameObject settingsItems;

    [Header("Game Settings Data")]
    [Range(0.0f, 1.0f)]
    public int qualityIndex = 2;
    public Dropdown qualityDropdown;

    [Header("In Game Settings")]
    public SettingsData settingsData;
    public Toggle learningModeCheckBox;
    public InputField hiddenLayerCountInput, hiddenLayerNeuronCountInput;
    public Slider distanceMultiplierSlider, averageSpeedMultiplierSlider, sensorMultiplierSlider, mutationRateSlider;

    public Text savedModelStatusText;
    public bool canStart = false;

    /// <summary>
    /// Called before the first frame.
    /// </summary>
    void Start()
    {

        settingsData = GameObject.Find("SettingsManager").GetComponent<SettingsData>();

        try
        {
            OptionsData op = SaveSystem.LoadOptionsData();

            qualityIndex = op.qualityIndex;

            qualityDropdown.value = qualityIndex;

        }
        catch (UnityException) {   }

        Back();

    }
    
    /// <summary>
    /// Called every frame.
    /// </summary>
    void Update()
    {
        
    }

    // <--------------------------------> Start Settings Functions <-------------------------------->

    /// <summary>
    /// Save the settings and start the simulation.
    /// </summary>
    public void StartGame()
    {

        if (!canStart && !learningModeCheckBox.isOn)
            return;

        settingsData.learningMode = learningModeCheckBox.isOn;
        settingsData.hiddenLayerCount = int.Parse(hiddenLayerCountInput.text);
        settingsData.hiddenLayerNeuronCount = int.Parse(hiddenLayerNeuronCountInput.text);
        settingsData.distanceMultiplier = distanceMultiplierSlider.value;
        settingsData.avgSpeedMultiplier = averageSpeedMultiplierSlider.value;
        settingsData.sensorMultiplier = sensorMultiplierSlider.value;
        settingsData.mutationRate = mutationRateSlider.value;

        SceneManager.LoadScene("Map1");

    }

    /// <summary>
    /// Load model data from file.
    /// </summary>
    /// <param name="lm">true if in learning mode, false if not.</param>
    public void TestModeLoadingDataFromFile(bool lm)
    {

        if (lm)
            return;

        try
        {
            //ModelSaveData op = SaveSystem.LoadNeuralNetData();

            if(op == null)
            {
                canStart = false;
                savedModelStatusText.text = "Could not load saved model";
            } else
            {
                canStart = true;
                savedModelStatusText.text = "Saved model ready for testing";
            }

        }
        catch (UnityException) {   }

    }

    // <--------------------------------> Settings Functions <-------------------------------->

    /// <summary>
    /// Set the overall graphics quality of the game.
    /// </summary>
    /// <param name="qi">Quality level index</param>
    public void SetQuality(int qi)
    {

        qualityIndex = qi;
        QualitySettings.SetQualityLevel(qi);

    }

    /// <summary>
    /// Apply the graphics settings.
    /// </summary>
    public void Apply()
    {

        SaveSystem.SaveOptionsData(this);

        Back();

    }

    // <--------------------------------> Menu Functions <-------------------------------->

    /// <summary>
    /// Go to the startItems submenu.
    /// </summary>
    public void StartItems()
    {

        settingsItems.SetActive(false);
        mainItems.SetActive(false);

        startItems.SetActive(true);

    }

    /// <summary>
    /// Go to the settingsItems submenu.
    /// </summary>
    public void SettingsItems()
    {
        
        startItems.SetActive(false);
        mainItems.SetActive(false);

        settingsItems.SetActive(true);

    }

    /// <summary>
    /// Go back to the main menu from any submenu.
    /// </summary>
    public void Back()
    {

        startItems.SetActive(false);
        settingsItems.SetActive(false);
        mainItems.SetActive(true);

    }

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void Quit()
    {

        Application.Quit();

    }

}
