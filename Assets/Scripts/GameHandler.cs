using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameHandler : MonoBehaviour {


    #region Variables
    [System.Serializable]
    public class Tilling
    {
        public enum TillingMode {None, Hexagonal, Triagonal, Random }
        public TillingMode tillingMode;
        public int Range;
        public float Radius;
        public Vector2 Center;
    }
    public static GameHandler gm;
    public static RoundHandler rm;
    public static Casual casm;
    public static bool BuildMode = false, OnOver = false;
    public static DotHandler CurrDot;
    public GameObject ConnectionPrefab, NodePrefab, FragmentPrefab; //AnchorPrefab
    [SerializeField]private DotHandler tst1, tst2, tst3; //declares Dots in code
    public Transform ConnectionFolder, AnchorFolder, NodeFolder;
    public Tilling tilling;
    public List<DotHandler> StartPoints = new List<DotHandler>();

    #endregion Variables

    #region RuntimeHandlers
    // Use this for initialization
    void Start () {
        gm = this;
        rm = FindObjectOfType<RoundHandler>();
        casm = FindObjectOfType<Casual>();
        Application.targetFrameRate = 1000;
        QualitySettings.vSyncCount = 0;
        if (tilling.tillingMode == Tilling.TillingMode.Hexagonal)
            HexagonalTilling(tilling.Range, tilling.Radius, tilling.Center);

        casm.InitializePlayers();
        RoundHandler.StartGame();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach(DotHandler d in RoundHandler.CurrPlayerMove.playerDotHandlers)
            {
                d.UpdateStrength(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /*using (System.IO.StreamWriter logfile = new System.IO.StreamWriter(@"C:\Logs\NCC\LogFile" + System.DateTime.Today.ToString()))
            {
                logfile.Write
            } */
            Application.Quit();
        }
        
    }

    #endregion RuntimeHandlers

    #region StaticMethods
    public static DotHandler CreateNode(Vector2 Position) ///Creates Anchor on Position Parameter
    {
        GameObject objectTemp = Instantiate(gm.NodePrefab, Position, new Quaternion(0, 0, 0, 0), gm.NodeFolder);
        return objectTemp.GetComponent<DotHandler>();
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
        Origin.Connections.Add(ConnectTemp);
        Destination.Connections.Add(ConnectTemp);
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
            RoundHandler.CurrPlayerMove = p;
        }
        else
            return RoundHandler.CurrPlayerMove;
        return null;
    }
    public static Player[] GetPlayerList()
    {
        return RoundHandler.PlayerList.ToArray();
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
        return gm.NodePrefab;
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


    #region Tilling


    public static DotHandler[] HexagonalTilling(int rangeX, int rangeY, float radius, Vector2 offset) ///Hexagonal Tilling
    {

        List<Vector2> seen = new List<Vector2>();
        List<DotHandler> anchors = new List<DotHandler>();
        Vector2 center = offset;
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
                    DotHandler anchor = CreateNode(p);
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

    public static DotHandler[] HexagonalTilling(int range, float radius, Vector2 offset) ///Hexagonal Tilling
    {
        int currRange = 0;
        List<Vector2> seen = new List<Vector2>();
        List<DotHandler> anchors = new List<DotHandler>();
        Vector2 center = offset;
        Vector2 currCenter = center;
        float deltaX = Mathf.Sin(60 * Mathf.Deg2Rad) * radius, deltaY = radius * 1.5f;

        while (currRange != range)
        {
            int hexag;
            Vector2 P_vPos = new Vector2(center.x + deltaX * 2 * (currRange + 1), center.y);
            Vector2 P_vNeg = new Vector2(center.x - deltaX * 2 * (currRange + 1), center.y);
            Vector2 N_vPos = P_vPos, N_vNeg = P_vNeg;

            hexag = 0;
            while (hexag != range)
            {
                Vector2 P_vPos_org = P_vPos;
                Vector2 P_vNeg_org = P_vNeg;

                P_vPos = new Vector2(P_vPos.x - deltaX, P_vPos.y + deltaY);
                P_vNeg = new Vector2(P_vNeg.x + deltaX, P_vNeg.y + deltaY);
                N_vPos = new Vector2(N_vPos.x - deltaX, N_vPos.y - deltaY);
                N_vNeg = new Vector2(N_vNeg.x + deltaX, N_vNeg.y - deltaY);
                Vector2[] t = new Vector2[] {P_vPos_org, P_vNeg_org, P_vPos, P_vNeg, N_vPos, N_vNeg};
                for (int j = 0; j < 6; j++)
                {
                    currCenter = t[j];
                    for (int i = 0; i < 6; i++)
                    {
                        float angleRad = (60 * i - 30) * Mathf.Deg2Rad;
                        Vector2 p = new Vector2(currCenter.x + radius * Mathf.Cos(angleRad), currCenter.y + radius * Mathf.Sin(angleRad));
                        p.x = Mathf.Round(p.x * 1000) / 1000;
                        p.y = Mathf.Round(p.y * 1000) / 1000;
                        if (!seen.Contains(p))
                        {
                            seen.Add(p);
                            DotHandler anchor = CreateNode(p);
                            anchor.name = "Hex(" + currRange + ")[" + j + "][" + i + "]";
                            anchor.elevation = range - currRange;
                            if (anchor.elevation == 1)
                                gm.StartPoints.Add(anchor);
                            anchors.Add(anchor);

                        }
                    }
                }
                hexag++;
            }
            currRange++;
        }




        return null;
    }
    
    #endregion Tilling 
}