using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Puzzle : MonoBehaviour
{
    float unitsToPixels = 100f;
    public GameManager gameManager;
    SpriteRenderer puzzleSpriteRenderer;
    public Metadata meta;
    public GameObject puzzlePeace;
    PuzzleStatus puzzleStatus;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        puzzleSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(GetTexture());  

        if(meta.Peaces == null){
            return;
        }
        puzzleStatus = new PuzzleStatus();
        for(int i = 0 ; i < meta.Peaces.Count; i++)
        {
            GameObject newPuzzlePeace = Instantiate(
                puzzlePeace, 
                Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 0, Random.Range(1,360))));
            PuzzlePeace p = newPuzzlePeace.GetComponent<PuzzlePeace>();
            p.meta = meta;
            p.name = meta.Peaces[i].Id;
            p.peace = meta.Peaces[i];
            Point center = meta.Peaces[i].Center;
            Point dim = meta.Peaces[i].Dim;
            p.SetPuzzle(gameObject);
            puzzleStatus.AddNewPeace(meta.Peaces[i].Id);
        }
        gameManager.UpdatePuzzleStatus();

    }


    // Vector3 getMatchCoords(GameObject puzzlePeace){
    //     Vector3 puzzleOffset = new Vector3((500 / (unitsToPixels)) ,(500 / (unitsToPixels)), 0);
    //     Vector3 currPos = transform.position;
    //     PuzzlePeace p = puzzlePeace.GetComponent<PuzzlePeace>();
    //     Point dim = p.peace.Dim;
    //     return new Vector3(
    //         (currPos.x + puzzleOffset.x) - ((1000 -(dim.x + (dim.w/2f))) / unitsToPixels),  
    //         (currPos.y + puzzleOffset.y) - ((dim.y + (dim.h/2f)) / unitsToPixels), 
    //         9.25f);
    // }

       
    IEnumerator GetTexture() {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(NFTManager.DEFAULT_MEDIA_PATH + "puzzle/" + meta.Url + "/holed");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            // StartCoroutine(GetTexture());   //TODO error manager
        }
        else {
            Texture2D  myTexture = (Texture2D) ((DownloadHandlerTexture)www.downloadHandler).texture;
            // myTexture
            puzzleSpriteRenderer.sprite = Sprite.Create(
                myTexture,
                new Rect(
                    0 , 
                    0 ,  
                    myTexture.width, 
                    myTexture.height
                ),
                Vector2.one / 2f,
                unitsToPixels
                );
        }
    }


    public void OnPeacePlaced(string id){
        puzzleStatus.OnPeaceSuccess(id);
        gameManager.OnSuccessfullPlacement();
    }


    public PuzzleStatus GetPuzzleStatus(){
        return this.puzzleStatus;
    }
}


public class PuzzleStatus {
    public List<string> pending;
    public List<string> done;

    public PuzzleStatus()
    {
        this.pending = new List<string>();
        this.done = new List<string>();
    }

    public void AddNewPeace(string id){
        this.pending.Add(id);
    }

    public void OnPeaceSuccess(string id){
        string pendingId = this.pending.Find(x => x == id);
        if(pendingId != null){
            this.pending.Remove(id);
            this.done.Add(id);
        }
    }

    public int GetPendingCount(){
        return this.pending.Count;
    }
    public int GetDoneCount(){
        return this.done.Count;
    }
}