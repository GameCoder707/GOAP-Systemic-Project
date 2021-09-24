using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifyWeaponAction : GoapAction
{

    private bool statusApplied;

    private LayerMask interactableLayer = 1 << 6;

    public ElectrifyWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("isStatusAppliedToWeapon", true);
    }

    public override void reset()
    {
        // Reset
        statusApplied = false;
    }

    public override bool isDone()
    {
        return statusApplied;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //target = null;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
        {

            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.STORM)
                {
                    Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

                    if (interactables.Length > 0)
                    {
                        for (int i = 0; i < interactables.Length; i++)
                        {
                            if (interactables[i].gameObject.name.ToLower().Contains("weapon"))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().conductive &&
                                    interactables[i].gameObject.GetComponent<Weapon>().owner == agent.GetComponent<Entity>())
                                    return true;
                            }
                        }
                    }
                }

        }

        return false;
    }

    public override bool movementPass(GameObject agent)
    {
        return true;
    }

    public override bool perform(GameObject agent)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.STORM)
                {
                    if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.ELECTRIFIED)
                        statusApplied = true;

                    return true;
                }
        }

        return false;
    }
}
