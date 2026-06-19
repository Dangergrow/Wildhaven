       Unity - Scripting API: Physics.Raycast                   

 

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

LanguageEnglish

-   English

-   C#

[](https://docs.unity3d.com)

## Scripting API

 

Version: Unity 6.0 Select a different version

LanguageEnglish

-   English

# [Physics](Physics.html).Raycast

Leave feedback

Suggest a change

## Success!

Thank you for helping us improve the quality of Unity Documentation. Although we cannot accept all submissions, we do read each suggested change from our users and will make updates where applicable.

Close

## Submission failed

For some reason your suggested change could not be submitted. Please <a>try again</a> in a few minutes. And thank you for taking the time to help us improve the quality of Unity Documentation.

Close

Your name  Your email  Suggestion\* Submit suggestion

Cancel

## Declaration

public static bool Raycast([Vector3](Vector3.html) origin, [Vector3](Vector3.html) direction, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, [QueryTriggerInteraction](QueryTriggerInteraction.html) queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);

### Parameters

Parameter

Description

origin

The starting point of the ray in world coordinates.

direction

The direction of the ray.

maxDistance

The max distance the ray should check for collisions.

layerMask

A [Layer mask](../Manual/Layers.html) that is used to selectively filter which colliders are considered when casting a ray.

queryTriggerInteraction

Specifies whether this query should hit Triggers.

### Returns

**bool** Returns true if the ray intersects with a Collider, otherwise false.

### Description

Casts a ray, from point `origin`, in direction `direction`, of length `maxDistance`, against all colliders in the Scene.

To select which layers a ray should collide with, use a [LayerMask](LayerMask.html).  
  
Specifying `queryTriggerInteraction` allows you to control whether or not Trigger colliders generate a hit, or whether to use the global [Physics.queriesHitTriggers](Physics-queriesHitTriggers.html) setting.  
  
**Notes:** Raycasts will not detect Colliders for which the Raycast origin is inside the Collider. In all these examples [FixedUpdate](PlayerLoop.FixedUpdate.html) is used rather than [Update](PlayerLoop.Update.html). Refer to [Order of execution for event functions](../Manual/execution-order.html) to understand the difference between [Update](PlayerLoop.Update.html) and [FixedUpdate](PlayerLoop.FixedUpdate.html), and to see how they relate to physics queries.

using UnityEngine;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    [LayerMask](LayerMask.html) layerMask;  
  
    void Awake()
    {
        layerMask = [LayerMask.GetMask](LayerMask.GetMask.html)("Wall", "[Character](TextCore.Text.Character.html)");
    }
    
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {  
  
        [RaycastHit](RaycastHit.html) hit;
        // Does the ray intersect any objects excluding the player layer
        if ([Physics.Raycast](Physics.Raycast.html)(transform.position, transform.TransformDirection([Vector3.forward](Vector3-forward.html)), out hit, [Mathf.Infinity](Mathf.Infinity.html), layerMask))  
  
        { 
            [Debug.DrawRay](Debug.DrawRay.html)(transform.position, transform.TransformDirection([Vector3.forward](Vector3-forward.html)) \* hit.distance, [Color.yellow](Color-yellow.html)); 
            [Debug.Log](Debug.Log.html)("Did Hit"); 
        }
        else
        { 
            [Debug.DrawRay](Debug.DrawRay.html)(transform.position, transform.TransformDirection([Vector3.forward](Vector3-forward.html)) \* 1000, [Color.white](Color-white.html)); 
            [Debug.Log](Debug.Log.html)("Did not Hit"); 
        }  
  
    }
}

This example creates a simple Raycast, projecting forwards from the position of the object's current position, extending for 10 units.

using UnityEngine;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {
        [Vector3](Vector3.html) fwd = transform.TransformDirection([Vector3.forward](Vector3-forward.html));  
  
        if ([Physics.Raycast](Physics.Raycast.html)(transform.position, fwd, 10))
            print("There is something in front of the object!");
    }
}

---

## Declaration

public static bool Raycast([Vector3](Vector3.html) origin, [Vector3](Vector3.html) direction, out [RaycastHit](RaycastHit.html) hitInfo, float maxDistance, int layerMask, [QueryTriggerInteraction](QueryTriggerInteraction.html) queryTriggerInteraction);

### Parameters

Parameter

Description

origin

The starting point of the ray in world coordinates.

direction

The direction of the ray.

hitInfo

If true is returned, `hitInfo` will contain more information about where the closest collider was hit. (Additional resources: [RaycastHit](RaycastHit.html)).

