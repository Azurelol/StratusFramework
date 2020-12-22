using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
    public class StratusLabeledObject
    {
        public StratusLabeledObject(string label, object target)
        {
            this.label = label;
            this.target = target;
        }

        public string label { get; private set; }
        public object target { get; private set; }
    }

}