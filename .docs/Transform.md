       Unity - Scripting API: Transform                   

 

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

# Transform

class in UnityEngine

/

Inherits from:[Component](Component.html)

/

Implemented in:[UnityEngine.CoreModule](UnityEngine.CoreModule.html)

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

[Switch to Manual](../Manual/class-Transform.html "Go to Transform Component in the Manual")

### Description

Position, rotation and scale of an object.

Every object in a Scene has a Transform. It's used to store and manipulate the position, rotation and scale of the object. Every Transform can have a parent, which allows you to apply position, rotation and scale hierarchically. This is the hierarchy seen in the Hierarchy pane. They also support enumerators so you can loop through children using:

using UnityEngine;  
  
public class Example : [MonoBehaviour](MonoBehaviour.html)
{
    // Moves all transform children 10 units upwards!
    void Start()
    {
        foreach ([Transform](Transform.html) child in transform)
        {
            child.position += [Vector3.up](Vector3-up.html) \* 10.0f;
        }
    }
}

Additional resources: [The component reference](../Manual/class-Transform.html), [Physics](Physics.html) class.

### Properties

Property

Description

[childCount](Transform-childCount.html)

The number of children the parent Transform has.

[eulerAngles](Transform-eulerAngles.html)

The rotation as Euler angles in degrees.

[forward](Transform-forward.html)

Returns a normalized vector representing the blue axis of the transform in world space.

[hasChanged](Transform-hasChanged.html)

Has the transform changed since the last time the flag was set to 'false'?

[hierarchyCapacity](Transform-hierarchyCapacity.html)

The transform capacity of the transform's hierarchy data structure.

[hierarchyCount](Transform-hierarchyCount.html)

The number of transforms in the transform's hierarchy data structure.

[localEulerAngles](Transform-localEulerAngles.html)

The rotation as Euler angles in degrees relative to the parent transform's rotation.

[localPosition](Transform-localPosition.html)

Position of the transform relative to the parent transform.

[localRotation](Transform-localRotation.html)

The rotation of the transform relative to the transform rotation of the parent.

[localScale](Transform-localScale.html)

The scale of the transform relative to the GameObjects parent.

[localToWorldMatrix](Transform-localToWorldMatrix.html)

Matrix that transforms a point from local space into world space (Read Only).

[lossyScale](Transform-lossyScale.html)

The global scale of the object (Read Only).

[parent](Transform-parent.html)

The parent of the transform.

[position](Transform-position.html)

The world space position of the Transform.

[right](Transform-right.html)

The red axis of the transform in world space.

[root](Transform-root.html)

Returns the topmost transform in the hierarchy.

[rotation](Transform-rotation.html)

A Quaternion that stores the rotation of the Transform in world space.

[up](Transform-up.html)

The green axis of the transform in world space.

[worldToLocalMatrix](Transform-worldToLocalMatrix.html)

Matrix that transforms a point from world space into local space (Read Only).

### Public Methods

Method

Description

[DetachChildren](Transform.DetachChildren.html)

Unparents all children.

[Find](Transform.Find.html)

Finds a child by name n and returns it.

[GetChild](Transform.GetChild.html)

Returns a transform child by index.

[GetLocalPositionAndRotation](Transform.GetLocalPositionAndRotation.html)

Gets the position and rotation of the Transform component in local space (that is, relative to its parent transform).

[GetPositionAndRotation](Transform.GetPositionAndRotation.html)

Gets the position and rotation of the Transform component in world space.

[GetSiblingIndex](Transform.GetSiblingIndex.html)

Gets the sibling index.

[InverseTransformDirection](Transform.InverseTransformDirection.html)

Transforms a direction from world space to local space. The opposite of Transform.TransformDirection.

[InverseTransformDirections](Transform.InverseTransformDirections.html)

Transforms multiple directions from world space to local space overwriting each original position with the transformed version. The opposite of Transform.TransformDirections.

[InverseTransformPoint](Transform.InverseTransformPoint.html)

Transforms position from world space to local space.

[InverseTransformPoints](Transform.InverseTransformPoints.html)

Transforms multiple positions from world space to local space overwriting each original position with the transformed version.

[InverseTransformVector](Transform.InverseTransformVector.html)

Transforms a vector from world space to local space. The opposite of Transform.TransformVector.

[InverseTransformVectors](Transform.InverseTransformVectors.html)

Transforms multiple vectors from world space to local space overwriting each original position with the transformed version. The opposite of Transform.TransformVectors.

[IsChildOf](Transform.IsChildOf.html)

Is this transform a child of parent?

[LookAt](Transform.LookAt.html)

Rotates the transform so the forward vector points at /target/'s current position.

[Rotate](Transform.Rotate.html)

Use Transform.Rotate to rotate GameObjects in a variety of ways. The rotation is often provided as an Euler angle and not a Quaternion.

