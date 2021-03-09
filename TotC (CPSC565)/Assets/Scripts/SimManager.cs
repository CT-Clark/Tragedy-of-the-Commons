/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the world state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 * 
 * TODO: The bonuses, penalties, etc need to be callibrated
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    #region Fields/Properties

    // Private fields
    private int numberOfAgents = 100;
    
    

    // Public fields
    public float foodProduction = 1f;
    public float totalFood = 1000000f; // Total amount of food the world starts with
    public float pollution = 0f;
    public float solarFoodValue = 0.01f; // The amount of food an agent using solar collects
    public float fossilFuelFoodBonus = 0.01f; // The bonus using fossil fuels to collect food
    public float fossilFuelAverageLifePenalty = 0.01f; // The penalty using fossil fuels applies to global lifespan expectation
    public float fossilFuelPollutionPenalty = 0.01f; // How much pollution to add when fossil fuels used
    public float fossilFuelLifePenalty = 0.01f;
    public float averageLifespan = 100f;

    public List<AgentManager> agents = new List<AgentManager>();
    public AgentManager agent; // An object to hold an instance of an agent
    public GameObject AgentTemplate; 

    #endregion

    #region BuiltInMethods

    // Start is called before the first frame update
    void Start()
    {
        PopulateAgents();
    }

    // Update is called once per frame, used for graphics
    void Update()
    {
        
    }

    // FixedUpdate is used for changing the simulation state 
    void FixedUpdate()
    {
        
    }

    // LateUpdate is called after FixedUpdate and is used to modify things after agents have acted
    void LateUpdate()
    {
        // Change the agents based on the world state
        foreach (AgentManager element in agents)
        {
            // The more pollution there is, the more it affects an agent's health (lifespan)
            element.lifespan -= pollution * (0.1f);
        }

        // Add food to the world, the higher pollution the less food added
        totalFood += (foodProduction - pollution) * 100f;
    }

    #endregion BuiltInMethods

    #region Methods

    /// <summary>
    /// This method populates the world with agents.
    /// </summary>
    private void PopulateAgents()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            // Instantiate an agent
            CreateAgent(i);
        }
    }

    /// <summary>
    /// This method instantiates a singular agent at a random location and assigns it a number.
    /// </summary>
    /// <param name="agentNumber"></param>
    private void CreateAgent(int agentNumber)
    {
        GameObject agent = GameObject.Instantiate(AgentTemplate);
        AgentManager agentScript = agent.GetComponent<AgentManager>();
        agents.Add(agentScript);

        // Change the player's location
        float spawnX = Random.Range(-100f, 100f);
        float spawnY = Random.Range(-100f, 100f);
        agent.transform.position = new Vector2(spawnX, spawnY);
    }

    #endregion Methods
}
