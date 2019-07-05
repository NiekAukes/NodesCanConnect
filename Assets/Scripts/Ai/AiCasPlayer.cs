using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : Player {
    public void decide()
    {
        //cycle through all owned dots
        foreach (DotHandler d in playerDotHandlers)
        {
            //check if dot can attack
            if (d.Strength > 2)
            {
                //get nearby dotHandlers and check nearby enemy dothandlers
                DotHandler[] dothandlers = d.GetNearbyDotHandlers();
                foreach (DotHandler otherDothandler in dothandlers)
                {
                    if (otherDothandler.Owner != this && otherDothandler.Strength < d.Strength + 1)
                    {
                        if (otherDothandler.Strength < d.Strength - 1)
                        {
                            //attack dot
                        }
                        else
                        {
                            
                        }
                    }
                }
            }
        }
    }
}
