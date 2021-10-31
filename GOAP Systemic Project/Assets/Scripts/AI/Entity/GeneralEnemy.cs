using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class GeneralEnemy : MonoBehaviour, IGoap
{
    public enum ENEMY_TYPE { LIGHT = 0, MEDIUM = 1, HEAVY = 2 }

    public float moveSpeed;
    public ENEMY_TYPE type;

    private EnemyStats stats;

    public string goalName;

    private NavMeshAgent agent;

    protected LayerMask interactableLayer = 1 << 6;
    protected LayerMask enemyLayer = 1 << 8;

    private Vector3 prevPosition;

    public Text currentGoalDisplay;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = transform.position;

        stats = GetComponent<EnemyStats>();

        agent = GetComponent<NavMeshAgent>();
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
        worldData.Add(new KeyValuePair<string, object>("attackPlayerFromCover", player.GetHealth() <= 0));
        worldData.Add(new KeyValuePair<string, object>("attackPlayerWithObjects", player.GetHealth() <= 0));
        worldData.Add(new KeyValuePair<string, object>("canAttack", GetComponent<EnemyBehaviour>().health >= 25));

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
        Vector3 targetPos = Vector3.zero;

        if (nextAction.target.GetComponent<Boulder>() != null)
            targetPos = new Vector3(nextAction.target.GetComponent<Boulder>().startPos.x,
                transform.position.y, nextAction.target.GetComponent<Boulder>().startPos.z);
        else
            targetPos = new Vector3(nextAction.target.transform.position.x,
                transform.position.y, nextAction.target.transform.position.z);

        agent.SetDestination(targetPos);
        agent.stoppingDistance = nextAction.minimumDistance;
        agent.speed = moveSpeed;

        Vector3 faceDir = (transform.position - prevPosition).normalized;

        if(faceDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 800 * Time.deltaTime);
        }

        prevPosition = transform.position;

        if (Vector3.Distance(transform.position, targetPos) <= nextAction.minimumDistance)
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }

    // ENEMY METHODS
    protected virtual bool CheckForWeaponSource(string statusCompatibility = "") { return false; }

    protected virtual bool CheckForObjects() { return false; }

    protected bool CheckForCover()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);
        PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();

        if (interactables.Length > 0 && Vector3.Distance(player.gameObject.transform.position, transform.position) >= 6f)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.name.ToLower().Contains("cover"))
                {
                    if (!interactables[i].gameObject.GetComponent<Barrier>().occupied)
                        return true;
                }
            }
        }

        return false;
    }

    public bool CheckForFireSource()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.HEAT_WAVE &&
                            CheckForWeaponSource("Fire"))
                {
                    return true;
                }

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("campfire") &&
                    hit.collider.gameObject.GetComponent<Weather>().weatherType != Weather.WEATHER_TYPE.RAINY)
                {
                    if (CheckForWeaponSource("Fire"))
                        return true;
                }
            }
        }

        return false;
    }

    protected bool CheckForElectricSource()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.STORM &&
                    CheckForWeaponSource("Electric"))
                {
                    return true;
                }

        return false;
    }

    public bool CanEnemyInteractWithObject(Collider[] interactables, string name, bool isEqual, ENEMY_TYPE eType) // WIP Function name
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 15.0f, enemyLayer);

        List<GameObject> interactablesInArea = new List<GameObject>();
        List<GameObject> otherEnemiesInArea = new List<GameObject>();

        for (int j = 0; j < interactables.Length; j++)
            if (interactables[j].gameObject.ToString().ToLower().Contains(name))
                interactablesInArea.Add(interactables[j].gameObject);

        for (int k = 0; k < enemies.Length; k++)
        {
            if (enemies[k].gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                if (isEqual)
                {
                    if (enemies[k].gameObject.GetComponent<GeneralEnemy>().type == eType)
                        if (enemies[k].GetComponentInChildren<Weapon>() == null && // Ignoring enemies who already have a weapon or a cutting tool
                            enemies[k].GetComponent<EnemyStats>().hasCuttingTool == false)
                            otherEnemiesInArea.Add(enemies[k].gameObject);
                }
                else
                {
                    if (enemies[k].gameObject.GetComponent<GeneralEnemy>().type != eType)
                        if (enemies[k].GetComponentInChildren<Weapon>() == null &&
                            enemies[k].GetComponent<EnemyStats>().hasCuttingTool == false)
                            otherEnemiesInArea.Add(enemies[k].gameObject);
                }
            }
        }


        // If the enemy buddy count is less than the weapon count, then he can go pick it up
        if (otherEnemiesInArea.Count < interactablesInArea.Count)
            return true;
        else
            return false;
    }
}
