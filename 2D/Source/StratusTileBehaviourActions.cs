using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    public abstract class StratusTileBehaviourAction<BehaviourType> : StratusUndoAction
        where BehaviourType : IStratusTileBehaviour
    {
        protected StratusTileBehaviourAction(BehaviourType behaviour)
        {
            this.behaviour = behaviour;
        }

        public BehaviourType behaviour { get; private set; }
    }

    public class StratusTileBehaviourMoveAction<BehaviourType>: StratusTileBehaviourAction<BehaviourType>
        where BehaviourType : IStratusTileBehaviour
    {
        public Vector3Int sourcePosition { get; private set; }
        public Vector3Int targetPosition { get; private set; }

        public StratusTileBehaviourMoveAction(BehaviourType behaviour, Vector3Int sourcePosition, Vector3Int targetPosition)
            : base(behaviour)
        {
            this.sourcePosition = sourcePosition;
            this.targetPosition = targetPosition;
            this.description = $"Move {behaviour} from {sourcePosition} to {targetPosition}";
        }

        protected override void OnUndo(Action onFinished = null)
        {
            behaviour.MoveToPosition(sourcePosition, false, onFinished);
        }

        protected override void OnExecute(Action onFinished = null)
        {
            behaviour.MoveToPosition(targetPosition, true, onFinished);
        }
    }

}