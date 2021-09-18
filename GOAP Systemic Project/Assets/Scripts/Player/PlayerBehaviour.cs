using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Health")]
    private const float maxHealth = 100;
    private float health = maxHealth;

    [Header("Movement and Aim")]
    private float moveSpeed = 7.0f;
    private LayerMask aimMask = 1 << 9;

    [Header("Burn Info")]
    private const float maxBurnPoints = 5;
    public float burnPoints = 0;
    private float burnDuration = 5.0f;
    private float burnDamage = 5.0f;

    [Header("Electric Info")]
    private const float maxElectricPoints = 5;
    public float electricPoints = 0;
    private float electricDuration = 1.5f;
    private float electricDamage = 3.0f;

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
    void Update()
    {
        if (health <= 0)
        {
            // Reset player
            health = 100;
            burnPoints = 0;
            burnDuration = 5.0f;
        }

        if (electricPoints < maxElectricPoints)
        {
            Move();
            Aim();
            Attack();
        }

        StatusEffects();

        DisplayHUD();
    }

    void StatusEffects()
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

    void DisplayHUD()
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

    void Aim()
    {
        Vector3 mousePos = Input.mousePosition;

        Ray camRay = Camera.main.ScreenPointToRay(mousePos); // Ray casted from screen position into the world
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, Mathf.Infinity, aimMask))
        {
            Vector3 aimTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(aimTarget);
        }
    }

    void Attack()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Called");
            GetComponentInChildren<Weapon>().anim.SetBool("isSwinging", true);
        }
    }

    public void InflictDamage(float damage)
    {
        health -= damage;
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