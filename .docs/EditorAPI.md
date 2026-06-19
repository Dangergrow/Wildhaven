       Unity - Scripting API: Editor                   

 

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

# Editor

class in UnityEditor

/

Inherits from:[ScriptableObject](ScriptableObject.html)

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

### Description

Derive from this base class to create a custom inspector or editor for your custom object.

using UnityEngine;
using System.Collections;  
  
// This is not an editor script.
public class MyPlayer : [MonoBehaviour](MonoBehaviour.html)
{
    public int armor = 75;
    public int damage = 25;
    public [GameObject](GameObject.html) gun;  
  
    void [Update](PlayerLoop.Update.html)()
    {
        // [Update](PlayerLoop.Update.html) logic here...
    }
}

For example, use a custom editor to change the appearance of the script in the Inspector.  
  
You can attach the Editor to a custom component by using the [CustomEditor](CustomEditor.html) attribute.  
  
There are multiple ways to design custom Editors. If you want the Editor to support multi-object editing, you can use the [CanEditMultipleObjects](CanEditMultipleObjects.html) attribute. Instead of modifying script variables directly, it's advantageous to use the [SerializedObject](SerializedObject.html) and [SerializedProperty](SerializedProperty.html) system to edit them, since this automatically handles multi-object editing, undo, and Prefab overrides. If this approach is used a user can select multiple assets in the hierarchy window and change the values for all of them at once.  
  
You can either use UIElements to build your custom UI or you can use IMGUI. To create a custom inspector using UIElements, you have to override the [Editor.CreateInspectorGUI](Editor.CreateInspectorGUI.html) on the [Editor](Editor.html) class. To create a custom inspector using IMGUI, you have to override the [Editor.OnInspectorGUI](Editor.OnInspectorGUI.html) on the [Editor](Editor.html) class. If you use UIElements and have [Editor.CreateInspectorGUI](Editor.CreateInspectorGUI.html) overwritten, any existing IMGUI implementation using [Editor.OnInspectorGUI](Editor.OnInspectorGUI.html) on the same Editor will be ignored.  
  
Here's an example of a custom inspector:  
  
![](../StaticFiles/ScriptRefImages/CustomEditorUIElements.png)  
*Custom editor in the Inspector.*

using [UnityEditor](UnityEditor.html);
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
\[[CustomEditor](CustomEditor.html)(typeof(MyPlayer))\]
public class MyPlayerEditor : [Editor](Editor.html)
{
    const string resourceFilename = "custom-editor-uie";
    public override [VisualElement](UIElements.VisualElement.html) CreateInspectorGUI()
    {
        [VisualElement](UIElements.VisualElement.html) customInspector = new [VisualElement](UIElements.VisualElement.html)();
        var visualTree = [Resources.Load](Resources.Load.html)(resourceFilename) as [VisualTreeAsset](UIElements.VisualTreeAsset.html);
        visualTree.CloneTree(customInspector);
        customInspector.styleSheets.Add([Resources.Load](Resources.Load.html)($"{resourceFilename}-style") as [StyleSheet](UIElements.StyleSheet.html));
        return customInspector;
    }
}

The following example defines the layout of a custom inspector in uxml. The definition loads as a resource and the [VisualTreeAsset.CloneTree](UIElements.VisualTreeAsset.CloneTree.html) method puts the hierarchy in a [VisualElement](UIElements.VisualElement.html) object.  
  
The InspectorWindow will instantiate an [InspectorElement](UIElements.InspectorElement.html) containing the custom inspector. The [InspectorElement](UIElements.InspectorElement.html) will call Bind on the custom inspector binding it to the MyPlayer object.

<UXML xmlns="UnityEngine.UIElements" xmlns:e="UnityEditor.UIElements">
    <[VisualElement](UIElements.VisualElement.html) class="player-property">
        <[VisualElement](UIElements.VisualElement.html) class="slider-row">
            <[Label](UIElements.Label.html) class="player-property-label" text="Damage"/>
            <[VisualElement](UIElements.VisualElement.html) class="input-container">
                <[SliderInt](UIElements.SliderInt.html) class="player-slider" name="damage-slider" high-value="100" direction="Horizontal" binding-path="damage"/>
                <e:[IntegerField](UIElements.IntegerField.html) class="player-int-field" binding-path="damage"/>
            </[VisualElement](UIElements.VisualElement.html)\>
        </[VisualElement](UIElements.VisualElement.html)\>
        <e:[ProgressBar](UIElements.ProgressBar.html) class="player-property-progress-bar" name="damage-progress" binding-path="damage" title="Damage"/>
    </[VisualElement](UIElements.VisualElement.html)\>  
  
    <[VisualElement](UIElements.VisualElement.html) class="player-property">
        <[VisualElement](UIElements.VisualElement.html) class="slider-row">
            <[Label](UIElements.Label.html) class="player-property-label" text="Armor"/>
            <[VisualElement](UIElements.VisualElement.html) class="input-container">
                <[SliderInt](UIElements.SliderInt.html) class="player-slider" name="armor-slider" high-value="100" direction="Horizontal" binding-path="armor"/>
                <e:[IntegerField](UIElements.IntegerField.html) class="player-int-field" binding-path="armor"/>
            </[VisualElement](UIElements.VisualElement.html)\>
        </[VisualElement](UIElements.VisualElement.html)\>
        <e:[ProgressBar](UIElements.ProgressBar.html) class="player-property-progress-bar" name="armor-progress" binding-path="armor" title="Armor"/>
    </[VisualElement](UIElements.VisualElement.html)\>  
  
    <e:[PropertyField](UIElements.PropertyField.html) class="gun-field" binding-path="gun" label="Gun Object"/>
