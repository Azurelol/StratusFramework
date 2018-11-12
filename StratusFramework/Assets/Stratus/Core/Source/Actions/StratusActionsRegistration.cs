using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Whenever an Monobehaviour on a GameObject creates an Action,
  /// this component is attached to it at runtime in order to handle book-keeping.
  /// It is only destroyed at the moment the GameObject is being destroyed.
  /// </summary>
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class StratusActionsRegistration : MonoBehaviour
  {
    private GameObject owner { get; set; }

    private void Awake()
    {
      this.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

      if (Application.isPlaying)
      {
        this.owner = this.gameObject;
        StratusActionSpace.Subscribe(this.owner);
      }
    }

    void OnDestroy()
    {
      if (Application.isPlaying)
      {
        StratusActionSpace.Unsubscribe(this.owner);
      }
    }

  }

}