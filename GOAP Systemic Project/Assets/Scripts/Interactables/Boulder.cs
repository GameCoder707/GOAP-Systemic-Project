using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private float tinyMoveAmount;
    private bool swap;

    public bool isTinyMoving;
    public bool isPushed;

    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        tinyMoveAmount = 0.01f;
        isTinyMoving = true;

        groundLayer = 1 << 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTinyMoving)
        {
            if (swap)
                transform.position = transform.position + (transform.right * tinyMoveAmount);
            else
                transform.position = transform.position - (transform.right * tinyMoveAmount);

            swap = !swap;
        }
        else
        {
            RaycastHit hit;

            if (!Physics.Raycast(transform.position, -Vector3.up, 5.0f, groundLayer))
                GetComponent<Rigidbody>().useGravity = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerBehaviour>() != null)
        {
            other.gameObject.GetComponent<PlayerBehaviour>().InflictDamage(15.0f);
        }
    }
}
