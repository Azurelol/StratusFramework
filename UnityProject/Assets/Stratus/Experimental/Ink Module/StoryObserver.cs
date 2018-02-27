using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// Observers a variable insided an ink story
      /// </summary>
      public class StoryObserver : Triggerable
      {
        public enum Type { Observe, Retrieve };

        public Type type = Type.Observe;
        public StoryReader reader;
        public Story.Variable variable;
        
        // Whether the variable observer has been set up
        private bool setObserver = false;

        protected override void OnAwake()
        {
          reader.gameObject.Connect<Story.EndedEvent>(this.OnStoryEndedEvent);
        }

        protected override void OnReset()
        {

        }

        protected override void OnTrigger()
        {
          switch (type)
          {
            case Type.Observe:
              ObserveValue();
              break;
            case Type.Retrieve:
              RetrieveValue();
              break;
            default:
              break;
          }
        }

        void OnStoryEndedEvent(Story.EndedEvent e)
        {
          //var removeEvent = new Story.RemoveVariableObserverEvent();
          //removeEvent.variableObserver = OnValueChanged;
          //reader.gameObject.Dispatch<Story.RemoveVariableObserverEvent>(removeEvent);
        }

        public void OnValueChanged(string variableName, object value)
        {
          switch (variable.type)
          {
            case Story.Types.Integer:
              variable.intValue = (int)value;
              break;
            case Story.Types.Boolean:
              variable.boolValue = (bool)value;
              break;
            case Story.Types.String:
              variable.stringValue = (string)value;
              break;
            case Story.Types.Float:
              variable.floatValue = (float)value;
              break;
            default:
              break;
          }

          PrintValue();
        }

        void ObserveValue()
        {
          if (setObserver)
            return;

          var observeEvent = new Story.ObserveVariableEvent();
          observeEvent.variableName = variable.name;
          observeEvent.variableObserver = OnValueChanged;
          reader.gameObject.Dispatch<Story.ObserveVariableEvent>(observeEvent);
          setObserver = true;
        }

        void RetrieveValue()
        {
          var findValueEvent = new Story.RetrieveVariableValueEvent();
          findValueEvent.variable = this.variable;
          this.gameObject.Dispatch<Story.RetrieveVariableValueEvent>(findValueEvent);

          PrintValue();
        }

    

        void PrintValue()
        {
          if (logging)
            Trace.Script($"The variable {variable.name} of type {variable.type} has a value of { variable.objectValue}", this);
        }

      }

    } 
  }
}