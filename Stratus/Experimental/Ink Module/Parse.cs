using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {

      /// <summary>
      /// Represents a grouped parse
      /// </summary>
      public class Parse
      {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="onParse"></param>
        /// <param name="groups"></param>
        public Parse(string label, string value, Dictionary<string, string> groups)
        {
          this.label = label;
          this.value = value;
          this.groups = groups;
        }

        /// <summary>
        /// The label for this parse
        /// </summary>
        public string label { get; private set; }
        /// <summary>
        /// The substring value of this parse (only valid for non-grouped)
        /// </summary>
        public string value { get; private set; }
        /// <summary>
        /// The captured groups for this parse
        /// </summary>
        private Dictionary<string, string> groups { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Retrieves a specific value from a group in this parse
        /// </summary>
        /// <param name="parseCategory"></param>
        /// <returns></returns>
        public string Find(string group)
        {
          if (!groups.ContainsKey(group))
          {
            throw new System.ArgumentOutOfRangeException($"The key '{group}' was not found among this parse!");
          }
          return groups[group];
        }

        /// <summary>
        /// Prints the details for all groups on this parse
        /// </summary>
        public string groupInformation
        {
          get
          {
            StringBuilder builder = new StringBuilder();
            foreach (var group in groups)
            {
              builder.Append($"[{group.Key},{group.Value}]");
            }
            return builder.ToString();
          }
        }

        public override string ToString()
        {
          return $"{label}: {groupInformation}";
        }

      }      

      /// <summary>
      /// Represents a parsed line of dialog. It may have multiple parses
      /// such as one for the speaker, another for the message, etc...
      /// </summary>
      public struct ParsedLine
      {
        private Dictionary<string, Parse> parses;

        public ParsedLine(Dictionary<string, Parse> parses, List<string> tags, string line)
        {
          this.parses = parses;
          this.line = line;
          this.tags = tags;
        }

        /// <summary>
        /// The unparsed line
        /// </summary>
        public string line { get; private set; }

        /// <summary>
        /// A list of all tags associated with this line
        /// </summary>
        public List<string> tags { get; private set; }

        /// <summary>
        /// Whether this line has any valid parses
        /// </summary>
        public bool isParsed => parses.Count > 0;

        /// <summary>
        /// Retrieves a specific parse from this line
        /// </summary>
        /// <param name="parseCategory"></param>
        /// <returns></returns>
        public Parse Find(string label)
        {
          if (!parses.ContainsKey(label))
          {
            //throw new System.ArgumentOutOfRangeException($"The label '{label}' was not found among the parses in this line!");
            return null;
          }
          return parses[label];
        }
      }

    }
  }
}