/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the agents' state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    #region Fields/Properties

    // Private fields
    public float foodQuantity; // This agents collection of food
    public float age; // The agent's age
    private Rigidbody2D rigidBody;
    private float travelTime;
    private bool moveStart = false;

    // Public fields
    public float altruism; // Scale from 0-100, how likely they are to change to solar
    public float altruismBonus; // The amount to change an agent's altruism after a collision
    public float charisma; // Scale from 0-100, their ability to influence other agents
    public float trust; // Scale from 0-100, their ability to be manipulated by other agents
    public float foresight; // Scale from 0-100, their ability to predict a calamity
    public float foodToBreed; // The amount of food needed to breed
    public string energySource; // Either "solar" or "fossilFuels"
    public int generation;
    public float lifespan; // When this agent will die due to time
    public Slider slider;
    public Image fillImage;
    public Color noFoodUI;
    public Color foodUI;

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
        altruismBonus = 5f;
        travelTime = 0.5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Handles graphics per frame. 
    /// </summary>
    void Update()
    {
        SetFoodUI();
    }

    // FixedUpdate is used for changing the simulation state.
    void FixedUpdate()
    {
        EatFood();
        CheckDeath();
        CheckSpawn();
        CheckCalamity();
        GatherFood();
        DrainAltruism();
        Move();
        age += simScript.AgingRateSliderUI.value;
    }

    // LateUpdate is called after FixedUpdate, used to handle collisions.
    void LateUpdate()
    {
        ResolveCollision();
    }

    /// <summary>
    /// Resolves collisions.
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<AgentManager>() == null) return;
        agentScript = col.gameObject.GetComponent<AgentManager>();
    }

    #endregion BuiltInMethods

    #region Methods

    /// <summary>
    /// This moves the agent.
    /// https://answers.unity.com/questions/664712/rotate-2d-object-to-facing-direction.html
    /// The above was referenced in creating a random wiggle
    /// </summary>
    private void Move()
    {
        if (!moveStart) // Give a little boost when starting
        {
            Vector2 firstMove = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
            rigidBody.AddForce(firstMove * 200, ForceMode2D.Force);
            moveStart = true;
        }
        else if (travelTime == 0.5f) // Give a boost when choosing a new direction
        {
            rigidBody.AddForce(transform.right * 200, ForceMode2D.Force); 
        }

        travelTime -= Time.deltaTime * UnityEngine.Random.Range(0, 2);

        if (travelTime <= 0)
        {
            // Decide which direction to move
            Vector2 dir = -rigidBody.velocity;
            if (dir.x == 0) dir.x = rigidBody.velocity.x + UnityEngine.Random.Range(-10, 10);
            if (dir.y == 0) dir.y = rigidBody.velocity.y + UnityEngine.Random.Range(-10, 10);

            // Move
            transform.Translate(Vector3.right * dir.x * Time.deltaTime, Space.World);
            transform.Translate(Vector3.up * dir.y * Time.deltaTime, Space.World);

            // Rotate to face moving direction
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle * UnityEngine.Random.Range(1, 2), Vector3.forward);

            travelTime = 0.5f; // Resets travel time
        }
    }
    

