namespace Voxelgon.Util {
    public class MiscUtil {
        public static void Swap<T>(ref T v1, ref T v2) {
            var temp = v1;
            v1 = v2;
            v2 = temp;
        }
    }
}