</UXML>

UIElements automatically updates the UI when data changes and vice-versa. To bind data and automatically update data and UI, set values for the "binding-path" attributes.  
  
Styling of the inspector is done in uss.

.slider-row {
    flex-direction: row;
    justify-content: space-between;
    margin-top: 4px;
}
.input-container {
    flex-direction: row;
    flex-grow: .6;
    margin-right: 4px;
}
.player-property {
    margin-bottom: 4px;
}
.player-property-label {
    flex:1;
    margin-left: 16;
}
.player-slider {
    flex:3;
    margin-right: 4px;
}
.player-property-progress-bar {
    margin-left: 16px;
    margin-right: 4px;
}
.player-int-field {
    min-width: 48px;
}
.gun-field {
    justify-content: space-between;
    margin-left: 16px;
    margin-right: 4px;
    margin-top: 6px;
    flex-grow: .6;
}

Here's an example of a custom inspector using IMGUI and multi-selection:

using [UnityEditor](UnityEditor.html);
using UnityEngine;
using System.Collections;  
  
// Custom [Editor](Editor.html) using SerializedProperties.
// [Automatic](Android.AndroidGame.Automatic.html) handling of multi-object editing, undo, and Prefab overrides.
\[[CustomEditor](CustomEditor.html)(typeof(MyPlayer))\]
\[[CanEditMultipleObjects](CanEditMultipleObjects.html)\]
public class MyPlayerEditor : [Editor](Editor.html)
{
    [SerializedProperty](SerializedProperty.html) damageProp;
    [SerializedProperty](SerializedProperty.html) armorProp;
    [SerializedProperty](SerializedProperty.html) gunProp;  
  
    void OnEnable()
    {
        // Setup the SerializedProperties.
        damageProp = serializedObject.FindProperty ("damage");
        armorProp = serializedObject.FindProperty ("armor");
        gunProp = serializedObject.FindProperty ("gun");
    }  
  
    public override void OnInspectorGUI()
    {
        // [Update](PlayerLoop.Update.html) the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update ();  
  
        // Show the custom [GUI](GUI.html) controls.
        [EditorGUILayout.IntSlider](EditorGUILayout.IntSlider.html) (damageProp, 0, 100, new [GUIContent](GUIContent.html) ("Damage"));  
  
        // Only show the damage progress bar if all the objects have the same damage value:
        if (!damageProp.hasMultipleDifferentValues)
            [ProgressBar](UIElements.ProgressBar.html) (damageProp.intValue / 100.0f, "Damage");  
  
        [EditorGUILayout.IntSlider](EditorGUILayout.IntSlider.html) (armorProp, 0, 100, new [GUIContent](GUIContent.html) ("Armor"));  
  
        // Only show the armor progress bar if all the objects have the same armor value:
        if (!armorProp.hasMultipleDifferentValues)
            [ProgressBar](UIElements.ProgressBar.html) (armorProp.intValue / 100.0f, "Armor");  
  
        [EditorGUILayout.PropertyField](EditorGUILayout.PropertyField.html) (gunProp, new [GUIContent](GUIContent.html) ("Gun Object"));  
  
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties ();
    }  
  
    // Custom [GUILayout](GUILayout.html) progress bar.
    void [ProgressBar](UIElements.ProgressBar.html) (float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        [Rect](Rect.html) rect = [GUILayoutUtility.GetRect](GUILayoutUtility.GetRect.html) (18, 18, "[TextField](UIElements.TextField.html)");
        [EditorGUI.ProgressBar](EditorGUI.ProgressBar.html) (rect, value, label);
        [EditorGUILayout.Space](EditorGUILayout.Space.html) ();
    }
}

If automatic handling of multi-object editing, undo, and Prefab overrides is not needed, the script variables can be modified directly by the editor without using the [SerializedObject](SerializedObject.html) and [SerializedProperty](SerializedProperty.html) system, as in the IMGUI example below.

