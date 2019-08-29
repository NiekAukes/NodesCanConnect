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
     */
    public GameObject PlayerPreset;
    public GameObject AiPreset;
    public void InitializePlayers()
    {
        //spawn Player
        GameObject pgo = PlayerPreset;
        IPlayer p = pgo.GetComponent<IPlayer>();
        RoundHandler.PlayerList.Enqueue(p);
        DotHandler d = GameHandler.gm.StartPoints[0];
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
    }
}
