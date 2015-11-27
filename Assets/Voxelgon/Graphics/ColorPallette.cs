using UnityEngine;
using System;
using System.Collections;
using Voxelgon;

namespace Voxelgon.Graphics {
	public static class ColorPallette {

		public static readonly Color grid = HexColor(0x7CFFFF);
		public static readonly Color gridHover = HexColor(0x92FFA8);
		public static readonly Color gridSelected = HexColor(0xFF5252);
		public static readonly Color gridSelectedHover = HexColor(0xFFA800);

		public static Color HexColor(byte r, byte g, byte b, byte a) {
			return new Color((int) r / 255f, (int) g / 255f, (int) b / 255f, (int) a / 255f);
		}

		public static Color HexColor(byte r, byte g, byte b) {
			return HexColor(r, g, b, 0xFF);
		}

		public static Color HexColor(int hex) {
			byte b = (byte) (hex & 0xFF) ;
			hex >>= 8;
			byte g = (byte) (hex & 0xFF);
			hex >>= 8;
			byte r = (byte) (hex & 0xFF);
			return HexColor(r, g, b);
		}

		public static Color HexColor(string hexString) {
			int hex = Convert.ToInt16(hexString, 16);
			return HexColor(hex);
		}

	}
}