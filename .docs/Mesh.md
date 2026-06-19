       Unity - Scripting API: Mesh                   

 

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

# Mesh

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

[Switch to Manual](../Manual/class-Mesh.html "Go to Mesh Component in the Manual")

### Description

A class that allows you to create or modify meshes.

Meshes contain vertices and multiple triangle arrays.  
  
Conceptually, all vertex data is stored in separate arrays of the same size. For example, if you have a mesh of 100 Vertices, and want to have a position, normal and two texture coordinates for each vertex, then the mesh should have [vertices](Mesh-vertices.html), [normals](Mesh-normals.html), [uv](Mesh-uv.html) and [uv2](Mesh-uv2.html) arrays, each being 100 in size. Data for i-th vertex is at index "i" in each array.  
  
For every vertex there can be a vertex position, normal, tangent, color and up to 8 texture coordinates. Texture coordinates most often are 2D data ([Vector2](Vector2.html)), but it is possible to make them [Vector3](Vector3.html) or [Vector4](Vector4.html) if needed. This is most often used for holding arbitrary data in mesh vertices, for special effects used in shaders. For skinned meshes, the vertex data can also contain [boneWeights](Mesh-boneWeights.html).  
  
The mesh face data, i.e. the triangles it is made of, is simply three vertex indices for each triangle. For example, if the mesh has 10 triangles, then the [triangles](Mesh-triangles.html) array should be 30 numbers, with each number indicating which vertex to use. The first three elements in the triangles array are the indices for the vertices that make up that triangle; the second three elements make up another triangle and so on.  
  
Note that while triangle meshes are the most common use case, Unity also supports other mesh topology types, for example Line or Point meshes. For line meshes, each line is composed of two vertex indices and so on. See [SetIndices](Mesh.SetIndices.html) and [MeshTopology](MeshTopology.html).  
  
**Simple vs Advanced Mesh API**  
  
The Mesh class has two sets of methods for assigning data to a Mesh from script. The "simple" set of methods provide a basis for setting the indices, triangle, normals, tangents, etc. These methods include validation checks, for example to ensure that you are not passing in data that would include out-of-bounds indices. They represent the standard way to assign Mesh data from script in Unity.  
  
The "simple" methods are: [SetColors](Mesh.SetColors.html), [SetIndices](Mesh.SetIndices.html), [SetNormals](Mesh.SetNormals.html), [SetTangents](Mesh.SetTangents.html), [SetTriangles](Mesh.SetTriangles.html), [SetUVs](Mesh.SetUVs.html), [SetVertices](Mesh.SetVertices.html), [SetBoneWeights](Mesh.SetBoneWeights.html).  
  
There is also an "advanced" set of methods, which allow you to directly write to the mesh data with control over whether any checks or validation should be performed. These methods are intended for advanced use cases which require maximum performance. They are faster, but allow you to skip the checks on the data you supply. If you use these methods you must make sure that you are not supplying invalid data, because Unity will not check for you.  
  
The "advanced" methods are: [SetVertexBufferParams](Mesh.SetVertexBufferParams.html), [SetVertexBufferData](Mesh.SetVertexBufferData.html), [SetIndexBufferParams](Mesh.SetIndexBufferParams.html), [SetIndexBufferData](Mesh.SetIndexBufferData.html), [SetSubMesh](Mesh.SetSubMesh.html), and you can use the [MeshUpdateFlags](Rendering.MeshUpdateFlags.html) to control which checks or validation are performed or omitted. Use [AcquireReadOnlyMeshData](Mesh.AcquireReadOnlyMeshData.html) to take a read-only snapshot of Mesh data that you can use with C# Jobs and Burst, and [AllocateWritableMeshData](Mesh.AllocateWritableMeshData.html) with [ApplyAndDisposeWritableMeshData](Mesh.ApplyAndDisposeWritableMeshData.html) to create Meshes from C# Jobs and Burst.  
  
  
  
**Manipulating meshes from a script**  
  
There are three common tasks that might want to use the Mesh API for:  
  
**1\. Building a mesh from scratch**: should always be done in the following order:  
a) Assign [vertices](Mesh-vertices.html)  
b) Assign [triangles](Mesh-triangles.html).

using UnityEngine;  
  
public class Example : [MonoBehaviour](MonoBehaviour.html)
{
    [Vector3](Vector3.html)\[\] newVertices;
    [Vector2](Vector2.html)\[\] newUV;
    int\[\] newTriangles;  
  
