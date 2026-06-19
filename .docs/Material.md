       Unity - Scripting API: Material                   

 

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

# Material

class in UnityEngine

/

Inherits from:[Object](Object.html)

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

[Switch to Manual](../Manual/class-Material.html "Go to Material Component in the Manual")

### Description

The material class.

This class exposes all properties from a material, allowing you to animate them. You can also use it to set custom shader properties that can't be accessed through the inspector (e.g. matrices).  
  
In order to get the material used by an object, use the [Renderer.material](Renderer-material.html) property.  
  
Additional resources: [Materials](../Manual/Materials.html), [Shaders](../Manual/Shaders.html).

### Properties

Property

Description

[color](Material-color.html)

The main color of the Material.

[doubleSidedGI](Material-doubleSidedGI.html)

Gets and sets whether the Double Sided Global Illumination setting is enabled for this material.

[enabledKeywords](Material-enabledKeywords.html)

An array containing the local shader keywords that are currently enabled for this material.

[enableInstancing](Material-enableInstancing.html)

Gets and sets whether GPU instancing is enabled for this material.

[globalIlluminationFlags](Material-globalIlluminationFlags.html)

Defines how the material should interact with lightmaps and lightprobes.

[isVariant](Material-isVariant.html)

Returns true if this material is a material variant.

[mainTexture](Material-mainTexture.html)

The main texture.

[mainTextureOffset](Material-mainTextureOffset.html)

The offset of the main texture.

[mainTextureScale](Material-mainTextureScale.html)

The scale of the main texture.

[parent](Material-parent.html)

Parent of this material.

[passCount](Material-passCount.html)

How many passes are in this material (Read Only).

[rawRenderQueue](Material-rawRenderQueue.html)

Raw render queue of this material.

[renderQueue](Material-renderQueue.html)

Render queue of this material.

[shader](Material-shader.html)

The shader used by the material.

[shaderKeywords](Material-shaderKeywords.html)

An array containing names of the local shader keywords that are currently enabled for this material.

### Constructors

Constructor

Description

[Material](Material-ctor.html)

### Public Methods

Method

Description

[ApplyPropertyOverride](Material.ApplyPropertyOverride.html)

Applies an override associated with a Material Variant to a target.

[ComputeCRC](Material.ComputeCRC.html)

Computes a CRC hash value from the content of the material.

[CopyMatchingPropertiesFromMaterial](Material.CopyMatchingPropertiesFromMaterial.html)

Copies properties, keyword states and settings from mat to this material, but only if they exist in both materials.

[CopyPropertiesFromMaterial](Material.CopyPropertiesFromMaterial.html)

Copy properties from other material into this material.

[DisableKeyword](Material.DisableKeyword.html)

Disables a local shader keyword for this material.

[EnableKeyword](Material.EnableKeyword.html)

Enables a local shader keyword for this material.

[FindPass](Material.FindPass.html)

Returns the index of the pass passName.

[GetBuffer](Material.GetBuffer.html)

Get a named Graphics Buffer value.

[GetColor](Material.GetColor.html)

Get a named color value.

[GetColorArray](Material.GetColorArray.html)

Get a named color array.

[GetConstantBuffer](Material.GetConstantBuffer.html)

Get a named Constant Buffer value.

[GetFloat](Material.GetFloat.html)

Get a named float value.

[GetFloatArray](Material.GetFloatArray.html)

Get a named float array.

[GetInt](Material.GetInt.html)

This method is deprecated. Use GetFloat or GetInteger instead.

[GetInteger](Material.GetInteger.html)

Get a named integer value.

[GetMatrix](Material.GetMatrix.html)

Get a named matrix value from the shader.

[GetMatrixArray](Material.GetMatrixArray.html)

Get a named matrix array.

[GetPassName](Material.GetPassName.html)

Returns the name of the shader pass at index pass.

[GetPropertyNames](Material.GetPropertyNames.html)

Retrieves a list of the named properties in the material that match the input property type.

[GetShaderPassEnabled](Material.GetShaderPassEnabled.html)

Checks whether a given Shader pass is enabled on this Material.

[GetTag](Material.GetTag.html)

Get the value of material's shader tag.

[GetTexture](Material.GetTexture.html)

Get a named texture.

[GetTextureOffset](Material.GetTextureOffset.html)

Gets the placement offset of texture propertyName.

[GetTexturePropertyNameIDs](Material.GetTexturePropertyNameIDs.html)

Return the name IDs of all texture properties exposed on this material.

