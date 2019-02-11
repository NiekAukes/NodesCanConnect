using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnchorHandler : MonoBehaviour {
    public int strength;
    public bool occupied = false;
    Color color;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        sprite = GetComponent<SpriteRenderer>();
        color = sprite.color;
	}
	
	// Update is called once per frame
	void Update () {
    }



    private void OnMouseOver()
    {
        if (GameHandler.gm.CurrPlayerMove.playerMode == Player.PlayerMode.Build && !occupied) //check if in build mode
        {
            float d = Vector3.Distance(GameHandler.CurrDot.transform.position, transform.position); //calculates distance between currdot and this anchor
            OnHighlightStart(); //activate highlight
            if (Input.GetButton("Fire1")) //did the user left click?
            {
                if (d < 10f)
                    GameHandler.gm.CurrPlayerMove.CreateNewNode(this);
                else OnRejectBuild();
            }
        }
    }
    private void OnMouseExit()
    {

            Debug.Log("exit");
            OnHighlightExit();
        
    }

    ///DrawMethod for Reject
    private void OnRejectBuild() 
    {
        Debug.Log("Rejected");
    }

    ///DrawMethod for HighlightStart
    private void OnHighlightStart() 
    {
        //the anchor highlights
        Debug.Log("Highlighted");
        sprite.color = new Color(255f,0f,0f);
    }

    ///DrawMethod for HighlightExit
    private void OnHighlightExit() 
    {
        sprite.color = color;
    }

}
