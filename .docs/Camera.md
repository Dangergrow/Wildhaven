       Unity - Scripting API: Camera                   

 

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

# Camera

class in UnityEngine

/

Inherits from:[Behaviour](Behaviour.html)

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

[Switch to Manual](../Manual/class-Camera.html "Go to Camera Component in the Manual")

### Description

A Camera is a device through which the player views the world.

A screen space point is defined in pixels. The bottom-left of the screen is (0,0); the right-top is ([pixelWidth](Camera-pixelWidth.html),[pixelHeight](Camera-pixelHeight.html)). The z position is in world units from the Camera.  
  
A viewport space point is normalized and relative to the Camera. The bottom-left of the Camera is (0,0); the top-right is (1,1). The z position is in world units from the Camera.  
  
A world space point is defined in global coordinates (for example, [Transform.position](Transform-position.html)).  
  
Additional resources: [camera component](../Manual/class-Camera.html).

### Static Properties

Property

Description

[allCameras](Camera-allCameras.html)

Returns all enabled cameras in the Scene.

[allCamerasCount](Camera-allCamerasCount.html)

The number of cameras in the current Scene.

[current](Camera-current.html)

The camera we are currently rendering with, for low-level render control only (Read Only).

[kMaxAperture](Camera-kMaxAperture.html)

The maximum allowed aperture.

[kMaxBladeCount](Camera-kMaxBladeCount.html)

The maximum blade count for the aperture diaphragm.

[kMinAperture](Camera-kMinAperture.html)

The minimum allowed aperture.

[kMinBladeCount](Camera-kMinBladeCount.html)

The minimum blade count for the aperture diaphragm.

[main](Camera-main.html)

The first enabled Camera component that is tagged "MainCamera" (Read Only).

[onPostRender](Camera-onPostRender.html)

Delegate that you can use to execute custom code after a Camera renders the scene.

[onPreCull](Camera-onPreCull.html)

Delegate that you can use to execute custom code before a Camera culls the scene.

[onPreRender](Camera-onPreRender.html)

Delegate that you can use to execute custom code before a Camera renders the scene.

### Properties

Property

Description

[activeTexture](Camera-activeTexture.html)

Gets the temporary RenderTexture target for this Camera.

[actualRenderingPath](Camera-actualRenderingPath.html)

The rendering path that is currently being used (Read Only).

[allowDynamicResolution](Camera-allowDynamicResolution.html)

Dynamic Resolution Scaling.

[allowHDR](Camera-allowHDR.html)

High dynamic range rendering.

[allowMSAA](Camera-allowMSAA.html)

MSAA rendering.

[anamorphism](Camera-anamorphism.html)

The camera anamorphism. To use this property, enable UsePhysicalProperties.

[aperture](Camera-aperture.html)

The camera aperture. To use this property, enable UsePhysicalProperties.

[areVRStereoViewMatricesWithinSingleCullTolerance](Camera-areVRStereoViewMatricesWithinSingleCullTolerance.html)

Determines whether the stereo view matrices are suitable to allow for a single pass cull.

[aspect](Camera-aspect.html)

The aspect ratio (width divided by height).

[backgroundColor](Camera-backgroundColor.html)

The color with which the screen will be cleared.

[barrelClipping](Camera-barrelClipping.html)

The camera barrel clipping. To use this property, enable UsePhysicalProperties.

[bladeCount](Camera-bladeCount.html)

The blade count in the lens of the camera. To use this property, enable UsePhysicalProperties.

[cameraToWorldMatrix](Camera-cameraToWorldMatrix.html)

Matrix that transforms from camera space to world space (Read Only).

[cameraType](Camera-cameraType.html)

Identifies what kind of camera this is, using the CameraType enum.

[clearFlags](Camera-clearFlags.html)

How the camera clears the background.

[clearStencilAfterLightingPass](Camera-clearStencilAfterLightingPass.html)

Should the camera clear the stencil buffer after the deferred light pass?

[commandBufferCount](Camera-commandBufferCount.html)

Number of command buffers set up on this camera (Read Only).

[cullingMask](Camera-cullingMask.html)

This is used to render parts of the Scene selectively.

[cullingMatrix](Camera-cullingMatrix.html)

Sets a custom matrix for the camera to use for all culling queries.

[curvature](Camera-curvature.html)

The curvature of the blades. To use this property, enable UsePhysicalProperties.

[depth](Camera-depth.html)

Camera's depth in the camera rendering order.

[depthTextureMode](Camera-depthTextureMode.html)

How and if camera generates a depth texture.

[eventMask](Camera-eventMask.html)

