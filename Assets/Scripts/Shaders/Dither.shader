Shader "Hidden/Dither" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    // https://www.youtube.com/watch?v=8wOUe32Pt-E

    SubShader {

        CGINCLUDE
            #include "UnityCG.cginc"

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Texture2D _MainTex; // seperating texture from sampler; normal shaders just use sampler2D, which is both at once.
            SamplerState point_clamp_sampler; // Unity auto-recognizes "point_clamp_sampler" as a field that contains a point clamp sampler. 
                                              // This means that it gets autofilled with a point clamp sampler. Hella wild.
            float4 _MainTex_TexelSize; // black unity magic: https://forum.unity.com/threads/_maintex_texelsize-whats-the-meaning.110278/

            v2f vp(VertexData v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            float _Spread;
            int _RedColorCount, _GreenColorCount, _BlueColorCount , _BayerLevel;

            static const int bayer2[2 * 2] = {
                0, 2,
                3, 1
            };

            float GetBayer2(float2 uv) {
                return float(bayer2[(uv.x % 2) + (uv.y % 2) * 2]) * (1.0f / 4.0f) - 0.5f;
            }

            static const int bayer4[4 * 4] = {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 5
            };

            float GetBayer4(float2 uv) {
                return float(bayer4[(uv.x % 4) + (uv.y % 4) * 4]) * (1.0 / 16.0) - 0.5;
            }

            static const int bayer8[8 * 8] = {
                0, 32, 8, 40, 2, 34, 10, 42,
                48, 16, 56, 24, 50, 18, 58, 26,  
                12, 44,  4, 36, 14, 46,  6, 38, 
                60, 28, 52, 20, 62, 30, 54, 22,  
                3, 35, 11, 43,  1, 33,  9, 41,  
                51, 19, 59, 27, 49, 17, 57, 25, 
                15, 47,  7, 39, 13, 45,  5, 37, 
                63, 31, 55, 23, 61, 29, 53, 21
            };

            float GetBayer8(float2 uv) {
                return float(bayer8[(uv.x % 8) + (uv.y % 8) * 8]) * (1.0f / 64.0f) - 0.5f;
            }

            fixed4 fp(v2f i) : SV_Target {
            
                float4 col = _MainTex.Sample(point_clamp_sampler, i.uv); // point_clamp_sampler

                int x = round(i.uv.x * _MainTex_TexelSize.z) + 0.5;  // width  
                int y = round(i.uv.y * _MainTex_TexelSize.w) + 0.5;  // height

                float4 output = col + _Spread * GetBayer4(float2(x,y));

                output.r = floor((_RedColorCount - 1.0) * output.r + 0.5) / (_RedColorCount - 1.0);
                output.g = floor((_GreenColorCount - 1.0) * output.g + 0.5) / (_GreenColorCount - 1.0);
                output.b = floor((_BlueColorCount - 1.0) * output.b + 0.5) / (_BlueColorCount - 1.0);

                // makes things a bit more red
                output.r = clamp(output.r * (1 + (1 - output.r)), 0.0, 1.0);

                // Bug time
                // it's clear to me that the reason why this stuff is breaking is because whenever you
                // change the rendering window size, some pixels bug out. I think this is because
                // the MainText TexelSize.z/.w multiplied by the uv produces some weird integers that might
                // or might not fuck stuff up considering this algo relies on exact ints.
                // how to solve this, idk. i need to make the numbers match a 16:9 ratio.
                //
                // ok i think this occurs because point sampling combined with division downscaling
                // leads to the 16/9 ratio getting slightly off, which leads to random pixel discoloration.


                return output;
            }
            ENDCG
        }


        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            fixed4 fp(v2f i) : SV_Target {
                return _MainTex.Sample(point_clamp_sampler, i.uv);
            }
            ENDCG
        }
    }
}