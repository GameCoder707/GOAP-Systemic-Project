using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerFromCoverAction : GoapAction
{
    private bool playerDead = true;

    private LayerMask interactableLayer = 1 << 6;

    public AttackPlayerFromCoverAction()
    {
        addEffect("attackPlayerFromCover", true);
    }

    public override void reset()
    {
        // Reset
        playerDead = false;
    }

    public override void secondaryReset() { }

    public override bool isDone()
    {
        return playerDead;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if(interactables.Length > 0)
        {
            for(int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.name.ToLower().Contains("cover"))
                {
                    if (!interactables[i].gameObject.GetComponent<Barrier>().occupied)
                    {
                        target = interactables[i].gameObject.GetComponent<Barrier>().posB.gameObject;
                        interactables[i].gameObject.GetComponent<Barrier>().occupied = true;
                        break;
                    }
                }
            }
        }

        return target != null;
    }

    public override bool movementPass(GameObject agent)
    {
        return true;
    }

    public override bool perform(GameObject agent)
    {
        PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();

        if (Vector3.Distance(player.gameObject.transform.position, transform.position) >= 6f)
            return true;
        else
            return false;

    }
}