[GetTexturePropertyNames](Material.GetTexturePropertyNames.html)

Returns the names of all texture properties exposed on this material.

[GetTextureScale](Material.GetTextureScale.html)

Gets the placement scale of texture propertyName.

[GetVector](Material.GetVector.html)

Get a named vector value.

[GetVectorArray](Material.GetVectorArray.html)

Get a named vector array.

[HasBuffer](Material.HasBuffer.html)

Checks if the ShaderLab file assigned to the Material has a ComputeBuffer property with the given name.

[HasColor](Material.HasColor.html)

Checks if the ShaderLab file assigned to the Material has a Color property with the given name.

[HasConstantBuffer](Material.HasConstantBuffer.html)

Checks if the ShaderLab file assigned to the Material has a ConstantBuffer property with the given name.

[HasFloat](Material.HasFloat.html)

Checks if the ShaderLab file assigned to the Material has a Float property with the given name. This also works with the Float Array property.

[HasInt](Material.HasInt.html)

This method is deprecated. Use HasFloat or HasInteger instead.

[HasInteger](Material.HasInteger.html)

Checks if the ShaderLab file assigned to the Material has an Integer property with the given name.

[HasMatrix](Material.HasMatrix.html)

Checks if the ShaderLab file assigned to the Material has a Matrix property with the given name. This also works with the Matrix Array property.

[HasProperty](Material.HasProperty.html)

Checks if the ShaderLab file assigned to the Material has a property with the given name.

[HasTexture](Material.HasTexture.html)

Checks if the ShaderLab file assigned to the Material has a Texture property with the given name.

[HasVector](Material.HasVector.html)

Checks if the ShaderLab file assigned to the Material has a Vector property with the given name. This also works with the Vector Array property.

[IsChildOf](Material.IsChildOf.html)

Returns True if the given material is an ancestor of this Material.

[IsKeywordEnabled](Material.IsKeywordEnabled.html)

Checks whether a local shader keyword is enabled for this material.

[IsPropertyLocked](Material.IsPropertyLocked.html)

Checks whether a property is locked by this material.

[IsPropertyLockedByAncestor](Material.IsPropertyLockedByAncestor.html)

Checks whether a property is locked by any of ancestor of this material.

[IsPropertyOverriden](Material.IsPropertyOverriden.html)

Checks whether a property is overriden by this material.

[Lerp](Material.Lerp.html)

Interpolate properties between two materials.

[RevertAllPropertyOverrides](Material.RevertAllPropertyOverrides.html)

Removes all property overrides on this material.

[RevertPropertyOverride](Material.RevertPropertyOverride.html)

Removes the override on a property.

[SetBuffer](Material.SetBuffer.html)

Sets a named buffer value.

[SetColor](Material.SetColor.html)

Sets the value of a color- or vector-type property.

[SetColorArray](Material.SetColorArray.html)

Sets a color array property.

[SetConstantBuffer](Material.SetConstantBuffer.html)

Sets a ComputeBuffer or GraphicsBuffer as a named constant buffer for the material.

[SetFloat](Material.SetFloat.html)

Sets a named float value.

[SetFloatArray](Material.SetFloatArray.html)

Sets a float array property.

[SetInt](Material.SetInt.html)

This method is deprecated. Use SetFloat or SetInteger instead.

[SetInteger](Material.SetInteger.html)

Sets a named integer value.

[SetKeyword](Material.SetKeyword.html)

Sets the state of a local shader keyword for this material.

[SetMatrix](Material.SetMatrix.html)

Sets a named matrix for the shader.

[SetMatrixArray](Material.SetMatrixArray.html)

Sets a matrix array property.

[SetOverrideTag](Material.SetOverrideTag.html)

Sets an override tag/value on the material.

[SetPass](Material.SetPass.html)

Activate the given pass for rendering.

[SetPropertyLock](Material.SetPropertyLock.html)

Sets the lock state of a property for this material.

[SetShaderPassEnabled](Material.SetShaderPassEnabled.html)

Enables or disables a Shader pass on a per-Material level.

[SetTexture](Material.SetTexture.html)

Sets a named texture.

[SetTextureOffset](Material.SetTextureOffset.html)

Sets the placement offset of a given texture. The name parameter is defined in the shader. This method creates a new Material instance.

[SetTextureScale](Material.SetTextureScale.html)

Sets the placement scale of texture propertyName.

[SetVector](Material.SetVector.html)

Sets a named vector value.

[SetVectorArray](Material.SetVectorArray.html)

Sets a vector array property.

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
