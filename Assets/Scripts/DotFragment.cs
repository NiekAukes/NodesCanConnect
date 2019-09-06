using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFragment : MonoBehaviour
{
    public bool OnmouseDown = false;
    public bool BlueRing = false;
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
        Outline.transform.localScale = new Vector3(0.02f / (level * scalelevel + 0.2f) + 1f, 0.02f / (level * scalelevel + 0.2f) + 1f, Outline.transform.localScale.z);
        //scaling
        if (MainDot.Owner != null)
        {
            if (BlueRing)
                Frame.color = new Color(0, 255, 255, 255);
            else
                Frame.color = MainDot.Owner.playercolor;

        }
        
        //more drawing
    }

    private void OnMouseOver()
    {
        OnmouseDown = true;
        MainDot.OnMouse = true;
        if (!RoundHandler.energyDistributionState)
            Frame.color = new Color(MainDot.Owner.playercolor.r + ((1f - MainDot.Owner.playercolor.r) / 2), MainDot.Owner.playercolor.g + ((1f - MainDot.Owner.playercolor.g) / 2), MainDot.Owner.playercolor.b + (1f - MainDot.Owner.playercolor.b) / 2);
        
    }
        private void OnMouseExit()
    {
        OnmouseDown = false;
        MainDot.OnMouse = false;
        if (DotHandler.clickRegist == null && !RoundHandler.energyDistributionState)
        {
            Frame.color = MainDot.Owner.playercolor;
        }
    }
    private void OnMouseDown()
    {
        if (BlueRing)
        {
            if (RoundHandler.amount > 0)
            {
                Debug.Log("amount: " + RoundHandler.amount);
                MainDot.UpdateStrength(1);
                RoundHandler.amount--;
                MainDot.CreateBlueRing();
            }
        }
        else
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

    public void SelectColor(bool selectcolor)
    {
        if (selectcolor)
        {
            Frame.color = new Color(
            MainDot.Owner.playercolor.r + ((1f - MainDot.Owner.playercolor.r) / 2),
            MainDot.Owner.playercolor.g + ((1f - MainDot.Owner.playercolor.g) / 2),
            MainDot.Owner.playercolor.b + ((1f - MainDot.Owner.playercolor.b) / 2));
        }
        else
        {
            Frame.color = MainDot.Owner.playercolor;
        }
    }
}
