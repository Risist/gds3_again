using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class UtilityMathHelpers
{
    public static float ScallingUpTo(float time, float utilityScale,  RangedFloat clamp)
    {
        float utility = time * utilityScale;
        return clamp.Clamp(utility);
    }
    public static float ScallingUpTo(float time, float utilityScale, float max)
    {
        float utility = time * utilityScale;
        return Math.Min(utility, max);
    }

    public static float LoosingValue(float time, float toZeroInSeconds, float baseUtility, float minUtility = 0, float startingTime = 0)
    {
        float f = 1 - (time - startingTime) / toZeroInSeconds;
        return minUtility + f * (baseUtility - minUtility);
    }

    public static float ConditionalFactor(bool condition, float value, float _default = 0.0f)
    {
        return condition ? value : _default;
    }

}
