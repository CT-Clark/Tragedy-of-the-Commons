# The Tragedy of the Commons - A Simulation of Climate Change and Self-Governance

![World Example](https://user-images.githubusercontent.com/23039052/114322017-3d2ff300-9adb-11eb-96fa-7716ffbee6a4.png)

> (A heavily populated world)

## Background

## Goal

## Implementation

The simulation is separated into two sections - the world state and the agents. Each section is managed by its own script, with the world state and UI managed by the SimScript and each agent managed by its own copy of the AgentScript. 

## The World

The world state is composed of the amount of food produced each discrete time step, the amount of pollution present in the world, and the amount of food left in the world. The SimScript also contains all of the configurable elements of the simulation: the amount of food each agent collects when using solar energy, the bonus using fossil fuel energy confers when collecting food, the amount of pollution using fossil fuels produces, the rate at which agents age, and the range agent traits are assigned within when producing new agents. 

![Polluted](https://user-images.githubusercontent.com/23039052/114322047-65b7ed00-9adb-11eb-8ff6-84afdb1cbfeb.png)

> (An example of a heavily polluted world state)

The amount of food produced is determined as a function of the number of agents which inhabit the world. The more agents in the simulation, the more food produced each time step. This represents the idea that as more people populate a society the society must also contain an increasing number of food producers (farmers) in order to support its population. 

## The Agents

![Agents](https://user-images.githubusercontent.com/23039052/114321997-1ffb2480-9adb-11eb-9f0c-1cdda8a48ecc.png)

> (An example of two agents - the green agent uses solar energy while the purple agent uses fossil fuels)

The AgentScript used for the agents contains the trait values for the individual agent as well as the logic which decides how the agent interacts with the world. 

### Agent Traits

The traits the agents possess are as follows:
1) Altruism
2) Charisma
3) Trust
4) Foresight
5) Food quantity
6) Age
7) Food to breed
8) Energy source
9) Lifespan

Altruism is the measure of how willing an agent is to change their energy source to solar - thereby reducing the amount of food to collect and their ability to breed. Charisma is how convincing this agent is when meeting other agents. Trust is a threshold other agents must exceed in order to convince this agent to change their ways. Energy source simply records whether this agent is using solar energy or fossil fuels. Finally, lifespan is how long this agent will live. 

### Agent Actions

Each discrete time step the agents perform the following actions:
1) Eat food
2) Check if they die
3) Check if they breed
4) Check the world state
5) Gather food
6) Move
7) Age

#### Eat Food

When eating food the agent subtracts an amount of food from their food pool equal to a random value between the amount of food gathered through solar energy and half the amount of food gathered through solar energy. This randomness is designed to stagger the times at which agents breed - due to breeding being based on amount of food possessed - as well to represent the hardship associated with using solar energy compared to fossil fuels, i.e., those who use fossil fuels always have an excess amount of food, whereas the agents who depend on solar energy could possibly end up with no food left over. 

#### Check Death

Agents are born with a limited lifespan and each time step they age a fixed amount towards that lifespan. If their age exceeds their lifespan then they die and are removed from the simulation. Additionally, the amount of pollution in the world state has a negative impact on their individual lifespans, reducing their time to live based on how polluted the world state is in order to more accurately reflect the consequences of real world pollution. This step also checks whether or not the agent starved. This is caused by the eat food step consuming enough food to reduce the agent’s food pool to zero. Likewise when this happens the agent dies and is removed from the simulation.

#### Check Breeding

After the agent has eaten their food and has not died yet, they check whether they’re capable of breeding. In order to breed the agents must satisfy the following conditions:
They possess enough food
They’re 20+ time units old
The world is not too polluted
The world has enough food
The amount of food needed to breed is randomly determined at the time of creation for each agent. Once an agent surpasses that amount (and all of the other conditions are met) then that amount is subtracted from their food pool and a new agent is created near this parent agent. This agent inherits a number of traits from its parent, as well as having these values randomly mutated by slight amounts. This ensures diversity within the simulation, and the random amount of food needed to breed likewise staggers new agent creation. We also reasoned that an agent would not feel comfortable bringing a child into the world if they thought the world may not support them; either the world is too polluted or there won’t be enough food for the child. In any case where the agent has enough food to breed yet doesn’t meet all of the other requirements they will simply continue to collect food until all of the requirements have been met.

#### Check Calamity

Next the agent checks the world state. This is where the bulk of the Tragedy of the Commons comes into play. The agent checks whether the amount of pollution - represented as a percentage of pollution to food production - is higher than their foresight ability score. This represents the agent being able to see that they may need to change their ways to ensure the survival of their species. They also check whether the percentage of pollution is high enough that it surpasses their altruism score. This represents their willingness to change. The consequences of these two traits is that if the agent is unable to see that the pollution is too high, or they’re unwilling to change at the current level of pollution, then they won’t switch to using the less beneficial solar energy. The other factor that will prompt an agent to change their energy source is if they notice that the average lifespan is too low as a result of pollution. In this case the same checks are required: can the agent see that the average lifespan is too low with their foresight? Is the agent’s altruism high enough for them to want to change? If the agent doesn’t see any reason to change to or continue using solar, i.e. they don’t see a problem with the pollution levels, they don’t see a problem with the average lifespan, or they’re not altruistic enough to change because of the current pollution/lifespan, then they’ll switch to or continue using fossil fuels.  

#### Collect Food

The agents will then collect food, with the amount they collect determined by their energy source. First the world has to contain enough food for them to collect. If the agent is using solar energy then they will collect a random amount of food between half to one and a half the configured amount of food solar energy provides. They will additionally lower the world’s pollution amount equal to half the amount using fossil fuels to collect food would generate. This represents the fact that it’s easier to pollute the world than to remove pollutants. If instead the agent is using fossil fuels they will collect the same amount of food as a solar energy user would collect plus an additional amount of food configured by the fossil fuel food bonus setting. This additional food will ensure this agent does not starve and is more quickly able to reach a state where they possess enough food to breed. However, using fossil fuels to collect food introduces an amount of pollution into the environment as configured by the pollution penalty setting. 

#### Movement and Collisions

The agents then move in a random direction some distance. This encourages agents to interact with other agents. If agents collide with each other then they use their charisma and trust scores in order to convince the other agent to become more altruistic. Should one agent have a higher charisma score than another agent’s trust, that charismatic agent successfully convinces the agent it collided with to become more altruistic. We reasoned that this would be an appropriate collision effect because altruistic agents want to convince other agents to become similarly altruistic, thus more willing to cooperate and self-sacrifice for the good of the community, and non-altruistic agents want to convince other agents to become altruistic because altruistic agents will likely consume less resources, thus leaving more resources for selfish agents. 

#### Aging

Finally the agents will age a preconfigured amount. These actions will be repeated as long as the agent is still alive.

### UI

The UI elements included within the simulation are sliders in order to change various values during the simulation, a background which changes from blue to black depending on the percentage of pollution in the world, agents which change colour depending on their energy source, and a “health bar” around the agents displaying the level of their food pool. 

![Silders](https://user-images.githubusercontent.com/23039052/114322032-5173f000-9adb-11eb-9c1b-edd99836d360.png)

> (Configurable sliders)
