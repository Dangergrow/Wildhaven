     Unity - Manual: Script serialization                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/script-serialization.html)
-   [中文](/cn/current/Manual/script-serialization.html)
-   [日本語](/ja/current/Manual/script-serialization.html)
-   [한국어](/kr/current/Manual/script-serialization.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/script-serialization.html)
-   [中文](/cn/current/Manual/script-serialization.html)
-   [日本語](/ja/current/Manual/script-serialization.html)
-   [한국어](/kr/current/Manual/script-serialization.html)

-   [Programming in Unity](scripting.html)
-   [Compilation and code reload](compilation-and-code-reload.html)
-   Script serialization

[](configurable-enter-play-mode-details.html)

Details of disabling domain and scene reload

[](script-serialization-rules.html)

Serialization rules

# Script serialization

**Serialization** is the automatic process of transforming data structures or **GameObject**The fundamental object in Unity scenes, which can represent characters, props, scenery, cameras, waypoints, and more. A GameObject's functionality is defined by the Components attached to it. [More info](class-GameObject.html)  
See in [Glossary](Glossary.html#GameObject) states into a format that Unity can store and reconstruct later.

How you organize data in your Unity project affects how Unity serializes that data, which can have a significant impact on the performance of your project. This page outlines serialization in Unity and how to optimize your project for it.

**Topic**

**Description**

[Serialization rules](script-serialization-rules.html)

Conditions that determine whether fields in your **scripts**A piece of code that allows you to create your own Components, trigger game events, modify Component properties over time and respond to user input in any way you like. [More info](creating-scripts.html)  
See in [Glossary](Glossary.html#Scripts) are serialized.

[Custom serialization](script-serialization-custom-serialization.html)

How to serialize additional items not supported by Unity's serializer.

[How Unity uses serialization](script-serialization-how-unity-uses.html)

More details about how serialization works in Unity.

[JSON Serialization](json-serialization.html)

Convert Unity objects to and from JSON format using the JsonUtility class.

[Serialization best practices](script-serialization-best-practices.html)

Best practices for serialization.

## Additional resources

-   [Script compilation](script-compilation.html)
-   [Scripting back ends](scripting-backends.html)
-   [Code reload in the Editor](code-reloading-editor.html)

[](configurable-enter-play-mode-details.html)

Details of disabling domain and scene reload

[](script-serialization-rules.html)

Serialization rules

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
