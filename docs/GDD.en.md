# City Race GDD v0.2 English

Game Design Document GDD v0.2 • 3 October 2026 • Draft for prototype development.

Revision 0.2 records the confirmed technology direction and early Web/Android validation. Gameplay scope is unchanged. See [technical stack](TECH_STACK.en.md) and [engineering guidelines](ENGINEERING_GUIDELINES.en.md).

A motorbike race through rush-hour traffic in Sài Gòn. Each player starts from a different home in the same neighbourhood, rides directly with one finger, and meets the others in traffic. The fun comes from observation, route choices, handling setbacks, and unexpected moments with friends.

This document records the agreed direction and proposes the initial implementation scope. Durations, content counts, and gameplay parameters below are testing targets, not validated results. The working project title is “City Race”.

[Vietnamese edition](GDD.vi.md)

## 1 Agreed direction

- Launch on iOS first, with Android expansion planned and Web supported by the chosen stack. Validate Android and Web early; their release timing remains undecided.

- Simultaneous multiplayer racing to a destination, with direct one-finger riding controls.

- The first setting is Ho Chi Minh City, referred to as Sài Gòn in the game.

- Each player leaves a different home in the same neighbourhood before their routes converge.

- Minimalist visuals, with believable environments, vehicles, and behaviour.

- Offer a small initial selection of appearance or gender, age group, clothing style, and motorbike types familiar in Vietnam.

- Everyday situations include blocked roads, buses squeezing riders for space, potholes, traffic police stopping riders for violations, punctures, and rain.

- Journey scenarios may include commuting to an office or government workplace, deliveries, and taking a romantic partner out.

**Experience goals:** easy to start, a tangible sense of riding, room for skill, and stories to tell after each race. Chaos should have causes and readable cues; random setbacks should not repeatedly punish players.

**Intended audience:** people who enjoy short games with friends and the familiarity of Vietnamese urban life. This is a hypothesis to validate through playtesting.

## 2 A race

**Initial proposal:** private rooms for 2–4 players, all playing a morning commute scenario, with races lasting about 2–4 minutes. Test the technology with two players first; public matchmaking is not initially required.

1. Join a room and quickly choose a character and bike; reuse these choices next race.

2. Each player receives a different starting home, with the destination visible to everyone.

3. A shared countdown ends and everyone starts simultaneously.

4. Leave the alley, merge into traffic, choose a route, and handle obstacles.

5. Reach the workplace entrance zone, slow down, and stop to register a finish.

6. See placements and times, then choose to play again.

**Proposed win condition:** the first valid finisher wins. Collisions and traffic stops cost time directly, without a complex post-race scoring system. Clocking in supplies the sense of urgency; being late does not immediately eliminate a player.

Once the first player finishes, the others get another 30 seconds, subject to an overall four-minute race limit. Players who do not arrive are marked “Did not finish” and cannot rank above finishers. These are trial parameters to adjust to the map length.

A valid finish requires entering the entrance zone at low speed with no unresolved traffic stop. There are no shortcuts through buildings, passing through vehicles, or bypassing collisions.

**Example:** you take an alley to pass a car blocking the road, but must slow down for a pothole. Your friend takes the main road and encounters a bus pulling into a stop. You meet at the final junction; your earlier decisions create a gap that both players can understand.

## 3 Controls and on-screen information

**Agreed:** direct one-finger controls. **To test:** relative dragging, portrait orientation, and a slightly tilted overhead camera.

The initial touch sets a temporary control origin. Drag direction indicates steering direction; drag distance sets throttle. Move closer to the origin to slow down; lift the finger to brake quickly. Touching again creates a new origin without teleporting the bike or causing sudden acceleration.

The bike moves forward and turns along arcs, with limits on acceleration and turning, plus a braking distance; it does not slide sideways after the finger. The first version has no reverse button. If completely stuck, a hold gesture resets the bike near a previously passed position, costing time and never moving it closer to the destination.

The preferred touch area is at the bottom of the screen. The camera shows enough road ahead so the finger does not cover obstacles requiring a response. The first version needs no separate horn or interaction button; stopping in the correct zone triggers an interaction.

Essential information includes time, the player’s position, direction to the destination, and nearby friends’ names. Until ranking across branching routes can be determined reliably, prioritise showing friends’ positions over constantly changing placements. Warnings need visual cues alongside audio.

## 4 The Sài Gòn neighbourhood and traffic

**Proposed first map:** a fictional neighbourhood with four starting homes, branching alleys, a main road, one signal-controlled junction, and a workplace entrance. Provide 2–3 viable routes. Initial branches converge after approximately 15–20 seconds so players encounter one another early.

Balance starting positions by travel time and difficulty, not just distance. A home near the main road may involve a harder merge. Rotate homes between test runs to identify routes with persistent advantages.

Townhouses, roller shutters, awnings, breakfast stalls, Vietnamese signs, trees, and roadside parked bikes establish the setting. Decorative details must be distinguishable from collidable objects. Shapes and relative proportions should distinguish underbone bikes, scooters, cars, and buses.

Computer-controlled traffic should stop at red lights, maintain spacing, merge from alleys, and respond to blocked roads. Buses signal before pulling into stops. Vehicles must not spawn directly in front of players or change direction instantly.

