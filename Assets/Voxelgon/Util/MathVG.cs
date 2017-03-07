
namespace Voxelgon.Util {

    public static class MathVG {

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
    }
}