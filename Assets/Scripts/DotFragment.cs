using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFragment : MonoBehaviour
{
    public DotHandler MainDot;
    public int level;
    private void Start()
    {

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