Traffic density should create frequent decisions while leaving a way to respond: wait briefly, slow down, or change route. The system needs a natural way to clear prolonged gridlock.

Planned sound includes engines, brief horns, street ambience, and rain in a later version. Humour comes from events and small character reactions, rather than exaggerated bodies or injuries.

## 5 Situations and consequences

The mechanics below are design proposals. “Initial version” means the small, complete playable level built after validating controls and multiplayer.

| Situation | Cues and player response | Proposed consequence | Scope |
| --- | --- | --- | --- |
| Pothole | Visible road damage; avoid it or slow down | Hitting it fast causes a brief wobble and speed loss while retaining control | Initial version |
| Bus pulling into a stop | Signals and gradually moves towards the kerb; brake or go around when safe | Temporarily occupies the route; contact slows the rider | Initial version |
| Car blocking the way | A visibly stopped or manoeuvring car; wait, pass through a sufficiently wide gap, or reroute | Time cost depends on the decision; no passing through vehicles | Initial version |
| Traffic police whistle | Initially test only running a red light at a monitored junction; clearly show the violation and stopping zone | Pull over for a brief stop; finishing is blocked until the stop is resolved | Initial version |
| Rain | Dark clouds and raindrops precede reduced grip; slow down | Longer braking distance and moderately reduced visibility; shared across the race | Later |
| Puncture | Proposed to follow riding over a clearly marked hazardous stretch, rather than an arbitrary random event | Reduced speed and a visit to a repair stall; repair time is shortened | Later |

Do not stack additional punishment onto a collision while the player has just lost control. Light contact between players causes limited slowing and deflection; an escape mechanism should prevent indefinite blocking.

Shared events must be consistent for everyone in the race. Do not create a pothole only in front of the leader to force a different result.

Traffic violations use simplified game rules. The game does not yet simulate complete enforcement procedures or actual fine amounts. Check accuracy and explain the rule clearly in the game when adding a new violation.

## 6 Characters and bikes

**Agreed:** minimal customisation. **Proposed quantities for testing:**

| Category | Initial choices |
| --- | --- |
| Appearance and gender | Two basic presets, without preset-specific clothing restrictions |
| Age group | Young adult and middle-aged |
| Clothing | Office, rugged streetwear, casual outing |
| Bike | Common underbone bike, small scooter, large scooter |
| Colour | Three top colours and three bike colours; coordinated helmets |

Choices share components to limit production work. Prioritise silhouettes, tops, bags, and bike colours visible from the camera; detailed facial editing is out of scope. Characters always wear helmets.

For the initial version, bikes are proposed to have equivalent performance and collision footprints to validate fair racing. This is a temporary simplification; each bike category should remain visually recognisable. If differences in visual size make collisions confusing, adjust the models before adding performance statistics.

Clothing style does not lock players into an occupation or scenario. Do not use specific brand or delivery-platform logos yet. A home is a starting position chosen in the room, not an asset to purchase or upgrade.

## 7 Implementation scope

**Riding prototype:** one player on an iPhone, one bike, one road segment, a few computer-controlled vehicles, one pothole, and one bus. Use simple shapes. The goal is to establish that riding is understandable and fun.

**Multiplayer prototype:** two players, two homes, shared traffic, and one destination. Check synchronisation of positions, obstacles, collisions, and results. Clearly communicate disconnections; a disconnected player’s bike must not block others indefinitely.

**First complete playable version:** one neighbourhood, a morning commute scenario, private rooms for 2–4 players, the four initial situations, basic customisation, results, and replay. An account is not required to start testing.

**Later:** rain and flooding, punctures, evening commutes, deliveries, taking a romantic partner out, new maps, bike performance differences, home decoration, public matchmaking, seasonal rankings, an in-game economy, and monetisation. Android release is a future expansion; Android and Web technical validation belong to the early prototypes.

Unity + C# is selected. The starting architecture uses Unity 6.3 LTS, URP and Input System, with NGO, Unity Transport and Multiplayer Services Sessions/Relay as the initial networking candidate. See the technical stack for validation gates, including latency and Web compatibility. Budget, production schedule, exact package versions and minimum supported devices remain undecided.

## 8 Hypotheses to validate

The criteria below are proposed internal decision thresholds for an initial test group of 5–8 people, not representative market evidence.

| Hypothesis | Test and desired signal |
| --- | --- |
| One-finger controls are easy enough | At least four out of five players can start, turn, and stop within one minute after brief instructions |
| Controls feel fair | After most collisions, players can explain the cause and what they should do next time |
| Riding is fun enough | Most testers independently want another round; ask what they enjoyed or disliked |
| Starting home does not determine the winner | Rotate homes across multiple runs and measure times with fixed traffic; adjust branches with recurring advantages |
| Multiplayer adds appeal | Both devices show the same events and results; no obvious collision disagreement changes the winner |
| Customisation provides enough recognition | Players recognise their own bike and distinguish friends while moving |

If players repeatedly lose track of their bike, struggle with controls, or do not understand penalties, fix those issues before adding content.

Decisions to finalise after the prototype: dragging and braking mechanics, camera angle, player-to-player collision severity, race duration, recovery from being stuck, and differences between bikes.

**Next step:** build the iPhone riding prototype within the scope in section 7, include early Android/Web checks from the technical stack, record findings against section 8, and update the GDD to v0.3.
