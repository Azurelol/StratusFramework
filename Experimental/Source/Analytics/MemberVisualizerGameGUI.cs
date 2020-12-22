using System.Collections.Generic;
using UnityEngine;


namespace Stratus
{
	[StratusSingleton("Member Visualizer Game GUI", true, true)]
	public class MemberVisualizerGameGUI : StratusLayoutGameViewWindow<MemberVisualizerGameGUI>
	{
		protected override bool draw => MemberVisualizer.gameGUIDrawCount > 0;
		protected override string title { get; } = "Member Visualizer";

		protected override void OnAwake()
		{

		}

		protected override void OnGUILayout(Rect position)
		{
			foreach (KeyValuePair<MemberVisualizer, MemberVisualizer.DrawList> drawList in MemberVisualizer.gameGUIDrawLists)
			{
				foreach (KeyValuePair<GameObject, List<MemberVisualizer.MemberVisualizationField>> dl in drawList.Value)
				{
					GameObject go = dl.Key;
					GUILayout.Label($"{go.name}", StratusGUIStyles.header);
					foreach (MemberVisualizer.MemberVisualizationField member in dl.Value)
					{
						//if (useCustomColors)
						//  GUILayout.Label($"<color={member.hexColor}>{member.description}</color>");
						//else
						GUILayout.Label($"{member.description}", StratusGUIStyles.miniText);
					}
				}
			}
		}

		public void Add(MemberVisualizer mv) { }
		public void Remove(MemberVisualizer mv) { }

	}

}