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
    public int agentCount = 0;

    // Public fields
    public Vector2 screenBounds;

    public float foodProduction;
    public float totalFood = 1000000f; // Total amount of food the world starts with
    public float pollution;
    public float solarFoodValue; // The amount of food an agent using solar collects
    public float fossilFuelFoodBonus; // The bonus using fossil fuels to collect food
    public float fossilFuelAverageLifePenalty; // The penalty using fossil fuels applies to global lifespan expectation
    public float fossilFuelPollutionPenalty; // How much pollution to add when fossil fuels used
    public float fossilFuelLifePenalty;
    public float averageLifespan;
    public float tempAverageLifespan;

    public List<AgentManager> agents = new List<AgentManager>();
    public AgentManager agent; // An object to hold an instance of an agent
    public GameObject AgentTemplate;

    #endregion

    #region UI

    public Text FoodProductionRateTextUI;
    public Slider FoodProductionRateSliderUI;
    public GameObject FoodProductionRateInputFieldUI;

    public Text SolarFoodValueTextUI;
    public Slider SolarFoodValueSliderUI;
    public GameObject SolarFoodValueInputFieldUI;

    public Text FossilFuelFoodBonusTextUI;
    public Slider FossilFuelFoodBonusSliderUI;
    public GameObject FossilFuelFoodBonusInputFieldUI;

    public Text FossilFuelAverageLifePenaltyTextUI;
    public Slider FossilFuelAverageLifePenaltySliderUI;
    public GameObject FossilFuelAverageLifePenaltyInputFieldUI;

    public Text FossilFuelPollutionPenaltyTextUI;
    public Slider FossilFuelPollutionPenaltySliderUI;
    public GameObject FossilFuelPollutionPenaltyInputFieldUI;

    public Text FossilFuelLifePenaltyTextUI;
    public Slider FossilFuelLifePenaltySliderUI;
    public GameObject FossilFuelLifePenaltyInputFieldUI;

    public Text AverageLifespanTextUI;
    public Slider AverageLifespanSliderUI;
    public GameObject AverageLifespanInputFieldUI;

    /// <summary>
    /// Display the current settings.
    /// </summary>
    private void DisplaySettings()
    {
        FoodProductionRateTextUI.text = string.Format("Food Production Rate ({0:0.00})", foodProduction);
        SolarFoodValueTextUI.text = string.Format("Solar Food Value ({0:0.00})", solarFoodValue);
        FossilFuelFoodBonusTextUI.text = string.Format("Fossil Fuel Food Bonus ({0:0.00})", fossilFuelFoodBonus);
        FossilFuelAverageLifePenaltyTextUI.text = string.Format("Fossil Fuel Average Life Penalty ({0:0.00})", fossilFuelAverageLifePenalty);
        FossilFuelPollutionPenaltyTextUI.text = string.Format("Fossil Fuel Pollution Penalty ({0:0.00})", fossilFuelPollutionPenalty);
        FossilFuelLifePenaltyTextUI.text = string.Format("Fossil Fuel Life penalty ({0:0.00})", fossilFuelLifePenalty);
        AverageLifespanTextUI.text = string.Format("Average Lifespan ({0:0.00})", averageLifespan);
    }

    /// <summary>
    /// Updates the settings.
    /// </summary>
    private void UpdateSettings()
    {
        //foodProduction = FoodProductionRateSliderUI.value;
        solarFoodValue = SolarFoodValueSliderUI.value;
        fossilFuelFoodBonus = FossilFuelFoodBonusSliderUI.value;
        fossilFuelAverageLifePenalty = FossilFuelAverageLifePenaltySliderUI.value;
        fossilFuelPollutionPenalty = FossilFuelPollutionPenaltySliderUI.value;
        fossilFuelLifePenalty = FossilFuelLifePenaltySliderUI.value;
    }

    /// <summary>
    /// Initializes UI properties.
    /// </summary>
    private void InitializeUI()
    {
        //FoodProductionRateSliderUI.maxValue = 1000f;
        //FoodProductionRateSliderUI.value = 1000f;
        SolarFoodValueSliderUI.maxValue = 1f;
        SolarFoodValueSliderUI.value = 0.5f;
        FossilFuelFoodBonusSliderUI.maxValue = 1f;
        FossilFuelFoodBonusSliderUI.value = 0.05f;
        FossilFuelAverageLifePenaltySliderUI.maxValue = 0.50f;
        FossilFuelAverageLifePenaltySliderUI.value = 0.001f;
        FossilFuelPollutionPenaltySliderUI.maxValue = 0.5f;
        FossilFuelPollutionPenaltySliderUI.value = 0.005f;
        FossilFuelLifePenaltySliderUI.maxValue = 0.5f;
        FossilFuelLifePenaltySliderUI.value = 0.001f;
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
        pollution = FoodProductionRateSliderUI.value / 100;
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
        
    }

    // LateUpdate is called after FixedUpdate and is used to modify things after agents have acted
    void LateUpdate()
    {
        tempAverageLifespan = 0;
        // Change the agents based on the world state
        foreach (AgentManager element in agents)
        {
            // The more pollution there is, the more it affects an agent's health (lifespan)
            element.ChangeLifespan(-(pollution/foodProduction / 100)); // An agent's lifespan decreases as the world is further polluted
            tempAverageLifespan += element.lifespan;

            //Keep position within boundaries
            Vector2 viewPos = element.transform.position;
            viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1, screenBounds.x);
            viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1, screenBounds.y);
            element.transform.position = viewPos;
        }
        averageLifespan = tempAverageLifespan / agents.Count;

        // Add food to the world, the higher pollution the less food added
        foodProduction = agents.Count * 2f;
        totalFood += Math.Max(0, (foodProduction - pollution));

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
