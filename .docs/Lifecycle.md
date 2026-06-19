       Unity - Scripting API: MonoBehaviour.OnEnable()                   

 

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

# [MonoBehaviour](MonoBehaviour.html).OnEnable()

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

Called when a component of an active GameObject is first enabled.

`OnEnable` is called in the following scenarios:

-   When entering Play mode, if the GameObject is active ([GameObject.activeInHierarchy](GameObject-activeInHierarchy.html) == `true`) and the script component is enabled ([Behaviour.enabled](Behaviour-enabled.html) == `true`).
-   When enabling the script component at runtime (via code or the Inspector), if the GameObject is already active.
-   When activating the GameObject (or one of its inactive parent GameObjects) at runtime, if the script component is already enabled.

`OnEnable` is always called after [MonoBehaviour.Awake](MonoBehaviour.Awake.html) and before [MonoBehaviour.Start](MonoBehaviour.Start.html) on entering Play Mode.  
  
`OnEnable` cannot be a [coroutine](../Manual/Coroutines.html).  
  
Additional resources: [MonoBehaviour.OnDisable](MonoBehaviour.OnDisable.html).

// Implement OnDisable and OnEnable script functions.
// These functions will be called when the script component
// is enabled.
// This example also supports the [Editor](Editor.html). The [Update](PlayerLoop.Update.html) function
// will be called, for example, when the position of the
// [GameObject](GameObject.html) is changed.  
  
using UnityEngine;  
  
\[[ExecuteInEditMode](ExecuteInEditMode.html)\]
public class PrintOnOff : [MonoBehaviour](MonoBehaviour.html)
{
    void OnDisable()
    {
        [Debug.Log](Debug.Log.html)("PrintOnDisable: script was disabled");
    }  
  
    void OnEnable()
    {
        [Debug.Log](Debug.Log.html)("PrintOnEnable: script was enabled");
    }  
  
    void [Update](PlayerLoop.Update.html)()
    {
#if UNITY\_EDITOR
        [Debug.Log](Debug.Log.html)("[Editor](Editor.html) causes this [Update](PlayerLoop.Update.html)");
#endif
    }
}

Is something described here not working as you expect it to? It might be a **Known Issue**. Please check with the Issue Tracker at [issuetracker.unity3d.com](https://issuetracker.unity3d.com).

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://unity3d.com/learn) [Community Answers](https://answers.unity3d.com) [Knowledge Base](https://support.unity3d.com/hc/en-us) [Forums](https://forum.unity3d.com) [Asset Store](https://unity3d.com/asset-store) [Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html) [Legal](https://unity.com/legal) [Privacy Policy](https://unity.com/legal/privacy-policy) [Cookies](https://unity.com/legal/cookie-policy) [Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
