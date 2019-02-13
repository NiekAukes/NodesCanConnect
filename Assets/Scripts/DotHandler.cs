using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DotHandler : MonoBehaviour {
    public bool OnMouse = false;
    public int Strength = 0;
    TextMeshPro text;
    public Player Owner;
    public List<Connection> Connections = new List<Connection>();
    Color color;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        sprite = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        UpdateStrength(0);
        color = sprite.color;
        
	}
	
	// Update is called once per frame
	void Update () { 

    }

    private void OnMouseEnter()
    {
        OnMouse = true;
    }
    private void OnMouseExit()
    {
        OnMouse = false;
    }

    ///DrawMethod for Reject
    private void OnRejectBuild()
    {
        Debug.Log("Rejected");
    }

    ///DrawMethod for HighlightStart
    private void OnHighlightStart()
    {
        //the anchor highlights
        Debug.Log("Highlighted");
        sprite.color = new Color(255f, 0f, 0f);
    }

    ///DrawMethod for HighlightExit
    private void OnHighlightExit()
    {
        sprite.color = color;
    }

    private void OnStrengthZeroEnter()
    {
        sprite.color = new Color(234f, 99f, 42f);
    }

    private void OnStrengthZeroExit()
    {
        sprite.color = new Color(255f, 0f, 0f);
    }


    public static void EnergyMove(DotHandler from, DotHandler to, int Amount)
    {
        from.UpdateStrength(-Amount);
        to.UpdateStrength(-Amount);
    }

    public static Connection CreateConnection(DotHandler Origin, DotHandler Destination) //static form for ConnectionCreation
    {
        GameObject temp = Instantiate(GameHandler.gm.ConnectionPrefab, GameHandler.gm.ConnectionFolder) as GameObject;
        Connection ConnectTemp = temp.GetComponent<Connection>();
        ConnectTemp.cube = temp.GetComponentInChildren<SpriteRenderer>().transform;
        if (Destination.transform.position.y > Origin.transform.position.y)
        {
            ConnectTemp.origin = Origin;
            ConnectTemp.destination = Destination;
        }
        else
        {
            ConnectTemp.origin = Destination;
            ConnectTemp.destination = Origin;
        }
        ConnectTemp.DrawConnection();
        temp.name = Origin.name + "-" + Destination.name;
        Origin.Connections.Add(ConnectTemp);
        Destination.Connections.Add(ConnectTemp);
        return ConnectTemp;
    }

    public static void AttackDot(DotHandler Origin, DotHandler Destination, int Amount)
    {
        //not yet featured
    }

    public void OnSwitchPlayer(Player p)
    {
        Owner = p;
        //add visual changes
    }

    public void UpdateStrength(int increment)
    {
        if (Strength == 0 && increment != 0)
        {
            Strength += increment;
            sprite.color = new Color(255f, 0f, 0f);
        }
        else
        {
            Strength += increment;
            text.text = Strength.ToString();
            sprite.color = new Color(234f, 99f, 42f);
        }
        //handle visual change
    }
}