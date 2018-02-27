using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(BaseEditor), true), CanEditMultipleObjects]
  public class BaseWindowEditor : BaseEditor<BaseWindow> 
  {
  }
}