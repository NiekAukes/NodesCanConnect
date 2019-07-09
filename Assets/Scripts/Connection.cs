using UnityEngine;

public class Connection : MonoBehaviour {
    public DotHandler origin, destination; // declares DotHandler vars
    public Transform cube; //declares transform
    public Collider2D collider;
    

    public float length, rotationZ, tst, DeltaX, DeltaY; //declares float vars

    public ContactFilter2D t;
    static LayerMask mask;
    static DotHandler hoverdot;
    public bool Abs = false;
    public bool cannotBuild = false;

    private void Awake()
    {
        mask = LayerMask.GetMask("Connection");
        collider = GetComponentInChildren<Collider2D>();
    }
    private void Update()
    {
        
        bool flag_onmouse = false;
        foreach (IPlayer p in RoundHandler.PlayerList)
        {
            foreach (DotHandler d in p.playerDotHandlers)
            {
                if (d != null && hoverdot != null)
                {
                    if (d.OnMouse)
                    {
                        if (hoverdot != d)
                        {
                            foreach (Connection c in hoverdot.Connections)
                            {
                                c.cube.gameObject.layer = 8;
                            }
                        }
                        if (!Abs)
                        {

                            foreach (Connection c in origin.Connections)
                            {
                                c.cube.gameObject.layer = 8;
                            }
                            foreach (Connection c in destination.Connections)
                            {
                                c.cube.gameObject.layer = 8;
                            }
                            hoverdot = null;
                        }
                        flag_onmouse = true;

                    }
                }
            }
        }
        if (flag_onmouse && !Abs)
        {

            foreach (Connection c in hoverdot.Connections)
            {
                c.cube.gameObject.layer = 9;
                Debug.Log("Hoverdot: " + hoverdot);
            }
            foreach (Connection c in origin.Connections)
            {
                c.cube.gameObject.layer = 9;
                Debug.Log("connection disabled: " + c);
            }
            foreach (Connection c in destination.Connections)
            {
                c.cube.gameObject.layer = 9;
                Debug.Log("connection disabled: " + c);
            }
        }
        if (collider.IsTouching(t))
        {
            Collider2D[] contacts = new Collider2D[10];
            collider.GetContacts(t, contacts);
            foreach (Collider2D retrievedcollider in contacts)
            {

                if (retrievedcollider != null && retrievedcollider.gameObject.transform.parent != transform.parent)
                {
                    cannotBuild = true;
                    Debug.Log(contacts[0].transform.parent.gameObject.name);
                }

            }
        }
        else
        {
            cannotBuild = false;
        }
        if (DotHandler.clickRegist != null && DotHandler.clickRegist.AbsConnection != null)
        {
            if (cannotBuild)
            {
                DotHandler.clickRegist.AbsConnection.cube.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                DotHandler.clickRegist.AbsConnection.cube.GetComponent<SpriteRenderer>().color = RoundHandler.CurrPlayerMove.playercolor;
            }
        }
        
    }

    public Connection DrawConnection() //draws new connection for the current connection
    {
        length = Vector2.Distance(a: origin.transform.position, b: destination.transform.position);
        tst = Mathf.Abs(origin.transform.position.y - destination.transform.position.y) / (origin.transform.position.x - destination.transform.position.x);
        DeltaY = Mathf.Abs(origin.transform.position.y - destination.transform.position.y);
        DeltaX = Mathf.Abs(origin.transform.position.x - destination.transform.position.x);
        transform.position = new Vector3((DeltaX / 2) + Mathf.Min(origin.transform.position.x, destination.transform.position.x), (DeltaY / 2) + Mathf.Min(origin.transform.position.y, destination.transform.position.y), 0.5f);

        rotationZ = 90 - Mathf.Atan(tst) * Mathf.Rad2Deg;
        cube.localScale = new Vector3(0.03f, length / 5.75f, 0.1f);
        cube.rotation = Quaternion.Euler(0, 0, rotationZ);
        GetComponentInChildren<SpriteRenderer>().color = origin.Owner.playercolor;
        return this;
    }

    public static Connection DrawConnection(Connection c) //static form of DrawConnection()
    {
        float _length, _rotationZ, _tst, _DeltaX, _DeltaY; 
        _length = Vector2.Distance(a: c.origin.transform.position, b: c.destination.transform.position); 
        _tst = Mathf.Abs(c.origin.transform.position.y - c.destination.transform.position.y) / (c.origin.transform.position.x - c.destination.transform.position.x); 
        _DeltaY = Mathf.Abs(c.origin.transform.position.y - c.destination.transform.position.y); 
        _DeltaX = Mathf.Abs(c.origin.transform.position.x - c.destination.transform.position.x); 
        c.transform.position = new Vector3((_DeltaX / 2) + Mathf.Min(c.origin.transform.position.x, c.destination.transform.position.x), (_DeltaY / 2) + Mathf.Min(c.origin.transform.position.y, c.destination.transform.position.y), 0.5f);
        
        _rotationZ = 90 - Mathf.Atan(_tst) * Mathf.Rad2Deg; 
        c.cube.localScale = new Vector3(0.03f, _length / 5.75f, 0.1f);
        c.cube.rotation = Quaternion.Euler(0, 0, _rotationZ);
        c.GetComponentInChildren<SpriteRenderer>().color = c.origin.Owner.playercolor;
        return c;
    }

    public Connection AbstractDraw(Vector2 Begin, Vector2 End)
    {
        length = Vector2.Distance(a: Begin, b: End);
        tst = Mathf.Abs(Begin.y - End.y) / (Begin.x - End.x);
        DeltaY = Mathf.Abs(Begin.y - End.y);
        DeltaX = Mathf.Abs(Begin.x - End.x);
        transform.position = new Vector3((DeltaX / 2) + Mathf.Min(Begin.x, End.x), (DeltaY / 2) + Mathf.Min(Begin.y, End.y), 20);

        rotationZ = 90 - Mathf.Atan(tst) * Mathf.Rad2Deg;
        cube.localScale = new Vector3(0.03f, length / 5.75f, 0.1f);
        cube.rotation = Quaternion.Euler(0, 0, rotationZ);
        Abs = true;
        return this;
    }

    public Connection InitializeAbstractConnection(DotHandler Origin, DotHandler Destination)
    {
        Connection ConnectTemp = GetComponent<Connection>();
        ConnectTemp.cube = GetComponentInChildren<SpriteRenderer>().transform;
        Debug.Log("Origin: " + Origin);
        Debug.Log("Destination: " + Destination);
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
        name = Origin.name + "-" + Destination.name;
        Origin.Connections.Add(ConnectTemp);
        Destination.Connections.Add(ConnectTemp);
        cube.gameObject.layer = 8;
        GetComponentInChildren<SpriteRenderer>().color = Origin.Owner.playercolor;
        Debug.Log("updated color");
        Abs = false;
        return ConnectTemp;
    }

    public static Connection FindConnectionBetween(DotHandler Origin, DotHandler Destination)
    {
        foreach (Connection c in FindObjectsOfType<Connection>())
        {
            if (c != DotHandler.clickRegist.AbsConnection)
            {

                if ((c.origin == Origin && c.destination == Destination) || (c.origin == Destination && c.destination == Origin) && (c.destination != null && c.origin != null))
                {
                    return c;
                }
                
            }
        }
        return null;
    }

    public void DestroyConnection()
    {
        Destroy(gameObject);
    }


}
