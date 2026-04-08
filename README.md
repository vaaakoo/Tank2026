# Tank 2026 (Battle City MVP)

A modern, fast-paced C# WPF clone of the classic 1990 retro game **Battle City**. Defend your eagle, gather powerups, and survive progressively frantic waves of enemies!

## Features

* **Retro Vector Graphics**: Authentic pixel-art stylized tanks and tiles, fully drawn using zero-dependency vector geometries natively in WPF XAML paths!
* **Map Progression System**: 5 carefully hand-crafted custom layouts. Destroy the required number of enemies to instantly snap into the next level.
* **4 Unique Enemy Variants**: 
    * **Basic Tank** (Silver) - Standard behavior.
    * **Fast Tank** (Pink) - Moves without a cooldown. Watch out!
    * **Power Tank** (Red) - Extremely aggressive firing patterns.
    * **Armor Tank** (Green) - Spawns with 4 layers of armor. Observe it change color as you wear down its shield down to white!
* **Dynamic Progressive Pacing**: The game starts at a very manageable pace, but the underlying engine clock globally scales the intensity linearly over the course of exactly **2 minutes** until the action becomes completely frantic.
* **Classic Powerups**: Red flashing enemies drop game-changing powerups when defeated:
    * 💣 **Grenade** `G`: Annihilates all currently active enemies on the screen.
    * ⏱️ **Timer / Freeze** `T`: Freezes all enemies on screen in place for several seconds.
    * 🛡️ **Shovel** `S`: Fortifies the base protecting your Eagle with indestructible steel temporarily.
    * ⭐ **Star/Life** `L`: Grants you an immediate bonus life!

## Controls

* **Movement**: `Up`, `Down`, `Left`, `Right` Arrow Keys
* **Shoot**: `Spacebar`
* **Pause**: `P`
* **Restart**: `R`

## How To Run

1. Open the `.slnx` solution file in **Visual Studio 2022**.
2. Press **F5** or click **Start** to run the game natively!

Alternatively, if using the .NET CLI:
```bash
cd Tank2026
dotnet run
```