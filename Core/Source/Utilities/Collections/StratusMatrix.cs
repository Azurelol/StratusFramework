using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    public abstract class StratusEnumRelationshipMatrix<RelationType, ValueType> 
        where RelationType : Enum
    {
        public class RelationValue
        {
            public RelationType source;
            public RelationType target;
            public ValueType value;
            public bool symmetrical = true;

            public RelationValue(RelationType source, RelationType target, ValueType value)
            {
                this.source = source;
                this.target = target;
                this.value = value;
            }
        }

        private ValueType[,] relationships { get; set; }
        private Dictionary<string, RelationType> enumValueDict { get; set; }
        private Type enumType { get; set; }

        public RelationType[] relations { get; private set; }
        public int relationCount { get; private set; }

        /// <summary>
        /// Value to self when the relation is to itself. 
        /// In a 2d rectangular matrix, this would be the cells where r == c (diagonals)
        /// </summary>
        protected abstract ValueType defaultValue { get; }
        protected abstract ValueType selfValue { get; }

        public StratusEnumRelationshipMatrix()
        {
            enumType = typeof(RelationType);
            relations = Enum.GetValues(enumType).ToArrayByCast<RelationType>();
            relationCount = relations.Length;

            enumValueDict = new Dictionary<string, RelationType>();
            foreach (var relation in relations)
            {
                enumValueDict.Add(relation.ToString(), relation);
            }
            Reset();
        }

        public StratusEnumRelationshipMatrix(IEnumerable<RelationValue> values)
            : this()
        {
            Set(values);
        }

        private void Reset()
        {
            relationships = new ValueType[relationCount, relationCount];
            for (int i = 0; i < relationCount; i++)
            {
                for (int j = 0; j < relationCount; j++)
                {
                    relationships[i, j] = (i == j) ? selfValue : defaultValue;
                }
            }
        }        

        public void Set(IEnumerable<RelationValue> relations)
        {
            foreach(var relation in relations)
            {
                Set(relation.source, relation.target, relation.value, relation.symmetrical);
            }
        }

        public void Set(RelationType source, RelationType target, ValueType relationship, bool symmetrical = true)
        {
            int sourceIndex = (int)(object)source;
            int targetIndex = (int)(object)target;
            relationships[sourceIndex, targetIndex] = relationship;
            if (symmetrical)
            {
                relationships[targetIndex, sourceIndex] = relationship;
            }
        }

        public ValueType GetRelation(RelationType source, RelationType target)
        {
            int sourceIndex = (int)(object)source;
            int targetIndex = (int)(object)target;
            return relationships[sourceIndex, targetIndex];
        }

        public ValueType GetRelation(string source, string target)
        {
            return GetRelation(enumValueDict[source], enumValueDict[target]);
        }
    }

}