/// <summary>
/// This can be called externally in order to change this agent's lifespan.
/// </summary>
/// <param name="amount"></param>
public void ChangeLifespan(float amount)
    {
        // Don't go into negative lifespan
        lifespan += amount;
        lifespan = Math.Max(0, lifespan);
    }

    /// <summary>
    /// This methods checks if an agent is capable of spawning a child.
    /// If it is, then it spawns a child.
    /// The agent must be at least 20 units old, have enough food, and is not concerned about the state of the world's pollution or total food amount
    /// </summary>
    private void CheckSpawn()
    {
        if (foodQuantity >= foodToBreed + 10f 
            && age > 20f 
            //&& simScript.pollutionPercentage < 100 - foresight 
            && simScript.totalFood > simScript.agents.Count * 1500)
        {
            foodQuantity -= foodToBreed; // Breeding uses food, that food is given to the spawned child
            GameObject agent = GameObject.Instantiate(AgentTemplate);
            AgentManager agentScript = agent.GetComponent<AgentManager>();
            agentScript.parentScript = GetComponent<AgentManager>(); // Used to pass on traits
            agent.name = "Agent" + simScript.agentCount;
            simScript.agents.Add(agentScript); // Add to the world agent list

            // Change the new agent's location
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
            //Debug.Log(gameObject.name + " has died from lack of food.");
            simScript.agents.Remove(GetComponent<AgentManager>());
            Destroy(gameObject);
        }
        if (age >= lifespan)
        {
            //Debug.Log(gameObject.name + " has died from old age.");
            simScript.agents.Remove(GetComponent<AgentManager>());
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This agent tries to predict if a calamity will happen (Average lifespan gets too low, or not enough food being produced, both as consequences of pollution)
    /// </summary>
    public void CheckCalamity()
    {
        // If pollution is too high and the agent is altruistic enough, change to (or continue to use) solar
        if (simScript.pollutionPercentage > 100 - foresight && 100 - simScript.pollutionPercentage < altruism) 
        {
            // If energy source was previously fossil fuels print this to log
            if (energySource != "solar")
            {
                //Debug.Log(gameObject.name + " has changed their energy source due to high levels of pollution affecting food growth rates.");
            }
            energySource = "solar";
            AgentTemplate.GetComponent<SpriteRenderer>().color = simScript.renewablesColor;
        }
        // If average lifespan is too low and the agent is altruistic enough, change to (or continue to use) solar
        else if (simScript.averageLifespan < foresight && simScript.averageLifespan < altruism)
        {
            if (energySource != "solar")
            {
                //Debug.Log(gameObject.name + " has changed energy sources due to low average lifespan.");
            }
            energySource = "solar";
            AgentTemplate.GetComponent<SpriteRenderer>().color = simScript.renewablesColor;
        }
        // Otherwise use fossil fuels (The agent believes everything is okay in the world and would prefer to collect more food)
        else
        {
            if (energySource == "solar")
            {
                //Debug.Log(gameObject.name + " has changed from solar to fossil fuels.");
            }
            energySource = "fossilFuels";
            AgentTemplate.GetComponent<SpriteRenderer>().color = simScript.fossilFuelsColor;
        }
    }

    /// <summary>
    /// When two agents collide have them try to convince each other to change their altruism trait.
    /// If their charisma score is higher than your trust score, you've been convinced to become more altruistic
    /// </summary>
    public void ResolveCollision()
    {
        if (agentScript != null)
        {
            if (agentScript.charisma > trust && altruism < 100f)
            {
                altruism += altruismBonus;
                altruism = Mathf.Clamp(altruism, 0, 100);
            }
        }

        agentScript = null;
    }

    /// <summary>
    /// This method increases the agents food supply, decreases the global food supply, and possibly applies global penalties.
    /// </summary>
    public void GatherFood()
    {
        // Check to make sure food can still be gained, if it can't then don't add any food
        if (simScript.totalFood > simScript.solarFoodValue + simScript.fossilFuelFoodBonus)
        {
            float foodGained = simScript.solarFoodValue; // Small amount of randomness, sometimes the agents find and eat more/less food

            if (energySource == "solar")
            {
                // Heal pollution
                if (simScript.pollution > 0f)
                {
                    simScript.pollution -= simScript.fossilFuelPollutionPenalty;
                    simScript.pollution = Math.Max(0, simScript.pollution);
                }
            }

            else if (energySource == "fossilFuels")
            {
                // Agent gets more food, but the environment and lifespan suffer
                foodGained += simScript.fossilFuelFoodBonus;
                simScript.pollution += simScript.fossilFuelPollutionPenalty;
            }

            foodQuantity += foodGained;
            simScript.totalFood -= foodGained;
            simScript.totalFood = Math.Max(0, simScript.totalFood);
        }
        else // If no food was gained then fossil fuels weren't used - Lower pollution
        {
            simScript.pollution -= simScript.fossilFuelPollutionPenalty;
            simScript.pollution = Math.Max(0, simScript.pollution);
        }
    }

    /// <summary>
    /// This method subtracts food from the agents pool of food so that they may live.
    /// Eat a variable amount of food, the maximum amount of food needed to eat is equal to the average amount produced by solar
    /// </summary>
    public void EatFood()
    {
        foodQuantity -= 0.25f;
    }

    /// <summary>
    /// This removes an amount of altruism from the agent each time step dependent on the amount of food this agent has.
    /// The more food, the less altruism drained. This is to account for the perpetual increase from collisions.
    /// </summary>
    public void DrainAltruism()
    {
        altruism -= (100 - Mathf.Clamp(foodQuantity, 0f, 100f)) * simScript.agingRate * 0.01f;
        altruism = Mathf.Clamp(altruism, 0f, 100f);
    }

    /// <summary>
    /// This method sets the colour of the circle around agents depending on how much food they've acquired
    /// </summary>
    public void SetFoodUI()
    {
        slider.value = foodQuantity;
        fillImage.color = Color.Lerp(noFoodUI, foodUI, Math.Min(1f, foodQuantity / foodToBreed));
    }

    private void Initialize()
    {
        if (parentScript) // If this agent has a parent then inherit a lot of traits from them
        {
            foodQuantity = parentScript.foodToBreed;
            altruism = Mathf.Clamp(parentScript.altruism +
                UnityEngine.Random.Range(-simScript.altruismRange, simScript.altruismRange), 0, 100);
            charisma = Mathf.Clamp(parentScript.charisma +
                UnityEngine.Random.Range(-simScript.charismaRange, simScript.charismaRange), 0, 100);
            trust = Mathf.Clamp(parentScript.trust +
                UnityEngine.Random.Range(-simScript.trustRange, simScript.trustRange), 0, 100);
            foresight = Mathf.Clamp(parentScript.foresight +
                UnityEngine.Random.Range(-simScript.foresightRange, simScript.foresightRange), 0, 100);
            generation = parentScript.generation++;
            energySource = parentScript.energySource; // Kids are more likely to follow their parents' lead, so use the same energy source
            lifespan = Math.Max(0, ((parentScript.lifespan +
                UnityEngine.Random.Range(0f, simScript.lifespanRange)) + simScript.averageLifespan) / 2);
            foodToBreed = Math.Max(25, parentScript.foodToBreed +
                UnityEngine.Random.Range(-simScript.foodToBreedRange, simScript.foodToBreedRange));
            AgentTemplate.GetComponent<SpriteRenderer>().color = parentScript.GetComponent<SpriteRenderer>().color;
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
            AgentTemplate.GetComponent<SpriteRenderer>().color = simScript.fossilFuelsColor;
            lifespan = simScript.averageLifespan +
                UnityEngine.Random.Range(-simScript.lifespanRange, simScript.lifespanRange);
            foodToBreed = 100f + UnityEngine.Random.Range(-simScript.foodToBreedRange, simScript.foodToBreedRange);
        }

        rigidBody = GetComponent<Rigidbody2D>();
    }

    #endregion Methods
}
