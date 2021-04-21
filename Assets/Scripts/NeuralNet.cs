using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;

using Random = UnityEngine.Random;

public class NeuralNet : MonoBehaviour
{

    // Nodes
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 3);
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

    // Weights and Biases
    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();

    public float fitness;

    /// <summary>
    /// Compare between the fitness of this neural net and another.
    /// </summary>
    /// <param name="other">The other net we want to compare to.</param>
    /// <returns>A number that is positive if this is better than the other, 0 if their equal and negative otherwise.</returns>
    public int CompareTo(NeuralNet other)
    {
        if (other == null)
            return 1;
        else
            return fitness.CompareTo(other.fitness) > 0 ? 1 : (fitness.CompareTo(other.fitness) < 0 ? -1 : 0);
    }

    /// <summary>
    /// Initialize the neural net with random values.
    /// </summary>
    /// <param name="hiddenLayerCount">How many hidden layers we want.</param>
    /// <param name="hiddenNeuronCount">How many neurons we want in each hidden layer.</param>
    public void InitializeNN(int hiddenLayerCount, int hiddenNeuronCount)
    {

        // Clear all the matrices.
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Matrix<float> tempMatrix = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(tempMatrix);

            biases.Add(Random.Range(-1f, 1f));

            // Initialize the weights
            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(3, hiddenNeuronCount);
                weights.Add(inputToH1);
            }

            Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(hiddenToHidden);

        }

        Matrix<float> outputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        biases.Add(Random.Range(-1f, 1f));

        RandomizeWeights();

    }

    /// <summary>
    /// Create a new NeuralNet object that is a copy of the current one.
    /// </summary>
    /// <returns>Copy of the current NeuralNet object.</returns>
    public NeuralNet CopyNeuralNet(int hiddenLayerCount, int hiddenNeuronCount)
    {

        NeuralNet copy = new NeuralNet();
        List<Matrix<float>> newWeights = new List<Matrix<float>>();
        List<float> newBiases = new List<float>(biases);

        for (int i = 0; i < weights.Count; i ++)
        {
            Matrix<float> copyOfWeights = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);
            weights[i].CopyTo(copyOfWeights);
            newWeights.Add(copyOfWeights);
        }

        copy.weights = newWeights;
        copy.biases = newBiases;
        copy.InitializeHidden(hiddenLayerCount, hiddenNeuronCount);

        return copy;

    }

    private void InitializeHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {

        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();


        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Matrix<float> tempMatrix = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(tempMatrix);
        }

    }

    internal static Array CompareTo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Randomize the weights of the neural net.
    /// </summary>
    public void RandomizeWeights()
    {

        for (int i = 0; i < weights.Count; i++)
        {
            for (int row = 0; row < weights[i].RowCount; row++)
            {
                for (int col = 0; col < weights[i].ColumnCount; col++)
                {
                    weights[i][row, col] = Random.Range(-1f, 1f);
                }
            }
        }

    }

    /// <summary>
    /// Run the neural network and calculate the output values.
    /// </summary>
    /// <param name="forward">The value of the forward sensor.</param>
    /// <param name="right">The value of the right sensor.</param>
    /// <param name="left">The value of the left sensor.</param>
    /// <returns>Acceleration amount and Turn amount</returns>
    public (float, float) RunNN(float forward, float right, float left)
    {

        inputLayer[0, 0] = forward;
        inputLayer[0, 1] = right;
        inputLayer[0, 2] = left;

        // Normalization function
        inputLayer = inputLayer.PointwiseTanh(); // O(n)

        // Calculating the first layer of the hidden layers with the input layer.
        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh(); // O(n^2)

        // Calculating all the hidden layers.
        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        } // O(n^3)

        // Calculating the output layer with the last layer of the hidden layers.
        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        // First output in acceleration and the second output is turn amount.
        // Acceleration and Trun amount have to be a value between -1 and 1 so we use the Tanh function to get an output in that range.
        float a = (float)Math.Tanh(outputLayer[0, 0]), t = (float)Math.Tanh(outputLayer[0, 1]);
        return (a, t);

    }

}