using [UnityEditor](UnityEditor.html);
using UnityEngine;
using System.Collections;  
  
// Example script with properties.
public class MyPlayerAlternative : [MonoBehaviour](MonoBehaviour.html)
{
    public int damage;
    public int armor;
    public [GameObject](GameObject.html) gun;  
  
    // ...other code...
}  
  
// Custom [Editor](Editor.html) the "old" way by modifying the script variables directly.
// No handling of multi-object editing, undo, and Prefab overrides!
\[[CustomEditor](CustomEditor.html) (typeof(MyPlayerAlternative))\]
public class MyPlayerEditorAlternative : [Editor](Editor.html)
{  
  
    public override void OnInspectorGUI()
    {
        MyPlayerAlternative mp = (MyPlayerAlternative)target;  
  
        mp.damage = [EditorGUILayout.IntSlider](EditorGUILayout.IntSlider.html) ("Damage", mp.damage, 0, 100);
        [ProgressBar](UIElements.ProgressBar.html) (mp.damage / 100.0f, "Damage");  
  
        mp.armor = [EditorGUILayout.IntSlider](EditorGUILayout.IntSlider.html) ("Armor", mp.armor, 0, 100);
        [ProgressBar](UIElements.ProgressBar.html) (mp.armor / 100.0f, "Armor");  
  
        bool allowSceneObjects = ![EditorUtility.IsPersistent](EditorUtility.IsPersistent.html) (target);
        mp.gun = ([GameObject](GameObject.html))[EditorGUILayout.ObjectField](EditorGUILayout.ObjectField.html) ("Gun Object", mp.gun, typeof([GameObject](GameObject.html)), allowSceneObjects);
    }  
  
    // Custom [GUILayout](GUILayout.html) progress bar.
    void [ProgressBar](UIElements.ProgressBar.html) (float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        [Rect](Rect.html) rect = [GUILayoutUtility.GetRect](GUILayoutUtility.GetRect.html) (18, 18, "[TextField](UIElements.TextField.html)");
        [EditorGUI.ProgressBar](EditorGUI.ProgressBar.html) (rect, value, label);
        [EditorGUILayout.Space](EditorGUILayout.Space.html) ();
    }
}

### Properties

Property

Description

[hasUnsavedChanges](Editor-hasUnsavedChanges.html)

This property specifies whether the Editor prompts the user to save or discard unsaved changes before the Inspector gets rebuilt.

[saveChangesMessage](Editor-saveChangesMessage.html)

The message that displays to the user if they are prompted to save.

[serializedObject](Editor-serializedObject.html)

A SerializedObject representing the object or objects being inspected.

[target](Editor-target.html)

The object being inspected.

[targets](Editor-targets.html)

An array of all the object being inspected.

### Public Methods

Method

Description

[CreateInspectorGUI](Editor.CreateInspectorGUI.html)

Implement this method to make a custom UIElements inspector.

[CreatePreview](Editor.CreatePreview.html)

Implement this method to make a custom UIElements inspector preview.

[DiscardChanges](Editor.DiscardChanges.html)

Discards unsaved changes to the contents of the editor.

[DrawDefaultInspector](Editor.DrawDefaultInspector.html)

Draws the built-in Inspector.

[DrawHeader](Editor.DrawHeader.html)

Call this function to draw the header of the editor.

[DrawPreview](Editor.DrawPreview.html)

The first entry point for Preview Drawing.

[GetInfoString](Editor.GetInfoString.html)

Implement this method to show asset information on top of the asset preview.

[GetPreviewTitle](Editor.GetPreviewTitle.html)

Override this method if you want to change the label of the Preview area.

[HasPreviewGUI](Editor.HasPreviewGUI.html)

Override this method in subclasses if you implement OnPreviewGUI.

[OnInspectorGUI](Editor.OnInspectorGUI.html)

Implement this function to make a custom inspector.

[OnInteractivePreviewGUI](Editor.OnInteractivePreviewGUI.html)

Implement to create your own interactive custom preview. Interactive custom previews are used in the preview area of the inspector and the object selector.

[OnPreviewGUI](Editor.OnPreviewGUI.html)

Creates a custom preview for the preview area of the Inspector, the headers of the primary Editor, and the object selector.You must implement Editor.HasPreviewGUI for this method to be called.

[OnPreviewSettings](Editor.OnPreviewSettings.html)

Override this method if you want to show custom controls in the preview header.

[RenderStaticPreview](Editor.RenderStaticPreview.html)

Override this method if you want to render a static preview.

[Repaint](Editor.Repaint.html)

Redraw any inspectors that shows this editor.

