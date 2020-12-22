using System.Collections;
using System.Collections.Generic;
using Ink;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace Stratus.Gameplay.Story.Examples
{
	public class SampleStoryReader : StratusStoryReader<StratusStoryRegexParser>
	{
		//------------------------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------------------------/
		protected override void OnBindExternalFunctions(StratusStory story)
		{
			story.runtime.BindExternalFunction("PlayMusic", new Action<string>(PlayMusic));
		}

		protected override void OnConfigureParser(StratusStoryRegexParser parser)
		{
			parser.AddPattern("Speaker", StratusStoryRegexParser.Presets.insideSquareBrackets, StratusStoryRegexParser.Target.Line, StratusStoryRegexParser.Scope.Default);
			parser.AddPattern("Message", StratusStoryRegexParser.Presets.insideDoubleQuotes, StratusStoryRegexParser.Target.Line, StratusStoryRegexParser.Scope.Default);

			// Variable = operand
			parser.AddPattern("Assignment", StratusStoryRegexParser.Presets.assignment, StratusStoryRegexParser.Target.Tag, StratusStoryRegexParser.Scope.Group, OnParse);
			// Variable++
			string incrementPattern = StratusStoryRegexParser.Presets.ComposeUnaryOperation("Variable", "Count", '+');
			parser.AddPattern("Increment", incrementPattern, StratusStoryRegexParser.Target.Tag, StratusStoryRegexParser.Scope.Group, OnParse);
			// Variable += value
			string incrementAssignPattern = StratusStoryRegexParser.Presets.ComposeBinaryOperation("Variable", "Value", "+=");
			parser.AddPattern("AddAssignment", incrementAssignPattern, StratusStoryRegexParser.Target.Tag, StratusStoryRegexParser.Scope.Group, OnParse);

		}

		void OnParse(StratusStoryParse parse)
		{
			StratusDebug.Log(parse.ToString());
		}

		//------------------------------------------------------------------------------------------/
		// External functions
		//------------------------------------------------------------------------------------------/
		public void PlayMusic(string trackName)
		{
			StratusDebug.Log("Playing music track '" + trackName + "'");
		}



	}
}
