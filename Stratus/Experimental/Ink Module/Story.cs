using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ink.Runtime;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// A story represents an Ink file
      /// </summary>
      [Serializable]
      public class Story
      {
        //--------------------------------------------------------------------/
        // Fields
        //--------------------------------------------------------------------/
        /// <summary>
        /// The ink compiled JSON file
        /// </summary>
        public TextAsset file;
        /// <summary>
        /// What knot in the conversation to start on
        /// </summary>
        public string startingKnot = string.Empty;
        /// <summary>
        /// The currently saved state of the story
        /// </summary>
        [SerializeField]
        public string savedState;
        /// <summary>
        /// How many times this story has been started
        /// </summary>
        public int timesRead = 0;
        /// <summary>
        /// The name of the story file
        /// </summary>
        public string name => file.name;
        /// <summary>
        /// Whether this story has been saved previously
        /// </summary>
        public bool isSaved => string.IsNullOrEmpty(savedState);
        /// <summary>
        /// The runtime data structure used to drive the story
        /// </summary>
        public Ink.Runtime.Story runtime { get; set; }

        /// <summary>
        /// Reloads the state of the story from the saved json
        /// </summary>
        public void LoadState()
        {
          if (!runtime)
            runtime = new Ink.Runtime.Story(file.text);
          runtime.state.LoadJson(savedState);
        }

        //--------------------------------------------------------------------/
        // Definitions
        //--------------------------------------------------------------------/
        /// <summary>
        /// The Ink language provides 4 common value types
        /// </summary>
        public enum ValueType { Integer, Boolean, String, Float }

        /// <summary>
        /// The different types of story events there are
        /// </summary>
        public enum EventType
        {
          Loaded,
          Started,
          Continue,
          Ended
        }

        /// <summary>
        /// Represents a variable supported by Ink
        /// </summary>
        [Serializable]
        public class Variable
        {
          public ValueType type;
          public string name;

          public int intValue { get; set; }
          public bool boolValue { get; set; }
          public string stringValue { get; set; }
          public float floatValue { get; set; }
          public object objectValue
          {
            get
            {
              object value = null;
              switch (type)
              {
                case ValueType.Integer:                  
                  value = intValue;
                  break;
                case ValueType.Boolean:
                  value = boolValue;
                  break;
                case ValueType.String:
                  value = stringValue;
                  break;
                case ValueType.Float:
                  value = floatValue;
                  break;
              }

              return value;
            }
          }
        }

        //--------------------------------------------------------------------/
        // Events
        //--------------------------------------------------------------------/
        /// <summary>
        /// Signals that a story should be loaded
        /// </summary>
        public class LoadEvent : Stratus.Event
        {
          /// <summary>
          /// The story to be loaded
          /// </summary>
          public TextAsset storyFile;
        }

        /// <summary>
        /// Signals that a story has been loaded
        /// </summary>
        public class LoadedEvent : Stratus.Event
        {
          /// <summary>
          /// A reference to the story that has just been loaded
          /// </summary>
          public Story story;
        }

        /// <summary>
        /// Signals that a story has started
        /// </summary>
        public class StartedEvent : Stratus.Event
        {
          public GameObject readerObject;
        }

        /// <summary>
        /// Signals that a story has ended
        /// </summary>
        public class EndedEvent : Stratus.Event
        {
        }

        /// <summary>
        /// Signals that the story should continue onto the next node
        /// </summary>
        public class ContinueEvent : Stratus.Event
        {
        }

        /// <summary>
        /// Signals that a choice should be presented to the reader
        /// </summary>
        public class PresentChoicesEvent : Stratus.Event
        {
          public List<Choice> Choices = new List<Choice>();
        }

        /// <summary>
        /// Signals that a choice has been selected by the reader
        /// </summary>
        public class SelectChoiceEvent : Stratus.Event
        {
          public Choice choice;
        }

        /// <summary>
        /// Represents a parsed line of dialog. It may have multiple parses
        /// such as one for the speaker, another for the message, etc...
        /// </summary>
        public class UpdateLineEvent : Stratus.Event
        {
          /// <summary>
          /// A parse of the current line, containing specified metadata (based on the reader used)
          /// </summary>
          public ParsedLine parse;

          /// <summary>
          /// Whether this line has been previously visited  in the story
          /// </summary>
          public bool visited { get; private set; }

          public UpdateLineEvent(ParsedLine parse, bool visited)
          {
            this.parse = parse;
            this.visited = visited;
          }
        }

        /// <summary>
        /// Base class for events involving ink variables
        /// </summary>
        public abstract class VariableValueEvent : Stratus.Event
        {
          /// <summary>
          /// Represents a variable in Ink
          /// </summary>
          public Variable variable;
        }

        /// <summary>
        /// Retrieves the value of a specified variable from an ink story
        /// </summary>
        public class RetrieveVariableValueEvent : VariableValueEvent
        {
        }

        /// <summary>
        /// Retrieves the value of a specified variable from an ink story
        /// </summary>
        public class SetVariableValueEvent : VariableValueEvent
        {
        }

        /// <summary>
        /// Observes the state of a variable
        /// </summary>
        public class ObserveVariableEvent : Stratus.Event
        {
          public string variableName;
          public Ink.Runtime.Story.VariableObserver variableObserver;
        }

        /// <summary>
        /// Observes the state of a multiple variables. It will call the function
        /// once per each variable
        /// </summary>
        public class ObserveVariablesEvent : Stratus.Event
        {
          public string[] variableNames;
          public Ink.Runtime.Story.VariableObserver variableObserver;
        }

        public class RemoveVariableObserverEvent : Stratus.Event
        {
          public Ink.Runtime.Story.VariableObserver variableObserver;
        }

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        /// <summary>
        /// Dispatches an event in order to retrieve the value of a given variable.
        /// </summary>
        /// <typeparam name="EventType"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="story">The Ink story object.</param>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="target">The recipient of the event</param>
        static public ValueType GetVariableValue<ValueType>(Ink.Runtime.Story story, string variableName)
        {
          // Edge case
          if (typeof(ValueType) == typeof(bool))
          {
            var value = (int)story.variablesState[variableName];
            return (ValueType)System.Convert.ChangeType(value, typeof(ValueType));
          }

          return (ValueType)story.variablesState[variableName];
        }

        /// <summary>
        /// Dispatches an event in order to retrieve the value of a given variable.
        /// </summary>
        /// <typeparam name="EventType"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="story">The Ink story object.</param>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="target">The recipient of the event</param>
        public static void SetVariableValue<ValueType>(Ink.Runtime.Story story, string variableName, ValueType value)
        {
          story.variablesState[variableName] = value;
        }
        
      } 
    }

  }
}
