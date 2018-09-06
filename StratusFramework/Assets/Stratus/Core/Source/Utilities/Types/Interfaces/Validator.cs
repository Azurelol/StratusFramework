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
  public class ObjectValidation  : Validation
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
    public Level type { get; protected set; }
    public UnityEngine.Object target { get; protected set; }
    public Func<bool> onValidate { get; protected set; }
    public System.Action onSelect { get; protected set; }
    public bool hasContext { get; protected set; }

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public ObjectValidation(string message, Level type, UnityEngine.Object target, Func<bool> onValidate = null) : base(!string.IsNullOrEmpty(message), message)
    {
      this.type = type;
      this.target = target;
      this.onSelect = null;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null;
    }

    public ObjectValidation(string message, Level type, System.Action onSelect, Func<bool> onValidate = null) : base(!string.IsNullOrEmpty(message), message)
    {
      this.type = type;
      this.target = null;
      this.onSelect = onSelect;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null || onSelect != null;
    }

    public ObjectValidation(Level type, UnityEngine.Object target, Func<bool> onValidate = null)
    {
      this.type = type;
      this.target = target;
      this.onSelect = null;
      this.onValidate = onValidate;
      hasContext = target != null || onValidate != null;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public static ObjectValidation[] Aggregate(IEnumerable<Interfaces.Validator> validators)
    {
      List<ObjectValidation> messages = new List<ObjectValidation>();
      foreach (var t in validators)
      {
        var msg = t.Validate();
        if (msg != null && msg.valid)
          messages.Add(msg);
      }
      return messages.ToArray();
    }

    public static ObjectValidation[] Aggregate(IEnumerable<Interfaces.ValidatorAggregator> validators)
    {
      List<ObjectValidation> messages = new List<ObjectValidation>();
      foreach (var t in validators)
      {
        var msg = t.Validate();
        if (msg != null)
          messages.AddRange(msg);
      }
      return messages.ToArray();
    }

    public static ObjectValidation[] Aggregate(Interfaces.ValidatorAggregator validator)
    {
      return validator.Validate();
    }

    public static ObjectValidation Generate(Interfaces.Validator validator)
    {
      return validator.Validate();
     }

    public static ObjectValidation NullReference(Behaviour behaviour, string description = null)
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
      ObjectValidation validation = new ObjectValidation(msg, Level.Warning, behaviour);
      return validation;
    }

    /// <summary>
    /// Concatenates two validation messages together, provided they are to the same target.
    /// The level is raised to the highest of the two.
    /// </summary>
    /// <param name="other"></param>
    public void Add(ObjectValidation other)
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
      ObjectValidation Validate();
    }

    /// <summary>
    /// Interface for a component at the top level of a hierarchy of validators
    /// </summary>
    public interface ValidatorAggregator
    {
      ObjectValidation[] Validate();
    }

  }

}