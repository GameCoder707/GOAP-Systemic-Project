using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemy : GeneralEnemy
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
            if (CheckForObjects())
            {
                goal.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", true));
                goalName = "attackPlayerWithStatWeapon";
                currentGoalDisplay.text = "Current Goal: Burning Boulder for Heavy Enemy to Push";
            }
            else if (CheckForCover())
            {
                goal.Add(new KeyValuePair<string, object>("attackPlayerFromCover", true));
                goalName = "attackPlayerFromCover";
                currentGoalDisplay.text = "Current Goal: Kill Player from behind Cover";
            }
            else if (CheckForFireSource() || CheckForElectricSource())
            {
                goal.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", true));
                goalName = "attackPlayerWithStatWeapon";
                currentGoalDisplay.text = "Current Goal: Kill Player with Status Applied Weapon";
            }
            else if (CheckForWeaponSource())
            {
                goal.Add(new KeyValuePair<string, object>("attackPlayerWithWeapon", true));
                goalName = "attackPlayerWithWeapon";
                currentGoalDisplay.text = "Current Goal: Kill Player with Weapon";
            }
            else
            {
                goal.Add(new KeyValuePair<string, object>("attackPlayer", true));
                goalName = "attackPlayer";
                currentGoalDisplay.text = "Current Goal: Kill Player with Bare Hands";
            }
        }

        return goal;
    }

    protected override bool CheckForObjects()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.name.ToLower().Contains("boulder"))
                {
                    if (!interactables[i].gameObject.GetComponent<Boulder>().isPushed &&
                        !interactables[i].gameObject.GetComponent<Boulder>().isStatusApplied &&
                        interactables[i].gameObject.GetComponent<Boulder>().burner == null &&
                        CheckForFireSource())
                    {
                        return true;
                    }

                }
            }
        }

        return false;
    }

    protected override bool CheckForWeaponSource(string statusCompatibility = "")
    {
        if (GetComponentInChildren<Weapon>() != null)
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
                    if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
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
}
