using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField]private Camera cam;
    public List<DotHandler> playerDotHandlers = new List<DotHandler>();
    [SerializeField]private float speed = 1;
    [SerializeField]private float ScrollSpeed = 1;
    public bool isTurn = false;
    public bool StartPlayer;

    public void Start()
    {
        GameHandler.CurrPlayerMove = StartPlayer && GameHandler.CurrPlayerMove == null ? this : null;
    }

    public void Update()
    {
        isTurn = GameHandler.GetCurrPlayer() == this;
        if (GetType() != typeof(AiPlayer) && isTurn)
        {
            float axisH = Input.GetAxis("Horizontal");
            float axisV = Input.GetAxis("Vertical");
            float axisS = Input.GetAxis("Mouse ScrollWheel");
            float t1 = transform.position.y + axisV * speed * Mathf.Pow(cam.orthographicSize / 10, 1.1f);
            float t2 = transform.position.x + axisH * speed * Mathf.Pow(cam.orthographicSize / 10, 1.1f);
            transform.position = new Vector3(t2, t1, -10);
            if ((cam.orthographicSize > 2 || axisS > 0) && (cam.orthographicSize < 12 || axisS < 0))
                cam.orthographicSize = cam.orthographicSize + axisS * ScrollSpeed * Mathf.Pow(cam.orthographicSize / 10, 1.1f);
        }
        else
            Debug.Log("Update In Ai");
    }

    public DotHandler CreateNewNode(AnchorHandler anchor) //will create a new node and connection with initialization when called
    {
        GameObject temp = Instantiate(GameHandler.GetNodePrefab(), GameHandler.GetNodeFolder());
        DotHandler node = temp.GetComponent<DotHandler>();
        node.Owner = this;
        playerDotHandlers.Add(node);
        //this will create a new node 
        temp.transform.position = anchor.transform.position;
        GameHandler.CreateConnection(GameHandler.CurrDot, node);
        GameHandler.CurrDot.UpdateStrength(-2);
        //initialization of the new node and creation of a new connection
        anchor.occupied = true;
        GameHandler.SwitchBuildMode();
        //applies buildmode and anchor updates
        return node;
    }


}
