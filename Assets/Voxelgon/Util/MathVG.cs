using UnityEngine;

namespace Voxelgon.Util {

    public static class MathVG {

        // CONSTANTS

        public const float e = 2.7182818285f;


        // FUNCTIONS

        // computes the greatest common denominator (GCD) using Stein's algorithm
        public static uint GCD(uint value1, uint value2) {
            if (value1 == 0) return value2;
            if (value2 == 0) return value1;
            if (value1 == value2) return value1;
            bool value1IsEven = (value1 & 1u) == 0;
            bool value2IsEven = (value2 & 1u) == 0;

            if (value1IsEven && value2IsEven) {
                return GCD(value1 >> 1, value2 >> 1) << 1;
            } else if (value1IsEven && !value2IsEven) {
                return GCD(value1 >> 1, value2);
            } else if (value2IsEven) {
                return GCD(value1, value2 >> 1);
            } else if (value1 > value2) {
                return GCD((value1 - value2) >> 1, value2);
            } else {
                return GCD(value1, (value2 - value1) >> 1);
            }
        }

        public static float MaxAbs(float v1, float v2) {
            return (Mathf.Abs(v1) > Mathf.Abs(v2)) ? v1 : v2;
        }

        // Smoothstep

        public static float SmoothStep(float value) {
            value = Mathf.Clamp01(value);
            return value * value * (3.0f - (2.0f * value));
        }

        public static float SmootherStep(float value) {
            return SmoothStep(SmoothStep(value));
        }

        // see: "Improving Noise" by Ken Perlin
        public static float PerlinStep(float value) {
            value = Mathf.Clamp01(value);
            return value * value * value * (10 + value * (-15 + value * 6));
        }

        public static float Smexperp(float value, float n = e) {
            return SmoothStep(Experp(value, n));
        }

        // Ease out
        public static float Sinerp(float value) {
            value = Mathf.Clamp01(value);
            return Mathf.Sin(value * Mathf.PI * 0.5f);
        }

        public static float Sinerp(float start, float end, float value) {
            return Mathf.Lerp(start, end, Sinerp(value));
        }

        public static Vector2 Sinerp(Vector2 start, Vector2 end, float value) {
            return Vector2.Lerp(start, end, Sinerp(value));
        }

        public static Vector3 Sinerp(Vector3 start, Vector3 end, float value) {
            return Vector3.Lerp(start, end, Sinerp(value));
        }

        public static Color Sinerp(Color start, Color end, float value) {
            return Color.Lerp(start, end, Sinerp(value));
        }


        // Ease in
        public static float Coserp(float value) {
            value = Mathf.Clamp01(value);
            return 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f);
        }

        public static float Coserp(float start, float end, float value) {
            return Mathf.Lerp(start, end, Coserp(value));
        }

        public static Vector2 Coserp(Vector2 start, Vector2 end, float value) {
            return Vector2.Lerp(start, end, Coserp(value));
        }

        public static Vector3 Coserp(Vector3 start, Vector3 end, float value) {
            return Vector3.Lerp(start, end, Coserp(value));
        }

        public static Color Coserp(Color start, Color end, float value) {
            return Color.Lerp(start, end, Coserp(value));
        }

        // exponential

        public static float Experp(float value, float n = e) {
            value = Mathf.Clamp01(value);
            return (Mathf.Pow(n, value) - 1.0f) / (n - 1.0f);
        }

        public static float Experp(float start, float end, float value, float n = e) {
            return Mathf.Lerp(start, end, Experp(value, n));
        }

        public static Vector2 Experp(Vector2 start, Vector2 end, float value, float n = e) {
            return Vector2.Lerp(start, end, Experp(value, n));
        }

        public static Vector3 Experp(Vector3 start, Vector3 end, float value, float n = e) {
            return Vector3.Lerp(start, end, Experp(value, n));
        }

        public static Color Experp(Color start, Color end, float value, float n = e) {
            return Color.Lerp(start, end, Experp(value, n));
        }
    }
}