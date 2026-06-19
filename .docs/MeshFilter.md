     Unity - Manual: Mesh Filter component reference                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/class-MeshFilter.html)
-   [中文](/cn/current/Manual/class-MeshFilter.html)
-   [日本語](/ja/current/Manual/class-MeshFilter.html)
-   [한국어](/kr/current/Manual/class-MeshFilter.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/class-MeshFilter.html)
-   [中文](/cn/current/Manual/class-MeshFilter.html)
-   [日本語](/ja/current/Manual/class-MeshFilter.html)
-   [한국어](/kr/current/Manual/class-MeshFilter.html)

-   [Assets and media](assets-and-media.html)
-   [Types of assets](AssetTypes.html)
-   [Meshes](mesh.html)
-   [Mesh components reference](mesh-components-reference.html)
-   Mesh Filter component reference

[](class-SkinnedMeshRenderer.html)

Skinned Mesh Renderer component reference

[](class-Mesh.html)

Mesh asset Inspector window reference

# Mesh Filter component reference

[Switch to Scripting](../ScriptReference/MeshFilter.html "Go to MeshFilter page in the Scripting Reference")

A **Mesh Filter** component holds a reference to a mesh. It works with a [Mesh Renderer](https://docs.unity3d.com/Manual/class-MeshRenderer.html)A mesh component that takes the geometry from the Mesh Filter and renders it at the position defined by the object's Transform component. [More info](class-MeshRenderer.html)  
See in [Glossary](Glossary.html#MeshRenderer) component on the same **GameObject**The fundamental object in Unity scenes, which can represent characters, props, scenery, cameras, waypoints, and more. A GameObject's functionality is defined by the Components attached to it. [More info](class-GameObject.html)  
See in [Glossary](Glossary.html#GameObject); the Mesh Renderer renders the mesh that the Mesh Filter references.

To render a deformable mesh, use a [Skinned Mesh Renderer](https://docs.unity3d.com/Manual/class-SkinnedMeshRenderer.html) instead. A Skinned Mesh Renderer component does not need a Mesh Filter component.

## Mesh Filter Inspector reference

 

**Property**

**Function**

**Mesh**The main graphics primitive of Unity. Meshes make up a large part of your 3D worlds. Unity supports triangulated or Quadrangulated polygon meshes. Nurbs, Nurms, Subdiv surfaces must be converted to polygons. [More info](mesh.html)  
See in [Glossary](Glossary.html#Mesh)

A reference to a [mesh asset](class-Mesh.html).  
  
To change the mesh asset that the Mesh Filter component references, use the picker (circle icon) next to the mesh's name.  
  
**Note:** The settings for other components on this GameObject don't change when you change the mesh that the Mesh Filter references. For example, a Mesh Renderer doesn't update its settings, which might cause Unity to render the mesh with unexpected properties. If this happens, adjust the settings of the other components as needed.  
  
Corresponds to the [MeshFilter.mesh](https://docs.unity3d.com/ScriptReference/MeshFilter-mesh.html) property.

MeshFilter

[](class-SkinnedMeshRenderer.html)

Skinned Mesh Renderer component reference

[](class-Mesh.html)

Mesh asset Inspector window reference

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
