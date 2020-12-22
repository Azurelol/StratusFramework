using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Stratus.UI
{
    public abstract class StratusInputLayerActionButton<LayerActionType, PlayerInputType> : StratusBehaviour
        where LayerActionType : System.Enum
        where PlayerInputType : IStratusPlayerInput
    {
        [SerializeField]
        private LayerActionType inputAction;
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private string label;

        public abstract PlayerInputType playerInput { get; }

        private void Start()
        {
            text.text = label.IsValid() ? label : inputAction.ToString();
            if (playerInput != null)
            {
                playerInput.onInputSchemeChanged += this.OnInputSchemeChanged;
                UpdateContents();
            }
        }

        private void OnInputSchemeChanged(StratusInputScheme scheme)
        {
            UpdateContents();
        }

        private void UpdateContents()
        {
            StratusInputActionDecorator decorator = playerInput.GetActionDecorator(inputAction.ToString());
            if (decorator != null)
            {
                image.sprite = decorator.sprite;
            }
            else
            {
                image.sprite = null;
            }
        }
    }

}