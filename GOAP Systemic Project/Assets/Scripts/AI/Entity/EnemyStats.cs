using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public bool hasWeapon = false;
    public bool combatReady = false;
    public bool hasCuttingTool = false;

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
