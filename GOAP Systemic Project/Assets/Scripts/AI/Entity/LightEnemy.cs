using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : GeneralEnemy
{
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

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

        return goal;
    }

    bool CheckForElementSource()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.STORM &&
                    CheckForWeaponSource("Electric"))
                {
                    return true;
                }
                else if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.HEAT_WAVE &&
                    CheckForWeaponSource("Fire"))
                {
                    return true;
                }


        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("campfire"))
                {
                    if (CheckForWeaponSource("Fire"))
                        return true;
                }
            }
        }

        return false;
    }

    bool CheckForWeaponSource(string statusCompatibility = "")
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