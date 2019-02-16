using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Camera cam;
    public List<DotHandler> playerDotHandlers = new List<DotHandler>();
    [SerializeField]private float speed = 1;
    [SerializeField]private float ScrollSpeed = 1;
    public bool isTurn = false;
    public bool StartPlayer;
    public enum PlayerMode
    {
        Select,
        Build
    }
    public PlayerMode playerMode;


  

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
    }

    public DotHandler CreateNewNode(DotHandler anchor) //will create a new node and connection with initialization when called
    {
        anchor.UpdateStrength(1);
        return anchor;
    }


}
