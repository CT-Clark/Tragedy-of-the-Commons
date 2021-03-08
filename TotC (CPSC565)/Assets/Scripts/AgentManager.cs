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
    
    private float foodQuantity; // This agents collection of food
    private float age; // The agent's age

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

    #endregion

    #region BuiltInMethods

    // Awake is called when this agent is instantiated, used to assign traits
    void Awake()
    {
        // TODO: Get SimManager simScript
        
        age = 0;
        foodQuantity = 50;

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        if (foodQuantity >= foodToBreed + 10f)
        {
            foodQuantity -= foodToBreed; // Breeding uses food
            // Spawn a new agent (Or maybe a few?)

        }
    }

    /// <summary>
    /// This method checks whether the agent has starved or is too old.
    /// If either is true, the agent dies.
    /// </summary>
    private void CheckDeath()
    {
        // Check if the agent starves or dies from old age
        if (foodQuantity <= 0 || age >= lifespan)
        {
            this.Destroy();
        }
    }

    /// <summary>
    /// This agent tries to predict if a calamity will happen (Average lifespan gets too low, or not enough food being produced, both as consequences of pollution)
    /// </summary>
    public void CheckCalamity()
    {
        // TODO: Factor in altruism
        if ((simScript.foodProduction - simScript.pollution) < foresight || simScript.averageLifespan < foresight)
        {
            energySource = "solar";
        }
    }

    /// <summary>
    /// When two agents collide have them try to convince each other to change their energy sources .
    /// </summary>
    public void ResolveCollision()
    {
        // TODO: Factor in altruism
        // If their charisma score is higher than your trust score, follow their lead
        if (agentScript.charisma > trust)
        {
            energySource = agentScript.energySource;
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
            float foodGained = simScript.solarFoodValue + Random.Range(-0.05f, 0.05f); // Small amount of randomness, sometimes the agents find and eat more/less food

            if (energySource == "solar")
            {
                // Heal pollution and lifespan
                simScript.pollution -= simScript.fossilFuelPollutionPenalty;
                simScript.averageLifespan += simScript.fossilFuelLifePenalty;
            }

            else if (energySource == "fossilFuels")
            {
                // Agent gets more food, but the environment and lifespan suffer
                foodGained += simScript.fossilFuelFoodBonus;
                simScript.pollution += simScript.fossilFuelPollutionPenalty;
                simScript.averageLifespan -= simScript.fossilFuelLifePenalty;
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
            foodQuantity -= Random.Range(0.05f, 0.2f); // Eat a variable amount of food
        }
    }

    #endregion Methods
}
