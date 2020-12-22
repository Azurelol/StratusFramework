using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace Stratus.Gameplay.Story
{
    public class StratusStoryChoice
    {
        public StratusStoryChoice(Choice choice)
        {
            this.choice = choice;
        }

        public string text => choice.text;
        public int index => choice.index;
        public Choice choice { get; private set; }
    }

}