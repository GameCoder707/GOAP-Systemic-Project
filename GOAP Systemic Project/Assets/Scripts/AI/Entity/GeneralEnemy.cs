using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralEnemy : MonoBehaviour, IGoap
{
    private float moveSpeed = 4;
    private EnemyStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<EnemyStats>();
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasWeapon", stats.hasWeapon));
        worldData.Add(new KeyValuePair<string, object>("combatReady", stats.combatReady));
        worldData.Add(new KeyValuePair<string, object>("hasCuttingTool", stats.hasCuttingTool));
        worldData.Add(new KeyValuePair<string, object>("isStatusAppliedToWeapon", stats.isStatusAppliedToWeapon));

        PlayerBehaviour player = (PlayerBehaviour)FindObjectOfType(typeof(PlayerBehaviour));

        worldData.Add(new KeyValuePair<string, object>("attackPlayer", player.health <= 0));
        worldData.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", player.health <= 0));

        return worldData;
    }

    public abstract HashSet<KeyValuePair<string, object>> createGoalState();

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {

    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;

        Vector3 targetPos = new Vector3(nextAction.target.transform.position.x, gameObject.transform.position.y, nextAction.target.transform.position.z);

        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, step);

        if (Vector3.Distance(gameObject.transform.position, nextAction.target.transform.position) <= 1.5f)
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }
}
