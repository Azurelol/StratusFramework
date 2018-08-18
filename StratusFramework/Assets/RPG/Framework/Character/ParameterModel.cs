using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Altostratus.Models
{
  public interface StandardParameterModel : Character.ParameterModel
  {
    float physicalOffense { get; }
    float physicalDefense { get; }
    float magicalOffense { get; }
    float magicalDefense { get; }
    float speed { get; }
    float accuracy { get; }
    float evasion { get; }
    float movement { get; }
    float weaponRange { get; }
  }

}