   void Start()
    {
        [Mesh](Mesh.html) mesh = new [Mesh](Mesh.html)();
        GetComponent<[MeshFilter](MeshFilter.html)\>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}

**2\. Modifying vertex attributes every frame**:  
a) Get vertices  
b) Modify them  
c) Assign them back to the mesh.

using UnityEngine;  
  
public class Example : [MonoBehaviour](MonoBehaviour.html)
{
    void [Update](PlayerLoop.Update.html)()
    {
        [Mesh](Mesh.html) mesh = GetComponent<[MeshFilter](MeshFilter.html)\>().mesh;
        [Vector3](Vector3.html)\[\] vertices = mesh.vertices;
        [Vector3](Vector3.html)\[\] normals = mesh.normals;  
  
       for (var i = 0; i < vertices.Length; i++)
        {
            vertices\[i\] += normals\[i\] \* [Mathf.Sin](Mathf.Sin.html)([Time.time](Time-time.html));
        }  
  
       mesh.vertices = vertices;
    }
}

**3\. Continously changing the mesh triangles and vertices**:  
a) Call [Clear](Mesh.Clear.html) to start fresh  
b) Assign vertices and other attributes  
c) Assign triangle indices.  
  
It is important to call [Clear](Mesh.Clear.html) before assigning new vertices or triangles. Unity always checks the supplied triangle indices whether they don't reference out of bounds vertices. Calling [Clear](Mesh.Clear.html) then assigning vertices then triangles makes sure you never have out of bounds data.

using UnityEngine;  
  
public class ExampleClass : [MonoBehaviour](MonoBehaviour.html)
{
    [Vector3](Vector3.html)\[\] newVertices;
    [Vector2](Vector2.html)\[\] newUV;
    int\[\] newTriangles;  
  
