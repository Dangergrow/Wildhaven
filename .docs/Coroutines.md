     Unity - Manual: Write and run coroutines                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/Coroutines.html)
-   [中文](/cn/current/Manual/Coroutines.html)
-   [日本語](/ja/current/Manual/Coroutines.html)
-   [한국어](/kr/current/Manual/Coroutines.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/Coroutines.html)
-   [中文](/cn/current/Manual/Coroutines.html)
-   [日本語](/ja/current/Manual/Coroutines.html)
-   [한국어](/kr/current/Manual/Coroutines.html)

-   [Programming in Unity](scripting.html)
-   [Object-oriented development](object-oriented-development.html)
-   [Split tasks across frames with coroutines](coroutines-section.html)
-   Write and run coroutines

[](coroutines-section.html)

Split tasks across frames with coroutines

[](coroutines-analyzing.html)

Analyzing coroutines

# Write and run coroutines

A coroutine is a method that can suspend execution and resume at a later time. In Unity applications, this means coroutines can start running in one frame and then resume in another, allowing you to spread tasks across several frames.

Regular, non-coroutine methods run to completion before returning control to the caller, which in the Unity runtime means their action completes within a single frame update. In situations where you want the work of a method to take effect over several frames, such as a gradual fade-out effect, you can use a coroutine. Coroutines are also useful for handling long asynchronous operations, such as waiting for HTTP transfers, asset loads, or file I/O to complete.

**Important**: Don't confuse coroutines with threads. Synchronous operations that run within a coroutine still execute on the main thread. If you want to reduce the amount of CPU time spent on the main thread, it's just as important to avoid blocking operations in coroutines as in any other script code. If you want to use multi-threaded code in Unity, your options are:

-   The [job system](job-system.html)
-   The .NET [async and await](async-await-support.html) and Unity's custom `Awaitable` support

## Writing coroutines

Consider the task of gradually reducing an object's alpha (opacity) value until it becomes invisible. For the fading effect to be visible, the opacity must reduce over a sequence of frames. If you tried to write a `Fade` method, you might write something like the following:

```
void Fade()
{
    Color c = renderer.material.color;
    for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
    {
        c.a = alpha;
        renderer.material.color = c;
    }
}
```

This method is not a coroutine, so it executes every iteration of its `for` loop within a single frame update and the object disappears instantly instead of appearing to fade out. One posible solution is to add code to the `Update` function that executes the fade on a frame-by-frame basis. However, it can be more convenient to use a coroutine.

Coroutines are methods with an [IEnumerator](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerator) return type and a [yield](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield) return statement included somewhere in the body. The `yield return` statement is the point at which execution is suspended. The previous `Fade` method can be rewritten as a coroutine as follows:

```
IEnumerator Fade()
{
    Color c = renderer.material.color;
    for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
    {
        c.a = alpha;
        renderer.material.color = c;
        yield return null;
    }
}
```

This version of the method executes one iteration of its `for` loop before suspending execution at the `yield return null` statement. It resumes and executes another iteration of the loop in the next frame, and so on, making the gradual fade effect visible. The loop counter in the `Fade` method maintains its correct value over the lifetime of the coroutine, and any variable or parameter is preserved between `yield` statements.

## Starting and stopping coroutines

To set a coroutine running, use the [StartCoroutine](../ScriptReference/MonoBehaviour.StartCoroutine.html) method:

```
void Update()
{
    if (Input.GetKeyDown("f"))
    {
        StartCoroutine(Fade());
    }
}
```

To stop a coroutine, use [StopCoroutine](../ScriptReference/MonoBehaviour.StopCoroutine.html) and [StopAllCoroutines](../ScriptReference/MonoBehaviour.StopAllCoroutines.html). A coroutine also stops if:

