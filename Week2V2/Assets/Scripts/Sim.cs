using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monobehaviour script that manages the behaviour of a Sim entity in the simulation.
public class Sim : MonoBehaviour
{
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text traitsText;
    public GameObject needVisualPrefab;
    public Transform needVisualsHolder;
    public SpriteRenderer bodyVisualRenderer;
    public float speed;
    public float moveDirectionChangeFrequency;
    public float timeSinceLastDirectionChange;
    public Vector3 currentDirection;

    //Takes in data and initializes the properties of this particular Sim entity.
    public void Initialize(SimData inSimData)
    {
        bodyVisualRenderer.color = inSimData.simColour;
        nameText.text = inSimData.simName;
        nameText.color = inSimData.simColour;

        string traitsValue = "";
        foreach(SimData.Trait trait in inSimData.traits)
        {
            traitsValue += trait.ToString() + ", ";
        }
        traitsText.text = traitsValue;

        //Initialize the needs map by adding an entry for every Need enum value
        //uses random number for the need: will be used to set a colour
        //Unity colours are a float 0 to 1, Random.Range upper value is EXCLUSIVE
        //so won't generate a value of 1 if the upper range is 1.0 (sigh)
        //(this data doesn't change during play...yet...but is where that will be kept track of)
        foreach (SimData.Need n in System.Enum.GetValues(typeof(SimData.Need)))
        {
            inSimData.needsMap.Add(n, Random.Range(0, 1.1f));
        }

        //Initialise the needs visual using the data in the needs map
        foreach(KeyValuePair<SimData.Need, float> need in inSimData.needsMap)
        {
            GameObject needObject = Instantiate(needVisualPrefab, needVisualsHolder);
            NeedVisual needVisual = needObject.GetComponent<NeedVisual>();
            needVisual.Initialize(need);
        }
        RandomizeMoveDirection();
    }


    private void Update()
    {
        MoveUpdate();
    }

    //Manages the movement of the character.
    private void MoveUpdate()
    {
        transform.position += currentDirection * speed * Time.deltaTime;

        //If the frequency threshold is reached then the timer resets and the direction of movement is changed.
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange > moveDirectionChangeFrequency)
        {
            RandomizeMoveDirection();
            timeSinceLastDirectionChange = 0f;
        }
    }

    //Randomizes the movement direction of the character
    public void RandomizeMoveDirection()
    {
        currentDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f,1f));

        //The direction is normalized to keep the movement speed stable.
        currentDirection.Normalize();
    }
}
