using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class EnumFlagAttribute : PropertyAttribute
  {
    public string enumName;
    public EnumFlagAttribute() {}
    public EnumFlagAttribute(string name) { enumName = name; }
  }

}