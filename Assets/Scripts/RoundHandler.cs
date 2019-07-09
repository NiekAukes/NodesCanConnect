using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundHandler : MonoBehaviour
{
    public static IPlayer CurrPlayerMove;
    public static Queue<IPlayer> PlayerList = new Queue<IPlayer>();


    public static IPlayer StartGame()
    {
        bool Failed = false;
        if (!Failed)
        {
            //sets currplayermove and assigns first turn
            CurrPlayerMove = PlayerList.ToArray()[0];
            CurrPlayerMove.cam.enabled = true;
            Debug.Log("Next round // " + CurrPlayerMove);

            return CurrPlayerMove;
        }
        else
        {
            Debug.LogError("Failed to Switch to next player");
            return null;
        }
    }
    public static IPlayer NextPlayer()
    {
        bool Failed = false;
        if (!Failed)
        {
            //dequeues and enqueues the player 
            CurrPlayerMove = PlayerList.Dequeue();
            PlayerList.Enqueue(CurrPlayerMove);
            CurrPlayerMove.cam.enabled = false;

            //assigns new player turn
            CurrPlayerMove = PlayerList.ToArray()[0];
            CurrPlayerMove.cam.enabled = true;
            Debug.Log("Next round // " + CurrPlayerMove);

            if (CurrPlayerMove.GetType() == typeof(AiCasPlayer))
            {
                (CurrPlayerMove as AiCasPlayer).decide();
            }
            return CurrPlayerMove;

        }
        else
        {
            Debug.LogError("Failed to Switch to next player");
            return null;
        }
    }

    public static void EnterEnergySetState()
    {

    }

    private void Update()
    {
        //end turn by pressing button
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("ctrl");
            NextPlayer();
        }
    }
}
