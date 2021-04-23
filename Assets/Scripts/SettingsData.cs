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

    [Header("Simulation Settings")]
    private GeneticAlgManager geneticAlgManager;
    private CarController carController;
    private bool testModeSetup = false; // Will be true after setup is complete.


    private void Awake()
    {

        DontDestroyOnLoad(this);

    }

    private void FixedUpdate()
    {
        
        // Things to do in the main menu.
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            MainMenuHandling();
            return;
        }

        // Things to do outside the main menu.
        if (!testModeSetup && !learningMode)
            TestModeSetup();

    }

    /// <summary>
    /// Things to handle in the main menu.
    /// </summary>
    private void MainMenuHandling()
    {

    }

    /// <summary>
    /// Things to handle in the test mode.
    /// </summary>
    private void TestModeSetup()
    {

        carController = GameObject.Find("Car").GetComponent<CarController>();
        geneticAlgManager = GameObject.Find("GeneticManager").GetComponent<GeneticAlgManager>();

        geneticAlgManager.gameObject.SetActive(false);

        carController.learningMode = false;

        // TEMP --->
        testModeLoadedNeuralNet = new NeuralNet();
        testModeLoadedNeuralNet.InitializeNN(carController.LAYERS, carController.NEURONS);
        // -------->

        carController.ResetCarWithNetwork(testModeLoadedNeuralNet);

        testModeSetup = true;

    }

}
