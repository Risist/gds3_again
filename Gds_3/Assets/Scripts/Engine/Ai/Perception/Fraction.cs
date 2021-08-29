using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    [CreateAssetMenu(fileName = "Fraction", menuName = "Ris/Ai/Fraction")]
    public class Fraction : ScriptableObject
    {
        public enum EAttitude
        {
            EFriendly,
            ENeutral,
            EEnemy,
            ENone
        }

        public Color fractionColorUi;
        public List<Fraction> friendlyFractions;
        public List<Fraction> enemyFractions;

        public EAttitude GetAttitude(Fraction fraction)
        {
            if (Equals(fraction))
                return EAttitude.EFriendly;

            if(friendlyFractions.Contains(fraction))
                return EAttitude.EFriendly;

            if (enemyFractions.Contains(fraction))
                return EAttitude.EEnemy;

            return EAttitude.ENeutral;
        }
    }
}
