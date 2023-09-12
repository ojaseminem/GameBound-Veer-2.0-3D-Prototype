Shader "Custom/Rock" {
    Properties {
        _MainTex ("Albedo", 2D) = "white" {}
        [Normal] _BumpMap("Normal", 2D) = "bump" {}
        [Normal] _DetailBump("Detail Normal", 2D) = "bump" {}
        _DetailScale("Detail Scale", Range(0, 10)) = 1.0
        [Normal] _OcclusionMap("Occlusion", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
        [Normal] _MetallicRough ("Metallic/Roughness (RGBA)", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0, 1)) = 0
        _Glossiness("Smoothness", Range(0, 1)) = 0.5
        [Normal] _TopAlbedo ("Top Albedo", 2D) = "white" {}
        [Normal] _TopNormal("Top Normal", 2D) = "bump" {}
        [Normal] _TopMetallicRough ("Metallic/Roughness (RGBA)", 2D) = "white" {}
        [Gamma] _TopMetallic("Top Metallic", Range(0, 1)) = 0
        _TopGlossiness("Top Smoothness", Range(0, 1)) = 0.5
        [Normal] _Noise ("Noise", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex, _TopAlbedo, _BumpMap, _TopNormal, _Noise, _OcclusionMap, _MetallicRough, _TopMetallicRough, _DetailBump;
            float4 _Top_ST;
            float4 _UVOffset;
            half _Glossiness, _FresnelAmount, _FresnelPower, _TopScale, _NoiseAmount, _NoiseFallOff, _Metallic, _TopMetallic, _TopGlossiness, _OcclusionStrength, _noiseScale;
            half _DetailScale;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.texcoord = v.texcoord;

                return o;
            }

            half3 blend_rnm(half3 n1, half3 n2) {
                n1.z += 1;
                n2.xy = -n2.xy;
                return n1 * dot(n1, n2) / n1.z - n2;
            }

            half4 frag(v2f i) : SV_Target {
                // top down UVs
                float2 uvY = i.vertex.xz * _TopScale;

                fixed4 noisetex = tex2D(_Noise, uvY * _noiseScale);
                half blend =  clamp(i.normal.y, 0 , 1);
                blend = smoothstep(noisetex.r, 1, blend);
                half noiseBlend = smoothstep(0.1, 0.2, blend);

                // tangent space normal map
                half3 tnormalY = UnpackNormal(tex2D(_TopNormal, uvY));
                half3 normalMain = UnpackNormal(tex2D(_BumpMap, i.texcoord.xy));
                half3 detailNormal = UnpackNormal(tex2D(_DetailBump, i.texcoord.xy * _DetailScale));
                normalMain = blend_rnm(normalMain, detailNormal);

                // flip normal maps' x axis to account for flipped UVs
                half3 absVertNormal = abs(i.normal);
                // swizzle world normals to match tangent space and apply reoriented normal mapping blend
                tnormalY = blend_rnm(half3(i.normal.xz, absVertNormal.y), tnormalY);
                // sizzle tangent normals to match world normal and blend together
                half3 worldNormal = normalize(tnormalY.xzy);

                //Albedo
                fixed4 colY = tex2D(_TopAlbedo, uvY);
                fixed4 colMain = tex2D(_MainTex, i.texcoord.xy);

                //Occlusion
                half occ =  lerp(1.f, tex2D(_OcclusionMap, i.texcoord.xy).r, (float)_OcclusionStrength);

                //Metallic/Smoothness
                half4 metallicSmoothness = tex2D(_MetallicRough, i.texcoord.xy);
                half4 TopMetallicSmoothness = tex2D(_TopMetallicRough, uvY);
                half m = lerp(metallicSmoothness.r * _Metallic, noisetex.r * _TopMetallic, noiseBlend);
                half s = lerp(metallicSmoothness.a * _Glossiness, TopMetallicSmoothness.a * _TopGlossiness, noiseBlend);

                // set surface ouput properties
                half3 finalNormal = lerp(normalMain, i.normal, noiseBlend);

                half3 albedo = lerp(colMain.rgb, colY.rgb, noiseBlend);
                
                half4 result;
                result.rgb = albedo;
                result.a = 1.0;
                return result;
            }
            ENDCG
        }
    }
}
