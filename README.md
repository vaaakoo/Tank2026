# Tank 2026 (Battle City MVP)

A modern, fast-paced C# WPF clone of the classic 1990 retro game **Battle City**. Defend your eagle, gather powerups, and survive progressively frantic waves of enemies!

## Download & Play Now

You can download the pre-compiled, fully standalone executable directly from GitHub without needing to compile it yourself:
1. Navigate to the **Releases** tab on the right side of the GitHub repository.
2. Download the `Tank2026.exe` asset from the latest release.
3. Double-click the `.exe` to start playing immediately (No installation required)!

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

## How to Publish a Release

If you are a developer looking to publish a new executable build to GitHub, run the following commands:
```powershell
# 1. Publish a self-contained single file executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 2. Upload the built executable to GitHub Releases (Requires GitHub CLI)
gh release create v1.0.0 "Tank2026\bin\Release\net10.0-windows\win-x64\publish\Tank2026.exe" --title "Tank 2026 v1.0" --notes "Initial public MVP release."
```