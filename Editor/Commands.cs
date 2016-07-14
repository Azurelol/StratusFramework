/******************************************************************************/
/*!
@file   Commands.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus
{
  namespace Editor
  {
    /**************************************************************************/
    /*!
    @class Commands 
    */
    /**************************************************************************/
    public abstract class Command
    {
      public string Name;
      public Command(string name) { Name = name;}
      public abstract void Execute();
    }

    public class DelegateCommand : Command
    {

      public delegate void Callback();
      public Callback Method;
      public DelegateCommand(string name, Callback func) : base(name) { Method = func; }
      public override void Execute() { Method.Invoke(); }
    }

    public class MenuItemCommand : Command
    {
      public string Path;
      public MenuItemCommand(string name, string path) : base(name) { Path = path; }
      public override void Execute() { UnityEditor.EditorApplication.ExecuteMenuItem(Path); }
    }

    /**************************************************************************/
    /*!
    @class Commands 
    */
    /**************************************************************************/
    public class Commands : MonoBehaviour
    {
      public static GameObject GameObjectInst;
      public static Vector3 GameObjectPos;

      static void PostCreate()
      {
        // Get the current scene camera through the Editor through some voodoo
        // http://answers.unity3d.com/questions/240999/instantiate-object-in-middle-of-editor-view-not-ma.html
        var camera = UnityEditor.SceneView.GetAllSceneCameras()[0].GetComponent<Camera>();
        GameObjectPos = camera.transform.TransformPoint(Vector3.forward * 1.1f);
        GameObjectInst.transform.position = GameObjectPos;
        UnityEditor.Undo.RegisterCreatedObjectUndo(GameObjectInst, "Create " + GameObjectInst.name);
        UnityEditor.Selection.activeObject = GameObjectInst;
      }

      //----------------------------------------------------------------------/
      // Creation
      //----------------------------------------------------------------------/
      static void CreatePrimitive(PrimitiveType type)
      {
        GameObjectInst = GameObject.CreatePrimitive(type); PostCreate();
      }

      static public void CreateEmpty() { }
      static public void CreateCube() { CreatePrimitive(PrimitiveType.Cube); }
      static public void CreateSphere() { CreatePrimitive(PrimitiveType.Sphere); }
      static public void CreatePlane() { CreatePrimitive(PrimitiveType.Plane); }
      static public void CreateCapsule() { CreatePrimitive(PrimitiveType.Capsule); }

      //----------------------------------------------------------------------/
      // 
      //----------------------------------------------------------------------/

    }


  }

}

