#define SETUP_DITHER sampler2D _TBlueNoise;
#define DITHER(texcoord) ((tex2D(_TBlueNoise, texcoord * 21.9 + _Time) * 2) - 1) / 256;