Mask to select which layers can trigger events on the camera.

[farClipPlane](Camera-farClipPlane.html)

The distance of the far clipping plane from the Camera, in world units.

[fieldOfView](Camera-fieldOfView.html)

The vertical field of view of the Camera, in degrees.

[focalLength](Camera-focalLength.html)

The camera focal length, expressed in millimeters. To use this property, enable UsePhysicalProperties.

[focusDistance](Camera-focusDistance.html)

The focus distance of the lens. To use this property, enable UsePhysicalProperties.

[forceIntoRenderTexture](Camera-forceIntoRenderTexture.html)

Should camera rendering be forced into a RenderTexture.

[gateFit](Camera-gateFit.html)

There are two gates for a camera, the sensor gate and the resolution gate. The physical camera sensor gate is defined by the sensorSize property, the resolution gate is defined by the render target area.

[iso](Camera-iso.html)

The sensor sensitivity of the camera. To use this property, enable UsePhysicalProperties.

[layerCullDistances](Camera-layerCullDistances.html)

Per-layer culling distances.

[layerCullSpherical](Camera-layerCullSpherical.html)

How to perform per-layer culling for a Camera.

[lensShift](Camera-lensShift.html)

The lens offset of the camera. The lens shift is relative to the sensor size. For example, a lens shift of 0.5 offsets the sensor by half its horizontal size.

[nearClipPlane](Camera-nearClipPlane.html)

The distance of the near clipping plane from the Camera, in world units.

[nonJitteredProjectionMatrix](Camera-nonJitteredProjectionMatrix.html)

Get or set the raw projection matrix with no camera offset (no jittering).

[opaqueSortMode](Camera-opaqueSortMode.html)

Opaque object sorting mode.

[orthographic](Camera-orthographic.html)

Is the camera orthographic (true) or perspective (false)?

[orthographicSize](Camera-orthographicSize.html)

Camera's half-size when in orthographic mode.

[overrideSceneCullingMask](Camera-overrideSceneCullingMask.html)

Sets the culling mask used to determine which objects from which Scenes to draw. See EditorSceneManager.SetSceneCullingMask.

[pixelHeight](Camera-pixelHeight.html)

How tall is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).

[pixelRect](Camera-pixelRect.html)

Where on the screen is the camera rendered in pixel coordinates.

[pixelWidth](Camera-pixelWidth.html)

How wide is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).

[previousViewProjectionMatrix](Camera-previousViewProjectionMatrix.html)

Get the view projection matrix used on the last frame.

[projectionMatrix](Camera-projectionMatrix.html)

Set a custom projection matrix.

[rect](Camera-rect.html)

Where on the screen is the camera rendered in normalized coordinates.

[renderCloudsInSceneView](Camera-renderCloudsInSceneView.html)

If false, clouds are not rendered in the scene view of this camera.

[renderingPath](Camera-renderingPath.html)

The rendering path that should be used, if possible.

[scaledPixelHeight](Camera-scaledPixelHeight.html)

How tall is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).

[scaledPixelWidth](Camera-scaledPixelWidth.html)

How wide is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).

[scene](Camera-scene.html)

If not null, the camera will only render the contents of the specified Scene.

[sensorSize](Camera-sensorSize.html)

The size of the camera sensor, expressed in millimeters.

[shutterSpeed](Camera-shutterSpeed.html)

The exposure time of the camera, in seconts. To use this property, enable UsePhysicalProperties.

[stereoActiveEye](Camera-stereoActiveEye.html)

Returns the eye that is currently rendering. If called when stereo is not enabled it will return Camera.MonoOrStereoscopicEye.Mono. If called during a camera rendering callback such as OnRenderImage it will return the currently rendering eye. If called outside of a rendering callback and stereo is enabled, it will return the default eye which is Camera.MonoOrStereoscopicEye.Left.

[stereoConvergence](Camera-stereoConvergence.html)

Distance to a point where virtual eyes converge.

[stereoEnabled](Camera-stereoEnabled.html)

Stereoscopic rendering.

[stereoSeparation](Camera-stereoSeparation.html)

The distance between the virtual eyes. Use this to query or set the current eye separation. Note that most VR devices provide this value, in which case setting the value will have no effect.

[stereoTargetEye](Camera-stereoTargetEye.html)

Defines which eye of a VR display the Camera renders into.

[targetDisplay](Camera-targetDisplay.html)

Set the target display for this Camera.

[targetTexture](Camera-targetTexture.html)

Destination render texture.

[transparencySortAxis](Camera-transparencySortAxis.html)

An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.

[transparencySortMode](Camera-transparencySortMode.html)

