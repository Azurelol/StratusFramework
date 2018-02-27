/******************************************************************************/
/*!
@file   PrimitiveHelper.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;

// REF:
// http://answers.unity3d.com/questions/514293/changing-a-gameobjects-primitive-mesh.html

namespace Stratus
{
  namespace Utilities
  {
    public static class PrimitiveHelper
    {
      private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

      /// <summary>
      /// Instantiates a primitive of the given type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="withCollider"></param>
      /// <returns></returns>
      public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider)
      {
        if (withCollider) { return GameObject.CreatePrimitive(type); }

        GameObject gameObject = new GameObject(type.ToString());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(type);
        gameObject.AddComponent<MeshRenderer>();

        return gameObject;
      }

      public static Mesh GetPrimitiveMesh(PrimitiveType type)
      {
        if (!PrimitiveHelper.primitiveMeshes.ContainsKey(type))
        {
          PrimitiveHelper.CreatePrimitiveMesh(type);
        }

        return PrimitiveHelper.primitiveMeshes[type];
      }

      private static Mesh CreatePrimitiveMesh(PrimitiveType type)
      {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        GameObject.DestroyImmediate(gameObject);

        PrimitiveHelper.primitiveMeshes[type] = mesh;
        return mesh;
      }
    } 
  }
}
