# The Tragedy of the Commons - A Simulation of Climate Change and Self-Governance

Authors: Cody Clark, Serena Schimert, Taylor Skaalrud

![Average](https://user-images.githubusercontent.com/23039052/114950993-6d480080-9e11-11eb-83b3-6cf84a9e1d05.png)

> (A heavily populated world)

## Background

Simply put, the tragedy of the commons surrounds itself around the concept of open-access resource systems. Originally concepted in the 1833 essay, Lloyd’s Pamphlet [1], which cited concerns over the over-grazing of goats on “common” grazing fields shared by neighbouring herders. 

While grazing additional animals on the commons was an individually rational decision, it imposed negative externalities onto the all commoners, who’s proportional share of the commons per animal either shrunk or suffered at the extra pressure applied by the animals. Were each commoner to come to the same individually rational decision, the commons would swiftly be depleted and/or destroyed ultimately exchanging a sustainable shared resource for a short-sighted boom and bust. Later, Garret Hardin’s 1968 article “The Tragedy of the Commons” argued for a shift in morality and the management of resources-as-commons to “preserve and nurture other and more precious freedoms”[2] in the face of such ‘rational individuals’ whose personal impositions would apply such negative externalities in their tragedies that more than just the freedoms -- that allowed the tragedies in the first place -- would be lost. Hardin also developed the concept of the tragedy of the [unregulated] commons to include the environment as a “negative common” with regards to pollution, which deals with the deliberate commonization of the common cost of pollution, rather than the original definition which was concerned with a privatization of a beneficial resource. Critics often cite the capacity for commons to self-regulate in order to maintain the resource and satisfy both individual and collective interests. This enticed us to model around the concept of self-regulation -- to see if and when self-regulation emerges, what triggers it, and the potential tipping points which prevent the commons from recovering, or conversely, when external regulation should be implemented in order to maintain a given resource. The applicability of the tragedy of the commons as it pertains to a number of modern social issues (e.g. pollutions, floral and faunal industries[3], and information technologies[4]) was also a motivating factor to model it. The problem of the tragedy of the commons and why it is so difficult to approach is because it falls into the class of problems known as “wicked problems”; climate change is actually considered a “super wicked problem”[5] due to having a time limit, no central authority, those seeking to solve it are also responsible for causing it, and policies discount the future irrationally.

[1] - W. F.  Lloyd. Two Lectures on the Checks to Population . England: Oxford University. JSTOR 1972412. OL 23458465M – via Wikisource. 1833.
[2] - G. Hardin, “The Tragedy of the Commons,” Science, vol. 162, no. 3859, pp. 1243–1248, 1968. 
[3] - E. Ostrom, R. Gardner, and J. Walker, Rules, Games, and Common-Pool Resources. Ann Arbor, MI, USA: Univ. Michigan Press, 1994. 
[4] - E. Ostrom, J. Burger, C. B. Field, R. B. Norgaard, and D. Policansky, “Revisiting the commons: Local lessons, global challenges,” Science, vol. 284, no. 5412, pp. 278–282, 1999. 
[5] - K. Levin, B. Cashore, S. Bernstein, and G. Auld, “Overcoming the tragedy of super wicked problems: constraining our future selves to ameliorate global climate change,” Policy Sciences, vol. 45, no. 2, pp. 123–152, 2012. 

## Goal

Our goal with this simulation is to discover exactly which agent and world state attributes are necessary for the emergence of self-regulation, or if self-regulation will occur at all. The issue of climate change is a challenge currently faced by the human race; a race notoriously terrible at predicting ,visualizing, and addressing slow, relatively invisible, but disasterous change. This provides relevency for the issue of collective action as applied by the tragedy of the commons to a modern scenario. 

## Implementation

The simulation is separated into two sections - the world state and the agents. Each section is managed by its own script, with the world state and UI managed by the SimScript and each agent managed by its own copy of the AgentScript. 

This project is implemented in Unity 2019.4.20f1 <DX11>.
  
To interact with this project one can download a copy of this repo as a .zip file, extract it, and then in the assets folder double click on the GreenOrGone unity scene file. As long as Unity version 2019.4.20f1 or newer is installed it should be playable. This project has been successfully converted and run on Unity 2021.1.0f1, so any version between those two should work. 

### Sample Configurations

(Default slider values assumed unless otherwise noted)

#### Stable

- Food Production Rate: 2.22
- Fossil Fuel Food Bonus: 0.15
- Fossil Fuel Pollution Penalty: 0.02

These settings provide for what appears to be an infinitely stable simulation which avoids an extinction event. According to our simulation we believe that in this case the phenomenon of self-regulation has emerged as our emergent.

#### Extinction

- Food Production Rate: 3.00
- Solar Food Value: 0.00
- Fossil Fuel Food Bonus: 0.50
- Fossil Fuel Pollution Penalty: 0.10
- Aging Rate: 0.01

These settings provide a shortcut to obtaining a quick extinction event.

## The World

The world state is composed of the amount of food produced each discrete time step, the amount of pollution present in the world, and the amount of food left in the world. The SimScript also contains all of the configurable elements of the simulation: the amount of food each agent collects when using solar energy, the bonus using fossil fuel energy confers when collecting food, the amount of pollution using fossil fuels produces, the rate at which agents age, and the range agent traits are assigned within when producing new agents. 

![Heavily Polluted](https://user-images.githubusercontent.com/23039052/114951012-75a03b80-9e11-11eb-8beb-838e05ec7f9a.png)

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
7) Drain altruism
8) Age

