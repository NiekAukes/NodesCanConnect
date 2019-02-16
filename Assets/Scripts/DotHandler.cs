using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DotHandler : MonoBehaviour {
    public bool OnMouse = false;
    public static bool OnDrag = false;
    public int Strength = 0;
    public static DotHandler selectedDot;
    static DotHandler tempSelectedDot = null;
    public static Connection AbsConnection;
    public static int energyOnSelect;
    public GameObject Gfx;
    TextMeshPro text;
    public Player Owner;
    public List<Connection> Connections = new List<Connection>();
    public List<DotFragment> DotFragments = new List<DotFragment>();
    Color color;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        sprite = GetComponentInChildren<SpriteRenderer>();
        text = gameObject.GetComponentInChildren<TextMeshPro>();
        UpdateStrength(0);
        color = sprite.color;
        Owner.playerDotHandlers.Add(this);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (OnDrag)
        {
            try
            {
                if (!(selectedDot.transform.position.y > Owner.cam.ScreenToWorldPoint(Input.mousePosition).y))
                {
                    AbsConnection.AbstractDraw(selectedDot.transform.position, Owner.cam.ScreenToWorldPoint(Input.mousePosition));
                }
                else
                {
                    AbsConnection.AbstractDraw(Owner.cam.ScreenToWorldPoint(Input.mousePosition), selectedDot.transform.position);
                }
            }
            catch (System.Exception e)
            {

                throw e;
            }
        }
    }

    private void OnMouseEnter()
    {
        OnMouse = true;
    }
    private void OnMouseExit()
    {
        OnMouse = false;
    }
    private void OnMouseDown()
    {
        OnRegisterEnter(null);
    }
    private void OnMouseUp()
    {
        OnRegisterRelease();
    }


    /*
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
            //Handle Connection 
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
        } */




    public void OnRegisterEnter(DotFragment dotFragment)
    {
        switch (GameHandler.CurrPlayerMove.playerMode)
        {
            case Player.PlayerMode.Build:

                break;
            case Player.PlayerMode.Select:
                AbsConnection = CreateConnection(transform.position, Owner.cam.ScreenToWorldPoint(Input.mousePosition));
                Debug.Log(AbsConnection);
                OnDrag = true;
                break;
        }
        /*
        switch (GameHandler.CurrPlayerMove.playerMode)
        {
            case Player.PlayerMode.Build:
                if (GameHandler.CurrPlayerMove.playerDotHandlers.Contains(this))
                {

                    //establish Connection or Move Energy or wants to select
                    if (selectedDot == this)
                    {
                        GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                        selectedDot = this;
                        energyOnSelect = DotFragments.Count - dotFragment.level;
                        //select dot
                    }
                }
                break;

            case Player.PlayerMode.Select:
                Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                selectedDot = this;
                break;
        } */
    }
    public void OnRegisterRelease()
    {
        DotHandler MouseDotHandler = null;
        Debug.Log("Release");
        switch (GameHandler.CurrPlayerMove.playerMode)
        {
            case Player.PlayerMode.Build:
                if (GameHandler.CurrPlayerMove.playerDotHandlers.Contains(this))
                {
                    foreach(DotHandler d in Owner.playerDotHandlers)
                    {
                        if(d.OnMouse)
                        {
                            MouseDotHandler = d;
                        }
                    }
                    //establish Connection or Move Energy or wants to cancel
                    if (tempSelectedDot == this)
                    {
                        GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
                        Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                        Debug.Log(selectedDot);
                        Debug.Log(MouseDotHandler);
                        Debug.Log(this);
                        tempSelectedDot = null;
                        //cancelled
                    }
                    else
                    {
                        //Establish connection or Move Energy
                        bool flag_connectionExist = false;
                        foreach (Connection c in FindObjectsOfType<Connection>())
                        {
                            if ((c.origin == selectedDot || c.destination == selectedDot) && (c.origin == MouseDotHandler || c.destination == MouseDotHandler) && !(c.destination != null || c.origin != null))
                            {
                                flag_connectionExist = true;
                                Debug.Log("flag exists");
                                break;
                            }
                        }

                        if (flag_connectionExist)
                        {
                            OnDrag = false;
                            Destroy(AbsConnection.gameObject);
                            //Move Energy
                        }
                        else
                        {
                            OnDrag = false;
                            Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + MouseDotHandler + "  //  " + this);
                            AbsConnection.InitializeAbstractConnection(tempSelectedDot, MouseDotHandler);  //CreateConnection(selectedDot, MouseDotHandler);
                            AbsConnection = null;
                            GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
                            //Establish connection
                        }
                    }
                }
                break;

            case Player.PlayerMode.Select:
                Debug.Log(selectedDot);
                Debug.Log(MouseDotHandler);
                Debug.Log(this);
                GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                tempSelectedDot = this;
                break;
        }
        selectedDot = tempSelectedDot;
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
        to.UpdateStrength(Amount);
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

    public static Connection CreateConnection(Vector2 Origin, Vector2 Destination) //static form for ConnectionCreation
    {
        GameObject temp = Instantiate(GameHandler.gm.ConnectionPrefab, GameHandler.gm.ConnectionFolder) as GameObject;
        Connection ConnectTemp = temp.GetComponent<Connection>();
        ConnectTemp.cube = temp.GetComponentInChildren<SpriteRenderer>().transform;
        ConnectTemp.AbstractDraw(Origin, Destination);
        temp.name = "AbsConnect";
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
        if (increment > 0)
        {
            for (int i = 0; i < increment; i++)
            {
                DotFragment fragment = gameObject.AddComponent<DotFragment>();
                DotFragments.Add(fragment);
                fragment.level = (Strength + i) - increment;
                fragment.MainDot = this;
            }
        }
        else
        {
            for (int i = 0; i > increment; i--)
            {
                Destroy(DotFragments[-1]);
                
            }
        }
    }
}