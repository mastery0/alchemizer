# Alchemizer

**Alchemizer** is a 2D metroidvania platformer made with Unity. Explore corrupted areas, defeat hostile creatures, collect elemental essences, and build a setup focused on aggressive and precise combat.

![Screenshot]()

## Gameplay at a glance

The current gameplay loop is focused on exploration and deliberate risk management:

1. Traverse the available areas and fight enemies.
2. Collect elemental essences dropped by defeated enemies.
3. Spend essences in the **Catalyst** skill tree to improve combat, movement, or sustain.
4. Take quests from Yara, the herbalist, to unlock additional potions.
5. Activate checkpoints to save progress, replenish the equipped potion's uses, and choose a different unlocked potion.

## Combat and Core Instability

* Combat uses a beam that originates from the Catalyst and immobilizes the player while it is being performed.
* The **Pressure** system, called *Core Instability*. Pressure increases by 10 when the player hits an enemy or takes damage; when the player avoids both for three seconds, Pressure decreases by 10 every half-second.

At 30 and 60 Pressure, the system increases its tier and the colors of the system also change. Each tier increases attack damage by 15% and defense by 10%. Pressure changes are displayed through its bar, located on the right side of the screen, with color variations based on the current Pressure level.

## Movement and Traversal Abilities

Available controls:

* Ground movement, jumping, fast-falling, and a dash with a cooldown.
* Air dash and an additional dash charge through the **Gale Step** Catalyst upgrade.
* Double jump (implemented in the game but currently not usable).
* Gliding while airborne by holding the jump direction (implemented in the game but currently not usable).

Wall climbing, wall jumping, and ground slam are not documented here because they are not implemented in the current player code.

| Keyboard Input          | Action                            |
| ----------------------- | --------------------------------- |
| `A` / `D` or arrow keys | Move left/right                   |
| `W` / Up Arrow          | Jump; hold while falling to glide |
| `S` / Down Arrow        | Fast-fall                         |
| `Shift`                 | Dash                              |
| Left mouse button       | Attack                            |
| `Q`                     | Use the equipped potion           |


## Catalyst Skill Tree

The Catalyst is a three-branch skill tree that can be upgraded using essences. Skills require the previous abilities in the same branch and the indicated elemental essences; unlocked skills are saved between sessions.

|  Branch  | First Skill  | Second Skill  | Third Skill | Fourth skill |
| -- | -- | -- | -- | -- |
| **Combat**   | Alchemist's Strength (+20% attack damage) | Burning Soul (faster attacks) | Core Instability  | Glass Cannon (stronger Core Instability modifiers)                    |
| **Movement** | Swift Step (+10% movement speed) | Wind Flow (reduced dash cooldown) | Air Dash (you can dash in air)  | Gale Step (one additional dash charge) |
| **Sustain**  | Vitality (+30% maximum health) | Flowing Health (stronger healing) | Fluid Body, which extends your invulnerability | Blooming Heals (chance for defeated enemies to spawn healing spheres)|

## Potions and Yara's Quests

Only one potion can be equipped at a time. Checkpoints allow the player to change the equipped potion and replenish its available uses.

| Potions | Uses per checkpoint | Effect | How to obtain
| -- |-- | -- | -- |
| **Base Potion**  | 3 | Restores 40% of maximum health and resets Pressure | The potion obtained at the beginning of the game |
| **Ember Potion** | 5   | Restores 25% of maximum health without resetting Pressure | Unlocked by Yara's *Clear the Spores* quest|
| **Rage Potion**  | 2  | Sacrifices 5% of current health, increases Pressure by one level, and increases attack damage by 20% for 10 seconds | Unlocked by Yara's *Defeat The Root Golem* quest |

Yara is the quest giver and can be found outside her house in the Enchanted Forest. Her first quest asks the player to eliminate 10 spores; completing it unlocks the Ember Potion. Her second quest culminates in the Root Golem encounter and unlocks the Rage Potion.

## Boss Encounters

Boss battle features:

* When a boss is encountered, the arena is closed so that the player cannot escape.
* A boss health bar is displayed.
* Once defeated, the boss places a checkpoint inside the arena, allowing the player to save and change potions.

1. **Giant Slime:** its attacks are jumping, dashing, and charging, depending on the player's distance.
2. **Root Golem:** its attacks are a short-range attack and a medium-range attack that launches roots which deal heavy damage.

## Current Areas

The current area manager configures two areas:

* **House:** where the cutscene takes place and which serves as the starting area for the player.
* **Enchanted Forest:** the first area, featuring several activities such as quests, boss battles, and exploration.

## Checkpoints and Saving

At a checkpoint, the player can set the respawn point, save the game, replenish potion uses, and change the equipped potion. Upon death, the player loses 20% of each type of essence they possess and respawns at the last checkpoint.

The save data also records unlocked Catalyst skills and potions, the equipped potion, essences, quest progress and objective progress, interactions with NPCs, and defeated bosses.

### Now a little bit of code

[`coreInstability.cs`](alchemizer/Assets/script/mechanics/coreInstability.cs) It determines the current power level based on Pressure, removes modifiers from the previous level, and applies new ones, ensuring that power levels do not stack and that only the level corresponding to the accumulated Pressure is active.

```csharp
int newTier = 0;

if (currentPressure >= 30) newTier = 1;
if (currentPressure >= 60) newTier = 2;

if (newTier != lastTier)
{
    if (lastTier >= 1) { player.attackDamage /= atkMod; player.defense /= defMod; }
    if (lastTier >= 2) { player.attackDamage /= atkMod; player.defense /= defMod; }

    if (newTier >= 1) { player.attackDamage *= atkMod; player.defense *= defMod; }
    if (newTier >= 2) { player.attackDamage *= atkMod; player.defense *= defMod; }

    lastTier = newTier;
}
```

### Catalyst Upgrades

[`skillSO.cs`](alchemizer/Assets/script/skills/skillSO.cs) Each upgrade is an item with characteristics that vary depending on the upgrade itself; furthermore, prerequisites and essence costs are verified before granting the power to the player.

```csharp
public void Unlock()
{
    if (canUnlock())
    {
        payEssences();
        applyEffects();
    }
}

public void payEssences()
{
    foreach (essence s in essences)
    {
        essenceManager.instance.essenceInv[s.type] -= s.amount;
    }
}

public void applyEffects()
{
    player.instance.attackDamage *= atkMult;
    player.instance.dashCooldown /= dashCDmult;
    player.instance.dashCount += dashCount;

    if (airDash) player.instance.airDash = true;
    if (glider) player.instance.hasGlider = true;
    isUnlocked = true;
}
```

## Running the Project

Download the executable file from the repository or visit the itch.io page and download the game from there.

[Download on Itch]()

# AI Usage Disclosure

AI tools were used for:

* Balancing formulas
* Debug code
* Code optimization using Unity-specific functions

No AI-generated images or sounds were directly used in the final version of the game.

# License
This project is proprietary software.
The repository is public exclusively for portfolio and viewing purposes.
Unauthorized use, reproduction, or redistribution is prohibited.