maxDistance

The max distance the ray should check for collisions.

layerMask

A [Layer mask](../Manual/Layers.html) that is used to selectively filter which colliders are considered when casting a ray.

queryTriggerInteraction

Specifies whether this query should hit Triggers.

### Returns

**bool** Returns true when the ray intersects any collider, otherwise false.

### Description

Casts a ray against all colliders in the Scene and returns detailed information on what was hit.

This example reports the distance between the current object and the reported Collider:

using UnityEngine;  
  
public class RaycastExample : [MonoBehaviour](MonoBehaviour.html)
{
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {
        [RaycastHit](RaycastHit.html) hit;  
  
        if ([Physics.Raycast](Physics.Raycast.html)(transform.position, -[Vector3.up](Vector3-up.html), out hit))
            print("Found an object - distance: " + hit.distance);
    }
}

This example re-introduces the `maxDistance` parameter to limit how far ahead to cast the Ray:

using UnityEngine;  
  
public class RaycastExample : [MonoBehaviour](MonoBehaviour.html)
{
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {
        [RaycastHit](RaycastHit.html) hit;  
  
        if ([Physics.Raycast](Physics.Raycast.html)(transform.position, -[Vector3.up](Vector3-up.html), out hit, 100.0f))
            print("Found an object - distance: " + hit.distance);
    }
}

---

## Declaration

public static bool Raycast([Ray](Ray.html) ray, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, [QueryTriggerInteraction](QueryTriggerInteraction.html) queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);

### Parameters

Parameter

Description

ray

The starting point and direction of the ray.

maxDistance

The max distance the ray should check for collisions.

layerMask

A [Layer mask](../Manual/Layers.html) that is used to selectively filter which colliders are considered when casting a ray.

queryTriggerInteraction

Specifies whether this query should hit Triggers.

### Returns

**bool** Returns true when the ray intersects any collider, otherwise false.

### Description

Same as above using `ray.origin` and `ray.direction` instead of `origin` and `direction`.

using UnityEngine;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {
        [Ray](Ray.html) ray = Camera.main.ScreenPointToRay([Input.mousePosition](Input-mousePosition.html));
        if ([Physics.Raycast](Physics.Raycast.html)(ray, 100))
            print("Hit something!");
    }
}

---

## Declaration

public static bool Raycast([Ray](Ray.html) ray, out [RaycastHit](RaycastHit.html) hitInfo, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, [QueryTriggerInteraction](QueryTriggerInteraction.html) queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);

### Parameters

Parameter

Description

ray

The starting point and direction of the ray.

hitInfo

If true is returned, `hitInfo` will contain more information about where the closest collider was hit. (Additional resources: [RaycastHit](RaycastHit.html)).

maxDistance

The max distance the ray should check for collisions.

layerMask

A [Layer mask](../Manual/Layers.html) that is used to selectively filter which colliders are considered when casting a ray.

queryTriggerInteraction

Specifies whether this query should hit Triggers.

### Returns

**bool** Returns true when the ray intersects any collider, otherwise false.

### Description

Same as above using `ray.origin` and `ray.direction` instead of `origin` and `direction`.

This example draws a line along the length of the Ray whenever a collision is detected:

using UnityEngine;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    // See [Order of Execution for Event Functions](../Manual/ExecutionOrder.html) for information on [FixedUpdate](PlayerLoop.FixedUpdate.html)() and [Update](PlayerLoop.Update.html)() related to physics queries
    void [FixedUpdate](PlayerLoop.FixedUpdate.html)()
    {
        [Ray](Ray.html) ray = Camera.main.ScreenPointToRay([Input.mousePosition](Input-mousePosition.html));
        [RaycastHit](RaycastHit.html) hit;  
  
        if ([Physics.Raycast](Physics.Raycast.html)(ray, out hit, 100))
            [Debug.DrawLine](Debug.DrawLine.html)(ray.origin, hit.point);
    }
}

Is something described here not working as you expect it to? It might be a **Known Issue**. Please check with the Issue Tracker at [issuetracker.unity3d.com](https://issuetracker.unity3d.com).

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://unity3d.com/learn) [Community Answers](https://answers.unity3d.com) [Knowledge Base](https://support.unity3d.com/hc/en-us) [Forums](https://forum.unity3d.com) [Asset Store](https://unity3d.com/asset-store) [Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html) [Legal](https://unity.com/legal) [Privacy Policy](https://unity.com/legal/privacy-policy) [Cookies](https://unity.com/legal/cookie-policy) [Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
