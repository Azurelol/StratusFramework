using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
    public class StratusCoroutineRunner : StratusSingletonBehaviour<StratusCoroutineRunner>
    {
        public static Coroutine Run(IEnumerator routine) 
        {
            return instance.StartCoroutine(routine);
        }

        protected override void OnAwake()
        {
        }
    }

}