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
        public Parse(string label)
        {
          this.label = label;
        }

        /// <summary>
        /// The label for this parse
        /// </summary>
        public string label { get; private set; }
        /// <summary>
        /// The substring value of this parse (only valid for non-grouped)
        /// </summary>
        public List<string> values { get; private set; } = new List<string>();
        /// <summary>
        /// The value of this parse (first valuee if grouped)
        /// </summary>
        public string value => values.Count > 0 ? values[0] : null;
        /// <summary>
        /// The captured groups for this parse
        /// </summary>
        public List<Dictionary<string, string>> matches { get; set; } = new List<Dictionary<string, string>>();
        /// <summary>
        /// The first match captured by this parse
        /// </summary>
        public Dictionary<string, string> firstMatch => matches.Count > 0 ? matches[0] : null;
        /// <summary>
        /// Whether this parse was successful
        /// </summary>
        public bool isValid => (values.Count > 0 || matches.Count > 0);

        /// <summary>
        /// Adds a substring value to this parse
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
          values.Add(value);
        }

        /// <summary>
        /// Adds a match (of groups) to this parse
        /// </summary>
        /// <param name="match"></param>
        public void Add(Dictionary<string, string> match)
        {
          matches.Add(match);
        }

        /// <summary>
        /// Retrieves a specific value from a group in this parse
        /// </summary>
        /// <param name="parseCategory"></param>
        /// <returns></returns>
        public string FindFirst(string group)
        {
          foreach(var match in matches)
          {
            if (match.ContainsKey(group))
              return match[group];
          }
          throw new System.ArgumentOutOfRangeException($"The key '{group}' was not found among this parse's groups!");                   
        }

        /// <summary>
        /// Retrieves all values from a given group in this parse
        /// </summary>
        /// <param name="parseCategory"></param>
        /// <returns></returns>
        public string[] Find(string group)
        {
          List<string> values = new List<string>();
          foreach (var match in matches)
          {
            if (match.ContainsKey(group))
              values.Add(match[group]);
          }
          if (values.NotEmpty())
            return values.ToArray();
          else
            throw new System.ArgumentOutOfRangeException($"The key '{group}' was not found among this parse's groups!");
        }

        /// <summary>
        /// Prints the details for all groups on this parse
        /// </summary>
        public string groupInformation
        {
          get
          {
            StringBuilder builder = new StringBuilder(); 
            foreach(var match in matches)
            {
              builder.Append("{");
              foreach (var group in match)
              {
                builder.Append($"[{group.Key},{group.Value}]");
              }
              builder.Append("}");
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