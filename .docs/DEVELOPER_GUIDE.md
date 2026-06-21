# Wildhaven Developer Guide

## Architecture Overview

### Master Connector: CentralIntegration.cs
The heart of the game. Runs on timers and calls ALL systems:
- Every 1s: Schedule → colonist behavior
- Every 5s: Auto-heal, roof, quests, forage, plants, cooking, religion, fire, hunting, taming, cleanup
- Every 30s: Raid scaling, seasons, trade caravans

### System Initializer: GameManager.cs
Auto-creates all systems if they don't exist. Called by CentralIntegration.

### Key systems and their files:

| System | File | What it does |
|---|---|---|
| World | GridManager.cs | Voxel terrain, chunks, mesh, save/load |
| Building | BuildManager.cs | Place/remove blocks, drag-build, copy/paste |
| Camera | CameraController.cs | WASD/QE/zoom/MMB movement |
| Colonists | ColonistAI.cs | Movement, orders, combat, auto-work |
| Needs | NeedsSystem.cs | Hunger, thirst, fatigue, mood decay |
| Schedule | ColonistSchedule.cs | 24h sleep/work/rec/anything |
| Combat | ColonistAI.HandleCombat | Auto-attack enemies, ranged support |
| Enemies | Enemy.cs, RaidManager.cs | Enemy AI, wave spawning |
| Trading | EconomyManager.cs, TradeUI.cs | Currency, buy/sell, caravans |
| Quests | QuestManager.cs | 5 quest types, rewards, time limits |
| Family | FamilySystem.cs | Marriage, pregnancy, children |
| Religion | ReligionSystem.cs | Faith, rituals, holy sites |
| Electricity | EnergyNetwork.cs | Generators, wires, consumers |
| Farming | PlantGrowth.cs | 12 crops, growth, harvest |
| Animals | AnimalManager.cs | 12 types, hunt/tame/breed |
| Cooking | CookingSystem.cs | 17 recipes (food + brewing) |
| Medicine | MedicineSystem.cs | 20 diseases, healing, surgery |
| Stability | StabilitySystem.cs | Blocks collapse unsupported |
| Fire | FireAndSeasons.cs | Fire spread, 4 seasons |
| Repair | RepairSystem.cs | Damage tracking, colonist repair |
| Forage | ForageSpawner.cs | Berries/mushrooms/herbs on map |
| Research | ResearchManager.cs | 60+ techs, 5 eras |
| Social | SocialSystem.cs | Relationships, marriage |
| Zones | ZoneMarker.cs | 12 zone types |
| Blueprint | BlueprintManager.cs | Designated construction |

### UI Systems:

| Panel | File | Hotkey |
|---|---|---|
| Main Menu | MainMenu.cs | Auto |
| Character Creator | CharacterCreator.cs | New Game flow |
| HUD | CanvasHUD.cs | Auto |
| Bottom Bar | GameBar.cs | F1-F4 |
| Colonist Info | ColonistPanel.cs | Select colonist |
| Work Priorities | WorkPanel.cs | F2 |
| Trade | TradeUI.cs | Caravan event |
| Settings | GameSettings.cs | Esc |

### Control Scheme (Going Medieval style):
- WASD/arrows = camera
- Q/E = rotate 45°
- Scroll = zoom
- MMB = free rotate
- F1-F4 = Architect/Work/Zone/Orders
- B = toggle build/select mode
- LMB = place block / select colonist
- RMB = mine block / order move / attack enemy
- Shift+LMB = blueprint
- Shift+RMB = demolish 3x3
- Ctrl+C/V = copy/paste structures
- R = draft combat mode
- Space = pause
- Num1/2/3 = speed
- F5/F9 = save/load

### Adding new content:
- Items: add to ItemType.cs enum
- Recipes: add to CookingSystem.InitDefaultRecipes()
- Crops: add to CropType enum + PlantGrowth cropDefs
- Diseases: add to MedicineSystem.Disease enum
- Blocks: add to BlockType.cs enum + GridManager color switch
