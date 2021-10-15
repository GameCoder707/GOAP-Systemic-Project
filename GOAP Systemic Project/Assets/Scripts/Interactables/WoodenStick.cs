using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenStick : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        damage = 4;

        MainStart();
    }

    // Update is called once per frame
    void Update()
    {
        MainUpdate();
    }
}
