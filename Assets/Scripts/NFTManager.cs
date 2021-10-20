using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NFTManager : MonoBehaviour
{
    static public string DEFAULT_MEDIA_PATH = "http://157.90.19.32:4445/media/";
    public GameObject puzzlePrefab;
    public Shader shader;
    public GameManager gameManager;
    
    public string error = "";
    public GameObject[] slots = new GameObject[3];
    public GameObject[] peaceSlots = new GameObject[72];
    public Boolean[] peaceSlotFull = new Boolean[28];
    private List<Metadata> puzzleMetadatas = new List<Metadata>();

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(GetPuzzleNames());

    }



    public GameObject getEmptyPeaceSlot(){
        int i = 0;
        int length = peaceSlots.Length;
        GameObject s = null;
        while(i < length){
           int candidate =  UnityEngine.Random.Range(0, length);
           if(!peaceSlotFull[candidate]){
                peaceSlotFull[candidate] = true;
                s = peaceSlots[candidate];
               break;
           }
           i++;
        }
        return s;
    }

    
    void onMetadataReceived(Metadata meta){
        puzzleMetadatas.Add(meta);
    }    
    void onStartGame(PuzzleList list){
        StartCoroutine(setupGame(list.names));
    }

    IEnumerator setupGame(List<string> _puzzles){
        yield return StartCoroutine(GetMetadatas(_puzzles, onMetadataReceived)); 
        for(int i = 0; i < puzzleMetadatas.Count; i++){
             setupPuzzle(puzzleMetadatas[i], i);
        }
    }
    void setupPuzzle(Metadata meta, int slot){
        GameObject newPuzzle = Instantiate(
               puzzlePrefab, 
               new Vector3(0,0,0), 
               Quaternion.identity);
        // newPuzzle.transform.SetParent(slots[slot].transform);

        newPuzzle.transform.position = transform.TransformPoint(slots[slot].transform.position);
     
        Puzzle p = newPuzzle.GetComponent<Puzzle>();
        p.meta = meta;
    }
    IEnumerator GetMetadatas(List<string> puzzles, System.Action<Metadata> onMetadataReceived){
        for(int i = 0 ; i < puzzles.Count; i++){
            string puzzleURL = puzzles[i];
            string url = NFTManager.DEFAULT_MEDIA_PATH + "puzzle/" + puzzleURL.Split('.')[0] + "/metadata.json";
            yield return StartCoroutine(GetMetadata(url, onMetadataReceived));
        }
        yield return "done";
    }
    IEnumerator GetMetadata(string uri, System.Action<Metadata> onMetadataReceived) {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();
                Debug.Log(uri);
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        this.error = webRequest.error;
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        this.error = webRequest.error;
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        this.error = webRequest.error;
                        break;
                    case UnityWebRequest.Result.Success:
                        onMetadataReceived(JsonUtility.FromJson<Metadata>(webRequest.downloadHandler.text));
                        break;
                }
        }        
    }  

    IEnumerator GetPuzzleNames() {
        string uri = NFTManager.DEFAULT_MEDIA_PATH + "puzzles/moonshotbotsv3";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    this.error = webRequest.error;
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    this.error = webRequest.error;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    this.error = webRequest.error;
                    break;
                case UnityWebRequest.Result.Success:
                    onStartGame(JsonUtility.FromJson<PuzzleList>(webRequest.downloadHandler.text));
                    break;
            }
        }        
    }
}


[System.Serializable]
public class Metadata {
    public string Name;
    public string HoledUrl;
    public string Url;
    public string BaseUrl;
    public List<Peace> Peaces;

    // public Metadata(string name, string url, string baseUrl, Dictionary<string,Peace> peaces){
    //     this.name =  name;
    //     this.url =  url;
    //     this.baseUrl =  baseUrl;
    //     this.peaces = (Dictionary<string,Peace>) peaces;
    // }
}

[System.Serializable]
public class Peace {
    public string Id;
    public string Url;
    public Point Center;
    public Point Dim;
}
[System.Serializable]
public class Point {
    public int x ;
    public int y ;
    public int w ;
    public int h ;
}
[System.Serializable]
public class PuzzleList {
    public List<string> names;
}