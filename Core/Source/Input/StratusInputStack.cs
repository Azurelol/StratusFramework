using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
   /// <summary>
   /// Manages pushing/popping input layers in a customized manner for the system
   /// </summary>
   /// <typeparam name="InputLayer"></typeparam>
    public class StratusInputStack<InputLayer>
        where InputLayer : StratusInputLayer
    {
        private Stack<InputLayer> _layers = new Stack<InputLayer>();
        private Queue<InputLayer> _queuedLayers = new Queue<InputLayer>();

        public InputLayer activeLayer => _layers.Count > 0 ? _layers.Peek() : null;
        public bool canPop => activeLayer != null && !activeLayer.pushed;
        public int layerCount => _layers.Count;
        public bool hasActiveLayers => layerCount > 0;
        public bool hasQueuedLayers => _queuedLayers.NotEmpty();

        public event Action<InputLayer> onInputLayerChanged;

        public StratusValidation Push(InputLayer layer)
        {
            return Push(layer, true);
        }

        private StratusValidation Push(InputLayer layer, bool update)
        {
            if (hasActiveLayers && !layer.ignoreBlocking)
            {
                // If the current layer is blocking, queue this layer for later
                if (activeLayer.blocking)
                {
                    _queuedLayers.Enqueue(layer);
                    return new StratusValidation(false, $"Active layer {activeLayer.label} is blocking. Queuing...");
                }
                activeLayer.active = false;
            }

            layer.pushed = true;
            if (update)
            {
                ActivateInputLayer(layer);
            }
            _layers.Push(layer);
            return true;
        }

        public InputLayer Pop()
        {
            InputLayer layer = null;


            // If there's layers remaining, remove the topmost
            if (hasActiveLayers)
            {
                layer = _layers.Pop();
                layer.active = false;
                layer.pushed = false;
            }

            bool queue = hasQueuedLayers && 
                (!hasActiveLayers || (hasActiveLayers && !activeLayer.blocking));

            StratusDebug.Log($"Popped {layer}. Check queue? {queue}");

            // If there's still layers left
            if (queue)
            {
                // If there are queud layers
                // and the topmost is not blocking
                if (hasQueuedLayers)
                {
                    while (_queuedLayers.NotEmpty())
                    {
                        layer = _queuedLayers.Dequeue();
                        bool blocking = layer.blocking;
                        StratusDebug.Log($"Popped layer {layer}, blocking ? {blocking}");
                        Push(layer, !blocking);
                        if (blocking)
                        {
                            StratusDebug.Log($"Breaking");
                            break;
                        }
                    }
                }
            }

            // Update the current layer if its active
            if (hasActiveLayers)
            {
                if (activeLayer.pushed)
                {
                    ActivateInputLayer(activeLayer);
                }
                else
                {
                    StratusDebug.LogError("Layer not enabled???");
                }
            }

            return layer;
        }

        private void ActivateInputLayer(InputLayer inputLayer)
		{
            inputLayer.active = true;
            onInputLayerChanged?.Invoke(inputLayer);
        }

    }

}