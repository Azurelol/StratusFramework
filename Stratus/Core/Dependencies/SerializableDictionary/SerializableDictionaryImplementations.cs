using System;
 
using UnityEngine;

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}
[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> {}
[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> {}

// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}
