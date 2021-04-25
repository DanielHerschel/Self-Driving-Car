using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SettingsData : MonoBehaviour
{

    [Header("Menu Settings")]
    public bool learningMode = false; // False is testing mode.

    // Car controller settings
    public int hiddenLayerCount;
    public int hiddenLayerNeuronCount;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    // Genetic algorithm settings
    public int initialPopulation = 10;
    [Range(0.0f, 1.0f)] public float mutationRate = 0.055f;

    // Test mode
    public NeuralNet testModeLoadedNeuralNet;
    public UICanvas uic;

    [Header("Simulation Settings")]
    private GeneticAlgManager geneticAlgManager;
    private CarController carController;
    public bool setup = false; // Will be true after setup is complete.


    private void Awake()
    {

        DontDestroyOnLoad(this);

    }

    private void FixedUpdate()
    {
        
        if (SceneManager.GetActiveScene().name == "MainMenu") // Things to do in the main menu.
        {
            MainMenuSetup();
        }
        else // Things to do outside the main menu.
        {
            if (!setup)
                if (learningMode)
                    LearnModeSetup();
                else
                    TestModeSetup();
        }

    }

    /// <summary>
    /// Things to handle in the main menu.
    /// </summary>
    private void MainMenuSetup()
    {

        setup = false;

    }

    /// <summary>
    /// Things to handle in the learn mode.
    /// </summary>
    private void LearnModeSetup()
    {

        carController = GameObject.Find("Car").GetComponent<CarController>();
        geneticAlgManager = GameObject.Find("GeneticManager").GetComponent<GeneticAlgManager>();

        // Apply settings.
        ApplyCarSettings();
        ApplyGeneticAlgManagerSettings();

        setup = true;

    }

    /// <summary>
    /// Apply car settings.
    /// </summary>
    public void ApplyCarSettings()
    {

        carController.learningMode = true;
        carController.LAYERS = hiddenLayerCount;
        carController.NEURONS = hiddenLayerNeuronCount;
        carController.distanceMultiplier = distanceMultiplier;
        carController.avgSpeedMultiplier = avgSpeedMultiplier;
        carController.sensorMultiplier = sensorMultiplier;

    }

    /// <summary>
    /// Apply genetic algorithm settings.
    /// </summary>
    public void ApplyGeneticAlgManagerSettings()
    {

        geneticAlgManager.mutationRate = mutationRate;

    }

    /// <summary>
    /// Things to handle in the test mode.
    /// </summary>
    private void TestModeSetup()
    {

        setup = true;

        carController = GameObject.Find("Car").GetComponent<CarController>();
        geneticAlgManager = GameObject.Find("GeneticManager").GetComponent<GeneticAlgManager>();

        geneticAlgManager.gameObject.SetActive(false);

        carController.learningMode = false;

        // TEMP --->
        // testModeLoadedNeuralNet = new NeuralNet();
        // testModeLoadedNeuralNet.InitializeNN(carController.LAYERS, carController.NEURONS);
        // -------->

        testModeLoadedNeuralNet.LoadWeights();

        NeuralNet net = new NeuralNet();
        net.InitializeNN(carController.LAYERS, carController.NEURONS);
        net.weights = testModeLoadedNeuralNet.weights;
        net.biases = testModeLoadedNeuralNet.biases;

        carController.ResetCarWithNetwork(net);  

    }

}
