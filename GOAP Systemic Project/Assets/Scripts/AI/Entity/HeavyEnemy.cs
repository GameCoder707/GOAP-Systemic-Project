using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEnemy : GeneralEnemy
{
    public override List<GoalInfo> createGoalStates()
    {
        List<GoalInfo> goals = new List<GoalInfo>();

        if (!GetComponent<EnemyBehaviour>().isHealthy())
        {
            goals.Add(new GoalInfo());
            goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("canAttack", true));
            goals[goals.Count - 1].goalName = "canAttack";
            goals[goals.Count - 1].goalDesc = "Current Goal: Flee from Player";
        }
        else // Combat Goals
        {
            if (CheckForObjects())
            {
                goals.Add(new GoalInfo());
                goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("attackPlayerWithObjects", true));
                goals[goals.Count - 1].goalName = "attackPlayerWithObjects";
                goals[goals.Count - 1].goalDesc = "Current Goal: Push Boulder at Player";
            }

            if (CheckForCover())
            {
                goals.Add(new GoalInfo());
                goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("attackPlayerFromCover", true));
                goals[goals.Count - 1].goalName = "attackPlayerFromCover";
                goals[goals.Count - 1].goalDesc = "Current Goal: Kill Player from behind Cover";
            }

            if (CheckForFireSource() || CheckForElectricSource())
            {
                goals.Add(new GoalInfo());
                goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", true));
                goals[goals.Count - 1].goalName = "attackPlayerWithStatWeapon";
                goals[goals.Count - 1].goalDesc = "Current Goal: Kill Player with Status Applied Weapon";
            }

            if (CheckForWeaponSource())
            {
                goals.Add(new GoalInfo());
                goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("attackPlayerWithWeapon", true));
                goals[goals.Count - 1].goalName = "attackPlayerWithWeapon";
                goals[goals.Count - 1].goalDesc = "Current Goal: Kill Player with Weapon";
            }

            goals.Add(new GoalInfo());
            goals[goals.Count - 1].goal.Add(new KeyValuePair<string, object>("attackPlayer", true));
            goals[goals.Count - 1].goalName = "attackPlayer";
            goals[goals.Count - 1].goalDesc = "Current Goal: Kill Player with Bare Hands";

        }

        return goals;
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
                        interactables[i].gameObject.GetComponent<Boulder>().pusher == null)
                        return true;
                }
            }
        }

        return false;
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
