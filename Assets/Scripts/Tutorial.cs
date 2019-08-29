using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    /*
     Game Tutorial:
        -Introduction
        -Movements
        -Nodes
        -EnergyLevels
        -TakeOvers

    tutorial goals:
        -Move around
        -Take over free node
        -Add energy
        -Take over Enemy node

     */
    public GameObject PlayerPreset;
    public GameObject AiPreset;
    public DotHandler playerstart = null;
    int DialogNum = -1;
    public void InitializePlayers()
    {
        //spawn Player
        GameObject pgo = PlayerPreset;
        IPlayer p = pgo.GetComponent<IPlayer>();
        RoundHandler.PlayerList.Enqueue(p);
        DotHandler d = GameHandler.gm.StartPoints[0];
        playerstart = d;
        pgo.transform.position = d.transform.position;
        d.Owner = p;
        d.UpdateStrength(3);
        p.StartPlayer = true;
        p.isTurn = true;


        //spawn aiPlayer
        pgo = AiPreset;
        p = pgo.GetComponent<IPlayer>();
        RoundHandler.PlayerList.Enqueue(p);
        d = GameHandler.gm.StartPoints[13];
        d.Owner = p;
        d.UpdateStrength(3);

        NextButtonInteraction();
    }

    public void NextButtonInteraction()
    {
        DialogNum++;
        switch(DialogNum)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;

        }
    }
}
