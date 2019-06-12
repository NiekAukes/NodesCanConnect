using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundHandler : MonoBehaviour
{
    public static Player CurrPlayerMove;
    public static Queue<Player> PlayerList = new Queue<Player>();


    public static Player StartGame()
    {
        bool Failed = false;
        if (!Failed)
        {
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
    public static Player NextPlayer()
    {
        bool Failed = false;
        if (!Failed)
        {
            CurrPlayerMove = PlayerList.Dequeue();
            PlayerList.Enqueue(CurrPlayerMove);
            CurrPlayerMove.cam.enabled = false;

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("ctrl");
            NextPlayer();
        }
    }
}
