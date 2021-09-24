using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public bool hasWeapon = false;
    public bool hasCuttingTool = false;

    private void Start()
    {
        if (GetComponentInChildren<Weapon>() != null)
            hasWeapon = true;
    }

    public bool isStatusAppliedToWeapon
    {
        get
        {
            if(GetComponentInChildren<Weapon>() != null)
            {
                return GetComponentInChildren<Weapon>().weaponStatus != Weapon.WEAPON_STATUS.NONE;
            }

            return false;

        }
    }

}
