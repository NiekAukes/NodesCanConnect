using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFragment : MonoBehaviour
{
    public DotHandler MainDot;
    public int level;
    private void OnMouseDown()
    {
        if (GameHandler.CurrPlayerMove.playerMode == Player.PlayerMode.Select)
        {
            GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
            //Select Dot
        }
        if (GameHandler.CurrPlayerMove.playerMode == Player.PlayerMode.Build)
        {
            if (MainDot.Owner == GameHandler.CurrPlayerMove)
            {
                //establish Connection or Move Energy
            }
            else
            {
                //attack other Dot (not yet established)
            }
        }
    }
    private void OnMouseDrag()
    {
        GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
    }
    private void OnMouseUp()
    {
        foreach(Player p in GameHandler.PlayerList)
        {
            if (p == GameHandler.CurrPlayerMove)
            {
                foreach(DotHandler d in p.playerDotHandlers)
                {
                    foreach(Connection c in d.Connections)
                    {
                        if (c.origin == MainDot || c.destination == MainDot)
                        {
                            //Handle energy move
                            break;
                        }
                    }
                }
                //handle Connection 
            }
            else
            {
                foreach(DotHandler d in p.playerDotHandlers)
                {
                    if (d.OnMouse)
                    {
                        //handle attacking
                        break;
                    }
                }
            }


        }
        //detect if player wants to establish connection | move Energy | Attack dot
    }
}
