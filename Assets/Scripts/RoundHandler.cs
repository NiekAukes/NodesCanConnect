using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundHandler : MonoBehaviour
{
    public static IPlayer CurrPlayerMove;
    public static Queue<IPlayer> PlayerList = new Queue<IPlayer>();
    public static bool energyDistributionState = false;
    public static int amount = 0;


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
            if (CurrPlayerMove.cam != null)
            {
                CurrPlayerMove.cam.enabled = false;
            }

            //assigns new player turn
            CurrPlayerMove = PlayerList.ToArray()[0];
            if (CurrPlayerMove.cam != null)
            {
                CurrPlayerMove.cam.enabled = true;
            }
            Debug.Log("Next round // " + CurrPlayerMove);

            Debug.Log("Decide: " + CurrPlayerMove.GetType());
            if (CurrPlayerMove.GetType() == typeof(AiCasPlayer))
            {
                (CurrPlayerMove as AiCasPlayer).decide();

                Debug.Log("Decide");
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

        //sets amount for new energy
        amount = CurrPlayerMove.playerDotHandlers.Count;

        foreach (DotHandler d in CurrPlayerMove.playerDotHandlers)
        {
            if (d != null && d.Strength < d.MaxStrength)
            {
                d.UpdateList();
                d.CreateBlueRing();
            }
        }
    }

    public static void ExitEnergySetState()
    {
        foreach (DotHandler d in CurrPlayerMove.playerDotHandlers)
        {
            if (d != null)
            {
                for (int i = 0; i < d.DotFragments.Count; i++)
                {
                    if (d.DotFragments[i].BlueRing)
                    {
                        Debug.Log("Destroyed: " + d.DotFragments[i].gameObject);
                        Destroy(d.DotFragments[i].gameObject);
                        d.DotFragments.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void Update()
    {
        //end turn by pressing button
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("ctrl");
            if (energyDistributionState)
            {
                ExitEnergySetState();
                NextPlayer();
                energyDistributionState = false;
            }
            else
            {
                EnterEnergySetState();
                energyDistributionState = true;
            }
        }
        if (amount < 1 && energyDistributionState)
        {
            ExitEnergySetState();
        }
    }
}
