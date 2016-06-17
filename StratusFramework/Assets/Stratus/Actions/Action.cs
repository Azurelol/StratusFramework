/******************************************************************************/
/*!
@file   Action.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/22/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{

  /**************************************************************************/
  /*!
  @class Action is the base class from which all other actions derive from.
  */
  /**************************************************************************/
  public abstract class Action
  {
    //--------------------------/
    static int Created = 0;
    static int Destroyed = 0;
    //--------------------------/
    public string Type;
    public int ID;
    public float Elapsed = 0.0f;
    public float Duration = 0.0f;
    public bool Finished = false;
    //--------------------------/

    public Action(string type) 
    {
      this.Type = type;
      this.ID = Created++;
    }

    ~Action()
    {
      Destroyed++;
    }


    public abstract float Update(float dt);

  }


}

