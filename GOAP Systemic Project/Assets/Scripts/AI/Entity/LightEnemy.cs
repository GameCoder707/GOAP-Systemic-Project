using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : GeneralEnemy
{
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        if (CheckForElementSource() && CheckForWeaponSource())
        {
            goal.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", true));
            goalName = "attackPlayerWithStatWeapon";
        }
        else if (CheckForWeaponSource())
        {
            goal.Add(new KeyValuePair<string, object>("attackPlayerWithWeapon", true));
            goalName = "attackPlayerWithWeapon";
        }
        else
        {
            goal.Add(new KeyValuePair<string, object>("attackPlayer", true));
            goalName = "attackPlayer";
        }

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
                    if (CheckForWeaponSource() && flammableWeaponsInArea())
                        return true;
                }
            }
        }

        return false;
    }

    bool CheckForWeaponSource()
    {
        if (GetComponent<EnemyStats>().hasWeapon)
            return true;
        else
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

            if (interactables.Length > 0)
            {
                for (int i = 0; i < interactables.Length; i++)
                {
                    if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                    {
                        if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false)
                            return true;
                    }
                }
            }

            return false;
        }
    }

    private bool flammableWeaponsInArea()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                        return true;
                }
            }
        }

        return false;

    }
}