[RequiresConstantRepaint](Editor.RequiresConstantRepaint.html)

Checks if this editor requires constant repaints in its current state.

[SaveChanges](Editor.SaveChanges.html)

Performs a save action on the contents of the editor.

[UseDefaultMargins](Editor.UseDefaultMargins.html)

Override this method in subclasses to return false if you don't want default margins.

### Protected Methods

Method

Description

[ShouldHideOpenButton](Editor.ShouldHideOpenButton.html)

Returns the visibility setting of the "open" button in the Inspector.

### Static Methods

Method

Description

[CreateCachedEditor](Editor.CreateCachedEditor.html)

On return previousEditor is an editor for targetObject or targetObjects. The function either returns if the editor is already tracking the objects, or destroys the previous editor and creates a new one.

[CreateCachedEditorWithContext](Editor.CreateCachedEditorWithContext.html)

Creates a cached editor using a context object.

[CreateEditor](Editor.CreateEditor.html)

Make a custom editor for targetObject or targetObjects.

[CreateEditorWithContext](Editor.CreateEditorWithContext.html)

Make a custom editor for targetObject or targetObjects with a context object.

[DrawFoldoutInspector](Editor.DrawFoldoutInspector.html)

Draws the inspector GUI with a foldout header for target.

### Messages

Message

Description

[HasFrameBounds](Editor.HasFrameBounds.html)

Validates whether custom bounds can be calculated for this Editor.

[OnGetFrameBounds](Editor.OnGetFrameBounds.html)

Gets custom bounds for the target of this editor.

[OnSceneGUI](Editor.OnSceneGUI.html)

Enables the Editor to handle an event in the Scene view.

### Events

Event

Description

[finishedDefaultHeaderGUI](Editor-finishedDefaultHeaderGUI.html)

An event raised while drawing the header of the Inspector window, after the default header items have been drawn.

### Inherited Members

### Properties

Property

Description

[hideFlags](Object-hideFlags.html)

Should the object be hidden, saved with the Scene or modifiable by the user?

[name](Object-name.html)

The name of the object.

### Public Methods

Method

Description

[GetInstanceID](Object.GetInstanceID.html)

Gets the instance ID of the object.

[ToString](Object.ToString.html)

Returns the name of the object.

### Static Methods

Method

Description

[Destroy](Object.Destroy.html)

Removes a GameObject, component, or asset.

[DestroyImmediate](Object.DestroyImmediate.html)

Destroys the specified object immediately. Use with caution and in Edit mode only.

[DontDestroyOnLoad](Object.DontDestroyOnLoad.html)

Do not destroy the target Object when loading a new Scene.

[FindAnyObjectByType](Object.FindAnyObjectByType.html)

Retrieves any active loaded object of Type type.

[FindFirstObjectByType](Object.FindFirstObjectByType.html)

Retrieves the first active loaded object of Type type.

[FindObjectsByType](Object.FindObjectsByType.html)

Retrieves a list of all loaded objects of Type type.

[Instantiate](Object.Instantiate.html)

Clones the object original and returns the clone.

[InstantiateAsync](Object.InstantiateAsync.html)

Captures a snapshot of the original object (that must be related to some GameObject) and returns the AsyncInstantiateOperation.

[CreateInstance](ScriptableObject.CreateInstance.html)

Creates an instance of a scriptable object.

### Operators

Operator

Description

[bool](Object-operator_Object.html)

Does the object exist?

[operator !=](Object-operator_ne.html)

Compares if two objects refer to a different object.

[operator ==](Object-operator_eq.html)

Compares two object references to see if they refer to the same object.

### Messages

Message

Description

[Awake](ScriptableObject.Awake.html)

Called when an instance of ScriptableObject is created.

[OnDestroy](ScriptableObject.OnDestroy.html)

This function is called when the scriptable object will be destroyed.

[OnDisable](ScriptableObject.OnDisable.html)

This function is called when the scriptable object goes out of scope.

[OnEnable](ScriptableObject.OnEnable.html)

This function is called when the object is loaded.

[OnValidate](ScriptableObject.OnValidate.html)

Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.

[Reset](ScriptableObject.Reset.html)

Reset to default values.

Is something described here not working as you expect it to? It might be a **Known Issue**. Please check with the Issue Tracker at [issuetracker.unity3d.com](https://issuetracker.unity3d.com).

Copyright (C)2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://unity3d.com/learn) [Community Answers](https://answers.unity3d.com) [Knowledge Base](https://support.unity3d.com/hc/en-us) [Forums](https://forum.unity3d.com) [Asset Store](https://unity3d.com/asset-store) [Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html) [Legal](https://unity.com/legal) [Privacy Policy](https://unity.com/legal/privacy-policy) [Cookies](https://unity.com/legal/cookie-policy) [Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void(0);)
