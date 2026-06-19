     Unity - Manual: Mesh collider component reference                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/class-MeshCollider.html)
-   [中文](/cn/current/Manual/class-MeshCollider.html)
-   [日本語](/ja/current/Manual/class-MeshCollider.html)
-   [한국어](/kr/current/Manual/class-MeshCollider.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/class-MeshCollider.html)
-   [中文](/cn/current/Manual/class-MeshCollider.html)
-   [日本語](/ja/current/Manual/class-MeshCollider.html)
-   [한국어](/kr/current/Manual/class-MeshCollider.html)

-   [Physics](PhysicsSection.html)
-   [Built-in 3D physics](PhysicsOverview.html)
-   [Collision](collision-section.html)
-   [Collider shapes](collider-shapes.html)
-   [Mesh colliders](mesh-colliders.html)
-   Mesh collider component reference

[](prepare-mesh-for-mesh-collider.html)

Prepare a Mesh for Mesh colliders

[](wheel-colliders.html)

Wheel colliders

# Mesh collider component reference

[Switch to Scripting](../ScriptReference/MeshCollider.html "Go to MeshCollider page in the Scripting Reference")

The **Mesh**The main graphics primitive of Unity. Meshes make up a large part of your 3D worlds. Unity supports triangulated or Quadrangulated polygon meshes. Nurbs, Nurms, Subdiv surfaces must be converted to polygons. [More info](mesh.html)  
See in [Glossary](Glossary.html#Mesh) **Collider**An invisible shape that is used to handle physical collisions for an object. A collider doesn't need to be exactly the same shape as the object's mesh - a rough approximation is often more efficient and indistinguishable in gameplay. [More info](CollidersOverview.html)  
See in [Glossary](Glossary.html#collider) component takes a [mesh asset](class-Mesh.html) and builds a collider that matches the geometry of that mesh.

Mesh colliders are a convenient way to represent complex shapes and can provide more accurate **collision detection**An automatic process performed by Unity which determines whether a moving GameObject with a Rigidbody and collider component has come into contact with any other colliders. [More info](CollidersOverview.html)  
See in [Glossary](Glossary.html#collisiondetection) for complex geometry than primitives. However, for moving or dynamic objects, primitive colliders can be more efficient because they offer more stability and better-performing physics simulation.

Mesh colliders that are marked as **Convex** can collide with other mesh colliders.

 

**Property**

**Description**

**Convex**

Make the mesh collider collide with other mesh colliders. **Convex** mesh colliders are limited to 255 triangles.

**Is Trigger**

Use the collider as a trigger for events. When **Is Trigger** is enabled, other colliders pass through this collider, and trigger the messages [`OnTriggerEnter`](../ScriptReference/Collider.OnTriggerEnter.html), [`OnTriggerStay`](../ScriptReference/Collider.OnTriggerStay.html), and [`OnTriggerExit`](../ScriptReference/Collider.OnTriggerExit.html).

**Provides Contacts**

Generate contact information for this collider at all times. Usually, a collider only generates contact data if there is something to send it to; in this case, the messages [`OnCollisionEnter`](../ScriptReference/Collider.OnCollisionEnter.html), [`OnCollisionStay`](../ScriptReference/Collider.OnCollisionStay.html), or [`OnCollisionExit`](../ScriptReference/Collider.OnCollisionExit.html). When **Provides Contacts** is enabled, the collider generates contact data for the physics system at all times. Contact generation is resource-intensive, so **Provides Contacts** is disabled by default.

**Cooking Options**

Enable or disable the mesh cooking options that affect how the physics system processes meshes. When you set the **Cooking Options** to any other value than the default settings (that is, everything enabled except **None**), the mesh collider must use a mesh that has an [`isReadable`](../ScriptReference/Mesh-isReadable.html) value of `true`. For details on mesh cooking, refer to [Prepare a mesh for mesh colliders](prepare-mesh-for-mesh-collider.html). Select from the following options:

-   **None**: Disables all **Cooking Options**. This is disabled by default.
-   **Everything**: Enables all **Cooking Options**. This is enabled by default.
-   **Cook for Faster Simulation**: The cooking process preprocesses the mesh data and stores it in memory to speed up runtime calculations. This is useful for complex meshes in the scene. When this setting is disabled, the physics system uses a faster cooking time, but retrieves the mesh data more slowly at runtime. This is enabled by default.
-   **Enable Mesh Cleaning**: The cooking process attempts to clear the mesh of degenerate triangles on the mesh (that is, triangles in which all three points are collinear, and do not form a triangle shape) and other geometrical artifacts. This results in a mesh that is better suited for use in collision detection, and tends to produce more accurate contact points. When this setting is disabled, the physics system uses a faster cooking time but implements less optimization. This is enabled by default.
-   **Weld Colocated Vertices**: The cooking process combines vertices that have the same position. This results in a mesh that is better suited for use in collision detection, and tends to produce more accurate contact points. When this setting is disabled, the physics system uses a faster cooking time but implements less optimization. This is enabled by default.
-   **Use Fast Midphase**: The cooking process uses the fastest mid-phase acceleration structure and algorithm available for your output platform. The fastest algorithm doesn't require any R-Trees for spatial access. If you encounter midphase issues at runtime, disable this option. Instead, Unity then uses the slower legacy midphase algorithm. This is enabled by default.

**Material**

Reference to the [Physics Material](class-PhysicsMaterial.html)A physics asset for adjusting the friction and bouncing effects of colliding objects. [More info](class-PhysicsMaterial.html)  
See in [Glossary](Glossary.html#PhysicsMaterial) that determines how this collider interacts with others.

**Mesh**

Reference to the mesh to use for collisions.

## Layer overrides

The Layer Overrides section provides properties that allow you to override the project-wide [Layer-based collision detection](LayerBasedCollision.html) settings for this collider.

 

**Property**

**Description**

**Layer Override Priority**

Define the priority of this collider override. When two colliders have conflicting overrides, the settings of the collider with the higher value priority are taken.  
For example, if a collider with a **Layer Override Priority** of 1 collides with a Collider with a **Layer Override Priority** of 2, the physics system uses the settings for the Collider with the **Layer Override Priority** of 2.

**Include Layers**

Choose which Layers to include in collisions with this collider.

**Exclude Layers**

Choose which Layers to exclude in collisions with this collider.

## Additional resources

-   [Collision](collision-section.html)A collision occurs when the physics engine detects that the colliders of two GameObjects make contact or overlap, when at least one has a Rigidbody component and is in motion. [More info](CollidersOverview.html)  
    See in [Glossary](Glossary.html#collision)
-   [Mesh colliders](mesh-colliders.html)
-   [Physics materials](class-PhysicsMaterial.html)
-   [Prepare a mesh for mesh colliders](prepare-mesh-for-mesh-collider.html)

[](prepare-mesh-for-mesh-collider.html)

Prepare a Mesh for Mesh colliders

[](wheel-colliders.html)

Wheel colliders

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
