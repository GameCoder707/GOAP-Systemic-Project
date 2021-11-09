using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class Weather : MonoBehaviour
{
    public enum WEATHER_TYPE { SUNNY = 0, RAINY = 1,  STORM = 2, HEAT_WAVE = 3 }

    public WEATHER_TYPE weatherType;
    private WEATHER_TYPE previousWeatherType;

    public float affectAreaX;
    public float affectAreaZ;

    public List<Weapon> affectingWeapons = new List<Weapon>();
    public List<Weapon> affectedWeapons = new List<Weapon>();
    private List<float> affectTimers = new List<float>();

    public GameObject lightningVFX;
    public GameObject heatWaveVFX;

    public bool isDynamic;
    private float swapTimer;

    // Start is called before the first frame update
    void Start()
    {
        swapTimer = 3.0f;
    }

    void Update()
    {
        if (isDynamic)
        {
            if (swapTimer <= 0)
            {
                previousWeatherType = weatherType;
                int weatherNum = Random.Range(0, 4);

                if (weatherNum == (int)previousWeatherType)
                {
                    if (weatherNum + 1 >= 4)
                        weatherNum = 0;
                    else
                        weatherNum++;
                }

                weatherType = (WEATHER_TYPE)weatherNum;
                swapTimer = 3.0f;
            }
            else
                swapTimer -= Time.deltaTime;
        }

        if (weatherType == WEATHER_TYPE.HEAT_WAVE)
            heatWaveVFX.SetActive(true);
        else
            heatWaveVFX.SetActive(false);

        if (weatherType != WEATHER_TYPE.RAINY)
        {
            Weapon[] weaponsUnderWeather = FindObjectsOfType<Weapon>();

            if (weaponsUnderWeather.Length > 0)
            {
                for (int i = 0; i < weaponsUnderWeather.Length; i++)
                {
                    if (weaponsUnderWeather[i].transform.parent != null)
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

                                    GameObject obj = Instantiate(lightningVFX);
                                    obj.GetComponent<LightningBoltScript>().StartObject.transform.position =
                                        new Vector3(affectingWeapons[i].gameObject.transform.position.x,
                                        transform.position.y,
                                        affectingWeapons[i].gameObject.transform.position.z);
                                    obj.GetComponent<LightningBoltScript>().EndObject.transform.position = affectingWeapons[i].transform.position;

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
