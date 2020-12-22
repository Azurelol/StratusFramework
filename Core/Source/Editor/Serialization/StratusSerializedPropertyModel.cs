using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// An abstract model for a property serializd either by Unity or by
	/// custom serialization
	/// </summary>
	public class StratusSerializedPropertyModel
	{
		public enum SerializationType
		{
			Unity,
			Custom
		}

		public struct Query
		{
			public StratusSerializedPropertyModel[] models;
			public SerializedProperty[] unitySerialized;
			public StratusSerializedField[] customSerialized;

			public Query(SerializedObject serializedObject, Type declaringType)
			{
				FieldInfo[] fields = declaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				List<StratusSerializedPropertyModel> propertyModels = new List<StratusSerializedPropertyModel>();
				List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
				List<StratusSerializedField> serializedFields = new List<StratusSerializedField>();

				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo field = fields[i];
					if (field != null && (field.Attributes != FieldAttributes.NotSerialized))
					{
						bool serializedbyOdin = OdinSerializer.UnitySerializationUtility.OdinWillSerialize(field, true);
						bool serializedByUnity = OdinSerializer.UnitySerializationUtility.GuessIfUnityWillSerialize(field);


						//Debug.LogWarning($"{field.Name} is serialized by odin?{serializedbyOdin} unity?{serializedByUnity}");


						// Odin
						if (serializedbyOdin && !serializedByUnity)
						{
							StratusSerializedField serializedField = new StratusSerializedField(field, serializedObject.targetObject);
							serializedFields.Add(serializedField);
							propertyModels.Add(new StratusSerializedPropertyModel(serializedField));
						}
						// Unity
						else
						{
							SerializedProperty property = serializedObject.FindProperty(field.Name);
							if (property != null)
							{
								serializedProperties.Add(property);
								propertyModels.Add(new StratusSerializedPropertyModel(property, field));
							}
						}

					}
				}

				this.models = propertyModels.ToArray();
				this.unitySerialized = serializedProperties.ToArray();
				this.customSerialized = serializedFields.ToArray();
			}

		}

		public SerializationType type { get; private set; }
		public SerializedProperty unitySerialized { get; private set; }
		public StratusSerializedField customSerialized { get; private set; }
		public FieldInfo field { get; private set; }
		public bool isExpanded
		{
			get => (this.type == SerializationType.Unity) ? this.unitySerialized.isExpanded : this.customSerialized.isExpanded;

			set
			{
				switch (this.type)
				{
					case SerializationType.Unity:
						this.unitySerialized.isExpanded = value;
						break;
					case SerializationType.Custom:
						this.customSerialized.isExpanded = value;
						break;
				}
			}
		}

		public Attribute[] attributes => field.GetAttributes().ToArray();

		public string displayName => (this.type == SerializationType.Unity) ? this.unitySerialized.displayName : this.customSerialized.displayName;

		public StratusSerializedPropertyModel(SerializedProperty serializedProperty, FieldInfo field)
		{
			this.unitySerialized = serializedProperty;
			this.field = field;
			this.type = SerializationType.Unity;
		}

		public StratusSerializedPropertyModel(StratusSerializedField serializedField)
		{
			this.customSerialized = serializedField;
			this.field = serializedField.field;
			this.type = SerializationType.Custom;
		}
	}

	

}