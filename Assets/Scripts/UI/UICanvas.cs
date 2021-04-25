using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{

    [Header("Game Objects to Take Info From")]
    public CarController carController;
    public GeneticAlgManager geneticAlgManager;

    [Header("UI Info")]
    private bool leariningMode = true;

    // Genetic algorithm info
    public int currentGeneration;
    public int currentGenome;
    public int initialPopulation;
    public float bestOfPopulation;

    // Neural net info
    public int hiddenLayerCount;
    public int neuronsCount;
    public float currentGenomeFitness;

    [Header("UI Objects")]
    // Genetic algorithm text objects
    public Text currentGenerationText;
    public Text currentGenomeText, initialPopulationText, bestOfPopulationText;

    // Neural net text objects
    public Text hiddenLayerCountText, neuronsCountText, currentGenomeFitnessText;

    [Header("On The Fly Settings")]
    // On the fly settings
    float timeSpeed, fixedDeltaTime;
    public Text timeSpeedText;
    public Slider timeSpeedSlider;

    /// <summary>
    /// Called when the object is first loaded.
    /// </summary>
    private void Awake()
    {

        // Make a copy of the fixedDeltaTime, it defaults to 0.02f.
        this.fixedDeltaTime = Time.fixedDeltaTime;

    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {

        // Enter the values.
        GetInfoFromObjects();

        // Show them on the screen.
        ShowInfoOnScreen();

        // On the fly settings change.
        OnTheFlySettings();

    }

    /// <summary>
    /// Change the seetings that you can control on the fly.
    /// </summary>
    private void OnTheFlySettings()
    {

        timeSpeed = timeSpeedSlider.value;

        timeSpeedText.text = "Time Scale: x" + timeSpeed;

        Time.timeScale = timeSpeed;

    }

    /// <summary>
    /// Get the info from the objects to display on the UI.
    /// </summary>
    private void GetInfoFromObjects()
    {

        if (leariningMode)
        {
            // Get the genetic algorithm info
            currentGeneration = geneticAlgManager.currentGeneration;
            currentGenome = geneticAlgManager.currentGenome;
            initialPopulation = geneticAlgManager.initialPopulation;
            bestOfPopulation = geneticAlgManager.bestOfPopulation;
        }

        // Get the neural network info
        hiddenLayerCount = carController.LAYERS;
        neuronsCount = carController.NEURONS;
        currentGenomeFitness = carController.overallFitness;

    }

    /// <summary>
    /// Show the info on the screen. Enter the info in each UI object.
    /// </summary>
    private void ShowInfoOnScreen()
    {

        // Enter the info in the genetic algorithm section.
        currentGenerationText.text = "Current Generatin: " + currentGeneration;
        currentGenomeText.text = "Current Genome: " + currentGenome;
        initialPopulationText.text = "Initial Population: " + initialPopulation;
        bestOfPopulationText.text = "Best of Population: " + bestOfPopulation;

        // Enter the info in the neural network section.
        hiddenLayerCountText.text = "Hidden Layer Count: " + hiddenLayerCount;
        neuronsCountText.text = "Neurons Per Hidden Layer Count: " + neuronsCount;
        currentGenomeFitnessText.text = "Current Genome Fitness: " + currentGenomeFitness;

    }

}
