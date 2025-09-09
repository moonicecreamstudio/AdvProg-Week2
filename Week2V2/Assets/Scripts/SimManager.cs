using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monobehaviour script used to create Sim entities.
public class SimManager : MonoBehaviour
{
    [Range(0, 10)] public float myNumber;
    public List<SimData> sims = new List<SimData>();
    public GameObject simPrefab;
    [Header("Simmy Sims")]
    [Tooltip("This is how you use it.")]
    public Transform simsHolder;

    //On initialization, the SimManager will create Sim entities for each instance of SimData it has stored.
    void Start()
    {
        foreach (SimData simData in sims)
        {
            CreateSimEntity(simData);
        }
    }

    //Given data about a Sim, it creates the entity associated with that data

    /// <summary>
    /// This function creates a new sim.
    /// </summary>
    /// <param name="simData">The data about the sim's likes.</param>
    public void CreateSimEntity(SimData simData)
    {
        GameObject simObject = Instantiate(simPrefab, simsHolder);
        Sim sim = simObject.GetComponent<Sim>();
        sim.Initialize(simData);
    }

    // Using this to trigger scripts when the game isn't running.
    // It must be a public function, and can't take arguments.
    [ContextMenu("Say Hello!")]
    public void PrintHello()
    {
        Debug.Log("Hello!");
    }
}
