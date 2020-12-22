using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
    public static class StratusCombatFormulas
    {
        /// <summary>
        /// When attack and defense are roughly the same, there is roughly half as much damage as attack.
        /// This is true no matter how large the values are.
        /// Even when the defense is pathetic compared to the attack value, there is never more damage than 
        /// attack.This gives you an upper limit on how much damage a character can inflict, which makes balancing far easier.
        /// On the other extreme, no matter how high the defense gets, it can never completely mitigate damage (except through rounding errors), 
        /// so there is always room for improvement for the defender and there is never a completely pointless attack.
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="defense"></param>
        /// <returns></returns>
        public static float CalculateDamage(float attack, float defense)
        {
            return (attack * attack) / (attack + defense);
        }

        /// <summary>
        /// Calculated by executing 2 rolls. First, against accuracy, then second after evasion.
        /// </summary>
        /// <param name="accuracy"></param>
        /// <param name="evasion"></param>
        /// <returns></returns>
        public static bool CalculateHit(float accuracy, float evasion)
        {
            if (accuracy < 1.0f)
            {
                float roll = Random.Range(0f, 1f);
                StratusDebug.Log($"Roll = {roll}, Accuracy = {accuracy}");
                if (roll <= accuracy)
                {
                    return false;
                }
            }

            if (evasion > 0.0f)
            {
                float roll = Random.Range(0f, 1f);
                StratusDebug.Log($"Roll = {roll}, Evasion = {evasion}");
                if (roll <= evasion)
                {
                    return false;
                }
            }

            return true;
        }
    }

}