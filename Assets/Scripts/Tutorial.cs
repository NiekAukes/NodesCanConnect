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
    public float UIScale = 1.5f;
    public List<Dialog> dialogs = new List<Dialog>();
    public Vector2 DialogPos = new Vector2();
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
        foreach(Dialog dial in dialogs)
        {
            if (dial != null)
                dial.transform.localScale *= UIScale;
        }
        DialogPos = dialogs[0].transform.position;

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
        if (DialogNum > 0)
        {
            DialogPos = dialogs[DialogNum - 1].transform.position;
            dialogs[DialogNum - 1].HideDialog();
        }
        dialogs[DialogNum].ShowDialog();
        dialogs[DialogNum].transform.position = DialogPos;
    }
    public void StartPosition()
    {
        PlayerPreset.transform.position = playerstart.transform.position;
    }
}
