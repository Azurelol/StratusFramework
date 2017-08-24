using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  namespace Internal
  {
    /// <summary>
    /// A list of string-color pairs for all tags in the project
    /// </summary>
    [Serializable]
    public class TagColors
    {
      [Serializable]
      public class TagColorPair
      {
        public string Tag;
        [ColorUsage(false)]
        public Color Color;
      }

      public bool enabled = true;
      [SerializeField]
      public List<TagColorPair> coloredTags = new List<TagColorPair>();
      private Dictionary<string, TagColorPair> lookupTable;

      /// <summary>
      /// The default color used for all tags
      /// </summary>
      private Color defaultColor { get; set; }

      public TagColors()
      {
        this.defaultColor = new Color(1f, 1f, 1f, 0f);
      }

      /// <summary>
      /// Verifies that all saved tags are still valid, pruning outdated ones
      /// </summary>
      public void Validate()
      {
        lookupTable = new Dictionary<string, TagColorPair>();
        var updatedColoredTags = new List<TagColorPair>();
        var currentTags = UnityEditorInternal.InternalEditorUtility.tags;
        
        // Start at 1 to skip 'Untagged' tag
        for(var i = 1; i < currentTags.Length; ++i)
        {
          var tag = currentTags[i];
          var pair = coloredTags.Find(p => p.Tag == tag);

          // If it was not present previously, create a new one with a default color
          if (pair == null)
          {
            pair = new TagColorPair();
            pair.Tag = tag;
            pair.Color = defaultColor;
          }

          updatedColoredTags.Add(pair);
          lookupTable.Add(pair.Tag, pair);
        }

        // Now replace the old one!
        coloredTags = updatedColoredTags; 
      }

      public void Clear()
      {
        coloredTags.Clear();
      }

      public void Reset()
      {
        foreach (var pair in coloredTags)
          pair.Color = defaultColor;
      }

      public Color GetColor(string tag)
      {
        if (lookupTable == null)
          Validate();

        if (!lookupTable.ContainsKey(tag))
          return defaultColor;

        return lookupTable[tag].Color;
      }
      
    }
  }

}