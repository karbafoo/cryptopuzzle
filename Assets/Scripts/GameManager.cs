using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    AudioSource bgSong;
    public GameObject ding;
    public Text timer;
    public Text score;
    public Slider bgMusicSlider;
    public GameObject mainMenu;
    public GameObject endMenu;
    public Text endMenuScore;
    int timeLapsed = 0;
    public bool gameIsOn = false;

    void Start()
    {
        bgMusicSlider.onValueChanged.AddListener(delegate {SliderValueChangeBGMusic();});
        bgSong = GetComponent<AudioSource>();
        bgSong.Play(0);
        bgSong.volume = bgMusicSlider.value;
        AudioSource d = ding.GetComponent<AudioSource>();
        d.volume = bgMusicSlider.value;
        gameIsOn = true;
        StartTimer();
    }

    void Update(){
         if (Input.GetMouseButtonDown(0))
         {
             RaycastHit raycastHit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
             if (Physics.Raycast(ray, out raycastHit, 100f))
             {
                if (raycastHit.transform != null  && raycastHit.transform.gameObject.tag == "PuzzlePeace")
                {
                    Interact(raycastHit.transform.gameObject);
                }
             }
         }
    }

    public void QuitGame(){
        Application.Quit();
    }
    public void GoToMenu(){
        SceneManager.LoadScene("mainmenu");
    }

    public void SetMainMenuVis(bool v){
       mainMenu.SetActive(v);
    }

    public void SliderValueChangeBGMusic(){
        bgSong.volume = bgMusicSlider.value;
        AudioSource d = ding.GetComponent<AudioSource>();
        d.volume = bgMusicSlider.value;
    }

    void Interact(GameObject t){
        PuzzlePeace p = t.GetComponent<PuzzlePeace>();
        p.SetActive();
    }

    public void OnSuccessfullPlacement(){
        AudioSource d = ding.GetComponent<AudioSource>();
        d.Play(0); 
        UpdatePuzzleStatus();
    }

    public void UpdatePuzzleStatus(){
        Puzzle[] puzzles = FindObjectsOfType<Puzzle>();
        int p = 0;
        int d = 0;
        for(int i = 0; i < puzzles.Length; i++){
            PuzzleStatus ps = puzzles[i].GetPuzzleStatus();
            if( ps != null ){
                p += ps.GetPendingCount() + ps.GetDoneCount();
                d += ps.GetDoneCount();
            }
        }
        string s = "";
        s += d.ToString(); 
        s += "/"; 
        s += p.ToString(); 
        score.text = s;
        if(p == d){
            endMenu.SetActive(true);
            gameIsOn = false;
            endMenuScore.text = ParseTime(timeLapsed);
        }
    }

    void StartTimer(){
        timeLapsed = 0;
        StartCoroutine(Interval());
    }
    
    IEnumerator Interval(){
        yield return new WaitForSeconds(1);
        timeLapsed = timeLapsed + 1;
        
        timer.text = ParseTime(timeLapsed);
        if(gameIsOn){
            StartCoroutine(Interval()); 
        }
    }

    string ParseTime(int t){
        string s = "";
        int seconds = t % 60;
        int min = Mathf.FloorToInt(t / 60);
        if(min < 10){
            s += "0";
        }
        s += min.ToString(); 
        s += ":"; 
        if(seconds < 10){
            s += "0";
        }
        s += seconds.ToString();
        return s;
    }
}
