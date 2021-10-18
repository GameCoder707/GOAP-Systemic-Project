using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private float tinyMoveAmount;
    private bool swap;

    public bool isTinyMoving;
    public bool isPushed;
    public bool isStatusApplied;

    public GameObject fireEffectSource; // Fire prefab
    private GameObject fireEffect;

    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        tinyMoveAmount = 0.015f;
        isTinyMoving = true;

        groundLayer = 1 << 0;
        fireEffect = Instantiate(fireEffectSource);
        fireEffect.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        fireEffect.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
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
            if (!Physics.Raycast(transform.position, -Vector3.up, 5.0f, groundLayer))
                gameObject.SetActive(false);
        }

        if(isStatusApplied)
            fireEffect.transform.position = transform.position + Vector3.up;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerBehaviour>() != null)
        {
            other.gameObject.GetComponent<PlayerBehaviour>().InflictDamage(15.0f);
        }
    }

    public void BurnBoulder()
    {
        fireEffect.SetActive(true);
        isStatusApplied = true;
    }
}
