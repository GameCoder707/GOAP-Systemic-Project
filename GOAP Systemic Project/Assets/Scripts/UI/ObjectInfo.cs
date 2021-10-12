using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
            gameObject.SetActive(false);
        else
            timer -= Time.deltaTime;
    }
}
