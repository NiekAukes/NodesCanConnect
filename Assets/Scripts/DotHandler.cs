using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DotHandler : MonoBehaviour {
    public bool OnMouse = false;
    public bool OnDrag = false;
    public int Strength = 0;
    public static DotHandler selectedDot;
    static DotHandler tempSelectedDot = null;
    public static Connection AbsConnection;
    public static int energyOnSelect;
    public GameObject Gfx;
    Collider2D w;
    TextMeshPro text;
    float counter = 0;
    public Player Owner;
    public List<Connection> Connections = new List<Connection>();
    public List<DotFragment> DotFragments = new List<DotFragment>();
    Color color;
    SpriteRenderer sprite;
    #region RuntimeHandlers
    // Use this for initialization
    void Start () {
        sprite = GetComponentInChildren<SpriteRenderer>();
        text = gameObject.GetComponentInChildren<TextMeshPro>();
        UpdateStrength(0);
        color = sprite.color;
        if (Owner != null)
            Owner.playerDotHandlers.Add(this);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (OnDrag && selectedDot == this && AbsConnection != null)
        {
            if (!(selectedDot.transform.position.y > Owner.cam.ScreenToWorldPoint(Input.mousePosition).y))
                AbsConnection.AbstractDraw(selectedDot.transform.position, Owner.cam.ScreenToWorldPoint(Input.mousePosition));
            else
                AbsConnection.AbstractDraw(Owner.cam.ScreenToWorldPoint(Input.mousePosition), selectedDot.transform.position);
            if (counter > 0.3)
            {
                GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                counter = 0;
            }
            else
                counter += Time.deltaTime;
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



    #endregion RuntimeMethods
    #region ClickRegister
    public void OnRegisterEnter(DotFragment dotFragment)
    {
       
        switch (GameHandler.CurrPlayerMove.playerMode)
        {
            case Player.PlayerMode.Build:

                break;
            case Player.PlayerMode.Select:
                tempSelectedDot = this;
                AbsConnection = CreateConnection(transform.position, Owner.cam.ScreenToWorldPoint(Input.mousePosition));
                Debug.Log(dotFragment.level);
                
                energyOnSelect = Strength - dotFragment.level + 1;
                OnDrag = true;
                break;
        }
    }
    public void OnRegisterRelease()
    {
        DotHandler MouseDotHandler = null;
        foreach (DotHandler d in FindObjectsOfType<DotHandler>())
        {
            Debug.Log("Searching in: " + d);
            if (d.OnMouse)
            {
                MouseDotHandler = d;
            }
        }
        Debug.Log("Release");
        if (Owner != null)
        {
            switch (GameHandler.CurrPlayerMove.playerMode)
            {
                case Player.PlayerMode.Build:
                    Debug.Log("in build mode");
                   
                    if (GameHandler.CurrPlayerMove.playerDotHandlers.Contains(this))
                    {
                        //establish Connection or Move Energy or wants to cancel
                        if (selectedDot == this)
                        {
                            CancelBuild();
                        }
                        else
                        {
                            //Establish connection or Move Energy
                            bool flag_connectionExist = false;
                            foreach (Connection c in FindObjectsOfType<Connection>())
                            {
                                if (c != AbsConnection)
                                {

                                    Debug.Log((c.origin == selectedDot || c.destination == selectedDot) + "  selectdotcheck");
                                    Debug.Log((c.origin == MouseDotHandler || c.destination == MouseDotHandler) + "  mousedotcheck");
                                    if ((c.origin == selectedDot && c.destination == MouseDotHandler) || (c.origin == MouseDotHandler || c.destination == selectedDot) && (c.destination != null && c.origin != null))
                                    {
                                        flag_connectionExist = true;
                                        Debug.Log("flag exists");
                                        break;
                                    }
                                }
                            }

                            if (flag_connectionExist)
                            {
                                if (energyOnSelect > 1)
                                {
                                    OnDrag = false;
                                    Destroy(AbsConnection.gameObject);
                                    AbsConnection = null;
                                    GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
                                    EnergyMove(tempSelectedDot, MouseDotHandler, energyOnSelect);
                                }
                                else
                                {
                                    CancelBuild();
                                }

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
                    else
                    {
                        if (MouseDotHandler.Strength == 0 && MouseDotHandler.Owner == null)
                        {
                            OnDrag = false;
                            Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + MouseDotHandler + "  //  " + this);
                            AbsConnection.InitializeAbstractConnection(tempSelectedDot, MouseDotHandler);  //CreateConnection(selectedDot, MouseDotHandler);
                            AbsConnection = null;
                            GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
                            Debug.Log("Taken over dot: " + this);
                            //handle empty dot takeover
                        }
                        else if (MouseDotHandler.Owner != Owner)
                        {
                            //player wants to attack other dot
                        }

                    }
                    break;

                case Player.PlayerMode.Select:
                    GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Build;
                    Debug.Log("current mode: " + GameHandler.CurrPlayerMove.playerMode);
                    tempSelectedDot = this;
                    break;
            }
            Debug.Log(energyOnSelect);
            selectedDot = tempSelectedDot;
        }
        else
        {
            //Dot without owner
            OnDrag = false;
            Owner = selectedDot.Owner;
            Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + MouseDotHandler + "  //  " + this);
            AbsConnection.InitializeAbstractConnection(tempSelectedDot, MouseDotHandler);  //CreateConnection(selectedDot, MouseDotHandler);
            AbsConnection = null;
            GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
            Debug.Log("Taken over dot: " + this);
            //Taking over empty dot
        }
    }
    public void CancelBuild()
    {
        GameHandler.CurrPlayerMove.playerMode = Player.PlayerMode.Select;
        Destroy(AbsConnection.gameObject);
        Debug.Log("cancelled");
        tempSelectedDot = null;
        //cancelled
    }
    #endregion ClickRegister
    #region DrawMethods
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

    #endregion DrawMethods
    #region UtilityMehtods
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

    public static DotFragment CreateDotFragment(DotHandler dotHandler, int level)
    {
        float size = level * 1.5f;
        GameObject gameObj = Instantiate(GameHandler.gm.FragmentPrefab, dotHandler.gameObject.transform);
        DotFragment Dotfragment = gameObj.GetComponent<DotFragment>();
        Dotfragment.level = level;
        Dotfragment.Draw();
        return Dotfragment;
    }

    public DotFragment CreateDotFragment(int level)
    {
        float size = level * 1.5f;
        GameObject gameObj = Instantiate(GameHandler.gm.FragmentPrefab, gameObject.transform);
        DotFragment Dotfragment = gameObj.GetComponent<DotFragment>();
        Dotfragment.level = level;
        Dotfragment.Draw();
        return Dotfragment;
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
        //handle visual change
        if (increment > 0)
        {
            for (int i = 0; i < increment; i++)
            {
                DotFragment fragment = CreateDotFragment(Strength + i + 1);
                DotFragments.Add(fragment);
                fragment.level = Strength + i + 1;
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
        Strength += increment;
        text.text = Strength.ToString();

    }
    #endregion UtilityMethods
}