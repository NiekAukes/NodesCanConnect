using UnityEngine;
using UnityEngine.Networking;

public class Connection : MonoBehaviour {
    public DotHandler origin, destination; // declares DotHandler vars
    public Transform cube; //declares transform
    

    public float length, rotationZ, tst, DeltaX, DeltaY; //declares float vars

    private void Start()
    {
        DrawConnection(); //draws connection from start (useful for predefined Connections)
    }

    public Connection DrawConnection() //draws new connection for the current connection
    {
        length = Vector2.Distance(a: origin.transform.position, b: destination.transform.position);
        tst = Mathf.Abs(origin.transform.position.y - destination.transform.position.y) / (origin.transform.position.x - destination.transform.position.x);
        DeltaY = Mathf.Abs(origin.transform.position.y - destination.transform.position.y);
        DeltaX = Mathf.Abs(origin.transform.position.x - destination.transform.position.x);
        transform.position = new Vector3((DeltaX / 2) + Mathf.Min(origin.transform.position.x, destination.transform.position.x), (DeltaY / 2) + Mathf.Min(origin.transform.position.y, destination.transform.position.y));

        rotationZ = 90 - Mathf.Atan(tst) * Mathf.Rad2Deg;
        cube.localScale = new Vector3(0.03f, length / 5.75f, 0.1f);
        cube.rotation = Quaternion.Euler(0, 0, rotationZ);
        return this;
    }

    public static Connection DrawConnection(Connection c) //static form of DrawConnection()
    {
        float _length, _rotationZ, _tst, _DeltaX, _DeltaY; 
        _length = Vector2.Distance(a: c.origin.transform.position, b: c.destination.transform.position); 
        _tst = Mathf.Abs(c.origin.transform.position.y - c.destination.transform.position.y) / (c.origin.transform.position.x - c.destination.transform.position.x); 
        _DeltaY = Mathf.Abs(c.origin.transform.position.y - c.destination.transform.position.y); 
        _DeltaX = Mathf.Abs(c.origin.transform.position.x - c.destination.transform.position.x); 
        c.transform.position = new Vector3((_DeltaX / 2) + Mathf.Min(c.origin.transform.position.x, c.destination.transform.position.x), (_DeltaY / 2) + Mathf.Min(c.origin.transform.position.y, c.destination.transform.position.y));
        
        _rotationZ = 90 - Mathf.Atan(_tst) * Mathf.Rad2Deg; 
        c.cube.localScale = new Vector3(0.03f, _length / 5.75f, 0.1f);
        c.cube.rotation = Quaternion.Euler(0, 0, _rotationZ); 
        return c;
    }

    public Connection AbstractDraw(Vector2 Begin, Vector2 End)
    {
        length = Vector2.Distance(a: Begin, b: End);
        tst = Mathf.Abs(Begin.y - End.y) / (Begin.x - End.x);
        DeltaY = Mathf.Abs(Begin.y - End.y);
        DeltaX = Mathf.Abs(Begin.x - End.x);
        transform.position = new Vector3((DeltaX / 2) + Mathf.Min(Begin.x, End.x), (DeltaY / 2) + Mathf.Min(Begin.y, End.y));

        rotationZ = 90 - Mathf.Atan(tst) * Mathf.Rad2Deg;
        cube.localScale = new Vector3(0.03f, length / 5.75f, 0.1f);
        cube.rotation = Quaternion.Euler(0, 0, rotationZ);
        return this;
    }

    public Connection InitializeAbstractConnection(DotHandler Origin, DotHandler Destination)
    {
        Connection ConnectTemp = GetComponent<Connection>();
        ConnectTemp.cube = GetComponentInChildren<SpriteRenderer>().transform;
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
        return ConnectTemp;
    }

    private void OnDestroy()
    {
        
    }


}
