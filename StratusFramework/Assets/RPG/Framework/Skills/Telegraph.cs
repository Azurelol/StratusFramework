/******************************************************************************/
/*!
@file   Telegraph.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using Stratus.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Altostratus
{
  /// <summary>
  /// A representation of a skill attack
  /// </summary>
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(MeshRenderer))]
  public class Telegraph : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum Delivery
    {
      [Tooltip("The telegraph is placed directly on top of the source")]
      Source,
      [Tooltip("The telegraph is placed directly on top of the target")]
      Target,
      [Tooltip("The telegraph is projected from the source to the target")]
      Projection
    }

    /// <summary>
    /// How the telegraph should be rendered
    /// </summary>
    [Serializable]
    public class Configuration
    {
      public GameObject Prefab;
      [Tooltip("How the skill should be delivered to the target")]
      public Delivery Delivery = Delivery.Target;
      public Material Material;
      public Color Color = Color.red;
      public Vector3 Scale = new Vector3(5f, 0.5f, 5f);
      [Tooltip("How closely the telegraph follows the target. if at all")]
      [Range(0f, 1f)]
      public float Damping = 1f;
      //public float Length = 5f;
      //public float Width = 5f;
      //public float Height = 0.5f;
    }

    /// <summary>
    /// Where the telegraph should be rendered
    /// </summary>
    public struct Placement
    {
      public Transform Source;
      public Vector3 Target;
      public Vector3 Rotation;

      public Placement(Transform source, Vector3 position, Vector3 rotation)
      {
        Source = source;
        Target = position;
        Rotation = rotation;
      }
    }


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Configuration CurrentConfiguration = new Configuration();


    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private List<CombatController> TargetsInsideArea = new List<CombatController>();

    //------------------------------------------------------------------------/
    // Construction
    //------------------------------------------------------------------------/
    public static Telegraph Construct(Configuration config, Placement targeting)
    {
      var go = GameObject.Instantiate(config.Prefab);
      var telegraph = go.GetComponent<Telegraph>();
      //telegraph.conf
      telegraph.Configure(config, targeting);
      return telegraph;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    /// <summary>
    /// Start with this telegraph being invisible.
    /// </summary>
    private void Start()
    {
      //Collider = GetComponent<Collider>();
      //Trace.Script("Telegraph started!", this);

      //StartCoroutine(Routines.Fade(this.gameObject, 0f, 0f)); 
    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
      var target = other.gameObject.GetComponent<CombatController>();
      if (target != null)
      {
        //Trace.Script(target.name + " has entered the area!", this);
        TargetsInsideArea.Add(target);
      }
    }

    private void OnTriggerExit(Collider other)
    {
      var target = other.gameObject.GetComponent<CombatController>();
      if (target != null)
      {
        //Trace.Script(target.name + " has exited the area!", this);
        TargetsInsideArea.Remove(target);
      }
    }

    //------------------------------------------------------------------------/
    // Routines
    //------------------------------------------------------------------------/
    /// <summary>
    /// Fades in this telegraph.
    /// </summary>
    /// <param name="duration"></param>
    public void Start(float duration)
    {
      //Trace.Script("Displaying telegraph over " + duration + " seconds!", this);
      StartCoroutine(Routines.Fade(this.gameObject, 1f, duration));
    }

    /// <summary>
    /// Fades out this telegraph. It will destroy it shortly after its been completely faded out.
    /// </summary>
    /// <param name="fadeSpeed"></param>
    public void End(float fadeSpeed, float destroySpeed = 0.0f)
    {
      //Trace.Script("Hiding telegraph over " + duration + " seconds!", this);
      StartCoroutine(Routines.Fade(this.gameObject, 0f, fadeSpeed));
      if (destroySpeed > 0.0f)
        GameObject.Destroy(this.gameObject, destroySpeed);
    }

    /// <summary>
    /// Places this telegraph in the proper position
    /// </summary>
    /// <param name="placement"></param>
    public void Place(Placement placement, bool instant = false)
    {
      // Position
      if (CurrentConfiguration.Delivery == Delivery.Projection)
        ProjectFromSource(placement, instant);
      else if (CurrentConfiguration.Delivery == Delivery.Target)
        ProjectOnTarget(placement.Target, instant);
      else if (CurrentConfiguration.Delivery == Delivery.Source)
        transform.position = placement.Source.position;
      // Rotation
      transform.rotation = Quaternion.LookRotation(placement.Rotation);
    }

    /// <summary>
    /// Projects the telegraph so it emanates from the source towards the target
    /// </summary>
    /// <param name="placement"></param>
    void ProjectFromSource(Placement placement, bool instant)
    {
      // For the first part of the offset we will use the scale of the object
      var sourceOffset = placement.Source.localScale.magnitude;
      // For the second part we will add half the size of the depth (z) of the object.         
      var telegraphOffset = CurrentConfiguration.Scale.z / 2;
      // Calculate the target position
      var target = placement.Source.position + (placement.Source.transform.forward * (sourceOffset + telegraphOffset));

      if (instant)
      {
        transform.position = target;
        return;
      }

      // Interpolate to the new target position
      transform.position = Vector3.Lerp(transform.position, target, Time.fixedDeltaTime * CurrentConfiguration.Damping * 3);
    }

    /// <summary>
    /// Projects the telegraph directly on top of the target
    /// </summary>
    /// <param name="target"></param>
    void ProjectOnTarget(Vector3 target, bool instant)
    {
      if (instant)
      {
        transform.position = target;
        return;
      }

      // Interpolate to the new target position
      transform.position = Vector3.Lerp(transform.position, target, Time.fixedDeltaTime * CurrentConfiguration.Damping * 3);
    }

    void Configure(Configuration config, Placement placement)
    {
      CurrentConfiguration = config;

      // Rendering
      var renderer = GetComponent<MeshRenderer>();
      renderer.material = config.Material;
      renderer.material.color = config.Color;
      renderer.material.color = renderer.material.color.ToAlpha(0f);

      // TRS
      transform.localScale = config.Scale;
      // Start with the telegraph on the target?
      Place(placement, true);
    }

    /// <summary>
    /// Finds all targets within the current telegraph object
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="telegraphData"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public CombatController[] FindTargetsWithinBoundary()
    {
      return TargetsInsideArea.ToArray();

      //Collider[] hits = null;
      //var availableTargets = (from Collider hit in hits
      //                        where hit.GetComponent<CombatController>()
      //                        select hit.GetComponent<CombatController>()).ToArray();
      //
      //return availableTargets;
    }

    //public static void Draw(Targeting targeting, Configuration config)
    //{
    //  Vector3 position, scale;
    //  // The scale is a little different for different geometries
    //  switch (config.Type)
    //  {
    //    case Type.Circle:
    //      position = targeting.Position;
    //      scale = config.Circle;
    //      break;
    //    case Type.Rectangle:
    //      position = (targeting.Source.position + targeting.Position) / 2;
    //      scale = config.Rectangle;
    //      break;
    //    default:
    //    throw new System.Exception("Type not implemented!");
    //  }
    //
    //  Matrix4x4 drawMatrix = Matrix4x4.TRS(position, Quaternion.LookRotation(targeting.Rotation, Vector3.up), scale);
    //  config.Material.color = config.Color;
    //  Graphics.DrawMesh(config.Mesh, drawMatrix, config.Material, 0);
    //}

  }
}
