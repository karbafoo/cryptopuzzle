using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingComponent : MonoBehaviour
{

    public Boolean isLoading = false;

    private float[] ballDelays = new float[]{-0.5f,0.25f,0f,0.25f,0.5f};

    void Start(){
        StartLoading();
    }
    
    public void StartLoading(){
        BallLoader[] ballLaoders = GetComponentsInChildren<BallLoader>();
            // ballLaoders[0].StartBouncing(ballDelays[0 % ballDelays.Length]);
        for(int i = 0 ; i < ballLaoders.Length; i++){
            ballLaoders[i].StartBouncing(ballDelays[i % ballDelays.Length]);
        }
    }
}
