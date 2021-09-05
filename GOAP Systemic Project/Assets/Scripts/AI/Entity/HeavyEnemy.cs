using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEnemy : GeneralEnemy
{
    private LayerMask interactableLayer = 1 << 6;
    private LayerMask enemyLayer = 1 << 8;

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
                if (interactables[i].gameObject.ToString().ToLower().Contains("tree"))
                {
                    return true;
                }
                else if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    Collider[] enemies = Physics.OverlapSphere(transform.position, 15.0f, enemyLayer);

                    List<GameObject> weaponsInArea = new List<GameObject>();
                    List<GameObject> nonHeavyEnemies = new List<GameObject>();

                    for (int j = 0; j < interactables.Length; j++)
                        if (interactables[j].gameObject.ToString().ToLower().Contains("weapon"))
                            weaponsInArea.Add(interactables[j].gameObject);

                    for (int k = 0; k < enemies.Length; k++)
                        if (enemies[k].gameObject.GetComponent<GeneralEnemy>().type != ENEMY_TYPE.HEAVY)
                            nonHeavyEnemies.Add(enemies[k].gameObject);

                    // If the non-heavy enemy count is less than the weapon count, then he can go pick it up
                    if (nonHeavyEnemies.Count < weaponsInArea.Count)
                        return true;
                }
            }
        }

        return false;
    }
}
