using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// A validation message
  /// </summary>
  public class Validation
  {
    public bool valid { get; protected set; }
    public string message { get; protected set; }

    public Validation()
    {
      this.valid = true;
      this.message = string.Empty;
    }

    public Validation(bool valid, string message) 
    {
      this.valid = valid;
      this.message = message;
    }

    public Validation(Exception exception)
    {
      this.valid = false;
      this.message = exception.Message;
    }

    public static implicit operator bool(Validation validation) => validation.valid;
  }
}