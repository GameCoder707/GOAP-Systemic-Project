using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    public enum WEATHER_TYPE { SUNNY = 0, RAINY = 1, SNOW = 2, STORM = 3, HEAT_WAVE = 4 }

    public WEATHER_TYPE weatherType;

    public float affectAreaX;
    public float affectAreaZ;

    public List<Weapon> affectingWeapons = new List<Weapon>();
    private List<float> affectTimers = new List<float>();

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        Weapon[] weaponsUnderWeather = FindObjectsOfType<Weapon>();

        if (weaponsUnderWeather.Length > 0)
        {
            for (int i = 0; i < weaponsUnderWeather.Length; i++)
            {
                if (!affectingWeapons.Contains(weaponsUnderWeather[i]) && // Not readding the same weapon to the affecting list
                    weaponsUnderWeather[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() != null && // It has to be picked up
                    IsWeaponInArea(weaponsUnderWeather[i].gameObject.transform.position) && // It has to be within weather area
                    weaponsUnderWeather[i].weaponStatus == Weapon.WEAPON_STATUS.NONE) // No status has been applied yet
                {

                    affectingWeapons.Add(weaponsUnderWeather[i]);
                    affectTimers.Add(1.0f);
                }
            }
        }

        if (affectingWeapons.Count > 0)
        {
            for (int i = 0; i < affectingWeapons.Count; i++)
            {
                if (affectTimers[i] < 0)
                {
                    switch (weatherType)
                    {
                        case WEATHER_TYPE.STORM:
                            if(affectingWeapons[i].conductive)
                            {
                                affectingWeapons[i].ElectrifyWeapon();

                                affectingWeapons.Remove(affectingWeapons[i]);
                                affectTimers.Remove(affectTimers[i]);
                                i--;
                            }
                            break;
                        case WEATHER_TYPE.HEAT_WAVE:
                            if(affectingWeapons[i].flammable)
                            {
                                affectingWeapons[i].BurnWeapon();

                                affectingWeapons.Remove(affectingWeapons[i]);
                                affectTimers.Remove(affectTimers[i]);
                                i--;
                            }
                            break;
                    }
                }
                else
                    affectTimers[i] -= Time.deltaTime;
            }
        }

    }

    private bool IsWeaponInArea(Vector3 weaponPos)
    {
        if (weaponPos.x >= transform.position.x - affectAreaX &&
            weaponPos.x <= transform.position.x + affectAreaX &&
            weaponPos.z >= transform.position.z - affectAreaZ &&
            weaponPos.z <= transform.position.z + affectAreaZ)
            return true;
        else
            return false;
    }
}
