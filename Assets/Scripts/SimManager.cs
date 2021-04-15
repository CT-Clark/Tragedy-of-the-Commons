/* 
 * Authors: Taylor Skaalrud, Serena Schimert, Cody Clark
 * This script manages the world state for the Tragedy of the Commons (TofC) simulation written for CPSC565.
 * 
 * TODO: The bonuses, penalties, etc need to be callibrated
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float totalFood = 100f; // Total amount of food the world starts with
    public float pollution;
    public float solarFoodValue; // The amount of food an agent using solar collects
    public float fossilFuelFoodBonus; // The bonus using fossil fuels to collect food
    public float fossilFuelPollutionPenalty; // How much pollution to add when fossil fuels used
    public float averageLifespan;
    public float pollutionPercentage;
    public float agingRate;
    public float altruismRange;
    public float charismaRange;
    public float trustRange;
    public float foresightRange;
    public float lifespanRange;
    public float foodToBreedRange;
    public Color fossilFuelsColor; //color used for fossil fuel users
    public Color renewablesColor; //color used for renewables users

    public List<AgentManager> agents = new List<AgentManager>();
    public AgentManager agent; // An object to hold an instance of an agent
    public GameObject AgentTemplate;
    public Text messageText;

    //Colors showing pollution relative to food production
    public Color productiveCol = new Color(0.4f, 0.6f, 0.8f, 1);
    public Color pollutionCol = Color.black;

    #endregion

    #region UI

    // Food Production Rate
    public Text FoodProductionRateTextUI;
    public Slider FoodProductionRateSliderUI;

    // Solar Food Value
    public Text SolarFoodValueTextUI;
    public Slider SolarFoodValueSliderUI;

    // Fossil Fuel Food Bonus
    public Text FossilFuelFoodBonusTextUI;
    public Slider FossilFuelFoodBonusSliderUI;

    // Fssil Fuel Pollution Penalty
    public Text FossilFuelPollutionPenaltyTextUI;
    public Slider FossilFuelPollutionPenaltySliderUI;

    // Aging Rate
    public Text AgingRateTextUI;
    public Slider AgingRateSliderUI;

    // Altruism Range
    public Text AltruismRangeTextUI;
    public Slider AltruismRangeSliderUI;

    // Charisma Range
    public Text CharismaRangeTextUI;
    public Slider CharismaRangeSliderUI;

    // Trust Range
    public Text TrustRangeTextUI;
    public Slider TrustRangeSliderUI;

    // Foresight Range
    public Text ForesightRangeTextUI;
    public Slider ForesightRangeSliderUI;

    // Lifespan Range
    public Text LifespanRangeTextUI;
    public Slider LifespanRangeSliderUI;

    // Food To breed Range
    public Text FoodToBreedRangeTextUI;
    public Slider FoodToBreedRangeSliderUI;

    /// <summary>
    /// Display the current settings.
    /// </summary>
    private void DisplaySettings()
    {
        FoodProductionRateTextUI.text = string.Format("Food Production Rate ({0:0.00})", foodProduction);
        SolarFoodValueTextUI.text = string.Format("Solar Food Value ({0:0.00})", solarFoodValue);
        FossilFuelFoodBonusTextUI.text = string.Format("Fossil Fuel Food Bonus ({0:0.00})", fossilFuelFoodBonus);
        FossilFuelPollutionPenaltyTextUI.text = string.Format("Fossil Fuel Pollution Penalty ({0:0.00})", fossilFuelPollutionPenalty);
        AgingRateTextUI.text = string.Format("Aging Rate ({0:0.00})", agingRate);

        AltruismRangeTextUI.text = string.Format("Altruism Range ({0:0.00})", altruismRange);
        CharismaRangeTextUI.text = string.Format("Charisma Range ({0:0.00})", charismaRange);
        TrustRangeTextUI.text = string.Format("Trust Range ({0:0.00})", trustRange);
        ForesightRangeTextUI.text = string.Format("Foresight Range ({0:0.00})", foresightRange);
        LifespanRangeTextUI.text = string.Format("Lifespan Range ({0:0.00})", lifespanRange);
        FoodToBreedRangeTextUI.text = string.Format("Food To Breed Range ({0:0.00})", foodToBreedRange);
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
        agingRate = AgingRateSliderUI.value;
        altruismRange = AltruismRangeSliderUI.value;
        charismaRange = CharismaRangeSliderUI.value;
        trustRange = TrustRangeSliderUI.value;
        foresightRange = ForesightRangeSliderUI.value;
        lifespanRange = LifespanRangeSliderUI.value;
        foodToBreedRange = FoodToBreedRangeSliderUI.value;
    }

    /// <summary>
    /// Initializes UI properties.
    /// </summary>
    private void InitializeUI()
    {
        FoodProductionRateSliderUI.maxValue = 3f;
        FoodProductionRateSliderUI.value = 1.5f;
        SolarFoodValueSliderUI.maxValue = 1f;
        SolarFoodValueSliderUI.value = 0.5f;
        FossilFuelFoodBonusSliderUI.maxValue = 0.5f;
        FossilFuelFoodBonusSliderUI.value = 0.05f;
        FossilFuelPollutionPenaltySliderUI.maxValue = 0.01f;
        FossilFuelPollutionPenaltySliderUI.minValue = 0.001f;
        FossilFuelPollutionPenaltySliderUI.value = 0.005f;
        AgingRateSliderUI.maxValue = 1f;
        AgingRateSliderUI.minValue = 0.01f;
        AgingRateSliderUI.value = 0.1f;
        AltruismRangeSliderUI.maxValue = 50f;
        AltruismRangeSliderUI.value = 10f;
        CharismaRangeSliderUI.maxValue = 50f;
        CharismaRangeSliderUI.value = 10f;
        TrustRangeSliderUI.maxValue = 50f;
        TrustRangeSliderUI.value = 10f;
        ForesightRangeSliderUI.maxValue = 50f;
        ForesightRangeSliderUI.value = 10f;
        LifespanRangeSliderUI.maxValue = 50f;
        LifespanRangeSliderUI.value = 10f;
        FoodToBreedRangeSliderUI.maxValue = 25f;
        FoodToBreedRangeSliderUI.value = 5f;
    }

    #endregion UI

    #region BuiltInMethods

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        InitializeUI();
        UpdateSettings();
        pollution = FoodProductionRateSliderUI.value / 100; // Start with a small amount of pollution
        PopulateAgents();
        
    }

    // Update is called once per frame, used for graphics
    void Update()
    {
        UpdateSettings();
        DisplaySettings();
        messageText.text = String.Format("{0} | {1} | {2} | {3} | {4}", 
            String.Format("Agents: {0}", agents.Count), 
            String.Format("Total Food: {0}", totalFood.ToString("0")), 
            String.Format("Food Production: {0}", (agents.Count * FoodProductionRateSliderUI.value).ToString("0.0")), 
            String.Format("Pollution: {0} - {1}%", pollution.ToString("0.0"), pollutionPercentage.ToString("0.0")),
            String.Format("Average Lifespan: {0}", averageLifespan.ToString("0.0")));
    }

    // FixedUpdate is used for changing the simulation state 
    void FixedUpdate()
    {
        pollutionPercentage = pollution / foodProduction * 100;
        pollutionPercentage = Mathf.Clamp(pollutionPercentage, 0f, 100f);
    }

    // LateUpdate is called after FixedUpdate and is used to modify things after agents have acted
    void LateUpdate()
    {
        foreach (AgentManager agent in agents)
        {
            if (agent)
            {
                if (!agent.enabled)
                {
                    agent.enabled = true;
                }
            }
        }

        // Change the agents based on the world state
        tempAverageLifespan = 0;
        foreach (AgentManager element in agents)
        {
            // The more pollution there is, the more it affects an agent's health (lifespan)
            element.ChangeLifespan(-pollutionPercentage / 10000); // An agent's lifespan decreases as the world is further polluted
            tempAverageLifespan += element.lifespan;

            // Keep position within boundaries
            Vector2 viewPos = element.transform.position;
            viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1, screenBounds.x);
            viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1, screenBounds.y);
            element.transform.position = viewPos;
        }

        // Used for new agent creation
        averageLifespan = tempAverageLifespan / agents.Count;

        // Add food to the world, the higher pollution the less food added
        foodProduction = agents.Count * FoodProductionRateSliderUI.value; // Food production is dependent on the number of agents so that it grows with population
        totalFood += Math.Max(0, (foodProduction - pollution));

        // Update background to reflect severity of pollution
        Camera.main.backgroundColor = Color.Lerp(productiveCol, pollutionCol, pollution / foodProduction);
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

        // Set location
        float spawnX = UnityEngine.Random.Range(screenBounds.x * -1, screenBounds.x);
        float spawnY = UnityEngine.Random.Range(screenBounds.y * -1, screenBounds.y);
        agent.transform.position = new Vector2(spawnX, spawnY);

        agentCount++;
    }

    #endregion Methods
}
