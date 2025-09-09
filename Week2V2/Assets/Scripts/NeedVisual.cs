using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monobehaviour script that manages the visualization of a specific Need of a Sim.
public class NeedVisual : MonoBehaviour
{
    public TMPro.TMP_Text needText;
    
    //Takes in a Need and initializes the visuals.
    public void Initialize(KeyValuePair<SimData.Need, float> need)
    {
        needText.text = need.Key.ToString();
        needText.color = new Color(needText.color.r, needText.color.g, needText.color.b, need.Value);
    }
}
