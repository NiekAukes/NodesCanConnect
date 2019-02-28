using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DotHandler : MonoBehaviour {
    public class ClickRegist
    {
        /*
         ExitCodes:
         0: An Error Occurred
         1: Player Clicked on Same Node
         2: Couldn't move energy because not enough energy was selected
         3: Couldn't take over empty dot, not enough energy was selected
             */
        public DotHandler selectedDot;
        public DotHandler selectedDotBefore;
        public int energyOnSelect;
        public Connection AbsConnection;
        public bool OnDrag = false;
        float counter = 0;
        public ClickRegist(DotHandler selectdot, DotFragment d = null)
        {
            selectedDot = selectdot;
            if (d == null)
                energyOnSelect = selectedDot.Strength;
            else
                energyOnSelect = selectedDot.Strength - d.level + 1;
            AbsConnection = CreateConnection(selectedDot.transform.position, selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition));

            energyOnSelect = selectedDot.Strength - d.level + 1;
            selectedDot.OnDrag = true;
        }
        public void OnUpdate()
        {
            if (!(selectedDot.transform.position.y > selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition).y))
                AbsConnection.AbstractDraw(selectedDot.transform.position, selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition));
            else
                AbsConnection.AbstractDraw(selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition), selectedDot.transform.position);
            //counter for dragging
            if (counter > 0.3)
            {
                counter = 0;
                Debug.Log("Ondrag");
                OnDrag = true;
            }
            else
                counter += Time.deltaTime;
        }
        public void OnRegisterEnter(DotHandler node)
        {
            Debug.Log("Transit");
            selectedDotBefore = selectedDot;
            selectedDot = node;
        }
        public void OnRegisterRelease()
        {
            try
            {
                
                if (OnDrag)
                {
                    foreach(DotHandler d in FindObjectsOfType<DotHandler>())
                    {
                        if (d.OnMouse)
                        {
                            selectedDotBefore = selectedDot;
                            selectedDot = d;
                            break;
                        }
                    }
                    
                }
                if (selectedDot.Owner != null)
                {
                    if (selectedDotBefore != null)
                    {
                        if (GameHandler.CurrPlayerMove.playerDotHandlers.Contains(selectedDot))
                        {
                            //establish Connection or Move Energy or wants to cancel
                            if (selectedDot == selectedDotBefore && selectedDotBefore != null)
                            {
                                CancelBuild(1);
                            }
                            else
                            {
                                //Establish connection or Move Energy
                                bool flag_connectionExist = false;
                                foreach (Connection c in FindObjectsOfType<Connection>())
                                {
                                    if (c != AbsConnection)
                                    {

                                        if ((c.origin == selectedDot && c.destination == selectedDotBefore) || (c.origin == selectedDotBefore || c.destination == selectedDot) && (c.destination != null && c.origin != null))
                                        {
                                            flag_connectionExist = true;
                                            Debug.Log("flag exists");
                                            break;
                                        }
                                    }
                                }

                                if (flag_connectionExist)
                                {
                                    if (energyOnSelect < selectedDotBefore.Strength)
                                    {
                                        selectedDot.OnDrag = false;
                                        Destroy(AbsConnection.gameObject);
                                        AbsConnection = null;
                                        OnBuildEnd();
                                        EnergyMove(selectedDotBefore, selectedDot, energyOnSelect);
                                        Debug.Log("Move Energy // " + energyOnSelect);
                                        //move Energy
                                    }
                                    else
                                    {
                                        CancelBuild(2);
                                    }
                                }
                                else
                                {
                                    selectedDot.OnDrag = false;
                                    Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + selectedDotBefore + "  //  " + this);
                                    AbsConnection.InitializeAbstractConnection(selectedDotBefore, selectedDot);  //CreateConnection(selectedDot, selectedDotBefore);
                                    AbsConnection = null;
                                    OnBuildEnd();
                                    //Establish connection
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    //Dot without owner

                    if (energyOnSelect > 1 && selectedDotBefore.Strength > 2 && selectedDotBefore.Strength != energyOnSelect)
                    {
                        
                        selectedDot.OnDrag = false;
                        selectedDot.Owner = selectedDotBefore.Owner;
                        Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + selectedDotBefore);
                        AbsConnection.InitializeAbstractConnection(selectedDotBefore, selectedDot);  //CreateConnection(selectedDot, selectedDotBefore);
                        AbsConnection = null;
                        Debug.Log("Taken over dot: " + this);
                        EnergyMove(selectedDotBefore, selectedDot, energyOnSelect, 1);
                        GameHandler.CurrPlayerMove.playerDotHandlers.Add(selectedDot);
                        OnBuildEnd();
                        //Taking over empty dot
                    }
                    else
                    {
                        CancelBuild(3);
                    }
                }
            }
            catch (System.Exception e)
            {
                CancelBuild(0, e);
            }
    }
        public void CancelBuild(int exitcode, System.Exception e = null)
        {
            OnBuildEnd();
            Destroy(AbsConnection.gameObject);
            Debug.Log("cancelled, exitcode: " + exitcode);
            if (exitcode == 0)
                Debug.LogError(e);
            //cancelled
        }

        public void OnBuildEnd()
        {
            Debug.Log("Ended Build");
            OnDrag = false;
            clickRegist = null;
        }
    }
    public bool OnMouse = false;
    public bool OnDrag = false;
    public int Strength = 0;
    public GameObject Gfx;
    TextMeshPro text;
    public Player Owner;
    public List<Connection> Connections = new List<Connection>();
    public List<DotFragment> DotFragments = new List<DotFragment>();
    Color color;
    SpriteRenderer sprite;
    public static ClickRegist clickRegist;
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
        if (clickRegist != null)
        {
            if (clickRegist.selectedDot == this && clickRegist.AbsConnection != null)
                clickRegist.OnUpdate();
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
        if (clickRegist == null)
        {
            //first clicked
            clickRegist = new ClickRegist(this);
            Debug.Log("first clicked");
        }
        else
        {
            //second clicked
            clickRegist.OnRegisterEnter(this);
            Debug.Log("second clicked");
        }
    }
    private void OnMouseUp()
    {
        clickRegist.OnRegisterRelease();
    }



    #endregion RuntimeMethods
    #region ClickRegister
   
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
    public static void EnergyMove(DotHandler from, DotHandler to, int Amount, int evaporation = 0)
    {
        from.UpdateStrength(-Amount);
        to.UpdateStrength(Amount - evaporation);
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
                Destroy(DotFragments[DotFragments.Count-1].gameObject);
                DotFragments.RemoveAt(DotFragments.Count - 1);
            }
        }
        Strength += increment;
        text.text = Strength.ToString();

    }
    #endregion UtilityMethods
}