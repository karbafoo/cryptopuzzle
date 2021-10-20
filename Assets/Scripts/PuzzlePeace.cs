using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PuzzlePeace : MonoBehaviour
{
    public static int ROTATION_THRESH = 46;
    public static float POSITION_THRESH = 0.5f;
    float unitsToPixels = 100f; //TODO REMOVE DUP
    int puzzleSize = 1000; //TODO REMOVE DUP
    float padding = 0.02f;
    GameObject puzzle;
    public NFTManager nftManager;
    public CameraController camController;
    SpriteRenderer peaceMat;
    public Metadata meta;
    public Peace peace;
    Texture2D hollowText;
    Material glowyMaterial;
    Shader shader;
    bool IsActive;
    Vector3 matchCoords;
    bool isSet;
    void Start()
    {
        peaceMat = gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(GetTexture());  
        nftManager = FindObjectOfType<NFTManager>();
        camController = FindObjectOfType<CameraController>();
        shader = nftManager.shader;
        GameObject slot = nftManager.getEmptyPeaceSlot();
        if(slot != null){
            gameObject.transform.position = slot.transform.position;
        }
    }

    void Update() {
        if(IsActive){
            if (Input.GetMouseButton(0)){
                Vector3 mouse = Input.mousePosition;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 45f));
                transform.position = mousePos;

                float change = Input.mouseScrollDelta.y * 7;
                if(change != 0){
                    float c = Mathf.FloorToInt((transform.rotation.eulerAngles.z + change) / 5);
                    transform.rotation = 
                        Quaternion.Euler(0, 0, c * 5);
                }
            }
            else{
                ClearActive();
            }
        }
    }

    IEnumerator GetTexture() {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(NFTManager.DEFAULT_MEDIA_PATH + "puzzle/" + meta.Name + "/peace/" + peace.Url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            // StartCoroutine(GetTexture());   //TODO error manager
        }
        else {
            Texture2D  myTexture = (Texture2D) ((DownloadHandlerTexture)www.downloadHandler).texture;
            peaceMat.sprite = Sprite.Create(
                myTexture,
                new Rect(
                    0 , 
                    0 ,  
                    myTexture.width, 
                    myTexture.height
                ),
                Vector2.one / 2f
                );
            peaceMat.sortingOrder = 1;

            Material  _material = new Material(shader); 
            //  _material.SetFloat("_Outline", 0);
            peaceMat.material = _material;
            matchCoords = getMatchCoords();

            BoxCollider bx = GetComponent<BoxCollider>();
            bx.center = new Vector3(
                peaceMat.bounds.center.x / unitsToPixels, 
                peaceMat.bounds.center.y / unitsToPixels, 
                0);
            bx.size = new Vector3(
                peaceMat.bounds.size.x, 
                peaceMat.bounds.size.y, 
                1);
        }
    }

    SpriteRenderer GetMat(){
        return peaceMat;
    }

    public void SetPuzzle(GameObject p){
        puzzle = p;
    }

    Vector3 getMatchCoords(){
        float halfSize = puzzleSize / 2;
        Vector3 puzzleOffset = new Vector3((halfSize / (unitsToPixels) + padding) ,(halfSize / (unitsToPixels) - padding), 0);
        Vector3 puzzlePos = puzzle.transform.position;
        Point dim = peace.Dim;
        return new Vector3(
            (puzzlePos.x + puzzleOffset.x) - ((puzzleSize - (dim.x + (dim.w/2f))) / unitsToPixels),  
            (puzzlePos.y + puzzleOffset.y) - ((dim.y + (dim.h/2f)) / unitsToPixels), 
            0);
    }

    public void SetActive(){
        if(!isSet){
            peaceMat.material.SetFloat("_Outline", 1);
            this.IsActive = true;
        }
    }

    public void ClearActive(){
        float dist = Vector3.Distance(matchCoords, transform.position);
        float distRotation = Vector3.Distance(Vector3.zero, transform.rotation.eulerAngles);
        float distARotation = Vector3.Distance(Vector3.right * 360, transform.rotation.eulerAngles);
        if(dist < POSITION_THRESH && (distRotation < ROTATION_THRESH || distARotation < ROTATION_THRESH)){
            Puzzle p = puzzle.GetComponent<Puzzle>();
            p.OnPeacePlaced(this.peace.Id);
            transform.position = matchCoords;
            transform.rotation = Quaternion.identity;
            isSet = true;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        peaceMat.material.SetFloat("_Outline", 0);
        this.IsActive = false;
        camController.SetCanMove(true);
    }
}
