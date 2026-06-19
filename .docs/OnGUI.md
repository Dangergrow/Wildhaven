       Unity - Scripting API: MonoBehaviour.OnGUI()                   

 

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

# [MonoBehaviour](MonoBehaviour.html).OnGUI()

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

[Switch to Manual](../Manual/class-MonoBehaviour.html "Go to MonoBehaviour Component in the Manual")

### Description

OnGUI is called for rendering and handling GUI events.

OnGUI is the only function that can implement the ["Immediate Mode" GUI (IMGUI)](../Manual/GUIScriptingGuide.html) system for rendering and handling GUI events. Your OnGUI implementation might be called several times per frame (one call per event). For more information on GUI events see the [Event](Event.html) reference. If the MonoBehaviour's enabled property is set to false, OnGUI() will not be called.

using UnityEngine;
using System.Collections;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    void OnGUI()
    {
        if ([GUI.Button](GUI.Button.html)(new [Rect](Rect.html)(10, 10, 150, 100), "I am a button"))
        {
            print("You clicked the button!");
        }
    }
}

For more information, see the [GUI Scripting Guide](../Manual/GUIScriptingGuide.html).

Is something described here not working as you expect it to? It might be a **Known Issue**. Please check with the Issue Tracker at [issuetracker.unity3d.com](https://issuetracker.unity3d.com).

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://unity3d.com/learn) [Community Answers](https://answers.unity3d.com) [Knowledge Base](https://support.unity3d.com/hc/en-us) [Forums](https://forum.unity3d.com) [Asset Store](https://unity3d.com/asset-store) [Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html) [Legal](https://unity.com/legal) [Privacy Policy](https://unity.com/legal/privacy-policy) [Cookies](https://unity.com/legal/cookie-policy) [Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
