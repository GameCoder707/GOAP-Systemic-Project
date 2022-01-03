using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalInfo
{
    public HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
    public string goalName;
    public string goalDesc; // Goal Description
}