#### Eat Food

To eat food an agent subtracts a predetermined amount from their food pool. This predetermined amount should be lower than the solar food value so that even agents who neglect fossil fuels still end up with the surplus food amount needed to breed. 


#### Check Death

Agents are born with a limited lifespan and each time step they age a fixed amount towards that lifespan. If their age exceeds their lifespan then they die and are removed from the simulation. Additionally, the amount of pollution in the world state has a negative impact on their individual lifespans, reducing their time to live based on how polluted the world state is in order to more accurately reflect the consequences of real world pollution. This step also checks whether or not the agent starved. This is caused by the eat food step consuming enough food to reduce the agent’s food pool to zero. Likewise when this happens the agent dies and is removed from the simulation.

#### Check Breeding

After the agent has eaten their food and has not died yet, they check whether they’re capable of breeding. In order to breed the agents must satisfy the following conditions:
1) They possess enough food
2) They’re 20+ time units old
3) The world has enough food

The amount of food needed to breed is randomly determined at the time of creation for each agent. Once an agent surpasses that amount (and all of the other conditions are met) then that amount is subtracted from their food pool and a new agent is created near this parent agent. This agent inherits a number of traits from its parent, as well as having these values randomly mutated by slight amounts. This ensures diversity within the simulation, and the random amount of food needed to breed likewise staggers new agent creation. We also reasoned that an agent would not feel comfortable bringing a child into the world if they thought the world may not support them, that is, that there won’t be enough food for the child. In any case where the agent has enough food to breed yet doesn’t meet all of the other requirements they will simply continue to collect food until all of the requirements have been met.

#### Check Calamity

Next the agent checks the world state. This is where the bulk of the Tragedy of the Commons comes into play. The agent checks whether the amount of pollution - represented as a percentage of pollution to food production - is higher than their foresight ability score. This represents the agent being able to see that they may need to change their ways to ensure the survival of their species. They also check whether the percentage of pollution is high enough that it surpasses their altruism score. This represents their willingness to change. The consequences of these two traits is that if the agent is unable to see that the pollution is too high, or they’re unwilling to change at the current level of pollution, then they won’t switch to using the less beneficial solar energy. The other factor that will prompt an agent to change their energy source is if they notice that the average lifespan is too low as a result of pollution. In this case the same checks are required: can the agent see that the average lifespan is too low with their foresight? Is the agent’s altruism high enough for them to want to change? If the agent doesn’t see any reason to change to or continue using solar, i.e. they don’t see a problem with the pollution levels, they don’t see a problem with the average lifespan, or they’re not altruistic enough to change because of the current pollution/lifespan, then they’ll switch to or continue using fossil fuels.  

