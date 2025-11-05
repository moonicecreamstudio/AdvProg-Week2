using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
    public SimManager simManager;
    [Tooltip("File path + filename and extension please!")]
    public string savePath;

    [ContextMenu("Where is the data path?")]
    public void DisplayDataPath()
    {
        Debug.Log(Application.dataPath);
    }

    private void Start()
    {
        if (simManager == null || simManager.sims.Count == 0) return;

        //will work with simManager, but not a GameObject
        string data = JsonUtility.ToJson(simManager.sims[0]);
        Debug.Log(data);
    }

    public void SaveDataAsText()
    {
        if (simManager == null || simManager.sims.Count == 0) return; //no data to save
        if (savePath == "") return; //no file path to save to

        using(StreamWriter streamWriter = new StreamWriter(savePath))
        {
            ////this doesn't work with the list of sims, we just get {} as output:
            //string data = JsonUtility.ToJson(simManager.sims);
            //streamWriter.Write(data);

            //can't just thrown the list of sims at JsonUtility to make one Json string
            //but we can do the list one sim at a time
            foreach (SimData sim in simManager.sims)
            {
                string data = JsonUtility.ToJson(sim);
                streamWriter.WriteLine(data);
            }
        }

        Debug.Log("Success! " + savePath);
    }

    public void LoadDataAsText()
    {
        if (savePath == "") return; //no file path to load from

        string json = string.Empty; //initialise the string
        
        //remember to import System.Collections.Generic for the List type ;-)
        List<SimData> loadedSims = new List<SimData>(); 

        using(StreamReader streamReader = new StreamReader(savePath))
        {
            while (!streamReader.EndOfStream)
            {
                //we know each line is one SimData object as a json string
                json = streamReader.ReadLine();
                Debug.Log(json);
                //Convert json back into a SimData object
                SimData sim = JsonUtility.FromJson<SimData>(json);
                //store it in a list to action when all of the sims have been loaded
                loadedSims.Add(sim);
            }
        }

        //If we sucessfully loaded sims, replace the current value of simManager.sims
        //(if the load failed, we don't want to overwrite the current data with nothing)
        if(loadedSims.Count > 0)
        {
            simManager.sims = loadedSims;
            //Still need to tell the SimManager to nuke the existing sims & initialise the new list of sims ;-)
        }
    }

    public void SaveDataAsBinary()
    {
        if (simManager == null || simManager.sims.Count == 0) return; //no data to save
        if (savePath == "") return; //no file path to save to

        using (FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                ////this doesn't work with the list of sims, we just get {} as output:
                //string data = JsonUtility.ToJson(simManager.sims);
                //streamWriter.Write(data);

                //can't just thrown the list of sims at JsonUtility to make one Json string
                //but we can do the list one sim at a time

                //the BinaryReader can't tell if we're at the end of a file, so we need to tell
                //it how much data to expect:
                binaryWriter.Write(simManager.sims.Count);
                foreach (SimData sim in simManager.sims)
                {
                    string data = JsonUtility.ToJson(sim);
                    binaryWriter.Write(data);
                }
            }
        }

        Debug.Log("Success! " + savePath);
    }

    public void LoadDataAsBinary()
    {
        if (savePath == "") return; //no file path to load from

        string json = string.Empty; //initialise the string

        //remember to import System.Collections.Generic for the List type ;-)
        List<SimData> loadedSims = new List<SimData>();

        using (FileStream fileStream = new FileStream(savePath, FileMode.Open))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                int howManyLinesOfDataToRead = binaryReader.ReadInt32();
                Debug.Log("How many lines of binary data to read: " + howManyLinesOfDataToRead);
                for(int i = 0; i < howManyLinesOfDataToRead; i++)
                {
                    //we know each line is one SimData object as a json string
                    json = binaryReader.ReadString();
                    Debug.Log(json);
                    //Convert json back into a SimData object
                    SimData sim = JsonUtility.FromJson<SimData>(json);
                    //store it in a list to action when all of the sims have been loaded
                    loadedSims.Add(sim);
                }
            }
        }

        //If we sucessfully loaded sims, replace the current value of simManager.sims
        //(if the load failed, we don't want to overwrite the current data with nothing)
        if (loadedSims.Count > 0)
        {
            simManager.sims = loadedSims;
            //Still need to tell the SimManager to nuke the existing sims & initialise the new list of sims ;-)
        }
    }

    public void SaveDataAsBinaryNotSecure()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        //use the save data class to be serializable
        List<SimSaveData> simSaveData = new List<SimSaveData>();
        foreach(SimData sim in simManager.sims)
        {
            //use the SimSaveData constructor to stuff the data into a fully-serializable data class
            SimSaveData data = new SimSaveData(sim);
            simSaveData.Add(data);
        }

        using (FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fileStream, simSaveData);
        }

        Debug.Log("Success! " +  savePath);
    }

    public void LoadDataAsBinaryNotSecure()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        List<SimData> loadedSims = new List<SimData>();
        List<SimSaveData> serialzedSims = new List<SimSaveData>();

        using (FileStream fileStream = new FileStream(savePath, FileMode.Open))
        {
            try 
            {
                //object with a lower-case o, not upper-case O !!
                object data = formatter.Deserialize(fileStream);

                //now we cast the object into the type we expect it to be:
                //the fully-serializeable data class
                serialzedSims = (List<SimSaveData>)data;
            }
            catch
            {
                Debug.Log("Reading from binary file went wrong :-(");
            }
        }

        //If we sucessfully loaded sims, replace the current value of simManager.sims
        //(if the load failed, we don't want to overwrite the current data with nothing)
        if (serialzedSims.Count > 0)
        {
            //But it's still in the serializable save data class format
            //so use the helper method to stuff it back into the Unity-friendly SimData class :-)
            foreach(SimSaveData simSaveData in serialzedSims)
            {
                SimData sim = simSaveData.GetSimData();
                loadedSims.Add(sim);
            }
            simManager.sims = loadedSims;
            //Still need to tell the SimManager to nuke the existing sims & initialise the new list of sims ;-)
        }
        else
        {
            Debug.Log("Didn't find any data in the binary file");
        }

    }
}
