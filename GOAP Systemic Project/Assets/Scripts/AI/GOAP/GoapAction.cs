using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour
{
    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;

    private bool inRange = false;

    /* The cost of performing the action. 
	 Cheaper the action, the more likely it gets chosen by the planner. */
    public float cost = 1f;

    /* An object that an entity can execute an action upon. Not always mandatory. */
    public GameObject target;

    public bool finishedAction = false;

    /* The minimum distance the entity will have to be
       within the GameObject target to execute the action */
    public float minimumDistance = 1.5f;

    protected string movingActionText; // Text to display when the agent is moving
    protected string performingActionText; // Text to display when the agent is performing

    public GoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    public void doReset()
    {
        reset();
        inRange = false;
        //target = null;
    }

    /* Reset any variables that need to be reset before planning happens again. */
    public abstract void reset();

    /* A secondary reset function for actions that are no longer in the final plan of actions */
    public abstract void secondaryReset();

    /* Is the action done? */
    public abstract bool isDone();

    /*Procedurally check if this action can run. Not all actions
	  will need this, but some might. */
    public abstract bool checkProceduralPrecondition(GameObject agent);

    /* To check if object is moving towards the right target */
    public abstract bool movementPass(GameObject agent);

    /**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
    public abstract bool perform(GameObject agent);

    /**
	 * Does this action need to be within range of a target game object?
	 * If not then the moveTo state will not need to run for this action.
	 */
    public abstract bool requiresInRange();

    public string GetMovingActionName()
    {
        return "Current Action: " + movingActionText;
    }

    public string GetPerformingActionName()
    {
        return "Current Action: " + performingActionText;
    }

    /**
     * Are we in range of the target?
     * The MoveTo state will set this and it gets reset each time this action is performed.
     */
    public bool isInRange()
    {
        return inRange;
    }

    public void setInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    public void addPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void removePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in preconditions)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            preconditions.Remove(remove);
    }


    public void addEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }


    public void removeEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in effects)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }

    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }
}