#### Collect Food

The agents will then collect food, with the amount they collect determined by their energy source. First the world has to contain enough food for them to collect. If the agent is using solar energy then they will collect the amount configured by the solar food value slider. They will additionally lower the world’s pollution amount equal to the amount using fossil fuels to collect food would generate. If instead the agent is using fossil fuels they will collect the same amount of food as a solar energy user would collect plus an additional amount of food configured by the fossil fuel food bonus setting. This additional food will ensure this agent does not starve and is more quickly able to reach a state where they possess enough food to breed. However, using fossil fuels to collect food introduces an amount of pollution into the environment as configured by the pollution penalty setting. 


#### Movement and Collisions

The agents then move in a random direction some distance. This encourages agents to interact with other agents. If agents collide with each other then they use their charisma and trust scores in order to convince the other agent to become more altruistic. Should one agent have a higher charisma score than another agent’s trust, that charismatic agent successfully convinces the agent it collided with to become more altruistic. We reasoned that this would be an appropriate collision effect because altruistic agents want to convince other agents to become similarly altruistic, thus more willing to cooperate and self-sacrifice for the good of the community, and non-altruistic agents want to convince other agents to become altruistic because altruistic agents will likely consume less resources, thus leaving more resources for selfish agents. 

#### Draining Altruism

Each agent will have their altruism levels drained by an amount which is a function of how much food they possess. The more food they possess (such that they’re well fed) the less their altruism is drained. The hungrier they get, the less altruistic they become. This is implemented in order to combat collisions resulting in maximizing altruism levels across an unnaturally large number of agents. 

#### Aging

Finally the agents will age a preconfigured amount. These actions will be repeated as long as the agent is still alive.

## UI

