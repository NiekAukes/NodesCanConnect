using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DotHandler : MonoBehaviour {
    bool OnDrag = false;
    int Strength = 1;
    TextMeshPro text;
    public Player Owner;

    // Use this for initialization
    void Start () {
        text = GetComponentInChildren<TextMeshPro>();
        UpdateStrength(0);
	}
	
	// Update is called once per frame
	void Update () { 

    }


    private void OnMouseExit()
    {
        GameHandler.OnOver = false;
    }
    private void OnMouseOver()
    {
        GameHandler.OnOver = true;
        if (Input.GetButton("Fire2") && !GameHandler.BuildMode) //did the user right click?
        {
            GameHandler.SwitchBuildMode();
            GameHandler.CurrDot = this;
            UpdateStrength(2);
            //updates strength and buildmode
        }
        if (Input.GetButtonDown("Fire1") && GameHandler.BuildMode && GameHandler.CurrDot != this)
        {
            GameHandler.SwitchBuildMode();
            GameHandler.CreateBindConnection(this);
            //calls a function that creates a new connection
        }
    }

    //Make SwitchPlayer
    public void UpdateStrength(int increment)
    {
        Strength += increment;
        text.text = Strength.ToString();
    }
}
