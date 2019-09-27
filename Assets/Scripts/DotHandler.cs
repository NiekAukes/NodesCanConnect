using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DotHandler : MonoBehaviour {
    public class ClickRegist
    {
        /*
         ExitCodes:
         0: An Error Occurred
         1: Cancelled from outside class (check message)
         2: Player Clicked on Same Node
         3: Couldn't move energy because not enough energy was selected
         4: Couldn't take over empty dot, not enough energy was selected
         5: Couldn't create Connection, other connection interfering
         6: Couldn't Select node, Player does not own this node
             */
        public DotHandler selectedDot;
        public DotHandler selectedDotBefore;
        public int energyOnSelect;
        public Connection AbsConnection;
        public bool OnDrag = false;
        public bool SecondSelected= false;
        float counter = 0;


        public ClickRegist(DotHandler selectdot, DotFragment d = null)
        {
            if (!RoundHandler.energyDistributionState)
            {
                Debug.Log("dotfragments: " + (selectdot.DotFragments.Count) + "  //  " + d.level);
                for (int i = selectdot.DotFragments.Count - 1; i > d.level - 1; i--)
                {
                    Debug.Log("dotfragments: " + selectdot.DotFragments[i]);
                    selectdot.DotFragments[i].SelectColor(true);
                }
                if (selectdot.Owner != RoundHandler.CurrPlayerMove)
                {
                    CancelBuild(6);
                }
                //checks if the player owns this node

                selectedDot = selectdot;


                if (d == null)
                    energyOnSelect = selectedDot.Strength;
                else
                    energyOnSelect = selectedDot.Strength - d.level + 1;
                //initializes energy selected

                AbsConnection = CreateConnection(selectedDot.transform.position, selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition));
                AbsConnection.cube.gameObject.layer = 9;
                selectedDot.OnDrag = true;
                //creates AbsConnection and sets drag to true

                Debug.Log(energyOnSelect + "  //  " + d.level);
                foreach (Connection c in selectdot.Connections)
                {
                    if (c != null)
                    {
                        c.cube.gameObject.layer = 9;
                    }
                }
            }
        }
        public void OnUpdate()
        {
            if (selectedDot.Owner.GetType() == typeof(Player))
            {
                if (!(selectedDot.transform.position.y > selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition).y))
                    AbsConnection.AbstractDraw(selectedDot.transform.position, selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition));
                else
                    AbsConnection.AbstractDraw(selectedDot.Owner.cam.ScreenToWorldPoint(Input.mousePosition), selectedDot.transform.position);
            }


            //counter for dragging
            if (counter > 0.3)
            {
                counter = 0;
                OnDrag = true;
            }
            else
                counter += Time.deltaTime;
        }


        public void OnRegisterEnter(DotHandler node)
        {
            if (!RoundHandler.energyDistributionState && selectedDot != null)
            {
                selectedDotBefore = selectedDot;
                selectedDot = node;
                Debug.Log(selectedDotBefore + " // " + selectedDot);
                SecondSelected = true;

                if (selectedDot.Owner != null && selectedDotBefore.Owner != selectedDot.Owner)
                {
                    Destroy(AbsConnection.gameObject);
                    AbsConnection = null;
                }
            }
        }


        public void OnRegisterRelease()
        {
            if (!RoundHandler.energyDistributionState)
            {
                if (selectedDot != null)
                {
                    try
                    {

                        if (OnDrag && !SecondSelected)
                        {
                            Debug.Log("past here");
                            foreach (DotHandler d in FindObjectsOfType<DotHandler>())
                            {
                                if (d.OnMouse)
                                {
                                    selectedDotBefore = selectedDot;
                                    selectedDot = d;
                                    break;
                                }
                            }

                        }
                        if (selectedDot.Owner == RoundHandler.CurrPlayerMove)
                        {
                            if (selectedDotBefore != null)
                            {
                                if (RoundHandler.CurrPlayerMove.playerDotHandlers.Contains(selectedDot))
                                {
                                    //establish Connection or Move Energy or wants to cancel
                                    if (selectedDot == selectedDotBefore)
                                    {
                                        CancelBuild(2);
                                    }
                                    else
                                    {
                                        if (Connection.FindConnectionBetween(selectedDot, selectedDotBefore) != null)
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
                                                CancelBuild(3);
                                            }
                                        }
                                        if (!AbsConnection.cannotBuild)
                                        {
                                            selectedDot.OnDrag = false;
                                            Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + selectedDotBefore + "  //  " + this);
                                            AbsConnection.InitializeAbstractConnection(selectedDotBefore, selectedDot);  //CreateConnection(selectedDot, selectedDotBefore);
                                            AbsConnection = null;
                                            OnBuildEnd();
                                            EnergyMove(selectedDotBefore, selectedDot, energyOnSelect, 1);
                                            //Establish connection
                                        }
                                        else
                                        {
                                            //Move Energy over multiple nodes
                                            CancelBuild(5);
                                        }

                                    }
                                }

                            }
                        }
                        else
                        {
                            //Dot without owner or Dot owned by other player
                            if (selectedDot.Owner == null)
                            {
                                //Dot without owner
                                Debug.Log((energyOnSelect) + "  //  " + (selectedDotBefore) + "  //  " + (selectedDot) + "  //  " + (selectedDotBefore.Strength != energyOnSelect));
                                if (energyOnSelect > 1 && selectedDotBefore.Strength > 2 && selectedDotBefore.Strength != energyOnSelect && !AbsConnection.cannotBuild)
                                {

                                    selectedDot.OnDrag = false;
                                    selectedDot.Owner = selectedDotBefore.Owner;
                                    Debug.Log("Creating Connection  //  " + selectedDot + "  //  " + selectedDotBefore);
                                    AbsConnection.InitializeAbstractConnection(selectedDotBefore, selectedDot);  //CreateConnection(selectedDot, selectedDotBefore);
                                    AbsConnection = null;
                                    Debug.Log("Taken over dot: " + this);
                                    EnergyMove(selectedDotBefore, selectedDot, energyOnSelect, 1);
                                    RoundHandler.CurrPlayerMove.playerDotHandlers.Add(selectedDot);
                                    OnBuildEnd();
                                    //Taking over empty dot
                                }
                                else
                                {
                                    CancelBuild(4);
                                }
                            }
                            else
                            {
                                //player wants to attack other dot

                                Debug.Log(RoundHandler.CurrPlayerMove + " attacks " + selectedDot.Owner + "  //  " + energyOnSelect);
                                Debug.Log(selectedDotBefore + "  //  " + selectedDot);
                                if (selectedDotBefore != selectedDot) //small bug might be appearant where selectedDot = selectedDotBefore
                                {
                                    selectedDotBefore.AttackDot(selectedDot, energyOnSelect);
                                }

                                OnBuildEnd();

                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        CancelBuild(0, "", e);
                    }
                }
            }
    }
        public void CancelBuild(int exitcode, string msg = "", System.Exception e = null)
        {
            if (AbsConnection != null)
                Destroy(AbsConnection.gameObject);
            Debug.Log(msg + " cancelled, exitcode: " + exitcode);
            if (exitcode == 0)
                Debug.LogError(e);
            OnBuildEnd();
            //cancelled
        }

        public void OnBuildEnd()
        {
            //finishes the build event in order to prevent exceptions
            Debug.Log("Ended Build");
            OnDrag = false;
            RoundHandler.CheckGold();
            

            //restores layers and lists for selecteddot and selecteddotbefore
            if (selectedDot != null)
            {
                foreach (Connection c in selectedDot.Connections)
                {
                    c.cube.gameObject.layer = 8;
                }
                selectedDot.UpdateList();
                selectedDot.UpdateRecognition();
            }
            //restores color of selected fragments
            foreach (DotFragment fragment in selectedDotBefore.DotFragments)
            {
                fragment.SelectColor(true);
            }
            foreach (DotFragment fragment in selectedDot.DotFragments)
            {
                fragment.SelectColor(true);
            }
            if (selectedDotBefore != null)
            {
                foreach (Connection c in selectedDotBefore.Connections)
                {
                    c.cube.gameObject.layer = 8;
                }
                selectedDotBefore.UpdateList();
                selectedDotBefore.UpdateRecognition();
            }
            clickRegist = null;
            //Signals to the connections to revert all layers
            foreach (Connection c in FindObjectsOfType<Connection>())
            {
                c.cube.gameObject.layer = 8;
            }
        }
    }
    public bool OnMouse = false;
    public bool OnDrag = false;
    public int Strength = 0;
    public int MaxStrength = 6;
    public int elevation = 0;
    public GameObject Gfx;
    TextMeshPro text;
    public IPlayer Owner;
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
            if ((clickRegist.selectedDot == this && Strength != 0 && clickRegist.AbsConnection != null))
                clickRegist.OnUpdate();
        }
        if (Input.GetButtonDown("Fire2") && clickRegist != null)
        {
            clickRegist.CancelBuild(1);
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
        if (clickRegist != null)
        {
            clickRegist.OnRegisterRelease();
        }
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

    public void UpdateRecognition()
    {
        UpdateList();
        foreach (DotFragment d in DotFragments)
        {
            d.Frame.color = Owner.playercolor;
        }
        foreach (Connection c in Connections)
        {
            c.GetComponentInChildren<SpriteRenderer>().color = Owner.playercolor;
        }
    }

    #endregion DrawMethods
    #region UtilityMehtods

    public void UpdateList()
    {
        //filters null values out of lists
        for (int i = 0; i < Connections.Count; i++)
        {
            if (Connections[i] == null)
            {
                Connections.RemoveAt(i);
            }
        }
        for (int i = 0; i < DotFragments.Count; i++)
        {
            if (DotFragments[i] == null)
            {
                DotFragments.RemoveAt(i);
            }
        }
    }

    public static bool EnergyMove(DotHandler from, DotHandler to, int Amount, int evaporation = 0)
    {
        if (!(from.Strength - Amount < 1) && !(to.Strength + Amount - evaporation > 6))
        {
            from.UpdateStrength(-Amount);
            to.UpdateStrength(Amount - evaporation);
            return true;
        }
        return false;
    }

    public static Connection CreateConnection(DotHandler Origin, DotHandler Destination) //static form for ConnectionCreation
    {
        GameObject temp = Instantiate(GameHandler.gm.ConnectionPrefab, GameHandler.gm.ConnectionFolder) as GameObject;
        temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y, 1f);
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
        temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y, 1);
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
        dotHandler.UpdateRecognition();
        return Dotfragment;
    }

    public DotFragment CreateDotFragment(int level)
    {
        float size = level * 1.5f;
        GameObject gameObj = Instantiate(GameHandler.gm.FragmentPrefab, gameObject.transform);
        DotFragment Dotfragment = gameObj.GetComponent<DotFragment>();
        Dotfragment.level = level;
        Dotfragment.MainDot = this;
        Dotfragment.Draw();
        UpdateRecognition();
        return Dotfragment;
    }

    public DotHandler[] GetNearbyDotHandlers(float Multiplier)
    {
        List<DotHandler> dotHandlers = new List<DotHandler>();
        Collider2D[] temp = Physics2D.OverlapCircleAll(transform.position, GameHandler.gm.tilling.Radius * Multiplier);

        for(int i = 0; i < temp.Length; i++)
        {
            DotHandler checkD = temp[i].gameObject.GetComponent<DotHandler>();
            if (checkD != null)
                dotHandlers.Add(checkD);
        }
        return dotHandlers.ToArray();
    }
    
    public void AttackDot(DotHandler Destination, int Amount)
    {
        if (Amount > 0 && Amount < 20 && Strength > 1)
        {
            //if in the right conditions to make an attack

            if (Destination.Strength <= 1)
            {
                //takeover attack
                Destination.OnSwitchPlayer(Owner);
                CreateConnection(this, Destination);
                UpdateList();
                Destination.UpdateList();
                UpdateStrength(-Amount);
                Destination.UpdateStrength(Amount - 1);
                

            }
            else
            {
                Debug.Log("Amount: " + Amount);
                //normal attack (both nodes damage eachother)
                UpdateStrength(-1);
                Destination.UpdateStrength(-1);
                AttackDot(Destination, Amount - 1);
            }
        }
        

    }

    public void OnSwitchPlayer(IPlayer p)
    {
        for(int i = 0; i < Owner.playerDotHandlers.Count; i++)
        {
            if (Owner.playerDotHandlers[i] == this)
            {
                Owner.playerDotHandlers[i] = null;
            }
        }
        for (int i = 0; i < Connections.Count; i++)
        {
            if (Connections[i] != null)
                Connections[i].DestroyConnection();
        }
        Owner = p;
        p.playerDotHandlers.Add(this);
    }

    public void CreateBlueRing()
    {
       for (int i = 0; i < DotFragments.Count; i++)
        {
            if (DotFragments[i].BlueRing)
            {
                Debug.Log("Destroyed: " + DotFragments[i].gameObject);
                Destroy(DotFragments[i].gameObject);
                DotFragments.RemoveAt(i);
            }
        }
        DotFragment fragment = CreateDotFragment(Strength + 1);
        fragment.gameObject.name = "Blue Ring";
        fragment.BlueRing = true;
        fragment.Draw();
        DotFragments.Add(fragment);
        fragment.level = Strength + 1;
        fragment.MainDot = this;
    }

    public void UpdateStrength(int increment)
    {
        if (-Strength - 1 < increment)
        {
            //handle visual change
            int times = 0;
            int i = increment;
            while (increment != 0 || times > 100)
            {
                if (increment > 0)
                {
                    DotFragment fragment = CreateDotFragment(Strength + 1);
                    DotFragments.Add(fragment);
                    fragment.level = Strength + 1;
                    fragment.MainDot = this;
                    i++;
                    increment--;
                    Strength++;
                }
                if (increment < 0)
                {
                    if (DotFragments.Count > 0)
                    {
                        Destroy(DotFragments[DotFragments.Count - 1].gameObject);
                        DotFragments.RemoveAt(DotFragments.Count - 1);
                        increment++;
                    }
                    else
                    {
                        Debug.Log("could not change strength, an error occured: " + increment + "  //  " + DotFragments.Count);
                    }
                    Strength--;
                }
                times++;
            }
        }
        else
        {
            Debug.Log("could not do that: " + -Strength + "  //  " + increment);
        }
    }
    #endregion UtilityMethods
}
