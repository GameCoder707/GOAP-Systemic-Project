using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    protected float maxHealth;
    public float health;

    [Header("Burn Info")]
    protected const float maxBurnPoints = 5;
    public float burnPoints = 0;
    protected float burnDuration = 5.0f;
    protected float burnDamage = 5.0f;
    protected float burnPointsDrainDelay = 5.0f; // Delay to drain the meter after a few seconds

    [Header("Electric Info")]
    protected const float maxElectricPoints = 5;
    public float electricPoints = 0;
    protected float electricDuration = 1.5f;
    protected float electricDamage = 3.0f;
    protected float electricPointsDrainDelay;

    [Header("UI")]
    public Image healthFill;
    public GameObject burnMeterObj;
    public Image burnMeterFill;
    public GameObject electricMeterObj;
    public Image electricMeterFill;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}

    protected void StatusEffects()
    {
        // Apply Burn Effect
        if (burnPoints >= maxBurnPoints)
        {
            if (burnDuration <= 0) // Burn completed
            {
                burnPoints = 0;
                burnDuration = 5.0f;
                burnPointsDrainDelay = 6.0f;
            }
            else // Burning
            {
                health -= burnDamage * Time.deltaTime;

                burnDuration -= Time.deltaTime;
            }
        }
        else // Drain after a few seconds if not filled
        {
            if (burnPointsDrainDelay <= 0)
                burnPoints -=  0.5f * Time.deltaTime;
            else
                burnPointsDrainDelay -= Time.deltaTime;
        }

        // Apply Shock Effect
        if (electricPoints >= maxElectricPoints)
        {
            if (electricDuration <= 0)
            {
                electricPoints = 0;
                electricDuration = 1.5f;
                electricPointsDrainDelay = 6.0f;
            }
            else
                electricDuration -= Time.deltaTime;
        }
        else // Drain after a few seconds if not filled
        {
            if (electricPointsDrainDelay <= 0)
                electricPoints -= 0.5f * Time.deltaTime;
            else
                electricPointsDrainDelay -= Time.deltaTime;
        }
    }

    protected void DisplayUIElements()
    {
        healthFill.fillAmount = health / maxHealth;

        // Burn Meter Display
        if (burnPoints > 0)
        {
            burnMeterObj.SetActive(true);

            burnMeterFill.fillAmount = burnPoints / maxBurnPoints;
        }
        else
            burnMeterObj.SetActive(false);

        // Electric Meter Display
        if (electricPoints > 0)
        {
            electricMeterObj.SetActive(true);

            electricMeterFill.fillAmount = electricPoints / maxElectricPoints;
        }
        else
            electricMeterObj.SetActive(false);
    }

    public void IncreaseBurnPoints()
    {
        burnPoints += 1;
        burnPointsDrainDelay = 6.0f;
    }

    public void IncreaseElectricPoints(int power)
    {
        electricPoints += power;

        if (electricPoints >= maxElectricPoints)
            health -= electricDamage;

        electricPointsDrainDelay = 6.0f;
    }



    public bool isBurning() { return burnPoints >= maxBurnPoints; }

    public bool isElectrified() { return electricPoints >= maxElectricPoints; }

    public float GetHealth() { return health; }
}
