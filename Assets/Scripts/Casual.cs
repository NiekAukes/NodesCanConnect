using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casual : MonoBehaviour
{
    public int players = 2;
    public List<GameObject> PlayerPresets = new List<GameObject>();

    public void InitializePlayers()
    {
        for (int i = 0; i < players; i++)
        {
            GameObject pgo = Instantiate(PlayerPresets[i]);
            Player p = pgo.GetComponent<Player>();
            RoundHandler.PlayerList.Enqueue(p);
            int RandNum = (int)(Random.value * GameHandler.gm.StartPoints.Count - 1);
            DotHandler d = GameHandler.gm.StartPoints[RandNum];
            d.Owner = p;
            d.UpdateStrength(3);
            if (i == 0)
            {
                p.StartPlayer = true;
                p.isTurn = true;
            }
        }
    }
}
