using UnityEngine;

namespace Voxelgon {
    public static class Colormaps {
        //keeps track of colormaps for flatly colored "vector style" meshes

        public static Mesh ColorMesh(Mesh colorMesh, Texture2D colormap) {
            //colors a meshe's vector-colors to match its UV mapping over a colormap
            Vector2[] uvs = colorMesh.uv;
            Color32[] colors = new Color32[uvs.Length];
            int textureW = colormap.width;
            int textureH = colormap.height;

            for(int i = 0; i < uvs.Length; i++) {
                int pixelX = (int) ((uvs[i].x % 1) * textureW);
                int pixelY = (int) ((uvs[i].y % 1) * textureW);
                Color32 color = colormap.GetPixel(pixelX, pixelY);
                colors[i] = color;
            }
            colorMesh.colors32 = colors;

            return colorMesh;
        }
    }
}
