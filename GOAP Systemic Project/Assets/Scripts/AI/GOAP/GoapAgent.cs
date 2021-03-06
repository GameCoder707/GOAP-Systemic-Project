using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GoapAgent : MonoBehaviour
{
    private FSM stateMachine;

    private FSM.FSMState idleState; // finds something to do
    private FSM.FSMState moveToState; // moves to a target
    private FSM.FSMState performActionState; // performs an action

    private HashSet<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    private GoapPlanner planner;

    public bool isPatrolling; // Whether it's still or moving in its idle state
    private float searchTimer; // When the player leaves its detect zone
    public List<Transform> waypoints;
    private int currentWaypoint;
    private Vector3 prevPosition; // Previous Position
    private float moveSpeed;

    List<GoapAction> planList = new List<GoapAction>();

    private NavMeshAgent agent;

    public Text currentActionDisplay;
    public Text currentGoalDisplay;
    public GoalInfo currentGoal;

    void Start()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        findDataProvider();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.pushState(idleState);
        loadActions();

        moveSpeed = GetComponent<GeneralEnemy>().moveSpeed;
        prevPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (GetComponent<EnemyBehaviour>().playerInRange)
        {
            agent.isStopped = false;
            stateMachine.Update(gameObject);
            searchTimer = 3.0f;
        }
        else
        {
            if (searchTimer < 0)
            {
                currentGoalDisplay.text = "Current Goal: N/A";

                agent.speed = 0.5f * moveSpeed;
                agent.stoppingDistance = 0f;

                if (isPatrolling)
                {
                    agent.isStopped = false;

                    currentActionDisplay.text = "Current Action: Patrolling";

                    agent.SetDestination(waypoints[currentWaypoint].position);

                    if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) <= 1.0f)
                    {
                        if (currentWaypoint + 1 >= waypoints.Count)
                            currentWaypoint = 0;
                        else
                            currentWaypoint++;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, waypoints[0].position) <= 0.1f)
                    {
                        agent.isStopped = true;
                        currentActionDisplay.text = "Current Action: Standing By";
                    }
                    else
                    {
                        agent.isStopped = false;
                        currentActionDisplay.text = "Current Action: Moving to Resting Spot";
                        agent.SetDestination(waypoints[0].position);
                    }
                }

                Vector3 faceDir = (transform.position - prevPosition).normalized;

                if (faceDir != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 900 * Time.deltaTime);
                }

                prevPosition = transform.position;
            }
            else
            {
                agent.isStopped = true;
                searchTimer -= Time.deltaTime;

                if (searchTimer <= 0)
                {
                    foreach (GoapAction a in availableActions)
                        a.doReset();

                    stateMachine.popState();
                    stateMachine.pushState(idleState);
                }
            }
        }
    }


    public void addAction(GoapAction a)
    {
        availableActions.Add(a);
    }

    public GoapAction getAction(Type action)
    {
        foreach (GoapAction g in availableActions)
        {
            if (g.GetType().Equals(action))
                return g;
        }
        return null;
    }

    public void removeAction(GoapAction action)
    {
        availableActions.Remove(action);
    }

    private bool hasActionPlan()
    {
        return currentActions.Count > 0;
    }

    public Queue<GoapAction> GetActionPlan()
    {
        return currentActions;
    }

    private void createIdleState()
    {
        idleState = (fsm, gameObj) =>
        {
            // GOAP planning

            // get the world state and the goal we want to plan for
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            List<GoalInfo> goals = dataProvider.createGoalStates();
            currentGoal = new GoalInfo();

            // Plan
            Queue<GoapAction> plan = planner.plan(gameObject, availableActions, worldState, goals, ref currentGoal);

            currentGoalDisplay.text = currentGoal.goalDesc;

            if (plan != null)
            {
                // we have a plan, hooray!
                currentActions = plan;
                dataProvider.planFound(goals, plan);

                fsm.popState(); // move to PerformAction state
                fsm.pushState(performActionState);

            }
            else
            {
                // ugh, we couldn't get a plan
                Debug.Log("<color=orange>Failed Plan:</color>" + prettyPrint(goals));
                dataProvider.planFailed(goals);
                fsm.popState(); // move back to IdleAction state
                fsm.pushState(idleState);
            }

        };
    }

    private bool CheckActionInPlan(Queue<GoapAction> actionPlan, string actionName)
    {
        foreach (GoapAction a in actionPlan)
        {
            if (a.GetType().Name == actionName)
                return true;
        }

        return false;
    }

    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            // move the game object

            GoapAction action = currentActions.Peek();
            if (action.requiresInRange() && action.target == null)
            {
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.popState(); // move
                fsm.popState(); // perform
                fsm.pushState(idleState);
                return;
            }

            currentActionDisplay.text = action.GetMovingActionName();
            if (action.movementPass(gameObj))
            {
                if (dataProvider.moveAgent(action))
                {
                    fsm.popState();
                }
            }
            else
            {
                fsm.popState(); // move
                fsm.popState(); // perform
                fsm.pushState(idleState);
            }

        };
    }

    private void createPerformActionState()
    {

        performActionState = (fsm, gameObj) =>
        {
            // perform the action

            if (!hasActionPlan())
            {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            GoapAction action = currentActions.Peek();
            if (action.isDone())
            {
                // the action is done. Remove it so we can perform the next one
                currentActions.Dequeue();
            }

            if (hasActionPlan())
            {
                // perform the next action
                action = currentActions.Peek();
                bool inRange = action.requiresInRange() ? action.isInRange() : true;

                if (inRange)
                {
                    // we are in range, so perform the action

                    bool success = false;

                    currentActionDisplay.text = action.GetPerformingActionName();
                    success = action.perform(gameObj);

                    if (!success)
                    {
                        // action failed, we need to plan again
                        fsm.popState();
                        fsm.pushState(idleState);
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    // we need to move there first
                    // push moveTo state
                    fsm.pushState(moveToState);
                }

            }
            else
            {
                // no actions left, move to Plan state
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
            }

        };
    }

    private void findDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void loadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions)
        {
            availableActions.Add(a);
        }
        //Debug.Log("Found actions: " + prettyPrint(actions));
    }

    public static string prettyPrint(List<GoalInfo> states)
    {
        String s = "";
        foreach (GoalInfo gi in states)
            foreach (KeyValuePair<string, object> kvp in gi.goal)
            {
                s += kvp.Key + ":" + kvp.Value.ToString();
                s += ", ";
            }
        return s;
    }

    public static string prettyPrint(Queue<GoapAction> actions)
    {
        String s = "";
        int i = 0;

        foreach (GoapAction a in actions)
        {
            if (i < 1)
            {
                s += a.gameObject.name + ": ";
                i++;
            }
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string prettyPrint(GoapAction[] actions)
    {
        String s = "";
        foreach (GoapAction a in actions)
        {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(GoapAction action)
    {
        String s = "" + action.GetType().Name;
        return s;
    }
}
