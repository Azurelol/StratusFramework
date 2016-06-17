/******************************************************************************/
/*!
@file   PuzzleActivation.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Overworld
{
  /**************************************************************************/
  /*!
  @class PuzzleActivation 
  */
  /**************************************************************************/
  public class PuzzleActivation : MonoBehaviour
  {
    public bool Debugging = true;
    private Boolean Flashing = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      this.gameObject.Connect<PlayerInteract.InteractEvent>(this.OnInteractEvent);
      this.gameObject.Connect<PlayerInteract.ScanEvent>(this.OnScanEvent);
    }

    void OnScanEvent(PlayerInteract.ScanEvent e)
    {
      e.Context = "Activate";
    }


    void OnInteractEvent(PlayerInteract.InteractEvent e)
    {
      if (!Flashing)
      {       
        var renderer = this.gameObject.GetComponent<MeshRenderer>();
        ActionEffects.Flash(renderer, Color.red, this.Flashing);
        var transition = new CameraController.Transition();
        transition.Duration = 0.5f;
        transition.Return = true;
        DispatchCameraEvent.LookAt(this.gameObject, transition);
      }
    }

    void FlashMaterial(Material nextMaterial, float duration)
    {
      var seq = Actions.Sequence(this.gameObject.Actions());
      //Actions.Call(seq, this.ChangeMaterial(this.gameObject, nextMaterial));
    }

    void ChangeMaterial(GameObject obj, Material material)
    {
      obj.GetComponent<MeshRenderer>().material = material;
    }
  
}

}