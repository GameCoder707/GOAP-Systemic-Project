using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : Entity
{
    [Header("Movement and Aim")]
    private float moveSpeed = 7.0f;
    private LayerMask aimMask = 1 << 9;

    public Text weatherInfo;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            if (electricPoints < maxElectricPoints) // Stunning the enemy
            {
                Move();
                Aim();
                Attack();
            }
        }

        StatusEffects();
        DisplayUIElements();

    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
            {
                string dynamicStatus = "";

                if (hit.collider.gameObject.GetComponent<Weather>().isDynamic)
                    dynamicStatus = "ON";
                else
                    dynamicStatus = "OFF";

                weatherInfo.text = "Weather: " + hit.collider.gameObject.GetComponent<Weather>().weatherType.ToString() +
                    "\n Dynamic: " + dynamicStatus;
            }

        }
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
            GetComponentInChildren<Weapon>().anim.SetBool("isSwinging", true);
        }
    }

    public void InflictDamage(float damage)
    {
        health -= damage;
    }

}