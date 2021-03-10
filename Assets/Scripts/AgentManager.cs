/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the agents' state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    #region Fields/Properties

    // Private fields
    public float foodQuantity; // This agents collection of food
    public float age; // The agent's age

    // Public fields
    public float altruism; // Scale from 0-100, how likely they are to change to solar
    public float charisma; // Scale from 0-100, their ability to influence other agents
    public float trust; // Scale from 0-100, their ability to be manipulated by other agents
    public float foresight; // Scale from 0-100, their ability to predict a calamity
    public float foodToBreed; // The amount of food needed to breed
    public string energySource; // Either "solar" or "fossilFuels"
    public int generation;
    public float lifespan; // When this agent will die due to time
    public bool switchable;

    public SimManager simScript; // A reference to the world state
    public AgentManager agentScript; // A reference to another agent (used for collisions)
    public AgentManager parentScript; // A reference to this agent's parent to determine traits
    public GameObject AgentTemplate;

    #endregion

    #region BuiltInMethods

    // Awake is called when this agent is instantiated, used to assign traits
    void Awake()
    {
        // TODO: Get SimManager simScript
        switchable = true;

        simScript = GameObject.Find("SimManager").GetComponent<SimManager>();

        age = 0;
        foodQuantity = 50f;
        //if (Random.Range(0f, 100f) < 10f)
        //{
        //    switchable = false;
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        if (parentScript) // If this agent has a parent then inherit a lot of traits from them
        {
            altruism = parentScript.altruism + Random.Range(-10f, 10f);
            charisma = parentScript.charisma + Random.Range(-10f, 10f);
            trust = parentScript.trust + Random.Range(-10f, 10f);
            foresight = parentScript.foresight + Random.Range(-10f, 10f);
            generation = parentScript.generation++;
            energySource = parentScript.energySource; // Kids are more likely to follow their parents' lead, so use the same energy source
            lifespan = parentScript.lifespan + Random.Range(-5f, 5f);
        }
        else // Agent is first generation
        {
            altruism = Random.Range(0f, 100f);
            charisma = Random.Range(0f, 100f);
            trust = Random.Range(0f, 100f);
            foresight = Random.Range(0f, 100f);
            generation = 0;
            energySource = "fossilFuels";

            lifespan = simScript.averageLifespan + Random.Range(-10f, 10f);
            Debug.Log(lifespan);
            foodToBreed = 50f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is used for changing the simulation state 
    void FixedUpdate()
    {
        EatFood();
        CheckDeath();
        CheckSpawn();
        CheckCalamity();
        GatherFood();
        age += 0.1f;
    }

    // LateUpdate is called after FixedUpdate, used to handle collisions
    void LateUpdate()
    {
        ResolveCollision();
    }

    #endregion BuiltInMethods

    #region Methods

    /// <summary>
    /// This can be called externally in order to change this agent's lifespan.
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeLifespan(float amount)
    {
        lifespan += amount;
    }

    /// <summary>
    /// This methods checks if an agent is capable of spawning a child.
    /// If it is, then it spawns a child.
    /// </summary>
    private void CheckSpawn()
    {
        if (foodQuantity >= foodToBreed + 10f && age > 20f)
        {
            foodQuantity -= foodToBreed; // Breeding uses food
            GameObject agent = GameObject.Instantiate(AgentTemplate);
            AgentManager agentScript = agent.GetComponent<AgentManager>();
            agentScript.parentScript = GetComponent<AgentManager>();
            agent.name = "Agent" + simScript.agentCount;
            simScript.agents.Add(agentScript);

            // Change the player's location
            float spawnX = transform.position.x + Random.Range(-10f, 10f);
            float spawnY = transform.position.y + Random.Range(-10f, 10f);
            agent.transform.position = new Vector2(spawnX, spawnY);
            simScript.agentCount++;

        }
    }

    /// <summary>
    /// This method checks whether the agent has starved or is too old.
    /// If either is true, the agent dies.
    /// </summary>
    private void CheckDeath()
    {
        // Check if the agent starves or dies from old age
        if (foodQuantity <= 0)
        {
            Debug.Log("This agent has died from lack of food.");
            Destroy(gameObject);
        }
        if (age >= lifespan)
        {
            Debug.Log("This agent has died from old age.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This agent tries to predict if a calamity will happen (Average lifespan gets too low, or not enough food being produced, both as consequences of pollution)
    /// </summary>
    public void CheckCalamity()
    {
        // TODO: Factor in altruism
        if ((simScript.foodProduction - simScript.pollution) < foresight && energySource != "solar" && switchable) 
        {
            energySource = "solar";
            Debug.Log("This agent has changed their energy source due to lack of food production");
            Debug.Log(simScript.foodProduction + " " + simScript.pollution);
        }
        if (simScript.averageLifespan < foresight && energySource != "solar" && switchable)
        {
            energySource = "solar";
            Debug.Log("This agent has changed energy sources due to low average lifespan.");
        }
    }

    /// <summary>
    /// When two agents collide have them try to convince each other to change their energy sources .
    /// </summary>
    public void ResolveCollision()
    {
        // TODO: Factor in altruism
        // If their charisma score is higher than your trust score, follow their lead
        if (agentScript)
        {
            if (agentScript.charisma > trust)
            {
                energySource = agentScript.energySource;
            }
        }
    }

    /// <summary>
    /// This method increases the agents food supply, decreases the global food supply, and possibly applies global penalties.
    /// </summary>
    public void GatherFood()
    {
        // Check to make sure food can still be gained, if it can't then don't add any food
        if (simScript.totalFood > 0f)
        {
            float foodGained = simScript.solarFoodValue + Random.Range(-0.2f, 0.2f); // Small amount of randomness, sometimes the agents find and eat more/less food

            if (energySource == "solar")
            {
                // Heal pollution and lifespan
                if (simScript.pollution > 0f)
                {
                    simScript.pollution -= simScript.fossilFuelPollutionPenalty/2;
                }
                if (simScript.averageLifespan < 200f)
                {
                    simScript.averageLifespan += simScript.fossilFuelLifePenalty;
                }
            }

            else if (energySource == "fossilFuels")
            {
                // Agent gets more food, but the environment and lifespan suffer
                foodGained += simScript.fossilFuelFoodBonus;
                if (simScript.pollution < 100f)
                {
                    simScript.pollution += simScript.fossilFuelPollutionPenalty;
                }

                if (simScript.averageLifespan > 0)
                {
                    simScript.averageLifespan -= simScript.fossilFuelLifePenalty;
                }
            }

            foodQuantity += foodGained;
            simScript.totalFood -= foodGained;
        }
    }

    /// <summary>
    /// This method subtracts food from the agents pool of food so that they may live.
    /// </summary>
    public void EatFood()
    {
        // Check if there's food to eat
        if (foodQuantity > 0f)
        {
            foodQuantity -= simScript.solarFoodValue - Random.Range(0f, simScript.solarFoodValue/2); // Eat a variable amount of food
        }
    }

    #endregion Methods
}
