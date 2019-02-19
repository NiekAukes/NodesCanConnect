using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFragment : MonoBehaviour
{
    public DotHandler MainDot;
    public SpriteRenderer Frame;
    public SpriteRenderer Outline;
    public int level;
    private void Start()
    {

    }

    public void Draw()
    {
        float scalelevel = 0.2f;
        gameObject.transform.localScale = new Vector3(level * scalelevel + 0.2f, level * scalelevel + 0.2f, gameObject.transform.localScale.z);
        Outline.sortingOrder =  10 - level * 2;
        Frame.sortingOrder = Outline.sortingOrder + 1;
        //scaling
        Debug.Log("Scaled: " + level);
        //more drawing
    }

    private void OnMouseOver()
    {
        MainDot.OnMouse = true;
    }
    private void OnMouseExit()
    {
        MainDot.OnMouse = false;
    }
    private void OnMouseDown()
    {

        MainDot.OnRegisterEnter(this);
        /*
        if (!MainDot.OnMouse)
        {
        Debug.Log("I clicked");
            switch (GameHandler.CurrPlayerMove.playerMode)
            {
                case Player.PlayerMode.Build:
                    if (MainDot.Owner == GameHandler.CurrPlayerMove)
                    {
                        Debug.Log(DotHandler.selectedDot + "  //  " + MainDot + "  //  " + (DotHandler.selectedDot == this.MainDot));
                        //establish Connection or Move Energy or wnats to cancel
                        if (DotHandler.selectedDot == this.MainDot)
                        {
                            GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
                            Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                            //cancelled
                        }
                    }
                    else
                    {
                        //attack other Dot (not yet established)

                    }
                    break;

                case Player.PlayerMode.Select:
                    Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                    GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                    DotHandler.selectedDot = MainDot;
                    break;
            }
        }*/
    }
    private void OnMouseUp()
    {
        MainDot.OnRegisterRelease();
        //GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
    }

    //Detect if player wants to establish connection | move Energy | Attack dot --finished
    //handle the modes

    public void OnDestroy()
    {
        //handle Destroy visual change
    }
}
