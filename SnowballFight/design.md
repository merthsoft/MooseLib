***Snowball Fight!***
Snowballs are the only source of damage in the game! No destructive magic! No swords! Just snowballs! 

The objective is simple: knock out all your enemies by beaning them with snowballs and try not to get knocked out yourself. Remember to use cover and terrain gain a tactical advantage.

You can rearrange the positions units they take when spawned into the battlefield.

***Cover***

Since this is a game focused on ranged combat, using cover is crucial. The player must respect LOS to the enemy and not position themselves in the open. Not all cover is created equal - taking cover behind a low snow mound is not as good as a tree, for example. Therefore, we must distinguish between *Half Cover* and *Full Cover*. *Full Cover* completely blocks incoming snowball attacks unless the attacker is at a sharp angle; *Half Cover* is, as the name implies, about half as effective as Full Cover and will at best represent a 50% block chance (angle of attack permitting). *Full Cover* represents very valuable real estate on the battlefield and the player should endeavor to control as many *Full Cover* tiles as they can. Most cover objects will be Half (other units are also *Half Cover*).

Also, there's friendly fire. Be smart about your positioning and try not to hit your friends. If a unit is adjacent to the ally they're trying to throw a snowball through, there is no friendly fire risk.

***Ammunition***

Another consideration for a game about ranged combat is ammunition. The units don't carry infinite snowballs in their pockets - they must periodically Rearm (scooping new snowballs off the ground). *Units can't rearm if they aren't on a snow tile.* One rearming provides as many snowballs as the unit can carry.

*Every unit carries up to three snowballs*, meaning it's impossible for one unit to knock a regular, 4 HP enemy unit out of the game *from full health* with one armful of snowballs. This should compel the player to try and avoid pointless 1v1 situations, and instead use multiple units to focus down a single target when possible. The player must think tactically to create advantageous situations for their team!

***Range***

The range of a snowball attack is 8-12 tiles depending on the *Range* stat, and a unit moves between 3-6 tiles, depending on their *Movement* stat (6-12 tiles if double moving). It is more difficult to hit distant enemies than ones that are closer, so the player will tend to want to position units close to the enemy to do damage. However, since there is no melee in this game, the hit rate against enemies that are very close (within 4 tiles) is flat, meaning that there is no point to getting a tile away from an enemy. This should lead to a "sweet spot" of 5-6 tiles being the most reliable and safe range for fighting. Range is handled using a normal distribution of angle-of-attack offsets. Units’ accuracy will increase the sigma of the angle-of-attack offset, thus making it more likely that the snowball drifts, missing the target and potentially hitting a different target. 

***Damage***

Snowballs do a flat 1 HP damage when they successfully strike an enemy (or ally), provided the enemy does not block or dodge it. Successful blocks and dodges reduce damage to 0. 

***Battlefield***

There are only three types of ground tiles - snow, ice, and everything that's impassable. Units can only Rearm if they're on a snow tile - otherwise, it's normal terrain. Ice is difficult to walk on, so units move slowly on it (half *Movement*) - and if they take damage on ice, they slip and fall, leaving them defenseless. No matter what, don't end your unit's turn on ice! Patches of ice should be laid out as obstacles to prevent players from rushing to the most defensible areas on the battlefield straight away - but a player might be able to gain a decisive advantage early if they’re brave.

***Turns***

A simple truth: the fastest unit (highest *Speed*) will always get the first turn, unless somehow scripted not to. Similarly, the unit with the lowest Speed will always get the last turn. If a fast unit is fast enough, it could get two turns before that unit with slow Speed! *The turn order should always be made apparent to the player.*

Sometimes a unit will have to wait longer to get its next turn, owing to its actions on the current turn (some actions are "slower" than others). Taking a normal move and throwing a snowball is considered "fast" - doing these things won't affect the turn order. Rearming and standing up are both considered “slow” actions. Helping an ally up is a fast action but the ally unit is delayed.

On their turn, units either choose to **Move**, **Attack**, **Rearm** (*unavailable if fully armed*), **Defend**, or **Delay**. If the unit is prone, their only choices are **Stand** and **Delay**. If an adjacent unit is prone but this unit is standing, an additional action **Help Ally** is available. 

Attacking, rearming, defending, or delaying ends the turn, but movement does not - units can move, then act, but *cannot act and then move*. – I still need to decide on this [smm]

Units can move twice in one turn (double move). The unit cannot act after committing a double move, and the double move is considered a "slow" action (delays next turn). The standard and double move radii should be made clear to the player.

If the unit defends, they remain in place and have an increased chance to dodge incoming attacks. This is considered a "slow" action.

Delaying means the unit simply waits and does nothing for its turn. This is considered a "fast" action. *Not moving and delaying should be the absolute fastest way to get another turn.*

***Stats***

*Speed*: How quickly the unit gets their turn. This ranges from 1 to 6, with most units having an average speed of 3. A unit at Speed 6 gets its turns about twice as often as a unit at Speed 1.

*Movement*: How far the unit can move on their turn. This ranges from 3 to 6 and directly represents the number of tiles of movement - a unit with 4 Movement moves up to 4 tiles (up to 8 if they double move).

*Range*: How many tiles the unit can throw a snowball.

*Accuracy%:* Affects general chance to hit with a snowball attack. Represents inverse percentage sigma grows by when calculating angle-of-attack.

*Dodge%:* Affects chance to dodge snowball attacks. Units have a small innate dodge chance that scales with Speed and Movement, +1% for each point, making +12% the natural maximum. Flat percentage roll when unit is hit.

*HP:* The unit's "health" - how many hits, or regular snowball attacks, it can take before being downed. When reduced to 0 HP, the unit is downed and can not act unless revived by an ally. Most regular units have 4 HP at an absolute minimum, tougher units have around 8 HP, and Hero units have 12 or more. 

***Unit Types**
*Normal units*: These units have the lowest stats.

*Strong units:* These units have increased stats.

*Hero units:* These units have max stats, but are limited to one of each unit per map.

***Unit Placement***

At the beginning of play, the players choose their units and layout. Units are picked with a buy-in, where hero units cost three points each, strong units cost two points, and normal units cost 1 point. You have 12 points to build your army. 

Each map is designated with “starting locations” that determine where each player starts. Units are placed in an 8-square radius around the start locations.
