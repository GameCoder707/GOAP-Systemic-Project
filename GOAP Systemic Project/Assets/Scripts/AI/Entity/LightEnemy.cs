using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : GeneralEnemy
{
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("playerDead", true));
        return goal;
    }
}
