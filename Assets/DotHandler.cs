using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DotHandler : MonoBehaviour {
    bool OnDrag = false;
    int Strength = 1;
    TextMeshPro text;
    public Player Owner;
    Color color;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        sprite = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        UpdateStrength(0);
        color = sprite.color;
        
	}
	
	// Update is called once per frame
	void Update () { 

    }


    private void OnMouseExit()
    {
        GameHandler.OnOver = false;
        Debug.Log("exit");
        OnHighlightExit();
    }
    private void OnMouseOver()
    {
        GameHandler.OnOver = true;
        OnHighlightStart();
        if (Input.GetButton("Fire2") && !(GameHandler.gm.CurrPlayerMove.playerMode == Player.PlayerMode.Build)) //did the user right click?
        {
            GameHandler.gm.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
            GameHandler.CurrDot = this;
            UpdateStrength(2);
            //updates strength and buildmode
        }
        if (Input.GetButtonDown("Fire1") && GameHandler.gm.CurrPlayerMove.playerMode == Player.PlayerMode.Build && GameHandler.CurrDot != this)
        {
            GameHandler.gm.CurrPlayerMove.playerMode = Player.PlayerMode.Selection;
            GameHandler.CreateBindConnection(this);
            //calls a function that creates a new connection
        }
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
        sprite.color = new Color(255f, 0f, 0f);
    }

    ///DrawMethod for HighlightExit
    private void OnHighlightExit()
    {
        sprite.color = color;
    }

    //Make SwitchPlayer
    public void UpdateStrength(int increment)
    {
        Strength += increment;
        text.text = Strength.ToString();
    }
}
