using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float zoomSpeed = 2;
    public float dragSpeed = 1f;

    public float minZoomDist = 5;
    public float maxZoomDist = 21;

    private Camera cam;
    public GameObject puzzleBase;

    private Vector3 currentHOLD;
    private Vector3 bounds;
    bool canMove = true;
    void Awake ()
    {
        cam = GetComponent<Camera>();
        bounds = new Vector3(5,5);
    }
        

    void Update ()
    {
        if (Input.GetMouseButtonDown(0)){
            this.currentHOLD = Input.mousePosition;
            canMove = true;
            
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null && raycastHit.transform.gameObject.tag == "PuzzlePeace")
                {
                    canMove = false;
                }
            }
        }
        if (Input.GetMouseButton(0)){
            if(canMove){
                Vector3 pos = Input.mousePosition - this.currentHOLD;
                float coef = (dragSpeed / (Mathf.Abs((maxZoomDist - Mathf.Abs(cam.orthographicSize))) + 20));
                Vector3 move = new Vector3(pos.x * coef * -1f,  pos.y * coef * -1.0f, 0);

                float xBound = bounds.x + Mathf.Sqrt(cam.orthographicSize) * 3;
                float yBound = bounds.y + Mathf.Sqrt(cam.orthographicSize) * 3;
                if(transform.position.x < -1 * xBound && move.x < 0){
                    move.x = 0;
                }
                if(transform.position.x >  xBound && move.x > 0){
                    move.x = 0;
                }   
                if(transform.position.y < -1 * yBound && move.y < 0){
                    move.y = 0;
                }
                if(transform.position.y >  yBound && move.y > 0){
                    move.y = 0;
                }
                transform.Translate(move, Space.World);  
                this.currentHOLD = Input.mousePosition;
            }
        }
        Move();
        
    }

    void OnGUI()
    {
        if(canMove){
            Zoom();
        }
    }

    void Move ()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        Vector3 dir = transform.forward * zInput + transform.right * xInput;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Zoom ()
    {
        float dist = cam.orthographicSize;
        float change = Input.mouseScrollDelta.y * zoomSpeed * -1;
        if(cam.orthographicSize + change > maxZoomDist){
            return;
        }  
        if(cam.orthographicSize + change < minZoomDist){
            return;
        }
        cam.orthographicSize +=  change;
    }


    public void SetCanMove(bool c){
        this.canMove = c;
    }

}
