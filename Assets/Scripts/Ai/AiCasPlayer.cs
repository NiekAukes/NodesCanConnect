using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//Problems with AI:
//   -Ai will not use HelperDot
//   -Ai Needs to search for a helperDot further away
public class AiCasPlayer : IPlayer {
    public float decisionfloat = 1.8f;

    public void decide()
    {
        //cycle through all owned dots
        foreach (DotHandler d in playerDotHandlers.ToArray())
        {
            //check if dot can attack
            if (Random.Range(0.0f, 1.0f) < decisionfloat && d != null)
            {
                DotHandler helperDot = null;
                Debug.Log("decide if do something");
                //get nearby dotHandlers and check nearby enemy dothandlers
                DotHandler[] dothandlers = d.GetNearbyDotHandlers(1.5f);
                foreach (DotHandler otherDothandler in dothandlers)
                {
                    //check if this dot can attack other dot 
                    if (otherDothandler != null && Random.Range(0.0f, 1.0f) < decisionfloat)
                    {
                        if (otherDothandler.Owner != this && otherDothandler.Strength < d.Strength + 1)
                        {
                            foreach (DotHandler hD in d.GetNearbyDotHandlers(3f))
                            {
                                helperDot = hD;
                                //check if this dot meet the requirements to help
                                if (helperDot != null)
                                {
                                    if (!(helperDot.Strength > 1 && helperDot.Owner == this))
                                    {
                                        
                                        helperDot = null;
                                    }
                                }
                            }
                            if (d.Strength > 2 || helperDot != null)
                            {
                                if (helperDot != null)
                                {
                                    if (Connection.FindConnectionBetween(helperDot, d) != null)
                                    {
                                        DotHandler.EnergyMove(helperDot, d, helperDot.Strength - 1);
                                    }
                                }
                                if (otherDothandler.Strength < d.Strength - 1)
                                {
                                    if (otherDothandler.Owner == null)
                                    {
                                        //Take over Dot
                                        TakeOverDot(d, otherDothandler);
                                    }
                                    else
                                    {
                                        //Attack enemy dot
                                        AttackOtherDot(d, otherDothandler);
                                    }

                                }
                            }
                            else
                            {
                                //request strength from nearby dots

                            }
                        }
                    }
                }
            }
        }
        foreach (DotHandler d in RoundHandler.CurrPlayerMove.playerDotHandlers)
        {

            if (d != null)
            {
                d.UpdateList();
                d.UpdateStrength(1);
            }
        }
        RoundHandler.NextPlayer();
    }

    void TakeOverDot(DotHandler subjectDot, DotHandler otherDothandler)
    {
        otherDothandler.Owner = this;
        DotHandler.CreateConnection(subjectDot, otherDothandler);
        DotHandler.EnergyMove(subjectDot, otherDothandler, subjectDot.Strength - 1, 1);
        RoundHandler.CurrPlayerMove.playerDotHandlers.Add(otherDothandler);
    }
    void AttackOtherDot(DotHandler subject, DotHandler otherDot)
    {
        subject.AttackDot(otherDot, subject.Strength - 1);
        RoundHandler.CurrPlayerMove.playerDotHandlers.Add(otherDot);
    }
}
