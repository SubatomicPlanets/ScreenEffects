Shader "ScreenEffects/CRTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Curvature ("Curvature", Range(0, 0.5)) = 0.1
        _ScanlineStrength ("Scanline Strength", Range(0, 0.5)) = 0.2
        _NoiseStrength ("Noise Strength", Range(0, 0.3)) = 0.1
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.5
        _ColorOffset ("Color Offset", Range(0, 0.01)) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Curvature;
            float _ScanlineStrength;
            float _NoiseStrength;
            float _VignetteStrength;
            float _ColorOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Barrel distortion
                float2 center = i.uv - 0.5;
                float dist = length(center);
                float2 distortedUV = i.uv + center * (dist * dist * _Curvature);

                // Clamp UVs to avoid sampling outside the texture
                distortedUV = clamp(distortedUV, 0.0, 1.0);

                // Sample the screen texture with color offset
                float2 uvR = distortedUV + float2(_ColorOffset, 0);
                float2 uvB = distortedUV - float2(_ColorOffset, 0);
                fixed4 col;
                col.r = tex2D(_MainTex, uvR).b;
                col.g = tex2D(_MainTex, distortedUV).g;
                col.b = tex2D(_MainTex, uvB).r;
                col.a = 1.0;

                // Scanlines
                float scanline = sin(distortedUV.y * 1200.0) * _ScanlineStrength;
                col.rgb *= (1.0 - scanline);

                // Noise
                float noise = random(distortedUV * _Time.y) * _NoiseStrength;
                col.rgb += noise;

                // Vignette
                float vignette = 1.0 - _VignetteStrength * dist * dist * 4.0;
                col.rgb *= vignette;

                return col;
            }
            ENDCG
        }
    }
}