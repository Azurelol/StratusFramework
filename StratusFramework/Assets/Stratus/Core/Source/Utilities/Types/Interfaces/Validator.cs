using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// A validation message for the user
  /// </summary>
  public class Validation
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Maps to UnityEditor.MessageType
    /// </summary>
    public enum Level
    {
      Info = 1,
      Warning = 2,
      Error = 3
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public string message;
    public Level type;
    public UnityEngine.Object target;
    public Func<bool> onValidate;
    public System.Action onSelect;
    public bool hasContext;
    public bool valid => !string.IsNullOrEmpty(message);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/

    public Validation(string message, Level type, UnityEngine.Object target, Func<bool> onValidate = null)
    {
      this.message = message;
      this.type = type;
      this.target = target;
      this.onSelect = null;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null;
    }

    public Validation(string message, Level type, System.Action onSelect, Func<bool> onValidate = null)
    {
      this.message = message;
      this.type = type;
      this.target = null;
      this.onSelect = onSelect;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null || onSelect != null;
    }

    public Validation(Level type, UnityEngine.Object target, Func<bool> onValidate = null)
    {
      this.message = string.Empty;
      this.type = type;
      this.target = target;
      this.onSelect = null;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null;
    }

    public static Validation[] Aggregate(IEnumerable<Interfaces.Validator> validators)
    {
      List<Validation> messages = new List<Validation>();
      foreach (var t in validators)
      {
        var msg = t.Validate();
        if (msg != null && msg.valid)
          messages.Add(msg);
      }
      return messages.ToArray();
    }

    public static Validation[] Aggregate(IEnumerable<Interfaces.ValidatorAggregator> validators)
    {
      List<Validation> messages = new List<Validation>();
      foreach (var t in validators)
      {
        var msg = t.Validate();
        if (msg != null)
          messages.AddRange(msg);
      }
      return messages.ToArray();
    }

    public static Validation[] Aggregate(Interfaces.ValidatorAggregator validator)
    {
      return validator.Validate();
    }

    public static Validation Generate(Interfaces.Validator validator)
    {
      return validator.Validate();
     }

    public static Validation NullReference(Behaviour behaviour, string description = null)
    {
      FieldInfo[] nullFields = Stratus.Utilities.Reflection.GetFieldsWithNullReferences(behaviour);
      if (nullFields.Empty())
        return null;

      string label = behaviour.GetType().Name;
      if (description != null)
        label += $" {description}";

      string msg = $"{label} has the following fields currently set to null:";
      foreach (var f in nullFields)
        msg += $"\n - <i>{f.Name}</i>";
      Validation validation = new Validation(msg, Level.Warning, behaviour);
      return validation;
    }

    /// <summary>
    /// Concatenates two validation messages together, provided they are to the same target.
    /// The level is raised to the highest of the two.
    /// </summary>
    /// <param name="other"></param>
    public void Add(Validation other)
    {
      if (other == null || other.target != this.target)
        return;

      message += other.message;
      if (other.type > this.type)
        this.type = other.type;
    }

    /// <summary>
    /// Concatenates the given message onto this validation.
    /// </summary>
    /// <param name="other"></param>
    public void Add(string message)
    {
      this.message += message;
    }

  }

  namespace Interfaces
  {
    /// <summary>
    /// Interface for validating the settings of a behaviour
    /// </summary>
    public interface Validator
    {
      Validation Validate();
    }

    /// <summary>
    /// Interface for a component at the top level of a hierarchy of validators
    /// </summary>
    public interface ValidatorAggregator
    {
      Validation[] Validate();
    }

  }

}