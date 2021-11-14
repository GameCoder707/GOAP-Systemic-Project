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

    public GameObject sunnySunlight;
    public GameObject stormSunlight;
    public GameObject heatWaveVFX;
    public GameObject rainyVFX;

    public GameObject lightningVFX;
    private float lightningSpawnDelay;

    public bool isDynamic;
    private float weatherSwapTimer;

    // Start is called before the first frame update
    void Start()
    {
        weatherSwapTimer = 5.0f;
        lightningSpawnDelay = 0.75f;
    }

    void Update()
    {
        // ************************************************** //
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            isDynamic = !isDynamic;

            if (!isDynamic)
                weatherSwapTimer = 5.0f;
        }

        if (isDynamic)
        {
            if (weatherSwapTimer <= 0)
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
                weatherSwapTimer = 5.0f;
            }
            else
                weatherSwapTimer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                weatherType = WEATHER_TYPE.SUNNY;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                weatherType = WEATHER_TYPE.RAINY;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                weatherType = WEATHER_TYPE.STORM;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                weatherType = WEATHER_TYPE.HEAT_WAVE;
        }

        // ************************************************** //

        stormSunlight.SetActive(weatherType == WEATHER_TYPE.STORM);
        heatWaveVFX.SetActive(weatherType == WEATHER_TYPE.HEAT_WAVE);
        sunnySunlight.SetActive(weatherType == WEATHER_TYPE.SUNNY);
        rainyVFX.SetActive(weatherType == WEATHER_TYPE.RAINY);

        // ************************************************** //

        if (weatherType == WEATHER_TYPE.STORM)
            StormVFXManager();

        // ************************************************** //

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

    private void StormVFXManager()
    {
        if (lightningSpawnDelay <= 0)
        {
            float startX = Random.Range(transform.position.x - 15f, transform.position.x + 15f);
            float startZ = Random.Range(transform.position.z - 15f, transform.position.z + 15f);

            GameObject obj = Instantiate(lightningVFX);
            obj.GetComponent<LightningBoltScript>().StartObject.transform.position =
                new Vector3(startX, transform.position.y, startZ);

            obj.GetComponent<LightningBoltScript>().EndObject.transform.position = new Vector3(startX, 0f, startZ);

            lightningSpawnDelay = 0.75f;
        }
        else
            lightningSpawnDelay -= Time.deltaTime;
    }
}