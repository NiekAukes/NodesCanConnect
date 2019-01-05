﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameHandler : MonoBehaviour {
    /* To-Do
     * -Move Static Methods to GameHandler - #Done
     * -Add Highlights OnMouseOver etc. - #working
     * -Add NodeOwners - #Working
     * -Add Strength interaction 
     * 
     */

    /* Bugs-To-Fix
     * second choise RC on anchor doesn't call OnHighlightExit - Will be fixed on Future Build
     * Player Movement doesn't work
        */


    #region Variables
    [System.Serializable]
    public class Tilling
    {
        public enum TillingMode {None, Hexagonal, Triagonal, Random }
        public TillingMode tillingMode;
        public int XLength, YLength;
        public float Radius;
    }
    public static GameHandler gm;
    public static bool BuildMode = false, OnOver = false;
    public static DotHandler CurrDot;
    public static Player CurrPlayerMove;
    public static Queue<Player> PlayerList = new Queue<Player>();
    [SerializeField]private GameObject ConnectionPrefab, NodePrefab, AnchorPrefab; //AnchorPrefab
    [SerializeField]private DotHandler tst1, tst2, tst3; //declares Dots in code
    [SerializeField]private Transform ConnectionFolder, AnchorFolder, NodeFolder;
    public Tilling tilling;

    #endregion Variables

    #region RuntimeHandlers
    // Use this for initialization
    void Start () {
        gm = this;
        if (!(tst1 == null) && !(tst2 == null) && !(tst3 == null))
        {
            CreateConnection(tst1, tst2); //draws connection between tst1 and tst2 (Debug)
            CreateConnection(tst2, tst3); //draws connection between tst2 and tst3 (Debug)
            CreateConnection(tst1, tst3); //draws connection between tst1 and tst3 (Debug)
        }
        if (tilling.tillingMode == Tilling.TillingMode.Hexagonal)
            HexagonalTilling(tilling.XLength, Mathf.FloorToInt(tilling.YLength/2) + 1, tilling.Radius);
            HexagonalTilling(3, 3f);
    }
	
	// Update is called once per frame
	void Update () {
        if (BuildMode && Input.GetButton("Fire2") && !OnOver) //if the user Right-clicked while in build mode, it disables buildmode
        {
            BuildMode = false;
            CurrDot = null;
        }
        
    }

    #endregion RuntimeHandlers

    #region StaticMethods
    public static AnchorHandler CreateAnchor(Vector2 Position) ///Creates Anchor on Position Parameter
    {
        GameObject gm_Anchor = Instantiate(gm.AnchorPrefab, Position, Quaternion.Euler(0f, 0f, 0f), gm.AnchorFolder);
        AnchorHandler anchor = gm_Anchor.GetComponent<AnchorHandler>();
        return anchor;
    }


    public static Connection CreateConnection(DotHandler Origin, DotHandler Destination) //static form for ConnectionCreation
    {
        GameObject temp = Instantiate(gm.ConnectionPrefab, gm.ConnectionFolder) as GameObject;
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
        return ConnectTemp;
    }


    public static Connection CreateBindConnection(DotHandler ClickDot) //will create a new connection between 2 existing dots
    {
        bool alreadyExisting = false;
        foreach (Connection i in gm.ConnectionFolder.GetComponentsInChildren<Connection>())
        {
            if ((i.origin == CurrDot && i.destination == ClickDot) | (i.destination == CurrDot && i.origin == ClickDot))
            {
                alreadyExisting = true;
                return i;
            }
        }
        if (!alreadyExisting)
        {
            Debug.Log(CurrDot + " created " + ClickDot);
            Connection c = CreateConnection(ClickDot, CurrDot);
            CurrDot.UpdateStrength(-1);
            return c;
        }
        return null;
    } 

    
    #endregion StaticMethods

    #region Encapsulation
    public static bool SwitchBuildMode()
    {
        BuildMode = !BuildMode;
        return BuildMode;

    }
    public static Player GetCurrPlayer(Player p = null)
    {
        if (p != null)
        {
            CurrPlayerMove = p;
        }
        else
            return CurrPlayerMove;
        return null;
    }
    public static Player[] GetPlayerList()
    {
        return PlayerList.ToArray();
    }
    public static GameObject GetConnectionPrefab()
    {
        return gm.ConnectionPrefab;
    }
    public static GameObject GetNodePrefab()
    {
        return gm.NodePrefab;
    }
    public static GameObject GetAnchorPrefab()
    {
        return gm.AnchorPrefab;
    }
    public static Transform GetConnectionFolder()
    {
        return gm.ConnectionFolder;
    }
    public static Transform GetNodeFolder()
    {
        return gm.NodeFolder;
    }
    public static Transform GetAnchorFolder()
    {
        return gm.AnchorFolder;
    }

    #endregion Encapsulation

    #region RoundHandler
    public static Player NextPlayer()
    {
        bool Failed = false;
        if (!Failed)
        {
            CurrPlayerMove = PlayerList.Dequeue();
            PlayerList.Enqueue(CurrPlayerMove);
            return CurrPlayerMove;
        }
        else
        {
            Debug.LogError("Failed to Switch to next player");
            return null;
        }
    }
    #endregion RoundHandler

    #region Tilling


    public static AnchorHandler[] HexagonalTilling(int rangeX, int rangeY, float radius) ///Hexagonal Tilling
    {

        List<Vector2> seen = new List<Vector2>();
        List<AnchorHandler> anchors = new List<AnchorHandler>();
        Vector2 center = new Vector2(0f, 0f);
        int posX = 0;
        int posY = 0;
        int timesrun = 0;
        Debug.Log(center.x + " not Looping " + rangeX);
        while (posY != rangeY && timesrun < rangeX * rangeY && timesrun < 120)
        {
            timesrun++;
            for (int i = 0; i < 6; i++)
            {
                float angleRad = (60 * i - 30) * Mathf.Deg2Rad;
                Vector2 p = new Vector2(center.x + radius * Mathf.Cos(angleRad), center.y + radius * Mathf.Sin(angleRad));
                p.x = Mathf.Round(p.x * 10000) / 10000;
                p.y = Mathf.Round(p.y * 10000) / 10000;
                if (!seen.Contains(p))
                {
                    seen.Add(p);
                    AnchorHandler anchor = CreateAnchor(p);
                    anchor.name = "Hex(" + posX + ", " + posY + ")[" + i + "]";
                    anchors.Add(anchor);

                }

            }
            if (posX == rangeX - 1)
            {
                posY++;
                posX = 0;
                center.x = 0f;
                center.y += (radius + 0.5f * radius) * 2;
            }
            else
            {
                posX++;
                center.x += Mathf.Cos(30 * Mathf.Deg2Rad) * radius * 2;
            }
        }

        return anchors.ToArray();
    }

    public static AnchorHandler[] HexagonalTilling(int range, float radius) ///Hexagonal Tilling
    {
        int currRange = 0;
        List<Vector2> seen = new List<Vector2>();
        List<AnchorHandler> anchors = new List<AnchorHandler>();
        Vector2 center = new Vector2(0f, 0f);
        Vector2 currCenter = center;
        while (currRange != range)
        {
            for (int hexag = 0; hexag < (6 * currRange != 0 ? 6 * currRange : 1); hexag++)
            {
                float HexagAngle = 0;
                if (currRange != 0 && hexag % 2 == 0)
                {
                    HexagAngle = (60 / currRange * hexag) * Mathf.Deg2Rad;
                }
                else
                    HexagAngle = currRange > 1 ? (60 / currRange * hexag + 30 / currRange) * Mathf.Deg2Rad : 0;

                currCenter = new Vector2((center.x + radius * currRange * Mathf.Cos(HexagAngle)) * Mathf.Sqrt(3),(center.y + radius * currRange * Mathf.Sin(HexagAngle)) * 7/4);

                Debug.Log(HexagAngle * Mathf.Rad2Deg + " " + hexag + " " + currRange + " " + radius * currRange + " " + currCenter.x + "/" + currCenter.y);
                for (int i = 0; i < 6; i++)
                {
                    float angleRad = (60 * i - 30) * Mathf.Deg2Rad;
                    Vector2 p = new Vector2(currCenter.x + radius * Mathf.Cos(angleRad), currCenter.y + radius * Mathf.Sin(angleRad));
                    p.x = Mathf.Round(p.x * 1000) / 1000;
                    p.y = Mathf.Round(p.y * 1000) / 1000;
                    if (!seen.Contains(p))
                    {
                        seen.Add(p);
                        AnchorHandler anchor = CreateAnchor(p);
                        anchor.name = "Hex(" + currRange + ")[" + hexag + "][" + i + "]";
                        anchors.Add(anchor);

                    }
                }
                
                
            }
            currRange++;
        }
        return null;
    }
    #endregion Tilling 
}