using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeaponAction : GoapAction
{
    private LayerMask interactableLayer = 1 << 6;

    private bool hasWeapon;
    private bool weaponLocked;

    public PickUpWeaponAction()
    {
        addEffect("hasWeapon", true);

        movingActionText = "Moving towards Weapon";
        performingActionText = "Picking up Weapon";
    }

    public override void reset()
    {
        // Reset
        hasWeapon = false;
        weaponLocked = false;

        secondaryReset();

    }

    public override void secondaryReset()
    {
        if (target != null)
        {
            target.GetComponent<Weapon>().isOwned = false;
            target.GetComponent<Weapon>().owner = null;
            target = null;
        }
    }

    public override bool isDone()
    {
        return hasWeapon;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        if (agent.GetComponent<EnemyStats>().hasWeapon)
            return false; // No need to pick up another weapon if already carrying one

        target = null;

        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            bool flag = false;

            for (int i = 0; i < interactables.Length; i++)
            {

                if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    switch (agent.GetComponent<GeneralEnemy>().type)
                    {
                        case GeneralEnemy.ENEMY_TYPE.HEAVY: // Heavy enemies checks for non-heavy enemies
                            if (agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(interactables, "weapon", false, GeneralEnemy.ENEMY_TYPE.HEAVY))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                    interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                                {
                                    if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                    {
                                        if (interactables[i].gameObject.GetComponent<Weapon>().flammable ||
                                            interactables[i].gameObject.GetComponent<Weapon>().conductive)
                                        {
                                            interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                            interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                            target = interactables[i].gameObject;
                                            //weaponLocked = true;
                                            flag = true;
                                        }
                                    }
                                    else
                                    {
                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                        target = interactables[i].gameObject;
                                        //weaponLocked = true;
                                        flag = true;
                                    }
                                }
                            }

                            break;

                        case GeneralEnemy.ENEMY_TYPE.MEDIUM: // Medium enemy checks for light enemies
                            if (agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(interactables, "weapon", true, GeneralEnemy.ENEMY_TYPE.LIGHT))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                 interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                                {
                                    if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                    {
                                        if (interactables[i].gameObject.GetComponent<Weapon>().flammable ||
                                            interactables[i].gameObject.GetComponent<Weapon>().conductive)
                                        {
                                            interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                            interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                            target = interactables[i].gameObject;
                                            //weaponLocked = true;
                                            flag = true;
                                        }
                                    }
                                    else
                                    {
                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                        target = interactables[i].gameObject;
                                        //weaponLocked = true;
                                        flag = true;
                                    }
                                }
                            }

                            break;

                        case GeneralEnemy.ENEMY_TYPE.LIGHT: // Light enemies don't check for anything

                            if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                            {
                                if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                {
                                    if (interactables[i].gameObject.GetComponent<Weapon>().flammable ||
                                        interactables[i].gameObject.GetComponent<Weapon>().conductive)
                                    {

                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                        target = interactables[i].gameObject;
                                        //weaponLocked = true;
                                        flag = true;
                                    }
                                }
                                else
                                {
                                    interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                    interactables[i].gameObject.GetComponent<Weapon>().owner = GetComponent<Entity>();
                                    target = interactables[i].gameObject;
                                    //weaponLocked = true;
                                    flag = true;
                                }
                            }

                            break;
                    }

                    if (flag)
                        break;

                }
            }
        }

        return target != null;

    }

    public override bool movementPass(GameObject agent)
    {
        if (agent.GetComponent<GoapAgent>().currentGoal.goalName == "attackPlayerWithStatWeapon")
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

            if (interactables.Length > 0)
            {
                for (int i = 0; i < interactables.Length; i++)
                {
                    if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                    {
                        if ((interactables[i].gameObject.GetComponent<Weapon>().flammable &&
                            CheckActionInPlan(agent.GetComponent<GoapAgent>().GetActionPlan(), "BurnWeaponAction"))
                            ||
                            (interactables[i].gameObject.GetComponent<Weapon>().conductive &&
                            CheckActionInPlan(agent.GetComponent<GoapAgent>().GetActionPlan(), "ElectrifyWeaponAction")))
                        {

                            if (interactables[i].gameObject.GetComponent<Weapon>().owner == agent.GetComponent<Entity>())
                                return true;
                            else if (target.GetComponent<Weapon>().conductive == interactables[i].GetComponent<Weapon>().conductive ||
                                target.GetComponent<Weapon>().flammable == interactables[i].GetComponent<Weapon>().flammable)
                                return true;
                            else if (interactables[i].gameObject.GetComponent<Weapon>().owner == null)
                            {
                                if (target.GetInstanceID() != interactables[i].gameObject.GetInstanceID())
                                {
                                    target.GetComponent<Weapon>().isOwned = false;
                                    target.GetComponent<Weapon>().owner = null;
                                }

                                target = interactables[i].gameObject;

                                target.GetComponent<Weapon>().isOwned = true;
                                target.GetComponent<Weapon>().owner = agent.GetComponent<Entity>();

                                return true;

                            }
                        }
                    }
                }
            }

            target.GetComponent<Weapon>().isOwned = false;
            target.GetComponent<Weapon>().owner = null;

            return false;
        }
        else
            return true;
    }

    private bool CheckActionInPlan(Queue<GoapAction> actionPlan, string actionName)
    {
        foreach (GoapAction a in actionPlan)
        {
            if (a.GetType().Name == actionName)
                return true;
        }

        return false;
    }

    public override bool perform(GameObject agent)
    {
        EnemyStats stats = GetComponent<EnemyStats>();

        hasWeapon = true;
        stats.hasWeapon = true;
        target.GetComponent<Weapon>().WeaponPickedUp(GetComponent<Entity>());


        target.transform.parent = agent.transform;
        target.transform.position = agent.transform.position + (agent.transform.right * 0.5f);
        target.transform.rotation = Quaternion.identity;



        return true;
    }
}
