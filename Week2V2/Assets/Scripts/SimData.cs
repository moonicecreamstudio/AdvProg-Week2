using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data class used for propagating the details of a Sim when creating them in the simulation.
[System.Serializable]
public class SimData
{
    //The name of the Sim.
    public string simName;

    //How old the Sim is.
    public int age;

    //Data used to identify specific types of Traits that Sim entities can have.
    public enum Trait
    {
        ambitious, cheerful, childish, clumsy, creative, erratic, genius
    }
    //The specific list of Traits on this Sim that are used to dictate how they will hypothetically behave. 
    public List<Trait> traits = new List<Trait>();

    //Data used to identify the specific types of Needs that Sim entities can have.
    public enum Need
    {
        hunger, social, fun, comfort, hygiene, room, bladder, energy
    }
    //The specific map of Needs on this Sim, along with their initial values.
    public Dictionary<Need, float> needsMap = new Dictionary<Need, float>();

    //The colour associated with this Sim.
    public Color simColour;
}