[RotateAround](Transform.RotateAround.html)

Rotates the transform about axis passing through point in world coordinates by angle degrees.

[SetAsFirstSibling](Transform.SetAsFirstSibling.html)

Move the transform to the start of the local transform list.

[SetAsLastSibling](Transform.SetAsLastSibling.html)

Move the transform to the end of the local transform list.

[SetLocalPositionAndRotation](Transform.SetLocalPositionAndRotation.html)

Sets the position and rotation of the Transform component in local space (i.e. relative to its parent transform).

[SetParent](Transform.SetParent.html)

Set the parent of the transform.

[SetPositionAndRotation](Transform.SetPositionAndRotation.html)

Sets the world space position and rotation of the Transform component.

[SetSiblingIndex](Transform.SetSiblingIndex.html)

Sets the sibling index.

[TransformDirection](Transform.TransformDirection.html)

Transforms direction from local space to world space.

[TransformDirections](Transform.TransformDirections.html)

Transforms multiple directions from local space to world space overwriting each original direction with the transformed version.

[TransformPoint](Transform.TransformPoint.html)

Transforms position from local space to world space.

[TransformPoints](Transform.TransformPoints.html)

Transforms multiple points from local space to world space overwriting each original point with the transformed version.

[TransformVector](Transform.TransformVector.html)

Transforms vector from local space to world space.

[TransformVectors](Transform.TransformVectors.html)

Transforms multiple vectors from local space to world space overwriting each original vector with the transformed version.

[Translate](Transform.Translate.html)

Moves the transform in the direction and distance of translation.

### Inherited Members

### Properties

Property

Description

[gameObject](Component-gameObject.html)

The game object this component is attached to. A component is always attached to a game object.

[tag](Component-tag.html)

The tag of this game object.

[transform](Component-transform.html)

The Transform attached to this GameObject.

[hideFlags](Object-hideFlags.html)

Should the object be hidden, saved with the Scene or modifiable by the user?

[name](Object-name.html)

The name of the object.

### Public Methods

Method

Description

[BroadcastMessage](Component.BroadcastMessage.html)

Calls the method named methodName on every MonoBehaviour in this game object or any of its children.

[CompareTag](Component.CompareTag.html)

Checks the GameObject's tag against the defined tag.

[GetComponent](Component.GetComponent.html)

Gets a reference to a component of type T on the same GameObject as the component specified.

[GetComponentInChildren](Component.GetComponentInChildren.html)

Gets a reference to a component of type T on the same GameObject as the component specified, or any child of the GameObject.

[GetComponentIndex](Component.GetComponentIndex.html)

Gets the index of the component on its parent GameObject.

[GetComponentInParent](Component.GetComponentInParent.html)

Gets a reference to a component of type T on the same GameObject as the component specified, or any parent of the GameObject.

[GetComponents](Component.GetComponents.html)

Gets references to all components of type T on the same GameObject as the component specified.

[GetComponentsInChildren](Component.GetComponentsInChildren.html)

Gets references to all components of type T on the same GameObject as the component specified, and any child of the GameObject.

[GetComponentsInParent](Component.GetComponentsInParent.html)

Gets references to all components of type T on the same GameObject as the component specified, and any parent of the GameObject.

[SendMessage](Component.SendMessage.html)

Calls the method named methodName on every MonoBehaviour in this game object.

[SendMessageUpwards](Component.SendMessageUpwards.html)

Calls the method named methodName on every MonoBehaviour in this game object and on every ancestor of the behaviour.

[TryGetComponent](Component.TryGetComponent.html)

Gets the component of the specified type, if it exists.

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

### Operators

Operator

Description

[bool](Object-operator_Object.html)

Does the object exist?

[operator !=](Object-operator_ne.html)

Compares if two objects refer to a different object.

[operator ==](Object-operator_eq.html)

Compares two object references to see if they refer to the same object.

Is something described here not working as you expect it to? It might be a **Known Issue**. Please check with the Issue Tracker at [issuetracker.unity3d.com](https://issuetracker.unity3d.com).

Copyright ©2005-2026 Unity Technologies. All rights reserved. Built from job ID 70185496. Built on: 2026-06-17.

[Tutorials](https://unity3d.com/learn) [Community Answers](https://answers.unity3d.com) [Knowledge Base](https://support.unity3d.com/hc/en-us) [Forums](https://forum.unity3d.com) [Asset Store](https://unity3d.com/asset-store) [Terms of use](https://docs.unity3d.com/Manual/TermsOfUse.html) [Legal](https://unity.com/legal) [Privacy Policy](https://unity.com/legal/privacy-policy) [Cookies](https://unity.com/legal/cookie-policy) [Do Not Sell or Share My Personal Information](https://unity.com/legal/do-not-sell-my-personal-information)

[Your Privacy Choices (Cookie Settings)](javascript:void\(0\);)
