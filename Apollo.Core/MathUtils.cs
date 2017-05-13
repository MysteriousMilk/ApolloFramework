// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

namespace Apollo.Core
{
    public static class MathUtils
    {
        /// <summary>
        /// Given a rotation in degrees, the rotation value will be clamped between 0 and 359 degrees inclusive.
        /// Overflow will be added on after the rotation is returned to 0.
        /// </summary>
        /// <param name="rotationDegrees"></param>
        /// <returns>The adjusted rotation value in degrees.</returns>
        /// <example>
        /// 370 => 10
        /// 360 => 0
        /// 359.9998 => 359.9998
        /// 360.001 => 0.001
        /// </example>
        public static float ClampRotation(float rotationDegrees)
        {
            if (rotationDegrees > 360.0f)
                rotationDegrees -= 360.0f;

            if (rotationDegrees == 360.0f)
                rotationDegrees = 0;

            return rotationDegrees;
        }
    }
}
