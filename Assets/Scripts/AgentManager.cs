/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the agents' state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 */

using System;
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

    public SimManager simScript; // A reference to the world state
    public AgentManager agentScript; // A reference to another agent (used for collisions)
    public AgentManager parentScript; // A reference to this agent's parent to determine traits
    public GameObject AgentTemplate;

    #endregion

    #region BuiltInMethods

    // Awake is called when this agent is instantiated, used to assign traits
    void Awake()
    {
        simScript = GameObject.Find("SimManager").GetComponent<SimManager>();

        age = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (parentScript) // If this agent has a parent then inherit a lot of traits from them
        {
            foodQuantity = parentScript.foodToBreed;
            altruism = Math.Max(0, parentScript.altruism + UnityEngine.Random.Range(-10f, 10f));
            charisma = Math.Max(0, parentScript.charisma + UnityEngine.Random.Range(-10f, 10f));
            trust = Math.Max(0, parentScript.trust + UnityEngine.Random.Range(-10f, 10f));
            foresight = Math.Max(0, parentScript.foresight + UnityEngine.Random.Range(-10f, 10f));
            generation = parentScript.generation++;
            energySource = parentScript.energySource; // Kids are more likely to follow their parents' lead, so use the same energy source
            lifespan = Math.Max(0, ((parentScript.lifespan + UnityEngine.Random.Range(-10f, 10f)) + simScript.averageLifespan) / 2);
            foodToBreed = Math.Max(25, parentScript.foodToBreed + UnityEngine.Random.Range(-5f, 5f));
        }
        else // Agent is first generation
        {
            foodQuantity = 10f;
            altruism = UnityEngine.Random.Range(0f, 100f);
            charisma = UnityEngine.Random.Range(0f, 100f);
            trust = UnityEngine.Random.Range(0f, 100f);
            foresight = UnityEngine.Random.Range(0f, 100f);
            generation = 0;
            energySource = "fossilFuels";

            lifespan = simScript.averageLifespan + UnityEngine.Random.Range(-10f, 10f);
            foodToBreed = 80f + UnityEngine.Random.Range(-20f, 20f);
        }
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

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
        if (lifespan + amount > 0f)
        {
            lifespan += amount;
        }
    }

    /// <summary>
    /// This methods checks if an agent is capable of spawning a child.
    /// If it is, then it spawns a child.
    /// The agent must be at least 20 units old, have enough food, and is not concerned about the state of the world's pollution
    /// </summary>
    private void CheckSpawn()
    {
        if (foodQuantity >= foodToBreed + 10f && age > 20f && (simScript.pollution / simScript.foodProduction) * 100 > 100 - foresight)
        {
            foodQuantity -= foodToBreed + UnityEngine.Random.Range(-9f, 9f); // Breeding uses food
            GameObject agent = GameObject.Instantiate(AgentTemplate);
            AgentManager agentScript = agent.GetComponent<AgentManager>();
            agentScript.parentScript = GetComponent<AgentManager>();
            agent.name = "Agent" + simScript.agentCount;
            simScript.agents.Add(agentScript);

            // Change the player's location
            float spawnX = transform.position.x + UnityEngine.Random.Range(-10f, 10f);
            float spawnY = transform.position.y + UnityEngine.Random.Range(-10f, 10f);
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
            Debug.Log(gameObject.name + " has died from lack of food.");
            simScript.agents.Remove(GetComponent<AgentManager>());
            Destroy(gameObject);
        }
        if (age >= lifespan)
        {
            Debug.Log(gameObject.name + " has died from old age.");
            simScript.agents.Remove(GetComponent<AgentManager>());
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This agent tries to predict if a calamity will happen (Average lifespan gets too low, or not enough food being produced, both as consequences of pollution)
    /// </summary>
    public void CheckCalamity()
    {
        // TODO: Factor in altruism

        // If pollution is too high and the agent is altruistic enough, change to solar
        if ((simScript.pollution / simScript.foodProduction) * 100 > 100 - foresight && 100 - (simScript.pollution / simScript.foodProduction) * 100 < altruism && energySource != "solar") 
        {
            
            if (energySource != "solar")
            {
                Debug.Log(gameObject.name + " has changed their energy source due to high levels of pollution affecting food growth rates.");
                Debug.Log(gameObject.name + "'s foresight: " + foresight + ", altruism: " + altruism + " | Pollution percentage: " + ((simScript.pollution / simScript.foodProduction) * 100) + "%");
            }
            energySource = "solar";
        }
        // If average lifespan is too low and the agent is altruistic enough, change to solar
        else if (simScript.averageLifespan < foresight && simScript.averageLifespan < altruism)
        {
            if (energySource != "solar")
            {
                Debug.Log(gameObject.name + " has changed energy sources due to low average lifespan.");
            }
            energySource = "solar"; 
        }
        // Otherwise use fossil fuels
        else
        {
            energySource = "fossilFuels";
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
            float foodGained = simScript.solarFoodValue + UnityEngine.Random.Range(-simScript.solarFoodValue/2, simScript.solarFoodValue/2); // Small amount of UnityEngine.Randomness, sometimes the agents find and eat more/less food

            if (energySource == "solar")
            {
                // Heal pollution and lifespan
                if (simScript.pollution > 0f)
                {
                    simScript.pollution -= simScript.fossilFuelPollutionPenalty/2;
                }
                if (simScript.averageLifespan < 200f)
                {
                    simScript.averageLifespan += simScript.fossilFuelAverageLifePenalty/2;
                }
            }

            else if (energySource == "fossilFuels")
            {
                // Agent gets more food, but the environment and lifespan suffer
                foodGained += simScript.fossilFuelFoodBonus;
                if (simScript.pollution + simScript.fossilFuelPollutionPenalty <= simScript.foodProduction)
                {
                    simScript.pollution += simScript.fossilFuelPollutionPenalty;
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
            foodQuantity -= simScript.solarFoodValue - UnityEngine.Random.Range(0f, simScript.solarFoodValue / 2); // Eat a variable amount of food
        }
    }

    #endregion Methods
}