Transparent object sorting mode.

[useJitteredProjectionMatrixForTransparentRendering](Camera-useJitteredProjectionMatrixForTransparentRendering.html)

Should the jittered matrix be used for transparency rendering?

[useOcclusionCulling](Camera-useOcclusionCulling.html)

Whether or not the Camera will use occlusion culling during rendering.

[usePhysicalProperties](Camera-usePhysicalProperties.html)

Enable usePhysicalProperties to use physical camera properties to compute the field of view and the frustum.

[velocity](Camera-velocity.html)

Get the world-space speed of the camera (Read Only).

[worldToCameraMatrix](Camera-worldToCameraMatrix.html)

Matrix that transforms from world to camera space.

### Public Methods

Method

Description

[AddCommandBuffer](Camera.AddCommandBuffer.html)

Add a command buffer to be executed at a specified place.

[AddCommandBufferAsync](Camera.AddCommandBufferAsync.html)

Adds a command buffer to the GPU's async compute queues and executes that command buffer when graphics processing reaches a given point.

[CalculateFrustumCorners](Camera.CalculateFrustumCorners.html)

Given viewport coordinates, calculates the view space vectors pointing to the four frustum corners at the specified camera depth.

[CalculateObliqueMatrix](Camera.CalculateObliqueMatrix.html)

Calculates and returns oblique near-plane projection matrix.

[CopyFrom](Camera.CopyFrom.html)

Makes this camera's settings match other camera.

[CopyStereoDeviceProjectionMatrixToNonJittered](Camera.CopyStereoDeviceProjectionMatrixToNonJittered.html)

Sets the non-jittered projection matrix, sourced from the VR SDK.

[GetCommandBuffers](Camera.GetCommandBuffers.html)

Get command buffers to be executed at a specified place. This API is only available with the Built-in renderer.

[GetGateFittedFieldOfView](Camera.GetGateFittedFieldOfView.html)

Retrieves the effective vertical field of view of the camera, including GateFit. Fitting the sensor gate and the resolution gate has an impact on the final field of view. If the sensor gate aspect ratio is the same as the resolution gate aspect ratio or if the camera is not in physical mode, then this method returns the same value as the fieldofview property.

[GetGateFittedLensShift](Camera.GetGateFittedLensShift.html)

Retrieves the effective lens offset of the camera, including GateFit. Fitting the sensor gate and the resolution gate has an impact on the final obliqueness of the projection. If the sensor gate aspect ratio is the same as the resolution gate aspect ratio, then this method returns the same value as the lenshift property. If the camera is not in physical mode, then this methods returns Vector2.zero.

[GetStereoNonJitteredProjectionMatrix](Camera.GetStereoNonJitteredProjectionMatrix.html)

Gets the non-jittered projection matrix of a specific left or right stereoscopic eye.

[GetStereoProjectionMatrix](Camera.GetStereoProjectionMatrix.html)

Gets the projection matrix of a specific left or right stereoscopic eye.

[GetStereoViewMatrix](Camera.GetStereoViewMatrix.html)

Gets the left or right view matrix of a specific stereoscopic eye.

[RemoveAllCommandBuffers](Camera.RemoveAllCommandBuffers.html)

Remove all command buffers set on this camera.

[RemoveCommandBuffer](Camera.RemoveCommandBuffer.html)

Remove command buffer from execution at a specified place.

[RemoveCommandBuffers](Camera.RemoveCommandBuffers.html)

Remove command buffers from execution at a specified place.

[Render](Camera.Render.html)

Render the camera manually.

[RenderToCubemap](Camera.RenderToCubemap.html)

Render into a static cubemap from this camera.

[RenderWithShader](Camera.RenderWithShader.html)

Render the camera with shader replacement.

[Reset](Camera.Reset.html)

Revert all camera parameters to default.

[ResetAspect](Camera.ResetAspect.html)

Revert the aspect ratio to the screen's aspect ratio.

[ResetCullingMatrix](Camera.ResetCullingMatrix.html)

Make culling queries reflect the camera's built in parameters.

[ResetProjectionMatrix](Camera.ResetProjectionMatrix.html)

Make the projection reflect normal camera's parameters.

[ResetReplacementShader](Camera.ResetReplacementShader.html)

Remove shader replacement from camera.

[ResetStereoProjectionMatrices](Camera.ResetStereoProjectionMatrices.html)

Reset the camera to using the Unity computed projection matrices for all stereoscopic eyes.

[ResetStereoViewMatrices](Camera.ResetStereoViewMatrices.html)

Reset the camera to using the Unity computed view matrices for all stereoscopic eyes.

