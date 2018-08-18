using UnityEngine;
using System.Collections.Generic;

namespace Altostratus
{
  /// <summary>
  /// Represents a combat-capable group of characters
  /// </summary>
  [CreateAssetMenu(fileName ="Party", menuName = "Prototype/Party")]
  public class Party : ScriptableObject
  {
    public List<Character> Members = new List<Character>();
    public Character Lead;
  }

}