using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFragment : MonoBehaviour
{
    public bool OnmouseDown = false;
    public DotHandler MainDot;
    public SpriteRenderer Frame;
    public SpriteRenderer Outline;
    public int level;
    private void Start()
    {

    }

    public void Draw()
    {
        float scalelevel = 0.2f;
        gameObject.transform.localScale = new Vector3(level * scalelevel + 0.2f, level * scalelevel + 0.2f, gameObject.transform.localScale.z);
        Outline.sortingOrder =  10 - level * 2;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, 0.1f * level);
        Frame.sortingOrder = Outline.sortingOrder + 1;
        //scaling
        Frame.color = MainDot.Owner.playercolor;
        //more drawing
    }

    private void OnMouseOver()
    {
        OnmouseDown = true;
        MainDot.OnMouse = true;
    }
    private void OnMouseExit()
    {
        OnmouseDown = false;
        MainDot.OnMouse = false;
    }
    private void OnMouseDown()
    {
        if (DotHandler.clickRegist == null)
        {
            //first clicked
            DotHandler.clickRegist = new DotHandler.ClickRegist(MainDot, this);
            Debug.Log("first clicked");
        }
        else
        {
            //second clicked
            DotHandler.clickRegist.OnRegisterEnter(MainDot);
            Debug.Log("Second Clicked");
        }
    }
    private void OnMouseUp()
    {
        if (DotHandler.clickRegist != null)
            DotHandler.clickRegist.OnRegisterRelease();
    }

    //Detect if player wants to establish connection | move Energy | Attack dot --finished
    //handle the modes

    public void OnDestroy()
    {
        //handle Destroy visual change
    }
}
