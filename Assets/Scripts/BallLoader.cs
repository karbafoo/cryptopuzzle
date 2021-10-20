using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLoader : MonoBehaviour
{
       // Movement speed in units per second.
    private float speed = 7.75F;

    // Time when the movement started.
    private float startTime;
    private float duration = 2f;
    
    // Total distance between the markers.
    private Vector3 startMarker;
    private Vector3 endMarker;
    private float journeyLength;

    private Boolean isBouncing = false;
    private Boolean flip = false;
    public void StartBouncing (float delay){
        gameObject.transform.position = startMarker;
        StartCoroutine(setupGame(delay));
    }


    IEnumerator setupGame(float delay){
        yield return new WaitForSeconds(delay * 2);
        isBouncing = true;
        flip = false;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
    }
    
    RectTransform myRectTransform ;
    void Awake(){
        myRectTransform = GetComponent<RectTransform>();
        startMarker = myRectTransform.localPosition;
        endMarker =  new Vector3(startMarker.x, 10, startMarker.z);
    }

    void Update(){
        if(isBouncing){
            // Distance moved equals elapsed time times speed..
            startTime += Time.deltaTime * speed;

          
            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = startTime / duration;

    

            // Set our position as a fraction of the distance between the markers.
            myRectTransform.localPosition = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
           
            
            if(startTime > duration){
                int k = -1;
                if(flip){
                    k = 1;
                }
               
                startTime = 0f;
                flip = !flip;
                startMarker = myRectTransform.localPosition;
                endMarker =  new Vector3(startMarker.x, 10 * k, startMarker.z);
                
            }
        }
    }
}
