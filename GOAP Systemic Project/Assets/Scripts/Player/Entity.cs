using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Electric Info")]
    protected const float maxElectricPoints = 5;
    public float electricPoints = 0;
    protected float electricDuration = 1.5f;
    protected float electricDamage = 3.0f;

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
            if (burnDuration <= 0)
            {
                burnPoints = 0;
                burnDuration = 5.0f;
            }
            else
            {
                health -= burnDamage * Time.deltaTime;

                burnDuration -= Time.deltaTime;
            }
        }

        // Apply Shock Effect
        if (electricPoints >= maxElectricPoints)
        {
            if (electricDuration <= 0)
            {
                electricPoints = 0;
                electricDuration = 1.5f;
            }
            else
                electricDuration -= Time.deltaTime;
        }
    }

    public void IncreaseElectricPoints()
    {
        electricPoints += 1;

        if (electricPoints >= maxElectricPoints)
            health -= electricDamage;
    }

    public bool isBurning() { return burnPoints >= maxBurnPoints; }

    public bool isElectrified() { return electricPoints >= maxElectricPoints; }

    public float GetHealth() { return health; }
}
