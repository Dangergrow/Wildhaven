     Unity - Manual: MonoBehaviour                      

[](https://docs.unity3d.com)

-   [Manual](../Manual/index.html)
-   [Scripting API](../ScriptReference/index.html)

-   [unity.com](https://unity.com/)

Version: **Unity 6.0** (6000.0)

-   Supported
-   Legacy

Language : English

-   [English](/Manual/class-MonoBehaviour.html)
-   [中文](/cn/current/Manual/class-MonoBehaviour.html)
-   [日本語](/ja/current/Manual/class-MonoBehaviour.html)
-   [한국어](/kr/current/Manual/class-MonoBehaviour.html)

[](https://docs.unity3d.com)

## Unity Manual

Version: Unity 6.0Select a different version

Language : English

-   [English](/Manual/class-MonoBehaviour.html)
-   [中文](/cn/current/Manual/class-MonoBehaviour.html)
-   [日本語](/ja/current/Manual/class-MonoBehaviour.html)
-   [한국어](/kr/current/Manual/class-MonoBehaviour.html)

-   [Programming in Unity](scripting.html)
-   [Object-oriented development](object-oriented-development.html)
-   [Fundamental Unity types](fundamental-unity-types.html)
-   MonoBehaviour

[](class-Object.html)

Object

[](class-ScriptableObject.html)

ScriptableObject

# MonoBehaviour

[Switch to Scripting](../ScriptReference/MonoBehaviour.html "Go to MonoBehaviour page in the Scripting Reference")

The MonoBehaviour class provides the framework that allows you to attach your script to a **GameObject**The fundamental object in Unity scenes, which can represent characters, props, scenery, cameras, waypoints, and more. A GameObject's functionality is defined by the Components attached to it. [More info](class-GameObject.html)  
See in [Glossary](Glossary.html#GameObject) in the Editor. It also provides hooks into useful Events such as [Start](../ScriptReference/MonoBehaviour.Start.html) and [Update](../ScriptReference/MonoBehaviour.Update.html).

You can create new MonoBehaviour **scripts**A piece of code that allows you to create your own Components, trigger game events, modify Component properties over time and respond to user input in any way you like. [More info](creating-scripts.html)  
See in [Glossary](Glossary.html#Scripts) in the Editor as described in [Create scripts](creating-scripts.html).

For a complete reference of every member of the MonoBehaviour class, and its technical details, refer to the [MonoBehaviour script reference](../ScriptReference/MonoBehaviour.html).

## Content of a MonoBehaviour script file

Double-click a script Asset in Unity to open it in a text editor. By default, Unity uses Visual Studio, but you can select any editor you like from the **External Tools** panel in Unity's preferences (menu: **Unity** > **Preferences**).

If you choose to create a MonoBehaviour script, the initial contents of the file will look something like this:

```
using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour {

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

```

This script makes its connection with the internal workings of Unity by implementing a class which derives from the built-in class called `MonoBehaviour`. You can think of a class as a kind of blueprint for creating a new Component type that can be attached to GameObjects. Each time you attach a script component to a GameObject, it creates a new instance of the object defined by the blueprint. The name of the class is taken from the name you supplied when the file was created. It's best practice to keep the class name and file name in sync, refer to [Naming considerations](naming-scripts.html#script-file-names).

The main things to note, however, are the two functions defined inside the class. The **Update** function is the place to put code that will handle the frame update for the GameObject. This might include movement, triggering actions and responding to user input, basically anything that needs to be handled over time during gameplay. To enable the Update function to do its work, it is often useful to be able to set up variables, read preferences and make connections with other GameObjects before any game action takes place. The **Start** function will be called by Unity before gameplay begins (before the Update function is called for the first time) and is an ideal place to do any initialization.

**Note**: Experienced programmers may be surprised that initialization of an object is not done using a constructor function. This is because the construction of objects is handled by the Unity Editor and does not take place at the start of gameplay as you might expect. If you attempt to define a constructor for a MonoBehaviour, it will interfere with the normal operation of Unity and can cause major problems with the project.

## Controlling a GameObject

A script only defines a blueprint for a Component, so none of its code will be active until an instance of the script is attached to a GameObject. You can attach a script by dragging the script asset to a GameObject in the hierarchy panel or to the **Inspector**A Unity window that displays information about the currently selected GameObject, asset or project settings, allowing you to inspect and edit the values. [More info](UsingTheInspector.html)  
See in [Glossary](Glossary.html#Inspector) of the GameObject that is currently selected. There is also a Scripts submenu on the Component menu which will contain all the scripts available in the project, including those you have created yourself. The script instance looks much like any other Component in the Inspector:

![A script component attached to a GameObject in the Inspector.](../uploads/Main/ScriptInInspector.png)

A script component attached to a GameObject in the Inspector.

Once attached, the script will start working when you press Play and run the game. You can check this by adding the following code in the `Start` function:

```
// Use this for initialization
void Start () 
{
    Debug.Log("Hello world!");
}

```

**Debug.Log** is a simple command that just prints a message to Unity's console output. If you press Play now, you should see the message at the bottom of the main Editor window and in the Console window (menu: **Window** > **General** > **Console**Abbreviation of **game console**  
See in [Glossary](Glossary.html#console)).

## Coroutines

The MonoBehaviour class allows you to start, stop, and manage coroutines.

For more information on coroutines, refer to [Coroutines](Coroutines.html) and the [StartCoroutine method script reference](../ScriptReference/MonoBehaviour.StartCoroutine.html).

## Events

The MonoBehaviour class provides access to a large collection of [event functions](event-functions.html), which allows you to execute your code based on what is currently happening in your project. Here are a few of the more common examples. For a list of them all, see the **Messages** section on the [MonoBehaviour script reference page](../ScriptReference/MonoBehaviour.html)

## Additional resources

-   [Event functions](event-functions.html)
-   [MonoBehaviour API reference](../ScriptReference/MonoBehaviour.html)

[](class-Object.html)

Object

[](class-ScriptableObject.html)

ScriptableObject

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://learn.unity.com/)[Community Answers](https://answers.unity3d.com)[Knowledge Base](https://support.unity3d.com/hc/en-us)[Forums](https://forum.unity3d.com)[Asset Store](https://unity3d.com/asset-store)[Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html)[Legal](https://unity.com/legal)[Privacy Policy](https://unity.com/legal/privacy-policy)[Cookies](https://unity.com/legal/cookie-policy)[Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
