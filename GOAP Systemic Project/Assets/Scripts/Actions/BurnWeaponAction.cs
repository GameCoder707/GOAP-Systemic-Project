using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnWeaponAction : GoapAction
{
    private bool statusApplied;
    private bool weaponSwung;
    private bool performAction;

    private LayerMask interactableLayer = 1 << 6;

    public BurnWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("isStatusAppliedToWeapon", true);

        movingActionText = "Moving towards Fire Source";
        performingActionText = "Burning weapon using Fire Source";
    }

    public override void reset()
    {
        // Reset
        statusApplied = false;
        performAction = false;
    }

    public override void secondaryReset() { }

    public override bool isDone()
    {
        return statusApplied;
    }

    public override bool requiresInRange()
    {
        if (target != null)
            return true;
        else
            return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);
        RaycastHit hit;

        if (interactables.Length > 0)
        {
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                {
                    if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.RAINY)
                        return false;
                    else if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.HEAT_WAVE)
                    {
                        if (agent.GetComponent<EnemyStats>().hasWeapon)
                        {
                            if (agent.GetComponentInChildren<Weapon>().flammable)
                                return true;
                        }

                        for (int i = 0; i < interactables.Length; i++)
                        {
                            if (interactables[i].gameObject.name.ToLower().Contains("weapon"))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                                    return true;
                            }
                            else if (interactables[i].gameObject.name.ToLower().Contains("tree"))
                            {
                                switch (agent.GetComponent<GeneralEnemy>().type)
                                {
                                    case GeneralEnemy.ENEMY_TYPE.LIGHT:
                                        break;
                                    case GeneralEnemy.ENEMY_TYPE.HEAVY:
                                        return true;
                                    case GeneralEnemy.ENEMY_TYPE.MEDIUM:
                                        for (int j = 0; j < interactables.Length; j++)
                                        {
                                            if (interactables[j].gameObject.name.ToLower().Contains("cutting tool"))
                                                return true;
                                        }
                                        break;
                                }
                            }
                        }

                    }
                }
            }

            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("campfire"))
                {
                    target = interactables[i].gameObject;
                    return true;
                }
            }
        }

        return false;

    }

    public override bool movementPass(GameObject agent)
    {
        if (GetComponentInChildren<Weapon>() != null)
        {
            if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.NONE)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public override bool perform(GameObject agent)
    {
        if (target != null)
        {
            if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.NONE)
                performAction = true; // We can perform this action if no status has been applied prior to this

            if (performAction)
            {
                agent.transform.LookAt(new Vector3(target.transform.position.x, agent.transform.position.y, target.transform.position.z));

                if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.BURNING &&
                    agent.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponIdleAnim"))
                {
                    statusApplied = true;
                    weaponSwung = false;
                    performAction = false;
                }
                else if (agent.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponIdleAnim"))
                {
                    agent.GetComponentInChildren<Weapon>().anim.SetBool("isSwinging", true);
                }
            }
            else
                statusApplied = true;

            return true;

        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                    if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.HEAT_WAVE)
                    {
                        if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.BURNING)
                            statusApplied = true;

                        return true;
                    }
            }

            return false;
        }

    }
}
