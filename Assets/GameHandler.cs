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
        public int Range;
        public float Radius;
    }
    public static GameHandler gm;
    public static bool OnOver = false;
    public static DotHandler CurrDot;
    public Player CurrPlayerMove;
    public Queue<Player> PlayerList = new Queue<Player>();
    [SerializeField]private GameObject ConnectionPrefab, NodePrefab, AnchorPrefab; //AnchorPrefab
    [SerializeField]private DotHandler tst1, tst2, tst3; //declares Dots in code
    [SerializeField]private Transform ConnectionFolder, AnchorFolder, NodeFolder;
    public Tilling tilling;

    #endregion Variables

    #region RuntimeHandlers
    // Use this for initialization
    void Start () {
        gm = this;
        Debug.Log(PlayerList);
        Player.SelectStartPlayer();
        if (!(tst1 == null) && !(tst2 == null) && !(tst3 == null))
        {
            CreateConnection(tst1, tst2); //draws connection between tst1 and tst2 (Debug)
            CreateConnection(tst2, tst3); //draws connection between tst2 and tst3 (Debug)
            CreateConnection(tst1, tst3); //draws connection between tst1 and tst3 (Debug)
        }
        if (tilling.tillingMode == Tilling.TillingMode.Hexagonal)
            HexagonalTilling(tilling.Range, tilling.Radius);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(CurrPlayerMove);
        if (CurrPlayerMove.playerMode == Player.PlayerMode.Build && Input.GetButton("Fire2") && !OnOver) //if the user Right-clicked while in build mode, it disables buildmode
        {
            CurrPlayerMove.playerMode = Player.PlayerMode.Selection;
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
    
    public static Player[] GetPlayerList()
    {
        return gm.PlayerList.ToArray();
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
            gm.CurrPlayerMove.playerMode = Player.PlayerMode.Idle;
            gm.CurrPlayerMove = gm.PlayerList.Dequeue();
            gm.PlayerList.Enqueue(gm.CurrPlayerMove);
            gm.CurrPlayerMove.playerMode = Player.PlayerMode.Selection;
            return gm.CurrPlayerMove;
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
        Vector2 N_vPos, P_vPos, N_vNeg, P_vNeg;
        float deltaX = Mathf.Cos(60 * Mathf.Deg2Rad) * radius * Mathf.Sqrt(3f), deltaY = radius * 3/2;

        while (currRange != range)
        {
            int hexag;
            P_vPos = new Vector2(center.x + radius * currRange * Mathf.Sqrt(3f), 0);
            N_vPos = new Vector2(center.x - radius * currRange * Mathf.Sqrt(3f), 0);
            P_vNeg = P_vPos;
            N_vNeg = N_vPos;

            int iter = 0;
            hexag = 0;
            List<Vector2> t = new List<Vector2> {P_vPos, N_vPos};
            while (hexag != currRange)
            {
                
                P_vPos = new Vector2(P_vPos.x - deltaX, P_vPos.y + deltaY);
                P_vNeg = new Vector2(P_vNeg.x - deltaX, P_vNeg.y - deltaY);
                N_vPos = new Vector2(N_vPos.x + deltaX, N_vPos.y + deltaY);
                N_vNeg = new Vector2(N_vNeg.x + deltaX, N_vNeg.y - deltaY);
                t.Add(P_vPos);
                t.Add(P_vNeg);
                t.Add(N_vPos);
                t.Add(N_vNeg);

                hexag++;
            }
            if (currRange > 1) 
            {
                
                int p = currRange - 1;
                Vector2 fillCenter = new Vector2(-(p - 1) * deltaX, currRange * radius * 3/2);
                int h = 0;
                while(h < p) 
                {
                    t.Add(fillCenter);
                    t.Add(new Vector2(fillCenter.x, -fillCenter.y));
                    fillCenter = new Vector2(fillCenter.x + deltaX * 2, fillCenter.y);
                    
                    h++;
                }
            }
            foreach(Vector2 c in t)
                {
                    
                    currCenter = c;
                    Debug.Log(c);
                    for (int i = 0; i < 6; i++)
                    {
                        float angleRad = (60 * i - 30) * Mathf.Deg2Rad;
                        Vector2 p = new Vector2(currCenter.x + radius * Mathf.Cos(angleRad), currCenter.y + radius * Mathf.Sin(angleRad));
                        p.x = Mathf.Round(p.x * 10000) / 10000;
                        p.y = Mathf.Round(p.y * 10000) / 10000;
                        if (!seen.Contains(p))
                        {
                            seen.Add(p);
                            AnchorHandler anchor = CreateAnchor(p);
                            anchor.name = "Hex(" + currRange + ")[" + iter + "][" + i + "]";
                            anchors.Add(anchor);

                        }
                    }
                    iter++;
                }
            
            currRange++;
        }




            
        return null;
    }
    
    #endregion Tilling 
}