   void Start()
    {
        [Mesh](Mesh.html) mesh = GetComponent<[MeshFilter](MeshFilter.html)\>().mesh;  
  
       mesh.Clear();  
  
       // Do some calculations...
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}

### Properties

Property

Description

[bindposeCount](Mesh-bindposeCount.html)

The number of bind poses in the Mesh.

[bindposes](Mesh-bindposes.html)

The bind poses. The bind pose at each index refers to the bone with the same index.

[blendShapeCount](Mesh-blendShapeCount.html)

Returns BlendShape count on this mesh.

[boneWeights](Mesh-boneWeights.html)

The BoneWeight for each vertex in the Mesh, which represents 4 bones per vertex.

[bounds](Mesh-bounds.html)

The bounding volume of the Mesh.

[colors](Mesh-colors.html)

Vertex colors of the Mesh.

[colors32](Mesh-colors32.html)

Vertex colors of the Mesh.

[indexBufferTarget](Mesh-indexBufferTarget.html)

The intended target usage of the Mesh GPU index buffer.

[indexFormat](Mesh-indexFormat.html)

Format of the mesh index buffer data.

[isReadable](Mesh-isReadable.html)

Returns true if the Mesh is read/write enabled, or false if it is not.

[normals](Mesh-normals.html)

The normals of the Mesh.

[skinWeightBufferLayout](Mesh-skinWeightBufferLayout.html)

The dimension of data in the bone weight buffer.

[subMeshCount](Mesh-subMeshCount.html)

The number of sub-meshes inside the Mesh object.

[tangents](Mesh-tangents.html)

The tangents of the Mesh.

[triangles](Mesh-triangles.html)

An array containing all triangles in the Mesh.

[uv](Mesh-uv.html)

The texture coordinates (UVs) in the first channel.

[uv2](Mesh-uv2.html)

The texture coordinates (UVs) in the second channel.

[uv3](Mesh-uv3.html)

The texture coordinates (UVs) in the third channel.

[uv4](Mesh-uv4.html)

The texture coordinates (UVs) in the fourth channel.

[uv5](Mesh-uv5.html)

The texture coordinates (UVs) in the fifth channel.

[uv6](Mesh-uv6.html)

The texture coordinates (UVs) in the sixth channel.

[uv7](Mesh-uv7.html)

The texture coordinates (UVs) in the seventh channel.

[uv8](Mesh-uv8.html)

The texture coordinates (UVs) in the eighth channel.

[vertexAttributeCount](Mesh-vertexAttributeCount.html)

Returns the number of vertex attributes that the mesh has. (Read Only)

[vertexBufferCount](Mesh-vertexBufferCount.html)

Gets the number of vertex buffers present in the Mesh. (Read Only)

[vertexBufferTarget](Mesh-vertexBufferTarget.html)

The intended target usage of the Mesh GPU vertex buffer.

[vertexCount](Mesh-vertexCount.html)

Returns the number of vertices in the Mesh (Read Only).

[vertices](Mesh-vertices.html)

Returns a copy of the vertex positions or assigns a new vertex positions array.

### Constructors

Constructor

Description

[Mesh](Mesh-ctor.html)

Creates an empty Mesh.

### Public Methods

Method

Description

[AddBlendShapeFrame](Mesh.AddBlendShapeFrame.html)

Adds a new blend shape frame.

[Clear](Mesh.Clear.html)

Clears all vertex data and all triangle indices.

[ClearBlendShapes](Mesh.ClearBlendShapes.html)

Clears all blend shapes from Mesh.

[CombineMeshes](Mesh.CombineMeshes.html)

Combines several Meshes into this Mesh.

[GetAllBoneWeights](Mesh.GetAllBoneWeights.html)

Gets the bone weights for the Mesh.

[GetBaseVertex](Mesh.GetBaseVertex.html)

Gets the base vertex index of the given sub-mesh.

[GetBindposes](Mesh.GetBindposes.html)

Gets the bind poses of the Mesh.

[GetBlendShapeBuffer](Mesh.GetBlendShapeBuffer.html)

Retrieves a GraphicsBuffer that provides direct read and write access to GPU blend shape vertex data.

[GetBlendShapeBufferRange](Mesh.GetBlendShapeBufferRange.html)

Get the location of blend shape vertex data for a given blend shape.

[GetBlendShapeFrameCount](Mesh.GetBlendShapeFrameCount.html)

Returns the frame count for a blend shape.

[GetBlendShapeFrameVertices](Mesh.GetBlendShapeFrameVertices.html)

Retreives deltaVertices, deltaNormals and deltaTangents of a blend shape frame.

[GetBlendShapeFrameWeight](Mesh.GetBlendShapeFrameWeight.html)

Returns the weight of a blend shape frame.

[GetBlendShapeIndex](Mesh.GetBlendShapeIndex.html)

Returns index of BlendShape by given name.

[GetBlendShapeName](Mesh.GetBlendShapeName.html)

Returns name of BlendShape by given index.

[GetBonesPerVertex](Mesh.GetBonesPerVertex.html)

The number of non-zero bone weights for each vertex.

[GetBoneWeightBuffer](Mesh.GetBoneWeightBuffer.html)

Retrieves a GraphicsBuffer that provides direct read and write access to GPU bone weight data.

[GetBoneWeights](Mesh.GetBoneWeights.html)

Gets the bone weights for the Mesh.

[GetColors](Mesh.GetColors.html)

Gets the vertex colors of the Mesh.

[GetIndexBuffer](Mesh.GetIndexBuffer.html)

Retrieves a GraphicsBuffer to the GPU index buffer.

[GetIndexCount](Mesh.GetIndexCount.html)

Gets the index count of the given sub-mesh.

[GetIndexStart](Mesh.GetIndexStart.html)

Gets the starting index location within the Mesh's index buffer, for the given sub-mesh.

[GetIndices](Mesh.GetIndices.html)

Fetches the index list for the specified sub-mesh.

[GetNativeIndexBufferPtr](Mesh.GetNativeIndexBufferPtr.html)

Retrieves a native (underlying graphics API) pointer to the index buffer.

[GetNativeVertexBufferPtr](Mesh.GetNativeVertexBufferPtr.html)

Retrieves a native (underlying graphics API) pointer to the vertex buffer.

[GetNormals](Mesh.GetNormals.html)

Gets the vertex normals of the Mesh.

[GetSubMesh](Mesh.GetSubMesh.html)

Get information about a sub-mesh of the Mesh.

[GetTangents](Mesh.GetTangents.html)

Gets the tangents of the Mesh.

[GetTopology](Mesh.GetTopology.html)

Gets the topology of a sub-mesh.

[GetTriangles](Mesh.GetTriangles.html)

Fetches the triangle list for the specified sub-mesh on this object.

[GetUVDistributionMetric](Mesh.GetUVDistributionMetric.html)

The UV distribution metric can be used to calculate the desired mipmap level based on the position of the camera.

[GetUVs](Mesh.GetUVs.html)

Gets the texture coordinates (UVs) stored in a given channel.

[GetVertexAttribute](Mesh.GetVertexAttribute.html)

Returns information about a vertex attribute based on its index.

[GetVertexAttributeDimension](Mesh.GetVertexAttributeDimension.html)

Get dimension of a specific vertex data attribute on this Mesh.

[GetVertexAttributeFormat](Mesh.GetVertexAttributeFormat.html)

Get format of a specific vertex data attribute on this Mesh.

[GetVertexAttributeOffset](Mesh.GetVertexAttributeOffset.html)

Get offset within a vertex buffer stream of a specific vertex data attribute on this Mesh.

[GetVertexAttributes](Mesh.GetVertexAttributes.html)

Get information about vertex attributes of a Mesh.

[GetVertexAttributeStream](Mesh.GetVertexAttributeStream.html)

Gets the vertex buffer stream index of a specific vertex data attribute on this Mesh.

[GetVertexBuffer](Mesh.GetVertexBuffer.html)

Retrieves a GraphicsBuffer that provides direct acces to the GPU vertex buffer.

[GetVertexBufferStride](Mesh.GetVertexBufferStride.html)

Get vertex buffer stream stride in bytes.

[GetVertices](Mesh.GetVertices.html)

Gets the vertex positions of the Mesh.

[HasVertexAttribute](Mesh.HasVertexAttribute.html)

Checks if a specific vertex data attribute exists on this Mesh.

[MarkDynamic](Mesh.MarkDynamic.html)

Optimize mesh for frequent updates.

[MarkModified](Mesh.MarkModified.html)

Notify Renderer components of mesh geometry change.

[Optimize](Mesh.Optimize.html)

Optimizes the Mesh data to improve rendering performance.

[OptimizeIndexBuffers](Mesh.OptimizeIndexBuffers.html)

Optimizes the geometry of the Mesh to improve rendering performance.

[OptimizeReorderVertexBuffer](Mesh.OptimizeReorderVertexBuffer.html)

Optimizes the vertices of the Mesh to improve rendering performance.

[RecalculateBounds](Mesh.RecalculateBounds.html)

Recalculate the bounding volume of the Mesh and all of its sub-meshes with the vertex data.

[RecalculateNormals](Mesh.RecalculateNormals.html)

Recalculates the normals of the Mesh from the triangles and vertices.

[RecalculateTangents](Mesh.RecalculateTangents.html)

Recalculates the tangents of the Mesh from the normals and texture coordinates.

[RecalculateUVDistributionMetric](Mesh.RecalculateUVDistributionMetric.html)

Recalculates the UV distribution metric of the Mesh from the vertices and uv coordinates.

[RecalculateUVDistributionMetrics](Mesh.RecalculateUVDistributionMetrics.html)

Recalculates the UV distribution metrics of the Mesh from the vertices and uv coordinates.

[SetBindposes](Mesh.SetBindposes.html)

Sets the bind poses of the Mesh.

[SetBoneWeights](Mesh.SetBoneWeights.html)

Sets the bone weights for the Mesh.

[SetColors](Mesh.SetColors.html)

Set the per-vertex colors of the Mesh.

[SetIndexBufferData](Mesh.SetIndexBufferData.html)

Sets the data of the index buffer of the Mesh.

[SetIndexBufferParams](Mesh.SetIndexBufferParams.html)

Sets the index buffer size and format.

[SetIndices](Mesh.SetIndices.html)

Sets the index buffer for the sub-mesh.

[SetNormals](Mesh.SetNormals.html)

Set the normals of the Mesh.

[SetSubMesh](Mesh.SetSubMesh.html)

Sets the information about a sub-mesh of the Mesh.

[SetSubMeshes](Mesh.SetSubMeshes.html)

Sets information defining all sub-meshes in this Mesh, replacing any existing sub-meshes.

[SetTangents](Mesh.SetTangents.html)

Set the tangents of the Mesh.

[SetTriangles](Mesh.SetTriangles.html)

Sets the triangle list for the sub-mesh.

[SetUVs](Mesh.SetUVs.html)

Sets the texture coordinates (UVs) stored in a given channel.

[SetVertexBufferData](Mesh.SetVertexBufferData.html)

Sets the data of the vertex buffer of the Mesh.

[SetVertexBufferParams](Mesh.SetVertexBufferParams.html)

Sets the vertex buffer size and layout.

[SetVertices](Mesh.SetVertices.html)

Assigns a new vertex positions array.

[UploadMeshData](Mesh.UploadMeshData.html)

Upload previously done Mesh modifications to the graphics API.

### Static Methods

Method

Description

[AcquireReadOnlyMeshData](Mesh.AcquireReadOnlyMeshData.html)

Gets a snapshot of Mesh data for read-only access.

[AllocateWritableMeshData](Mesh.AllocateWritableMeshData.html)

Allocates data structures for Mesh creation using C# Jobs.

[ApplyAndDisposeWritableMeshData](Mesh.ApplyAndDisposeWritableMeshData.html)

Applies data defined in MeshData structs to Mesh objects.

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
