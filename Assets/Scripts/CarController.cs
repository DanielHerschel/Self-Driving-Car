using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(NeuralNet))]
public class CarController : MonoBehaviour
{

    public bool learningMode = true;

    private Vector3 startPosition, startRotation;

    private NeuralNet network;
    [Header("Network Options")]
    public int LAYERS = 10;
    public int NEURONS = 30;

    [Range(-1f, 1f)]
    public float acceleration, turn;
    public float maxSpeed = 11.4f;
    public float turnSensitivity = 0.02f;

    public float timeSinceStart = 0f;

    [Header("Fitness")]
    // Fitness variables
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;
    public float barFitness = 100f, barTime = 15f;

    private Vector3 lastPosition;
    private float totalDistanceDriven;
    private float avgSpeed;

    private float rightSensor, forwardSensor, leftSensor;

    /// <summary>
    /// This function will be called when the gameobject is awake
    /// </summary>
    private void Awake()
    {

        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        //network = GetComponent<NeuralNet>();

    }

    /// <summary>
    /// This fuction will be called every fixed time.
    /// </summary>
    private void FixedUpdate()
    {

        // Get the input from the sensors.
        InputSensors();

        // Set the last position to this position.
        lastPosition = transform.position;

        // Get the acceleration and turn amount from the neural net.
        (acceleration, turn) = network.RunNN(forwardSensor, rightSensor, leftSensor);

        // Move the care with the acceleration and turn amount we got from the nueral net.
        MoveCar(acceleration, turn);

        // Add to time and calculate the fitness.
        timeSinceStart += Time.deltaTime;
        CalculateFitness();

    }

    /// <summary>
    /// Calls the Rest() function while resetting the neural net.
    /// </summary>
    /// <param name="net">New neural network.</param>
    public void ResetCarWithNetwork(NeuralNet net)
    {

        network = net;
        Reset();

    }

    /// <summary>
    /// Reset the car to the first position and rotation and reset it's values.
    /// </summary>
    public void Reset()
    {
        // Reset the car and it's values

        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        timeSinceStart = 0f;
        totalDistanceDriven = 0f;
        avgSpeed = 0f;
        overallFitness = 0f;

    }

    private void Death()
    {

        if (learningMode)
        {
            GameObject.FindObjectOfType<GeneticAlgManager>().Death(overallFitness, network);
            return;
        }

        Reset();

    }

    /// <summary>
    /// Check if the car crashed into a wall.
    /// </summary>
    /// <param name="other">The object the car crashed into.</param>
    private void OnCollisionEnter(Collision other)
    {

        Death();

    }

    /// <summary>
    /// Calculate the fitness of the car.
    /// </summary>
    private void CalculateFitness()
    {

        totalDistanceDriven += (Mathf.Abs(acceleration) / acceleration) * Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceDriven / timeSinceStart;

        overallFitness = (totalDistanceDriven * distanceMultiplier) + (avgSpeed * avgSpeedMultiplier) + (((rightSensor + forwardSensor + leftSensor) / 3) * sensorMultiplier);
        //overallFitness *= (Mathf.Abs(acceleration) / acceleration);

        if (learningMode)
        {
            if (timeSinceStart > barTime && overallFitness < barFitness)
            {
                Death();
            }

            // CHANGE THIS TO 1000!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (overallFitness >= 1000)
            {
                // Save the network to a JSON file
                Debug.Log("Found best agent");



                SceneManager.LoadScene("MainMenu");
            }
        } else
        {
        }

    }

    /// <summary>
    /// Calculate the distance of the car from the walls with the sensors.
    /// </summary>
    private void InputSensors()
    {

        // Set the normalized values of the three sensors in rightSensor, forwardSensor and leftSensor
        Vector3 right = Vector3.Normalize(transform.forward + transform.right);
        Vector3 forward = Vector3.Normalize(transform.forward);
        Vector3 left = Vector3.Normalize(transform.forward - transform.right);

        Ray r = new Ray(transform.position, right);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            rightSensor = Sigmoid(hit.distance);
        }

        Debug.DrawLine(r.origin, hit.point);

        r.direction = forward;

        if (Physics.Raycast(r, out hit))
        {
            forwardSensor = Sigmoid(hit.distance);
        }

        Debug.DrawLine(r.origin, hit.point);

        r.direction = left;

        if (Physics.Raycast(r, out hit))
        {
            leftSensor = Sigmoid(hit.distance);
        }

        Debug.DrawLine(r.origin, hit.point);

    }


    private Vector3 input;
    /// <summary>
    /// Move the car with the values of the acceleration and turn amount.
    /// </summary>
    /// <param name="a">Acceleration.</param>
    /// <param name="t">Turn amount.</param>
    public void MoveCar(float a, float t)
    {

        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, a * maxSpeed), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.eulerAngles += new Vector3(0, t * 90 * turnSensitivity, 0);

    }

    /// <summary>
    /// Return a number between -1 and 1 (normalize the input).
    /// </summary>
    /// <param name="x">An input</param>
    /// <returns></returns>
    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

}
