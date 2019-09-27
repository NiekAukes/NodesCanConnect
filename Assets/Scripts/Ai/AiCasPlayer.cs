using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Problems with AI:
//   -Ai will not use HelperDot **FIXED**
//   -Ai Needs to search for a helperDot further away
//   -Ai Needs to create Connections
public class AiCasPlayer : IPlayer {
    public float decisionfloat = 1.8f;
    //public void decide()
    public IEnumerator decide()
    {
        List<DotHandler> RerouteDotHandlers = new List<DotHandler>();
        //cycle through all owned dots
        foreach (DotHandler d in playerDotHandlers.ToArray())
        {
            //underloaded dot
            


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
                                if (helperDot == null && hD != null)
                                {
                                    helperDot = hD;
                                }
                                //check if this dot meet the requirements to help
                                if (helperDot != null && hD != null)
                                {
                                    helperDot = hD.Strength > helperDot.Strength ? hD : helperDot;
                                    if (!(helperDot.Strength > 1 && helperDot.Owner == this && Connection.FindConnectionBetween(d, helperDot) != null))
                                    {

                                        helperDot = null;
                                    }
                                }
                            }
                            if ((d.Strength > 2 || helperDot != null) && Random.Range(0.0f, 1.0f) < decisionfloat)
                            {
                                if (helperDot != null)
                                {
                                    if (Connection.FindConnectionBetween(helperDot, d) != null)
                                    {
                                        DotHandler.EnergyMove(helperDot, d, helperDot.Strength - 1);
                                    }
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

                        }
                    }
                }
            }


            bool HostileNearby = false;
            foreach (DotHandler dotHandler in d.GetNearbyDotHandlers(3f))
            {
                if (dotHandler.Owner != this)
                {
                    HostileNearby = true;
                    break;
                }
            }
            if (HostileNearby)
            {
                //High Cap needed
                if (d.Strength < 4)
                {
                    // ask Re-enforcements
                    foreach (DotHandler dotHandler in d.GetNearbyDotHandlers(2f))
                    {
                        if (dotHandler.Strength > 4)
                        {
                            DotHandler.EnergyMove(dotHandler, d, dotHandler.Strength - 2);
                        }
                    }
                }
            }
            else
            {
                //Okay to Have less energy
                if (d.Strength > 3)
                {

                    //Get Some energy away
                    foreach (DotHandler dotHandler in d.GetNearbyDotHandlers(2f))
                    {
                        if (dotHandler.Strength < 4)
                        {
                            if (Connection.FindConnectionBetween(d, dotHandler) == null)
                            {
                                Connection c = DotHandler.CreateConnection(d, dotHandler);
                                c.Abs = true;
                                if (c.cannotBuild)
                                {
                                    c.DestroyConnection();
                                }
                                else
                                {
                                    DotHandler.EnergyMove(d, dotHandler, 1, 1);
                                }
                            }
                            DotHandler.EnergyMove(d, dotHandler, dotHandler.Strength - 2);
                        }
                    }
                }
            }
        }
        Dictionary<DotHandler, int> PriorityDots = new Dictionary<DotHandler, int>(); // TODO
        foreach (DotHandler d in RoundHandler.CurrPlayerMove.playerDotHandlers)
        {
            if (d != null)
            {
                d.UpdateList();
            }
            List<DotHandler> nearbyEnemyDothandlers = new List<DotHandler>();
            foreach(DotHandler otherDotHandler in d.GetNearbyDotHandlers(3f))
            {
                if (otherDotHandler.Owner != this)
                {
                    nearbyEnemyDothandlers.Add(otherDotHandler);
                }
            }
            if (d.Strength < 3 && nearbyEnemyDothandlers.Count > 1)
            {
                //add Energy
                //TODO
            }
        }
        EndTurn();
        yield return null;
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

    void EndTurn()
    {
        RoundHandler.CheckGold();
        RoundHandler.NextPlayer();
    }
}
