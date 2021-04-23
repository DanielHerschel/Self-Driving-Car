using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;
using Random = UnityEngine.Random;

public class GeneticAlgManager : MonoBehaviour
{

    [Header("Refrences")]
    public CarController controller;

    [Header("Genetic Controls")]
    public int initialPopulation = 10;
    [Range(0.0f, 1.0f)] public float mutationRate = 0.055f;
    [Range(0.0f, 1.0f)] public float mutationAmountMultiplier = 1f / 7f; // This variable represents the 
    // max percentage of weights we want to mutate.

    [Header("Crossover Controls")]
    public int bestAgentSelection = 8; // How many of the best agents we want to select.
    public int worstAgentSelection = 3; // How many of the worst agents we want to select.
    public int numberToCrossover;

    private List<int> genePool = new List<int>(); // Pool of indexes of the nets we want to crossover.
    private int naturallySelected; // This variable represents the amount of agents we want to 
    // crossover naturally in each generation. The rest of the population will be generated randomly every generation.
    private NeuralNet[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;
    public float bestOfPopulation = 0;
    public float totalAverageFitness = 0;
    private float sumOfFitnesses = 0;

    private void Start()
    {

        CreatePopulation();

    }

    private void CreatePopulation()
    {

        population = new NeuralNet[initialPopulation];
        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();

    }

    private void FillPopulationWithRandomValues(NeuralNet[] newPopulation, int startingIndex)
    {

        while (startingIndex < initialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNet();
            newPopulation[startingIndex].InitializeNN(controller.LAYERS, controller.NEURONS);
            startingIndex++;
        }

    }

    private void ResetToCurrentGenome()
    {

        controller.ResetCarWithNetwork(population[currentGenome]);

    }

    public void Death(float fitness, NeuralNet network)
    {

        if (currentGenome < population.Length - 1)
        {
            population[currentGenome].fitness = fitness;
            population[currentGenome] = network;
            currentGenome++;
            ResetToCurrentGenome();
        } else
        {
            Repopulate();
        }

    }

    private void Repopulate()
    {

        genePool.Clear();
        currentGeneration++;
        naturallySelected = 0;
        SortPopulation();
        bestOfPopulation = population[0].fitness;
        sumOfFitnesses += bestOfPopulation;
        totalAverageFitness = sumOfFitnesses / currentGeneration;

        NeuralNet[] newPopulation = PickPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;
        currentGenome = 0;
        ResetToCurrentGenome();

    }

    private NeuralNet[] PickPopulation()
    {

        NeuralNet[] newPopulation = new NeuralNet[initialPopulation];

        // Pick the best population.
        for (int i = 0; i < bestAgentSelection; i++)
        {
            newPopulation[naturallySelected] = population[i].CopyNeuralNet(controller.LAYERS, controller.NEURONS);
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;

            int timesToAddToGenePool = Mathf.RoundToInt(population[i].fitness * 10);
            for (int j = 0; j < timesToAddToGenePool; j++)
            {
                genePool.Add(i);
            }

        }

        // Pick the worst population.
        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1;
            last -= i;

            int timesToAddToGenePool = Mathf.RoundToInt(population[last].fitness * 10);
            for (int j = 0; j < timesToAddToGenePool; j++)
            {
                genePool.Add(last);
            }

        }

        return newPopulation;

    }

    private void Crossover(NeuralNet[] newPopulation)
    {
        if (genePool.Count > 1)
        {
            for (int i = naturallySelected; i < numberToCrossover; i += 2)
            {
                int AIndex = 1; // Index of the first parent.
                int BIndex = 2; // Index of the second parent.
                int count = 0;
                do
                {
                    AIndex = genePool[Random.Range(0, genePool.Count - 1)];
                    BIndex = genePool[Random.Range(0, genePool.Count - 1)];
                    count++;
                } while (AIndex == BIndex && count < 100); // Get 2 random indexes until they are different or until
                // iterated 100 times (to prevent infinite iterations).

                // Create the childs and make the crossover witht the parents.
                NeuralNet child1 = new NeuralNet(), child2 = new NeuralNet();
                child1.InitializeNN(controller.LAYERS, controller.NEURONS);
                child2.InitializeNN(controller.LAYERS, controller.NEURONS);
                child1.fitness = 0;
                child2.fitness = 0;

                // Crossover the weights.
                for (int weightIndex = 0; weightIndex < child1.weights.Count; weightIndex++)
                {
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        child1.weights[weightIndex] = population[AIndex].weights[weightIndex];
                        child2.weights[weightIndex] = population[BIndex].weights[weightIndex];
                    } else
                    {
                        child1.weights[weightIndex] = population[BIndex].weights[weightIndex];
                        child2.weights[weightIndex] = population[AIndex].weights[weightIndex];
                    }
                }

                // Crossover the biases.
                for (int biasIndex = 0; biasIndex < child1.biases.Count; biasIndex++)
                {
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        child1.biases[biasIndex] = population[AIndex].biases[biasIndex];
                        child2.biases[biasIndex] = population[BIndex].biases[biasIndex];
                    }
                    else
                    {
                        child1.biases[biasIndex] = population[BIndex].biases[biasIndex];
                        child2.biases[biasIndex] = population[AIndex].biases[biasIndex];
                    }
                }

                // Assign the new children to the population.
                newPopulation[i] = child1;
                newPopulation[i+ 1] = child2;

            }
        }

        naturallySelected = numberToCrossover;

    }

    private void Mutate(NeuralNet[] newPopulation)
    {

        // Loop through the new children.
        for (int i = 0; i < naturallySelected; i++)
        {
            // For each mutation matrix have a chance to mutate it.
            for (int j = 0; j < newPopulation[i].weights.Count; j++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[j] = MutateMatrix(newPopulation[i].weights[j]);
                }
            }
        }

    }

    private Matrix<float> MutateMatrix(Matrix<float> matrixToMutate)
    {

        int amountToMutate = Random.Range(1, (int)((matrixToMutate.RowCount * matrixToMutate.ColumnCount) * mutationAmountMultiplier));

        Matrix<float> newWeights = Matrix<float>.Build.DenseOfMatrix(matrixToMutate);

        for (int i = 0; i < amountToMutate; i++)
        {
            int randomRow = Random.Range(0, newWeights.RowCount - 1);
            int randomColumn = Random.Range(0, newWeights.ColumnCount - 1);

            newWeights[randomRow, randomColumn] = Mathf.Clamp(newWeights[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return newWeights;

    }

    /// <summary>
    /// Use Array.Sort() (quick sort) to sort the population.
    /// </summary>
    private void SortPopulation()
    {

        //Array.Sort(population, new Comparison<NeuralNet>((c, o) => c.CompareTo(o)));

        // TODO: different sort of bubblesort.

        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    NeuralNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }

    }

}
