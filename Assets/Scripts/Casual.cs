using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casual : MonoBehaviour
{
    public int players = 2;
    public List<GameObject> PlayerPresets = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < players; i++)
        {
            GameObject pgo = Instantiate(PlayerPresets[i]);
            Player p = pgo.GetComponent<Player>();
            RoundHandler.PlayerList.Enqueue(p);
        }
    }
}