![slider](https://user-images.githubusercontent.com/23039052/114951068-97012780-9e11-11eb-84db-cb7727425e1f.png)

> (Configurable sliders)

The UI elements included within the simulation are sliders in order to change various values during the simulation, a background which changes from blue to black depending on the percentage of pollution in the world, agents which change colour depending on their energy source, and a “health bar” around the agents displaying the level of their food pool. 

Additionally various relevant statistics are shown at the top of the game window. 

![stats](https://user-images.githubusercontent.com/23039052/114951144-bbf59a80-9e11-11eb-9f0f-2df410bf5926.png)

> (Statistics)

## Analysis

Periodic fluctuations of world state from dire to healthy (and back) can occur over the course of the simulation. There are still individuals who stick to fossil fuels even right up until the world ends, and so the world eventually ends if the fossil fuel bonus is at all significant. Pockets of agents sometimes form with the same stance, this could be due to limited space in the simulation but also could be due to central high-charisma individuals within those groups converting high-trust individuals, and the collections of like-minded stances resemble ideological-pockets.

Linear scaling - Depending on parameters probably exponential (in the case of an extinction event - this is the result of the emergent not being efficacious enough) or sinusoidal (in the cases of convergence).

The game resembles an evolutionary game of Hawk-Dove. A larger population of renewables users can generally support a smaller population of fossil fuels users. Although the resource scarcity makes this a precarious balance and an evolutionarily stable strategy is one that manages to balance these populations while also not unbalancing the environment.

Food stockpiles dictate the time that the population has to manage an environmental crisis. Agents create and manage pollution relative to the fossil fuel pollution penalty and thus thus only population composition is relevant to the relative rate of pollution change. What is relevant to population size, however is the food stockpile size as a small population will be able to stretch those resources out for a much longer time than a large population would. Thus climate management is more difficult -- or time constrained -- than it is for small populations.

Stabilities including pollution will slowly decrease the average lifespan of the population, resulting in an eventual collapse wherein the population has become too infirm to support itself.

### Slider Observations (from default positions):

#### Food Production Rate
As in indication of general technological ability for individuals to generate food. As each agent requires 0.25 food per time step, food collected in excess of that is put towards breeding/stockpiles but food production is hindered by pollution.
Solar Food Value: directly correlates to the proportion of renewables users as starving individuals will lose altruism and convert to fossil fuels which provide immediate benefits and can stave off starvation. Comfortable individuals have the freedom to become altruistic, but desperate people who won’t live to tomorrow will be, understandably, less concerned with the distant future.

#### Fossil Fuels Food Bonus
Allows fossil fuel users to gather more food per time step, creating an accelerated population boom. While the politics of the system can largely handle an increase to this, it does favor pollution production and also diminishes the longevity of the food stockpile, making the future come faster and reducing the amount of time the population will have to react and remedy a climate crisis.

#### Fossil Fuel Pollution Penalty 
Low penalty allows for fossil fuel users to rapidly increase population in the beginning, but eventually pollution increases and solar users increase at a greater rate, with the world state ending relatively quickly. A high pollution penalty from the start can stabilize the simulation for longer, although at a higher pollution world state.

#### Aging Rate 
A negligible rate of aging will allow agents to gather significant food in their lifetime and breed many times, and leads to more altruistic agents compared to a very high aging rate, with far more fossil fuel users. A high aging rate will also extirpate the population before it can breed new generations.

#### Altruism Range
A high altruism range introduces large amounts of random political dissidence in subsequent generations. This will likely destabilize the system and at some point cause the collapse of a stable system, resulting in a different stability or the potential for extinction.

#### Charisma Range: 
Slower spread of altruism at low charisma range, and much smaller altruism pockets. Larger pockets of altruistic agents can occur at a higher charisma range. 

#### Trust Range 
A low trust range makes it easier to predict the reaction to higher pollution levels, either more or less agents convert to solar energy at higher pollution levels when trust range is increased (unstable/dynamic).

#### Foresight Range 
A high foresight range will permit new lineages to destabilize a system with radically different politics. A low foresight range will have conformist generations which reinforce the status quo.

#### Lifespan Range 
A longer lifespan range can counteract the effects of pollution, resembling the effects of technology allowing us to surpass physical weaknesses and vulnerabilities. Increasing this simulates a tolerance to certain amounts of pollution, which would otherwise slowly reduce the average lifespan of the population.
Food to Breed Range: Increasing this range will encourage new lineage lines to thrive and old lineages to expire.

It should be reiterated that adjusting the world parameters, including the resources but particularly the social parameters, to try and encourage self-governance are likely to prove futile in long-term scenarios as the effects of pollution persist over the long-term as seen in the minimized food stockpile and dwindling average life span. One “successful” strategy centers around engaging in a sort of technological “arms race” against pollution by extending the uncapped lifespan range, which may be able to indefinitely postpone collapse (the metaphor here functioning along the lines of: if technology and medicine can keep making life easier while pollution makes life harder, we can find an equilibrium where our life expectancy stagnates). The ideal solution incorporates an aggressive set of policies which seek to eliminate pollution before it has had time to accumulate, since, left alone, pollution will accrete and cause a positive feedback loop. Many seemingly stable solutions will suffer from an extremely subtle and insidious side effect of pollution: the slipping average lifespan, which reduces the time each generation has to breed the next as well as eventually preventing breeding altogether due to a lack of generational maturity i.e. even if pollution is “managed”, and food stocks are maintained, the surviving populations are liable to slowly lose the capacity to continue doing so due to e.g. harmful hereditary mutations brought on by a polluted environment.

### Extinction Event

![Extinction](https://user-images.githubusercontent.com/23039052/114951039-86e94800-9e11-11eb-95d1-0fb44568a84a.png)

> (An example of an overpolluted world after an extinction event - Self-governance did not emerge)

We classify an extinction event in our model as a complete dying off of our population - no more agents remain. This often happens because of three scenarios:
1) Pollution reduces food production below sustainable levels.
2) Pollution reduces life expectancy below the level needed to reproduce.
3) When agents don't limit their reproduction then overpopulation occurs.

