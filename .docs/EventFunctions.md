     Unity - Manual: Event functions                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/event-functions.html)
-   [中文](/cn/current/Manual/event-functions.html)
-   [日本語](/ja/current/Manual/event-functions.html)
-   [한국어](/kr/current/Manual/event-functions.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/event-functions.html)
-   [中文](/cn/current/Manual/event-functions.html)
-   [日本語](/ja/current/Manual/event-functions.html)
-   [한국어](/kr/current/Manual/event-functions.html)

-   [Programming in Unity](scripting.html)
-   [Object-oriented development](object-oriented-development.html)
-   [Managing update and execution order](managing-update-order.html)
-   Event functions

[](script-execution-order.html)

Script execution order

[](execution-order.html)

Event function execution order

# Event functions

Event functions are a set of predefined callbacks that all [MonoBehaviour](class-MonoBehaviour.html) script components can potentially receive. The callbacks are triggered by various Unity Editor and Engine events, including:

-   [Regular frame and physics updates](#regular-updates)
-   **Scene**A Scene contains the environments and menus of your game. Think of each unique Scene file as a unique level. In each Scene, you place your environments, obstacles, and decorations, essentially designing and building your game in pieces. [More info](CreatingScenes.html)  
    See in [Glossary](Glossary.html#Scene) and object [lifecycle events](#initialization-events), such as initialization and destruction of objects in a scene.
-   [UI events](#gui-events)
-   [Input events](#input-events)
-   [Physics events](#physics-events)

Implement the appropriate method signature in your `MonoBehaviour`-derived class to allow your game objects to react to the source events.

For a full list of the available callbacks, refer to the [`MonoBehaviour`](../ScriptReference/MonoBehaviour.html) API reference where they are listed under **Messages**. The rest of this section gives an overview of some of the key groups of event functions.

## Regular update events

A playing game continuously renders a series of frames. A key concept in games programming is making changes to position, state, and behavior of objects just before each frame is rendered. The [`Update`](../ScriptReference/MonoBehaviour.Update.html) function is the main place for this kind of code in Unity. `Update` is called before the frame is rendered and also before animations are calculated.

The physics system also updates in discrete time steps in a similar way to the frame rendering. A separate event function called [`FixedUpdate`](../ScriptReference/MonoBehaviour.FixedUpdate.html) is called just before each physics update. Since the physics updates and frame updates don't occur with the same frequency, you can get more accurate results from physics code if you place it in the `FixedUpdate` function rather than `Update`.

It's also sometimes useful to make additional changes at a point after the `Update` and `FixedUpdate` functions have been called for all objects in the scene, and after all animations have been calculated. Some examples of this scenario are:

-   When a **camera**A component which creates an image of a particular viewpoint in your scene. The output is either drawn to the screen or captured as a texture. [More info](CamerasOverview.html)  
    See in [Glossary](Glossary.html#Camera) should remain trained on a target object. The adjustment to the camera's orientation must be made after the target object has moved.
-   When the script code should override the effect of an animation. For example, to make a character's head look towards a target object in the scene.

The [`LateUpdate`](../ScriptReference/MonoBehaviour.LateUpdate.html) function can be used for these kinds of situations.

For an overview of how frame and physics updates fit into the overall event function update sequence, refer to [Event function execution order](execution-order.html).

## Initialization events

It's often useful to be able to call initialization code in advance of any updates that occur during gameplay. The [`Start`](../ScriptReference/MonoBehaviour.Start.html) function is called before the first frame or physics update on an object. The [`Awake`](../ScriptReference/MonoBehaviour.Awake.html) function is called for each object in the scene at the time when the scene loads. Note that although the various objects' `Start` and `Awake` functions are called in arbitrary order, all instances of `Awake` will have finished before the first `Start` is called. This means that code in a `Start` function can make use of other initializations previously carried out in the `Awake` phase.

For an overview of how initialization functions fit into the overall event function update sequence, refer to [Event function execution order](execution-order.html).

## GUI events

For projects that use the legacy [IMGUI UI system](gui-Basics.html), the [`MonoBehavior.OnGUI`](../ScriptReference/MonoBehaviour.OnGUI.html) callback is called periodically to draw and manage UI elements.

**Note**: Adding an `OnGUI` callback, even if empty, to any object in a scene also adds IMGUI processing to the frame handling process, which creates extra overhead. Only use `OnGUI` if your project uses the [IMGUI UI system](gui-Basics.html), which is no longer recommended.

For more information, refer to [UI systems](UIToolkits.html).

## Input events

The `MonoBehaviour` class also has a set of event functions named with the prefix `OnMouse` (e.g., [`OnMouseOver`](../ScriptReference/MonoBehaviour.OnMouseOver.html), [`OnMouseDown`](../ScriptReference/MonoBehaviour.OnMouseDown.html)) for reacting to user actions with the mouse. These callbacks are only supported for projects using the legacy [Input Manager](class-InputManager.html)Settings where you can define all the different input axes, buttons and controls for your project. [More info](class-InputManager.html)  
See in [Glossary](Glossary.html#InputManager), which is no longer recommended.

For more information, refer to [Input](Input.html).

## Physics events

The physics system reports **collisions**A collision occurs when the physics engine detects that the colliders of two GameObjects make contact or overlap, when at least one has a Rigidbody component and is in motion. [More info](CollidersOverview.html)  
See in [Glossary](Glossary.html#collision) against an object by calling event functions on that object's script component.

The [`OnCollisionEnter`](../ScriptReference/MonoBehaviour.OnCollisionEnter.html), [`OnCollisionStay`](../ScriptReference/MonoBehaviour.OnCollisionStay.html) and [`OnCollisionExit`](../ScriptReference/MonoBehaviour.OnCollisionExit.html) functions are called as contact is made, held and broken. The corresponding [`OnTriggerEnter`](../ScriptReference/MonoBehaviour.OnTriggerEnter.html), [`OnTriggerStay`](../ScriptReference/MonoBehaviour.OnTriggerStay.html) and [`OnTriggerExit`](../ScriptReference/MonoBehaviour.OnTriggerExit.html) functions are called when the object's **collider**An invisible shape that is used to handle physical collisions for an object. A collider doesn't need to be exactly the same shape as the object's mesh - a rough approximation is often more efficient and indistinguishable in gameplay. [More info](CollidersOverview.html)  
See in [Glossary](Glossary.html#collider) is configured as a trigger collider.

For more information on physics event functions, refer to [Collider interactions](collider-interactions.html).

## Additional resources

-   [MonoBehaviour](class-MonoBehaviour.html)
-   [Event function execution order](execution-order.html)
-   [`MonoBehaviour` API reference](../ScriptReference/MonoBehaviour.html)

[](script-execution-order.html)

Script execution order

[](execution-order.html)

Event function execution order

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