-   The value of [`GameObject.activeSelf`](../ScriptReference/GameObject-activeSelf.html) becomes `false` for the **GameObject**The fundamental object in Unity scenes, which can represent characters, props, scenery, cameras, waypoints, and more. A GameObject's functionality is defined by the Components attached to it. [More info](class-GameObject.html)  
    See in [Glossary](Glossary.html#GameObject) the script is attached to.
-   The MonoBehaviour script is destroyed with a call to [Destroy](../ScriptReference/Object.Destroy.html).

**Note:** Disabling the MonoBehaviour script by setting [enabled](../ScriptReference/Behaviour-enabled.html) to `false` doesn't stop coroutines.

## Resuming coroutines

When a suspended coroutine resumes execution depends on the yield instruction provided in the `yield return` statement. A `yield return null` resumes on the next frame. Unity has a set of custom yield instructions that you can use to resume after a specified time, when a specified conditions is met, or at specific points in the Player loop. For more information, refer to [Yield instruction reference](coroutines-yield-instructions.html).

In the case of fade effect example, you might want the fade effect to happen at a lower and more consistent rate than the frame rate. You can `yield return` the [`WaitForSeconds`](../ScriptReference/WaitForSeconds.html) instruction to introduce a fixed time delay between iterations of the `Fade` method as follows:

```
IEnumerator Fade()
{
    Color c = renderer.material.color;
    for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
    {
        c.a = alpha;
        renderer.material.color = c;
        // Wait for 0.1 seconds before the next iteration
        yield return new WaitForSeconds(.1f);
    }
}
```

It's also possible to `yield return` a Unity [`Awaitable`](../ScriptReference/Awaitable.html) from within a coroutine. This can be useful if you want to integrate coroutines with asynchronous code that uses `async` and `await`. For example, in the previous example you could `yield return Awaitable.WaitForSecondsAsync(.1f)` instead of `yield return new WaitForSeconds(.1f)` to achieve the same effect.

**Important**: It's not supported to `yield return` the generic [`Awaitable<T0>`](../ScriptReference/Awaitable_1.html) from a coroutine.

## Coroutines in Edit mode

Coroutines are primarily a runtime feature. The associated [runtime yield instructions](coroutines-yield-instructions.html) are in the `UnityEngine` namespace and run in the Editor's Play mode or in a standalone platform Player. They can also run in Edit mode if your **scripts**A piece of code that allows you to create your own Components, trigger game events, modify Component properties over time and respond to user input in any way you like. [More info](creating-scripts.html)  
See in [Glossary](Glossary.html#Scripts) use the [`[ExecuteInEditMode]`](../ScriptReference/ExecuteInEditMode.html) or [`[ExecuteAlways]`](../ScriptReference/ExecuteAlways.html) attributes, but the update loop in Edit mode is not as fixed and regular as the Player loop.

For coroutines designed specifically to run in Edit mode, use the [Editor coroutines package](https://docs.unity3d.com/Packages/com.unity.editorcoroutines@latest).

## Coroutines in tests

Unity Test Framework Play mode tests marked with the `[UnityTest]` attribute run as coroutines and allow you to yield custom instructions for the Unity Editor from tests. For more information, refer to [Yield instructions for the Editor](test-framework/reference-custom-yield-instructions).

## Coroutine performance

Coroutines can cause hidden allocations and garbage collector spikes if misused. Each coroutine creates an `IEnumerator` **state machine**The set of states in an Animator Controller that a character or animated GameObject can be in, along with a set of transitions between those states and a variable to remember the current state. The states available will depend on the type of gameplay, but typical states include things like idling, walking, running and jumping. [More info](StateMachineBasics.html)  
See in [Glossary](Glossary.html#StateMachine). Starting them frequently (for example, per frame) allocates and adds overhead. A `yield return null` does not allocate but yield instructions like `new WaitForSeconds` do. Cache commonly reused ones and avoid lambdas in [`WaitUntil`](../ScriptReference/WaitUntil.html) and [`WaitWhile`](../ScriptReference/WaitWhile.html) to prevent delegate and capture allocations.

Prefer long-lived coroutines that loop with `yield return null` instead of repeatedly starting new ones. Cache or pool [`WaitForSeconds`](../ScriptReference/WaitForSeconds.html) with fixed durations. Coroutines retain references to their owner and captured variables. Ensure they end or are stopped with [`MonoBehaviour.StopCoroutine`](../ScriptReference/MonoBehaviour.StopCoroutine.html) to avoid leaks.

Always profile, especially on constrained platforms, to confirm and locate allocations. For more information, refer to [Analyzing coroutines](coroutines-analyzing.html).

## Additional resources

-   [Coroutine API reference](../ScriptReference/Coroutine.html)
-   [MonoBehaviour.StartCoroutine](../ScriptReference/MonoBehaviour.StartCoroutine.html)

[](coroutines-section.html)

Split tasks across frames with coroutines

[](coroutines-analyzing.html)

Analyzing coroutines

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
