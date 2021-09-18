using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEnemy : GeneralEnemy
{
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        if (!GetComponent<EnemyBehaviour>().isHealthy())
        {
            goal.Add(new KeyValuePair<string, object>("canAttack", true));
            goalName = "canAttack";
        }
        else // Combat Goals
        {
            if (CheckForElementSource())
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
        }

        return goal;
    }

    protected override bool CheckForWeaponSource(string statusCompatibility = "")
    {
        if (GetComponent<EnemyStats>().hasWeapon)
        {
            if (statusCompatibility != "")
            {
                switch (statusCompatibility)
                {
                    case "Fire":
                        if (GetComponentInChildren<Weapon>().flammable)
                            return true;
                        break;
                    case "Electric":
                        if (GetComponentInChildren<Weapon>().conductive)
                            return true;
                        break;
                }

                return false;
            }
            else
                return true;
        }
        else
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

            if (interactables.Length > 0)
            {
                for (int i = 0; i < interactables.Length; i++)
                {
                    if (interactables[i].gameObject.ToString().ToLower().Contains("tree"))
                        return true;
                    else if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                    {
                        if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false)
                        {
                            if (statusCompatibility != "")
                            {
                                switch (statusCompatibility)
                                {
                                    case "Fire":
                                        if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                                            return true;
                                        break;
                                    case "Electric":
                                        if (interactables[i].gameObject.GetComponent<Weapon>().conductive)
                                            return true;
                                        break;
                                }

                            }
                            else
                                return true;
                        }
                    }
                }
            }

            return false;
        }
    }


}