[ResetTransparencySortSettings](Camera.ResetTransparencySortSettings.html)

Resets this Camera's transparency sort settings to the default. Default transparency settings are taken from GraphicsSettings instead of directly from this Camera.

[ResetWorldToCameraMatrix](Camera.ResetWorldToCameraMatrix.html)

Make the rendering position reflect the camera's position in the Scene.

[ScreenPointToRay](Camera.ScreenPointToRay.html)

Returns a ray going from camera through a screen point.

[ScreenToViewportPoint](Camera.ScreenToViewportPoint.html)

Transforms position from screen space into viewport space.

[ScreenToWorldPoint](Camera.ScreenToWorldPoint.html)

Transforms a point from screen space into world space, where world space is defined as the coordinate system at the very top of your game's hierarchy.

[SetReplacementShader](Camera.SetReplacementShader.html)

Make the camera render with shader replacement.

[SetStereoProjectionMatrix](Camera.SetStereoProjectionMatrix.html)

Sets a custom projection matrix for a specific stereoscopic eye.

[SetStereoViewMatrix](Camera.SetStereoViewMatrix.html)

Sets a custom view matrix for a specific stereoscopic eye.

[SetTargetBuffers](Camera.SetTargetBuffers.html)

Sets the Camera to render to the chosen buffers of one or more RenderTextures.

[SubmitRenderRequest](Camera.SubmitRenderRequest.html)

Submit a renderRequest.

[TryGetCullingParameters](Camera.TryGetCullingParameters.html)

Get culling parameters for a camera.

[ViewportPointToRay](Camera.ViewportPointToRay.html)

Returns a ray going from camera through a viewport point.

[ViewportToScreenPoint](Camera.ViewportToScreenPoint.html)

Transforms position from viewport space into screen space.

[ViewportToWorldPoint](Camera.ViewportToWorldPoint.html)

Transforms position from viewport space into world space.

[WorldToScreenPoint](Camera.WorldToScreenPoint.html)

Transforms position from world space into screen space.

[WorldToViewportPoint](Camera.WorldToViewportPoint.html)

Transforms position from world space into viewport space.

### Static Methods

Method

Description

[CalculateProjectionMatrixFromPhysicalProperties](Camera.CalculateProjectionMatrixFromPhysicalProperties.html)

Calculates the projection matrix from focal length, sensor size, lens shift, near plane distance, far plane distance, and Gate fit parameters. To calculate the projection matrix without taking Gate fit into account, use Camera.GateFitMode.None . Additional resources: GateFitParameters

[FieldOfViewToFocalLength](Camera.FieldOfViewToFocalLength.html)

Converts field of view to focal length. Use either sensor height and vertical field of view or sensor width and horizontal field of view.

[FocalLengthToFieldOfView](Camera.FocalLengthToFieldOfView.html)

Converts focal length to field of view.

[GetAllCameras](Camera.GetAllCameras.html)

Fills an array of Camera with the current cameras in the Scene, without allocating a new array.

[HorizontalToVerticalFieldOfView](Camera.HorizontalToVerticalFieldOfView.html)

Converts the horizontal field of view (FOV) to the vertical FOV, based on the value of the aspect ratio parameter.

[VerticalToHorizontalFieldOfView](Camera.VerticalToHorizontalFieldOfView.html)

Converts the vertical field of view (FOV) to the horizontal FOV, based on the value of the aspect ratio parameter.

### Messages

Message

Description

[OnPostRender](Camera.OnPostRender.html)

Event function that Unity calls after a Camera renders the scene.

[OnPreCull](Camera.OnPreCull.html)

Event function that Unity calls before a Camera culls the scene.

[OnPreRender](Camera.OnPreRender.html)

Event function that Unity calls before a Camera renders the scene.

[OnRenderImage](Camera.OnRenderImage.html)

Event function that Unity calls after a Camera has finished rendering, that allows you to modify the Camera's final image.

[OnRenderObject](Camera.OnRenderObject.html)

OnRenderObject is called after camera has rendered the Scene.

[OnWillRenderObject](Camera.OnWillRenderObject.html)

OnWillRenderObject is called for each camera if the object is visible.

### Delegates

Delegate

Description

[CameraCallback](Camera.CameraCallback.html)

Delegate type for camera callbacks.

### Inherited Members

### Properties

Property

Description

[enabled](Behaviour-enabled.html)

Enabled Behaviours are Updated, disabled Behaviours are not.

[isActiveAndEnabled](Behaviour-isActiveAndEnabled.html)

Checks whether a component is enabled, attached to a GameObject that is active in the hierarchy, and the component's OnEnable has been called.

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
