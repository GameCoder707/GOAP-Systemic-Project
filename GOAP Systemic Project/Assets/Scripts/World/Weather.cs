using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weather : MonoBehaviour
{
    public enum WEATHER_TYPE { SUNNY = 0, RAINY = 1, SNOW = 2, STORM = 3, HEAT_WAVE = 4 }

    public WEATHER_TYPE weatherType;

    public float affectAreaX;
    public float affectAreaZ;

    public List<Weapon> affectingWeapons = new List<Weapon>();
    public List<Weapon> affectedWeapons = new List<Weapon>();
    private List<float> affectTimers = new List<float>();

    public Text weatherInfo;

    // Start is called before the first frame update
    void Start()
    {
        weatherInfo.text = "Weather: " + weatherType.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(weatherType != WEATHER_TYPE.RAINY)
        {
            Weapon[] weaponsUnderWeather = FindObjectsOfType<Weapon>();

            if (weaponsUnderWeather.Length > 0)
            {
                for (int i = 0; i < weaponsUnderWeather.Length; i++)
                {
                    if (!affectingWeapons.Contains(weaponsUnderWeather[i]) && // Don't readd the weapon if it's currently being affected...
                        !affectedWeapons.Contains(weaponsUnderWeather[i]) && // or has a status element applied to it
                        weaponsUnderWeather[i].gameObject.transform.parent.gameObject.GetComponent<Entity>() != null && // It has to be picked up
                        IsWeaponInArea(weaponsUnderWeather[i].gameObject.transform.position) &&
                        weaponsUnderWeather[i].weaponStatus == Weapon.WEAPON_STATUS.NONE) // It has to be within weather area
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
                    switch (weatherType)
                    {
                        case WEATHER_TYPE.STORM:
                            if (affectingWeapons[i].conductive)
                            {
                                if (affectTimers[i] < 0)
                                {
                                    affectingWeapons[i].ElectrifyWeapon();
                                    affectedWeapons.Add(affectingWeapons[i]);

                                    affectingWeapons.Remove(affectingWeapons[i]);
                                    affectTimers.Remove(affectTimers[i]);

                                    i--;
                                }
                                else
                                    affectTimers[i] -= Time.deltaTime;
                            }
                            break;
                        case WEATHER_TYPE.HEAT_WAVE:
                            if (affectingWeapons[i].flammable)
                            {
                                if (affectTimers[i] < 0)
                                {
                                    affectingWeapons[i].BurnWeapon();
                                    affectedWeapons.Add(affectingWeapons[i]);

                                    affectingWeapons.Remove(affectingWeapons[i]);
                                    affectTimers.Remove(affectTimers[i]);
                                    i--;
                                }
                                else
                                    affectTimers[i] -= Time.deltaTime;
                            }
                            break;
                    }
                }

            }

            if (affectedWeapons.Count > 0)
            {
                for (int i = 0; i < affectedWeapons.Count; i++)
                {
                    if (affectedWeapons[i].weaponStatus == Weapon.WEAPON_STATUS.NONE)
                    {
                        affectedWeapons.Remove(affectedWeapons[i]);
                        i--;
                    }
                }
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