### Self-Governance

![extremelystable](https://user-images.githubusercontent.com/23039052/114954449-d894d100-9e17-11eb-9585-729b52220258.png)

> (An extremely stable world state (Maybe due to clumping) - 100000+ agents have lived thoughout the course of this simulation, with 106 generations)

Self-governance can typically be difficult to observe, for the sole reason that the world is continually in flux. It's simple to observe when an extinction event occurs, but it's tougher to deliminate what constitutes a stable and self-governed population. Often a world consisting of a mix of fossil fuel and solar power users can be seen, with a stable pollution percentage and stable life expectancy rates over an arbitrary period of time indicating that the agents are limiting their consumption habits on a wide enough scale so as to ensure their future survival. 

### Possible Futures

In this simulation we place a lot of emphasis on altruism, but less obvious factors such as openness to experience have a close tie to environmentalism, as those tend to be people who appreciate the outdoors as well (greater creativity, appreciation of beauty etc.).  This would lead to agents placing a value on not just the physical resource of food, but resources related to leisure as well (i.e., rather than the environmental impact just affecting food decline, the environmental decline would correlate to a decline in enjoyment of said outdoor activities in daily life). The observation made about lowering solar food value and mass conversions to fossil fuels also highlighted a concept to explorer further in regards to barriers to green energy use- the need to survive if impoverished, and the lack of time and mental energy or means to spend cultivating something like green energy  when basic needs aren’t met. An extension could be to vary this trait among individuals as a proxy for income level, or to further develop related traits.
We could also delve into ‘group-think’, and give our agents a level of social self-awareness. For instance, agents could have a desire to conform to the views of their neighbors, perhaps weighted higher if there is a ‘familial’ relationship between agents, i.e. keep track of agent parents. Negotiations could also be made more realistic by giving agents a trait where they have to encounter many different agents before forming their own stance, more similar to how people generally make decisions (this number could be lower with higher trust in addition to trust affecting the negotiation itself, and skepticism could be mitigated or overcome after encountering enough agents with the same stance). It would also be pertinent to enhance the learning/inheritance mechanism, such that knowledge is passed down after a near world-ending disaster with a greater weighting than other information. 

The type of agents could be expanded from representing individuals to representing a simplified corporation or service provider. As we would still want to focus primarily on self-governance, the interaction with these corporate agents would mostly be to simulate the ‘stepping’ of change and the development of available green energy technologies over time, giving agents more choices to work with rather than treating the decision as one that naturally available to individual agents from the beginning (i.e. supply and demand, etc.). It may also be a greater test of the importance of the foresight trait if greater front-end costs and ‘planning’ are needed to make the energy switch. 

### Known Bugs/Possible Changes

A limit has been applied on breeding in order to reduce mass overpopulation reducing the food source to 0 - rather than climate change. There is an artificial amount of food the agents must see in the world in order to feel comfortable breeding, but this may be too heavy-handed. Alternatively, unrestricted breeding that does not lead to overpopulation crushing the food supply results in poor performance on our hardware. Both of these issues are likely solvable, but perhaps outside of the scope of the assignment.

Although eventually discarded in the implementation the idea of pollution being more difficult to remove than to add would make the simulation more relevant. The issue with this is that it caused the world to disolve into an extinction event very rapidly, before the agents had time to stabalize it through self-governance. Perhaps this is a haunting prediction of what's in store for us in the real world, but it would be interesting to expand this simulation to account for this discrepency.

Because of the bounds of the screen often clumping of agents occur - whereby agents are unabled to escape from a mass of other agents and as a result of the massive number of collisions their altruism scores tend to skyrocket artificially. Fixing this would involve wrapping around the screen boundaries such that agents are able to freely move.

![clumping](https://user-images.githubusercontent.com/23039052/114953545-d7fb3b00-9e15-11eb-8b5b-2940afcf1b24.png)

> (After 70000+ agents during this simulation clumping can be observed)

