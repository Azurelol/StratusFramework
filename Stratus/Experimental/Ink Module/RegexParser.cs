using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// A parser that uses regular expression pattern matching
      /// </summary>
      public class RegexParser : Parser
      {
        //--------------------------------------------------------------------/
        // Definitions
        //--------------------------------------------------------------------/
        /// <summary>
        /// What part of the input is being targeted by this parse pattern
        /// </summary>
        public enum Target
        {
          Line,
          Tag
        }

        /// <summary>
        /// What kind of matching to perform
        /// </summary>
        public enum Scope
        {
          /// <summary>
          /// Non-capturing, with no subexpressions
          /// </summary>
          Default,
          /// <summary>
          /// Capturing groups, with subexpressions
          /// </summary>
          Group
        }

        /// <summary>
        /// A single category for a parse type
        /// </summary>
        public class Category
        {
          public Category(string label, string pattern, Target target, System.Action<Parse> onParse, Scope scope)
          {
            this.label = label;
            this.pattern = pattern;
            this.target = target;
            this.onParse = onParse;
            this.scope = scope;
          }

          /// <summary>
          /// The name of the category this parse is for (Example: "Speaker", "Message", etc ...)
          /// </summary>
          public string label { get; private set; }

          /// <summary>
          /// The regular expression pattern that will be used for this parse
          /// </summary>
          public string pattern { get; private set; }

          /// <summary>
          /// Whether this parse applies to a line or to a tag
          /// </summary>
          public Target target { get; private set; }

          /// <summary>
          /// A provided callback function to invoke when this parse was successful
          /// </summary>
          public System.Action<Parse> onParse { get; private set; }

          /// <summary>
          /// What kind of matching to perform for this category
          /// </summary>
          public Scope scope { get; private set; }
        }

        //--------------------------------------------------------------------/
        // Properties
        //--------------------------------------------------------------------/
        /// <summary>
        /// All categories set for parsing
        /// </summary>
        public List<Category> patterns { get; private set; } = new List<Category>();

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        /// <summary>
        /// Parses a line of ink dialog, using whatever parses have been set
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public override ParsedLine Parse(string line, List<string> tags)
        {
          Dictionary<string, Parse> parses = new Dictionary<string, Parse>();

          // Try every parse
          foreach (var category in patterns)
          {
            Regex r = new Regex(category.pattern, RegexOptions.IgnoreCase);
            Dictionary<string, string> groups = new Dictionary<string, string>();
            string value = null;

            // Whether a match has been found
            bool foundMatch = false;
            // Whether to use capturing groups
            bool isGrouped = category.scope == Scope.Group;

            // Check the line
            if (category.target == Target.Line)
            {
              if (isGrouped)
                foundMatch = MatchGroups(ref groups, r, line);
              else
                foundMatch = Match(ref value, r, line);
            }

            // Check every tag
            else if (category.target == Target.Tag)
            {
              foreach (var tag in tags)
              {
                if (isGrouped)
                  foundMatch = MatchGroups(ref groups, r, tag);
                else
                  foundMatch = Match(ref value, r, tag);
                if (foundMatch)
                  break;
              }
            }

            // If a match was found, lets add this parse
            if (foundMatch)
            {
              Parse parse = new Parse(category.label, value, groups);
              parses.Add(category.label, parse);
              category.onParse?.Invoke(parse);
            }

          }

          var parsedLine = new ParsedLine(parses, tags, line);
          return parsedLine;
        }

        /// <summary>
        /// Parses a single line of text using a specified regex with capture groups
        /// </summary>
        /// <param name="text"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool MatchGroups(ref Dictionary<string, string> groups, Regex r, string text)
        {
          Match m = r.Match(text);

          if (m.Success && m.Groups.Count > 1)
          {
            string[] names = r.GetGroupNames();
            for (int i = 1; i < m.Groups.Count; ++i)
            {
              Group group = m.Groups[i];
              groups.Add(names[i], group.Value);
            }
            return true;
          }

          return false;
        }

        /// <summary>
        /// Parses a single line of text using a specified regex
        /// </summary>
        /// <param name="value"></param>
        /// <param name="r"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool Match(ref string value, Regex r, string text)
        {
          Match m = r.Match(text);
          if (m.Success)
          {
            value = m.Value;
            return true;
          }
          return false;
        }

        /// <summary>
        /// Adds a pattern to the parsing filter
        /// </summary>
        /// <param name="label">The name of the category</param>
        /// <param name="groupPattern">A regular expression pattern</param>
        public void AddPattern(string label, string groupPattern, Target target, Scope scope, System.Action<Parse> onParseSuccessful = null)
        {
          patterns.Add(new Category(label, groupPattern, target, onParseSuccessful, scope));
        }

        //--------------------------------------------------------------------/
        // Helper
        //--------------------------------------------------------------------/
        /// <summary>
        /// A collection of useful regex presets for parsing
        /// </summary>
        public static class Presets
        {
          /// <summary>
          /// Speaker : Message
          /// </summary>
          public static string dialogueBasic = @"";

          /// <summary>
          /// "Value"
          /// </summary>
          public static string insideDoubleQuotes => "\"[^\"]*\"";

          /// <summary>
          /// [Value]
          /// </summary>
          public static string insideSquareBrackets => @"\[([a-zA-Z0-9-\s]+)\]";

          /// <summary>
          /// Variable = Operand
          /// </summary>
          public static string assignment => ComposeBinaryOperation("Variable", "Value", "=");

          /// <summary>
          /// Variable++
          /// </summary>
          public static string incrementOperator => @"(?<Variable>\w+)\+\+";

          /// <summary>
          /// Variable++
          /// </summary>
          public static string decrementOperator => @"(?<Variable>\w+)\-\-";

          /// <summary>
          /// Composes a regular expression that will try to match an assignment with specified key/value labels and a provided operator.
          /// For example ("Character", "Mood", "=") will match "John = happy" with groups ("Character"="John", "Mood"="Happy").
          /// </summary>
          /// <param name="variable"></param>
          /// <param name="value"></param>
          /// <returns></returns>
          public static string ComposeBinaryOperation(string variableLabel, string operandLabel, string binaryOperator)
          {
            binaryOperator = Regex.Escape(binaryOperator);
            return $"(?<{variableLabel}>\\w+)[ ]*{binaryOperator}[ ]*(?<{operandLabel}>\\w+)";
          }

          /// <summary>
          /// Composes an unary operation with a specific unary operator, counting the matches for the operator.
          /// For example, ("Mood", "++") will match "cool++" with groups ("Mood"="cool", "1"="++").
          /// </summary>
          /// <param name="variableLabel"></param>
          /// <param name="unaryOperator"></param>
          /// <returns></returns>
          public static string ComposeUnaryOperation(string variableLabel, string operatorLabel, char unaryOperator)
          {
            return $"(?<{variableLabel}>\\w+)[ ]*(?<{operatorLabel}>[{unaryOperator}]+$)";
          }

          /// <summary>
          /// Composes the regular expression that will look for a file with a specific extension.
          /// FOr example ("Music", "ogg") will match "dropthebeat.ogg" with groups ("Music"="dropthebeat")
          /// </summary>
          /// <param name="label"></param>
          /// <param name="extension"></param>
          /// <returns></returns>
          public static string ComposeFileRequest(string label, string extension)
          {
            return $"(?<{label}>\\w+)[ ]*.{extension}";
          }

          /// <summary>
          /// Composes an assignment regex, with a specified key. (As in, it will only parse back the operand for a specific key)
          /// </summary>
          /// <param name="key"></param>
          /// <param name="op"></param>
          /// <returns></returns>
          public static string ComposeSpecificAssignment(string key, string op)
          {
            return $"(?({key})\\w+)[ ]*{op}[ ]*(?<Value>\\w+)";
          }




        }


      }

    }
  }
}
