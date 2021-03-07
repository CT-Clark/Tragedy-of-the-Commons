/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the world state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    #region Fields/Properties

    // Private fields
    private int numberOfAgents = 100;
    private float averageLifespan = 100f;
    private float fossilFuelFoodPenalty = 0.01f; // The penalty using fossil fuels applies to global food production
    private float fossilFuelLifePenalty = 0.01f; // The penalty using fossil fuels applies to global lifespan expectation

    // Public fields
    public float foodProduction = 100f; // In %
    public float pollution = 0f; // In %
    public float solarFoodValue = 0.01f; // The amount of food an agent using solar collects
    public float fossilFuelFoodBonus = 0.01f; // The bonus using fossil fuels to collect food

    public AgentManager agent; // An object to hold an instance of an agent

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

        // Change the player's location
        float spawnX = Random.Range(-100f, 100f);
        float spawnY = Random.Range(-100f, 100f);
        agent.transform.position = new Vector2(spawnX, spawnY);

        // Assign the agent a number
        agentScript.agentNumber = agentNumber; 
    }

    #endregion Methods
}
