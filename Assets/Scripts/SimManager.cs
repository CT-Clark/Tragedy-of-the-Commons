/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the world state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 * 
 * TODO: The bonuses, penalties, etc need to be callibrated
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SimManager : MonoBehaviour
{
    #region Fields/Properties

    // Private fields
    private int numberOfAgents = 100;

    private float tempAverageLifespan;

    // Public fields
    public int agentCount = 0;
    
    public Vector2 screenBounds;

    public float foodProduction;
    public float totalFood = 1000000f; // Total amount of food the world starts with
    public float pollution;
    public float solarFoodValue; // The amount of food an agent using solar collects
    public float fossilFuelFoodBonus; // The bonus using fossil fuels to collect food
    public float fossilFuelPollutionPenalty; // How much pollution to add when fossil fuels used
    public float averageLifespan;
    public float pollutionPercentage;
    public Color fossilFuelsColor; //color used for fossil fuel users
    public Color renewablesColor; //color used for renewables users

    public List<AgentManager> agents = new List<AgentManager>();
    public AgentManager agent; // An object to hold an instance of an agent
    public GameObject AgentTemplate;

    #endregion

    #region UI

    public Text FoodProductionRateTextUI;
    public Slider FoodProductionRateSliderUI;

    public Text SolarFoodValueTextUI;
    public Slider SolarFoodValueSliderUI;

    public Text FossilFuelFoodBonusTextUI;
    public Slider FossilFuelFoodBonusSliderUI;

    public Text FossilFuelPollutionPenaltyTextUI;
    public Slider FossilFuelPollutionPenaltySliderUI;

    /// <summary>
    /// Display the current settings.
    /// </summary>
    private void DisplaySettings()
    {
        FoodProductionRateTextUI.text = string.Format("Food Production Rate ({0:0.00})", foodProduction);
        SolarFoodValueTextUI.text = string.Format("Solar Food Value ({0:0.00})", solarFoodValue);
        FossilFuelFoodBonusTextUI.text = string.Format("Fossil Fuel Food Bonus ({0:0.00})", fossilFuelFoodBonus);
        FossilFuelPollutionPenaltyTextUI.text = string.Format("Fossil Fuel Pollution Penalty ({0:0.00})", fossilFuelPollutionPenalty);
    }

    /// <summary>
    /// Updates the settings.
    /// </summary>
    private void UpdateSettings()
    {
        foodProduction = FoodProductionRateSliderUI.value;
        solarFoodValue = SolarFoodValueSliderUI.value;
        fossilFuelFoodBonus = FossilFuelFoodBonusSliderUI.value;
        fossilFuelPollutionPenalty = FossilFuelPollutionPenaltySliderUI.value;
    }

    /// <summary>
    /// Initializes UI properties.
    /// </summary>
    private void InitializeUI()
    {
        FoodProductionRateSliderUI.maxValue = 2f;
        FoodProductionRateSliderUI.value = 1.5f;
        SolarFoodValueSliderUI.maxValue = 1f;
        SolarFoodValueSliderUI.value = 0.5f;
        FossilFuelFoodBonusSliderUI.maxValue = 0.5f;
        FossilFuelFoodBonusSliderUI.value = 0.05f;
        FossilFuelPollutionPenaltySliderUI.maxValue = 0.01f;
        FossilFuelPollutionPenaltySliderUI.value = 0.005f;
    }

    #endregion UI

    #region BuiltInMethods

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        PopulateAgents();
        InitializeUI();
        UpdateSettings();
        pollution = FoodProductionRateSliderUI.value / 100; // Start with a small amount of pollution
    }

    // Update is called once per frame, used for graphics
    void Update()
    {
        UpdateSettings();
        DisplaySettings();
    }

    // FixedUpdate is used for changing the simulation state 
    void FixedUpdate()
    {
        pollutionPercentage = pollution / foodProduction * 100;
    }

    // LateUpdate is called after FixedUpdate and is used to modify things after agents have acted
    void LateUpdate()
    {
        tempAverageLifespan = 0;

        foodProduction = agents.Count * FoodProductionRateSliderUI.value; // Food production is dependent on the number of agents so that it grows with population

        // Change the agents based on the world state
        foreach (AgentManager element in agents)
        {
            // The more pollution there is, the more it affects an agent's health (lifespan)
            element.ChangeLifespan(-pollutionPercentage / 10000); // An agent's lifespan decreases as the world is further polluted
            tempAverageLifespan += element.lifespan;

            //Keep position within boundaries
            Vector2 viewPos = element.transform.position;
            viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1, screenBounds.x);
            viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1, screenBounds.y);
            element.transform.position = viewPos;
        }

        // Used for new agent creation
        averageLifespan = tempAverageLifespan / agents.Count;

        // Add food to the world, the higher pollution the less food added

        foodProduction = agents.Count * 2f;
        totalFood += Math.Max(0, (foodProduction - pollution));

        Color start = new Color(0.4f, 0.6f, 0.8f, 1);

        // Update background to reflect severity of pollution
        Camera.main.backgroundColor = Color.Lerp(start, Color.black, pollution / foodProduction);

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
        agent.name = "Agent" + agentNumber;
        agents.Add(agentScript);

        /*
        // Change the player's location
        float spawnX = UnityEngine.Random.Range(-100f, 100f);
        float spawnY = UnityEngine.Random.Range(-100f, 100f);
        agent.transform.position = new Vector2(spawnX, spawnY);
        */

        // set location
        float spawnX = UnityEngine.Random.Range(screenBounds.x * -1, screenBounds.x);
        float spawnY = UnityEngine.Random.Range(screenBounds.y * -1, screenBounds.y);
        agent.transform.position = new Vector2(spawnX, spawnY);

        agentCount++;

    }

    #endregion Methods

    
}
