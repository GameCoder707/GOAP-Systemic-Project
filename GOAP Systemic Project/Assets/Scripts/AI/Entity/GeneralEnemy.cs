using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralEnemy : MonoBehaviour, IGoap
{
    public enum ENEMY_TYPE { LIGHT = 0, MEDIUM = 1, HEAVY = 2}

    public float moveSpeed;
    public ENEMY_TYPE type;

    private EnemyStats stats;

    public string goalName;

    protected LayerMask interactableLayer = 1 << 6;
    protected LayerMask enemyLayer = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<EnemyStats>();
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasWeapon", stats.hasWeapon));
        worldData.Add(new KeyValuePair<string, object>("hasCuttingTool", stats.hasCuttingTool));
        worldData.Add(new KeyValuePair<string, object>("isStatusAppliedToWeapon", stats.isStatusAppliedToWeapon));

        PlayerBehaviour player = (PlayerBehaviour)FindObjectOfType(typeof(PlayerBehaviour));

        worldData.Add(new KeyValuePair<string, object>("attackPlayer", player.GetHealth() <= 0));
        worldData.Add(new KeyValuePair<string, object>("attackPlayerWithWeapon", player.GetHealth() <= 0));
        worldData.Add(new KeyValuePair<string, object>("attackPlayerWithStatWeapon", player.GetHealth() <= 0));

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

        Vector3 targetPos = new Vector3(nextAction.target.transform.position.x, transform.position.y, nextAction.target.transform.position.z);

        Vector3 prevPosition = transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        Vector3 faceDir = (transform.position - prevPosition).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextAction.target.transform.position) <= 1.5f)
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }

    // ENEMY METHODS

    public bool CanEnemyInteractWithObject(Collider[] interactables, string name, bool isEqual, ENEMY_TYPE eType) // WIP Function name
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 15.0f, enemyLayer);

        List<GameObject> weaponsInArea = new List<GameObject>();
        List<GameObject> otherEnemiesInArea = new List<GameObject>();

        for (int j = 0; j < interactables.Length; j++)
            if (interactables[j].gameObject.ToString().ToLower().Contains(name))
                weaponsInArea.Add(interactables[j].gameObject);

        for (int k = 0; k < enemies.Length; k++)
        {
            if(enemies[k].gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                if (isEqual)
                {
                    if (enemies[k].gameObject.GetComponent<GeneralEnemy>().type == eType)
                        otherEnemiesInArea.Add(enemies[k].gameObject);
                }
                else
                {
                    if (enemies[k].gameObject.GetComponent<GeneralEnemy>().type != eType)
                        otherEnemiesInArea.Add(enemies[k].gameObject);
                }
            }
        }


        // If the non-heavy enemy count is less than the weapon count, then he can go pick it up
        if (otherEnemiesInArea.Count < weaponsInArea.Count)
            return true;


        return false;
    }
}
