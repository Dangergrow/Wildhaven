# Research: Fixed Timestep Accumulator Pattern

**Source:** "Fix Your Timestep!" by Glenn Fiedler (gafferongames.com, 2004)
**Relevance:** Wildhaven needs physics+AI updates decoupled from rendering framerate.

---

## 1. Why NOT Variable Timestep

```
// DANGEROUS: variable dt
void Update() {
    float dt = Time.deltaTime;  // varies every frame
    SimulatePhysics(dt);
}
```

**Problems:**
- Simulation behavior changes with framerate (different "feel").
- Springs can explode, objects tunnel through walls, collisions fail.
- Determinism is impossible — no two runs are the same.
- "It's utterly unrealistic to expect your simulation to correctly handle any delta time."

---

## 2. Fixed Timestep Accumulator (The Gold Standard)

Decouple **simulation rate** from **render rate**:

```
Simulation runs at fixed dt (e.g., 1/60s = 16.67ms)
Renderer runs as fast as possible
Accumulator bridges the gap
```

**C# Implementation for Unity / Wildhaven:**

```csharp
public class FixedTimestepGameLoop
{
    private const float FIXED_DT = 1f / 60f;       // 60 Hz simulation
    private const float MAX_FRAME_TIME = 0.25f;     // prevent spiral of death
    private float accumulator = 0f;
    private float gameTime = 0f;

    // For interpolation between physics states
    private State previousState;
    private State currentState;

    public void FrameUpdate(float deltaTime)
    {
        // Clamp max frame time to avoid spiral of death
        if (deltaTime > MAX_FRAME_TIME)
            deltaTime = MAX_FRAME_TIME;

        accumulator += deltaTime;

        // Consume accumulated time in fixed-size chunks
        while (accumulator >= FIXED_DT)
        {
            previousState = currentState;
            FixedUpdate(FIXED_DT);  // physics, AI, etc.
            gameTime += FIXED_DT;
            accumulator -= FIXED_DT;
        }

        // Alpha for interpolation between previous and current state
        float alpha = accumulator / FIXED_DT;

        State renderState = Lerp(previousState, currentState, alpha);
        Render(renderState);
    }

    private void FixedUpdate(float dt)
    {
        // ALL deterministic logic goes here:
        // - Physics integration
        // - Colonist AI updates
        // - Block updates (water, sand, growth)
        // - Pathfinding steps
    }
}
```

---

## 3. The Spiral of Death

If simulation takes longer than real time, the accumulator grows without bound:

```
Frame time = 0.05s, but simulation takes 0.10s
→ Accumulator grows: +0.05s per frame, never catches up
→ Eventually crashes or stalls
```

**Prevention:**
1. **Clamp `MAX_FRAME_TIME`** (e.g., 0.25s) — discard huge spikes.
2. **Clamp max steps per frame**:
   ```csharp
   int maxSteps = 5;
   int steps = 0;
   while (accumulator >= FIXED_DT && steps < maxSteps)
   {
       FixedUpdate(FIXED_DT);
       accumulator -= FIXED_DT;
       steps++;
   }
   // If we hit maxSteps, discard remaining accumulator
   if (steps >= maxSteps) accumulator = 0f;
   ```
   Under heavy load the game slows down instead of crashing.

---

## 4. Interpolation (Smooth Rendering)

Without interpolation, physics objects "stutter" because render frames don't align with physics steps:

```
Physics:   |---step---|---step---|---step---|
Render:    |-frame-|-frame-|-frame-|-frame-|
                               ↑ rendered at wrong physics time
```

**With interpolation (alpha = accumulator / dt):**
```csharp
// 1D / float interpolation
float renderX = Mathf.Lerp(previous.x, current.x, alpha);

// For Vector3 positions
Vector3 renderPos = Vector3.Lerp(previous.position, current.position, alpha);

// For Quaternion rotations
Quaternion renderRot = Quaternion.Slerp(previous.rotation, current.rotation, alpha);
```

**For colonist grid movement:**
```csharp
// If colonist moves 1 block per tick:
//   previousPos = (0,0,0)  — position before current tick
//   currentPos  = (1,0,0)  — position after tick applied
//   alpha = 0.5 → render at (0.5, 0, 0) — visually smooth
```

---

## 5. Comparison: Approaches at a Glance

| Approach | Deterministic | Safe at low FPS | Smooth | Complexity |
|---|---|---|---|---|
| Variable dt | No | Yes (but buggy) | Yes | Trivial |
| Semi-fixed (cap dt) | No | Yes | Yes | Low |
| **Fixed accumulator** | **Yes** | **Yes (with clamping)** | **Yes (with interpolation)** | **Medium** |
| Fixed accumulator + interpolate | Yes | Yes | Best | Medium |

---

## 6. Wildhaven Recommendations

1. **Use fixed accumulator at 60 Hz** (`FIXED_DT = 1/60`). All simulation runs at this rate.
2. **Interpolate colonist positions** using `alpha = accumulator / FIXED_DT`. Colonists move on a grid but should visually slide between cells.
3. **Cap max steps per frame at 5** to prevent spiral of death during lag spikes.
4. **Don't interpolate for discrete events** (block placement, damage). These snap to the current physics state.
5. **Clamp frame time to 0.25s** — if the window is dragged or minimized, don't try to catch up.

**Unity-specific note:** Unity already has `FixedUpdate()` running at a configurable timestep. For Wildhaven you can either:
- Use Unity's built-in `FixedUpdate` + `Time.fixedDeltaTime` (simplest)
- Or implement your own accumulator in `Update()` (more control, better for split-screen or pause menus)
