using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAIScript : MonoBehaviour
{
    private LayerMask interactableLayer;

    private bool isCarryingWeapon;
    private bool targetLocked;

    private Transform target;

    public GameObject woodenStick;
    private GameObject carryingWeapon;
    public GameObject fireOnObject;

    // Start is called before the first frame update
    void Start()
    {
        interactableLayer = 1 << 6;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCarryingWeapon)
        {
            if(targetLocked)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, 2 * Time.deltaTime);

                if (Vector3.Distance(transform.position, target.position) <= 2.0f)
                {
                    target = null;
                    Instantiate(fireOnObject, carryingWeapon.transform.position + Vector3.up, carryingWeapon.transform.rotation, transform);
                    enabled = false;
                }
            }
            else
            {
                Collider[] interactables = Physics.OverlapSphere(transform.position, 10.0f, interactableLayer);

                if (interactables.Length > 0)
                {
                    for (int i = 0; i < interactables.Length; i++)
                    {
                        if (interactables[i].gameObject.ToString().ToLower().Contains("campfire"))
                        {
                            target = interactables[i].gameObject.transform;
                            targetLocked = true;
                        }
                    }
                }
            }
        }
        else
        {
            if(targetLocked)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, 2 * Time.deltaTime);

                if(Vector3.Distance(transform.position, target.position) <= 2.0f)
                {
                    target = null;
                    isCarryingWeapon = true;
                    targetLocked = false;
                    carryingWeapon = Instantiate(woodenStick, transform.position + Vector3.up, transform.rotation, transform);
                }
            }
            else
            {
                Collider[] interactables = Physics.OverlapSphere(transform.position, 10.0f, interactableLayer);

                if (interactables.Length > 0)
                {
                    for (int i = 0; i < interactables.Length; i++)
                    {
                        if (interactables[i].gameObject.ToString().ToLower().Contains("tree"))
                        {
                            target = interactables[i].gameObject.transform;
                            targetLocked = true;
                        }
                    }
                }
            }
            
        }
    }
}
