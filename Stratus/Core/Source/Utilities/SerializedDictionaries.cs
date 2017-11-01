using UnityEngine;
using Stratus;
using Rotorz.Extras.Collections;
using System;

namespace Stratus
{
  namespace Types
  {
    public sealed class StringStringDictionaryEditable : EditableEntry<StringStringDictionary> { }
    [Serializable, EditableEntry(typeof(StringStringDictionaryEditable))]
    public sealed class StringStringDictionary : OrderedDictionary<string, string> { }

  }
}