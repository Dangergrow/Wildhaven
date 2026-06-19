     Unity - Manual: Unlit shader material Inspector window reference for URP                      

[](https://docs.unity3d.com)

-   [Manual](../index.html)
-   [Scripting API](../../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.5** (6000.5)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/urp/unlit-shader.html)
-   [中文](/cn/current/Manual/urp/unlit-shader.html)
-   [日本語](/ja/current/Manual/urp/unlit-shader.html)
-   [한국어](/kr/current/Manual/urp/unlit-shader.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.5Select a different version

Language : English

-   [English](/Manual/urp/unlit-shader.html)
-   [中文](/cn/current/Manual/urp/unlit-shader.html)
-   [日本語](/ja/current/Manual/urp/unlit-shader.html)
-   [한국어](/kr/current/Manual/urp/unlit-shader.html)

-   [Materials and shaders](../materials-and-shaders.html)
-   [Prebuilt materials and shaders](../built-in-materials-and-shaders.html)
-   [Prebuilt shaders in URP](../urp/shaders-in-universalrp.html)
-   [Shader Material Inspector window reference for URP](../urp/shaders-in-universalrp-reference.html)
-   Unlit shader material Inspector window reference for URP

[](../urp/baked-lit-shader.html)

Baked Lit shader material Inspector window reference for URP

[](../urp/shader-terrain-lit.html)

Terrain Lit shader material Inspector window reference for URP

# Unlit shader material Inspector window reference for URP

Explore the properties and settings you can use to customize the default Unlit **shader**A program that runs on the GPU. [More info](../Shaders.html)  
See in [Glossary](../Glossary.html#shader) in the Universal **Render Pipeline**A series of operations that take the contents of a Scene, and displays them on a screen. Unity lets you choose from pre-built render pipelines, or write your own. [More info](../render-pipelines.html)  
See in [Glossary](../Glossary.html#renderpipeline) (URP).

## Surface Options

The **Surface Options** section controls how URP renders the material on-screen.

  

**Property**

**Sub-property**

**Description**

**Surface Type**

Sets whether the surface is opaque or transparent. The options are the following:

-   **Opaque**: The surface is fully opaque and doesn't use transparency blending. URP renders opaque surfaces first.
-   **Transparent**: Blends the surface with the background. URP renders transparent surfaces in a separate render pass after opaque surfaces.

This property affects batching performance.

**Blending Mode**

Sets how the shader blends the color of a transparent material with the background. This property is available only when you set **Surface Type** to **Transparent**. The options are:

-   **Alpha**: Calculates transparency using the alpha value of the material.
-   **Premultiply**: Calculates transparency using the alpha value of the material, but multiplies the RGB values by the alpha value first.
-   **Additive**: Adds the color of the material to the background color.
-   **Multiply**: Multiplies the color of the material with the background color.

For more information about blending modes, refer to [Blending modes](blending-modes.html). This property affects batching performance.

**Render Face**

Specifies which faces of the mesh the shader renders. The options are:

-   **Front**: Renders only front‑facing triangles. This is the default.
-   **Back**: Renders only back‑facing triangles.
-   **Both**: Renders both front and back faces.

**Alpha Clipping**

Discards **pixels**The smallest unit in a computer image. Pixel size depends on your screen resolution. Pixel lighting is calculated at every screen pixel. [More info](../ShadowPerformance.html)  
See in [Glossary](../Glossary.html#pixel) if their alpha value is lower than the **Threshold** value. The default value is 0.5. Use this property to create hard edges between opaque and transparent areas, for example to create blades of grass. This property affects batching performance.

**Threshold**

Sets the minimum alpha value for a pixel to be visible. The default value is 0.5. This property is only available if you enable **Alpha Clipping**.

## Surface Inputs

The **Surface Inputs** describe the surface itself. For example, use these properties to make your surface look wet, dry, rough, or smooth.

 

Property

Description

**Base Map**

Sets the surface color, also known as diffuse. To assign a texture, select the picker (**⊙**). To assign a color, either as the main color or a tint on top of the texture, select the swatch or the dropper.  
  
If you set **Surface Type** to **Transparent** or enable **Alpha Clipping**, Unity uses the alpha channel to calculate the transparency of the surface.

**Tiling**

Scales the surface input textures along their UV axes. The default value is 1, which means no scaling. Set a higher value to make the textures repeat across your **mesh**The main graphics primitive of Unity. Meshes make up a large part of your 3D worlds. Unity supports triangulated or Quadrangulated polygon meshes. Nurbs, Nurms, Subdiv surfaces must be converted to polygons. [More info](../mesh.html)  
See in [Glossary](../Glossary.html#Mesh). Set a lower value to stretch the textures.

**Offset**

Positions the surface input textures on the mesh.

## Advanced Options

The **Advanced Options** section controls the rendering calculations Unity uses.

 

Property

Description

**Sorting Priority**

Determines when Unity renders the material. Unity renders materials with lower values first. Use this property to prevent Unity from rendering pixels twice by rendering materials behind other materials. This property works similarly to the [render queue](../../ScriptReference/Material-renderQueue.html) in the Built-In Render Pipeline.

**Enable GPU Instancing**

Renders meshes with the same geometry and material in a single batch when possible. This makes rendering faster. URP can't render meshes in one batch if they have different materials or if the hardware doesn't support GPU instancing. For more information, refer to [GPU instancing](../GPUInstancing-landing.html).

**Alembic Motion Vectors**

Enables motion vector support for meshes streamed through [Alembic](../com.unity.formats.alembic) files. For more information, refer to [Built-in shader support for motion vectors](features/motion-vectors-shader-support.html).

****XR**An umbrella term encompassing Virtual Reality (VR), Augmented Reality (AR) and Mixed Reality (MR) applications. Devices supporting these forms of interactive applications can be referred to as XR devices. [More info](../XR.html)  
See in [Glossary](../Glossary.html#XR) Motion Vectors Pass (SpaceWarp)**

Enables URP running a motion vector render pass in XR to improve performance. This property is only available if your project meets the prerequisites for [URP Application SpaceWarp](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest?subfolder=/manual/features/spacewarp.html).

[](../urp/baked-lit-shader.html)

Baked Lit shader material Inspector window reference for URP

[](../urp/shader-terrain-lit.html)

Terrain Lit shader material Inspector window reference for URP

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70272010. Built on: 2026-06-18.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
