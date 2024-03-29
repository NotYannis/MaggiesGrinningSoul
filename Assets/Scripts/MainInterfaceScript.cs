﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainInterfaceScript : MonoBehaviour {
    SoundEffectsHelper soundEffects;
    HangManScript hangMan;

    GameObject menu;
    GameObject planchette;
    GameObject scripts;

    public GameObject newQuestionSprite;

    Vector3 toPositionMenu;
    Vector3 toPositionPlanchette;

    public Sprite baseMenuButton;


    bool addHangManPart;
    public bool menuSlide;
    bool isMenuOpen;
    private bool spiritTalking;
    private bool hasAnswer;
    private bool isAffirmative;
    private bool isAskingQuestion;
    private bool isLastQuestion;
    private bool unlockSomething;

    public float planchetteMoveSpeed;
    public float menuMoveSpeed;

    public float spiritThinkingTime;
    private float spiritThinkingCooldown;

    private int xPeriod = 1;
    private int yPeriod = 1;
    private int frameCountx = 1;
    private int frameCounty = 1;
    private int planchetteAmplitudeX = 100;
    private int planchetteAmplitudeY = 80;


    // Use this for initialization
    void Start () {
        scripts = GameObject.Find("Scripts");
        menu = GameObject.Find("MainInterface/Menu");
        planchette = GameObject.Find("MainInterface/Menu/OuijaBoard/Planchette");
        hangMan = GameObject.Find("MainInterface/Menu/OuijaBoard/HangMan").GetComponent<HangManScript>();

        if (GameObject.Find("Sounds").GetComponent<SoundEffectsHelper>() != null)
        {
            soundEffects = GameObject.Find("Sounds").GetComponent<SoundEffectsHelper>();
        }

        toPositionMenu = new Vector3(0, 0, 0);
        spiritThinkingCooldown = spiritThinkingTime;
        soundEffects.MakeStartSound(Camera.main.transform.position);
        soundEffects.MakeAmbianceMusic();
    }
	
	// Update is called once per frame
	void Update () {
        //Move the planchette
        if (spiritTalking)
        {
            if(spiritThinkingCooldown >= 0.0f)
            {
                ++frameCountx;
                ++frameCounty;

                spiritThinkingCooldown -= Time.deltaTime;

                if (frameCountx == xPeriod)
                {
                    xPeriod = Random.Range(300, 400);
                    frameCountx = 1;
                }
                if(frameCounty == yPeriod)
                {
                    yPeriod = Random.Range(200, 300);
                    frameCounty = 1;
                }

                float xPosition = planchetteAmplitudeX * Mathf.Cos((Mathf.PI * 2) * frameCountx / xPeriod);
                float yPosition = planchetteAmplitudeY * Mathf.Sin((Mathf.PI * 2) * frameCounty / yPeriod);
                planchette.GetComponent<RectTransform>().localPosition = new Vector3(xPosition, yPosition, 0);
            }
            else
            {
                float speed = planchetteMoveSpeed * Time.deltaTime;
                Vector3 planchettePos = planchette.GetComponent<RectTransform>().localPosition;

                planchette.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(planchettePos, toPositionPlanchette, speed);
                if(planchette.GetComponent<RectTransform>().localPosition.x == toPositionPlanchette.x)
                {
                    spiritTalking = false;
                    spiritThinkingCooldown = spiritThinkingTime;
                    if (addHangManPart)
                    {
                        hangMan.EnableHangManPart();
                        addHangManPart = false;
                        soundEffects.MakeWrongQuestionSound(Camera.main.transform.position);
                        if (isLastQuestion && hangMan.hangManDisabledParts.Count != 1)
                        {
                            soundEffects.Invoke("MakePaperMouvement", 4.0f);
                            soundEffects.Invoke("MakeEndSound", 4.0f);
                            soundEffects.Invoke("MakeEndMusic", 4.0f);
                            scripts.GetComponent<ScreenShake>().Invoke("StartShaking", 2.0f);
                            scripts.GetComponent<PosterScript>().enabled = true;
                            scripts.GetComponent<PosterScript>().PosterVener();
                        }
                    }
                    else
                    {
                        if (unlockSomething)
                        {
                            soundEffects.MakeUnlockSomethingSound(Camera.main.transform.position);
                            GameObject newQuestionSpr = Instantiate(newQuestionSprite, GameObject.Find("newQuestions").transform, false) as GameObject;
                            Destroy(newQuestionSpr, 5.0f);
                            unlockSomething = false;
                        }
                    }
                }
            }
        }

        //Slide the menu on the game
        if (menuSlide)
        {
            GameObject.Find("MainInterface/ToggleMenu").GetComponent<Image>().sprite = baseMenuButton;
            float speed = menuMoveSpeed * Time.deltaTime;
            Vector3 menuPosition = menu.GetComponent<RectTransform>().localPosition;

            menu.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(menuPosition, toPositionMenu, speed);
            if (menuPosition == toPositionMenu)
            {
                menuSlide = false;
            }
        }
	}

    public void OnMenuCall() {
        if (isMenuOpen)
        {
            toPositionMenu = new Vector3(0, -1080, 0);
        }
        else
        {
            toPositionMenu = new Vector3(0, 0, 0);
        }
        isMenuOpen = !isMenuOpen;
        menuSlide = true;
    }

    public void OnOuijaCall()
    {
        isAskingQuestion = false;
        if (hasAnswer)
        {

            if (isAffirmative)
            {
                toPositionPlanchette = new Vector3(-123, 39, 0);
            }
            else
            {
                toPositionPlanchette = new Vector3(125, 39, 0);
            }
        }
        else
        {
            toPositionPlanchette = new Vector3(0, -60, 0);
            addHangManPart = true;
        }
        spiritTalking = true;

        //Initialize planchette position
        xPeriod = Random.Range(300, 400);
        yPeriod = Random.Range(200, 300);

        float xPosition = planchetteAmplitudeX * Mathf.Cos((Mathf.PI * 2) * frameCountx / xPeriod);
        float yPosition = planchetteAmplitudeY * Mathf.Sin((Mathf.PI * 2) * frameCounty / yPeriod);
        planchette.GetComponent<RectTransform>().localPosition = new Vector3(xPosition, yPosition, 0);

        soundEffects.MakeAnswerSpiritSound(Camera.main.transform.position);
    }

    public void SetSpiritTalking(bool enable){
        spiritTalking = enable;
    }

    public void SetHasAnswer(bool enable){
        hasAnswer = enable;
    }

    public void SetIsAffirmative(bool enable){
        isAffirmative = enable;
    }

    public void SetIsAskingQuestion(bool enable){
        isAskingQuestion = enable;
    }

    public void SetIsLastQuestion(bool enable){
        isLastQuestion = enable;
    }

    public void SetUnlockSomething(bool enable){
        unlockSomething = enable;
    }
    public bool GetSpiritTalking(){
        return spiritTalking;
    }

    public bool GetIsAskingQuestion(){
        return isAskingQuestion;
    }
}
