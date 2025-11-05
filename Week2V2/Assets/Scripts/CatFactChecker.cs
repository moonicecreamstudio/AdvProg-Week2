using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using TMPro;

public class CatFactChecker : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    [System.Serializable]
    public class CatFactData
    {
        public string fact;
        public int length;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FactChecker());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator FactChecker()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://catfact.ninja/fact"))
        {
            // Request and wait for the desired page (must be a coroutine !)
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;

                CatFactData data = JsonUtility.FromJson<CatFactData>(jsonString);

                _tmp.text = data.fact;
            }
        }
    }
}
