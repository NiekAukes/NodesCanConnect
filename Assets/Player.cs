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
    public bool StartPlayer = false;
    public enum PlayerMode
    {
        Idle,
        Selection,
        Build,
        Attack
    }
    public PlayerMode playerMode = PlayerMode.Selection;

    public void Start()
    {

    }

    public static void SelectStartPlayer()
    {
        foreach(Player p in GameHandler.gm.PlayerList)
        if (p.StartPlayer)
        {
            GameHandler.gm.CurrPlayerMove = p;
            Debug.Log(p + "  " + GameHandler.gm.CurrPlayerMove);
        }
    }

    public void Update()
    {
        isTurn = GameHandler.gm.CurrPlayerMove == this;
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
        if (!ScreenUpdater())
            throw new Exception("failed updating screen");
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
        GameHandler.gm.CurrPlayerMove.playerMode = PlayerMode.Selection;
        //applies buildmode and anchor updates

        return node;
    }
    /// <summary>
    /// updates the screen according to the mode the player is currently in
    /// </summary>
    /// <returns>returns false if the operation fails</returns>
    public bool ScreenUpdater()
    {
        try
        {
            //handle ui and screenrelated things
        }
        catch(Exception e)
        {
            Debug.LogError("an exception occured, operation cancelled \n" + e);
            return false;
        }
        return true;
    }


}
