# Wildhaven — Quick Setup Guide

## What you need to do in Unity (5 minutes)

### 1. Open the project
- Launch Unity Hub → Open → select `Wildhaven-main` folder
- Wait for import (Library rebuilds, ~2 minutes)

### 2. Add CentralIntegration (the master connector)
- In SampleScene, Create Empty GameObject → name it "Core"
- Add Component → `CentralIntegration`
- This single component connects ALL systems automatically

### 3. Verify Components on World GameObject
- Select "World" in Hierarchy
- It should have: `GridManager`, `NetworkWorldSync` (if Mirror), `ForageSpawner`
- If missing: Add Component → search and add

### 4. Verify Camera
- Select "Camera" in Hierarchy
- Should have: `CameraController`, `MainMenu`
- Position: (50, 55, 50), Rotation: (75, 45, 0)

### 5. Press Play!

## What should happen:
1. Main menu appears: "WILDHAVEN"
2. Click "New Game" → Character Creator (3 colonists)
3. Customize colonists → "START GAME"
4. World generates, colonists spawn
5. Bottom bar shows: Architect/Work/Zone/Orders (F1-F4)
6. Select colonist (B → LMB), right-click to order
7. Build blocks (F1 → click category → click block → LMB)
8. F5 save, F9 load, Space pause, 1/2/3 speed

## If something is missing:
- Check Console for errors
- All systems auto-create via CentralIntegration + GameManager
- If a UI panel is missing, check that the Canvas was created

## File overview:
- `Assets/Scripts/World/GridManager.cs` — voxel world
- `Assets/Scripts/Colonists/ColonistAI.cs` — AI
- `Assets/Scripts/Core/CentralIntegration.cs` — master connector
- `Assets/Scripts/Core/GameManager.cs` — system initializer
- `Assets/Scripts/UI/CanvasHUD.cs` — main HUD
- `Assets/Scripts/UI/GameBar.cs` — bottom bar
- `.docs/` — 31 documentation pages
