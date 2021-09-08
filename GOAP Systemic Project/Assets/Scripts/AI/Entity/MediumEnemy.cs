using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemy : GeneralEnemy
{

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        if (CheckForElementSource() && CheckForWeaponSource())
            goal.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", true));
        else if (CheckForWeaponSource())
            goal.Add(new KeyValuePair<string, object>("attackPlayerWithWeapon", true));
        else
            goal.Add(new KeyValuePair<string, object>("attackPlayer", true));

        return goal;
    }

    bool CheckForElementSource()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("campfire"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool CheckForWeaponSource()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (//interactables[i].gameObject.ToString().ToLower().Contains("cutting tool") ||
                    interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false)
                        return true;
                }
                else if (interactables[i].gameObject.ToString().ToLower().Contains("tree"))
                {
                    if (GetComponent<EnemyStats>().hasCuttingTool)
                        return true;
                    else
                    {
                        for (int j = 0; j < interactables.Length; j++)
                            if (interactables[j].gameObject.ToString().ToLower().Contains("cutting tool"))
                                return true;
                    }
                }
            }
        }

        return false;
    }

}