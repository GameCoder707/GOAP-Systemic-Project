using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Health")]
    private const float maxHealth = 100;
    private float health = maxHealth;

    [Header("Movement")]
    private float moveSpeed = 7.0f;

    [Header("Burn Info")]
    private const float maxBurnPoints = 5;
    public float burnPoints = 0;
    private float burnDuration = 5.0f;
    private float burnDamage = 5.0f;

    [Header("UI")]
    public Image healthFill;
    public GameObject burnMeterObj;
    public Image burnMeterFill;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            // Reset player
            health = 100;
            burnPoints = 0;
            burnDuration = 5.0f;
        }

        // Apply Burn Damage
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

        Move();

        healthFill.fillAmount = health / maxHealth;

        if (burnPoints > 0)
        {
            burnMeterObj.SetActive(true);

            burnMeterFill.fillAmount = burnPoints / maxBurnPoints;
        }
        else
            burnMeterObj.SetActive(false);
    }

    void Move()
    {
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        Vector3 moveVector = new Vector3(xMove, 0.0f, zMove);

        if (moveVector.magnitude > 0.01f)
        {
            transform.position += (moveVector.normalized * moveSpeed * Time.deltaTime);
        }
    }

    public void InflictDamage(float damage, Weapon.WEAPON_STATUS status = Weapon.WEAPON_STATUS.NONE)
    {
        health -= damage;

        //switch (status)
        //{
        //    case Weapon.WEAPON_STATUS.BURNING:
        //        if (burnPoints < maxBurnPoints)
        //            burnPoints += 1;
        //        break;
        //}

    }

    public float GetHealth() { return health; }
}
