Shader "Andtech/Star Pack/Particle Standard Glow" {
    Properties {
        // Main Options
        [HDR] [Gamma] _Color("_Color", Color) = (1.0, 0.0, 0.0, 1.0)
        [NoScaleOffset]
        _MainTex("Glow Mask", 2D) = "white" {}
        _Brightness("Brightness", Range(0.0, 1.0)) = 1.0
        _TwinkleAmount("Twinkle Amount", Range(0.0, 1.0)) = 0.8
        _TwinkleSpeed("Twinkle Speed", Range(1.0, 10.0)) = 5.0
        
        // Soft Particles
        [HideInInspector] [Toggle] _SoftParticlesEnabled("_SoftParticlesEnabled", Float) = 0.0
        _SoftParticlesNearFadeDistance("_SoftParticlesNearFadeDistance", Float) = 0.0
        _SoftParticlesFarFadeDistance("_SoftParticlesFarFadeDistance", Float) = 1.0
    
        // Camera Fading
        [HideInInspector] [Toggle] _CameraFadingEnabled("_CameraFadingEnabled", Float) = 0.0
        _CameraNearFadeDistance("_CameraNearFadeDistance", Float) = 1.0
        _CameraFarFadeDistance("_CameraFarFadeDistance", Float) = 2.0
        
        // Twinkle Options
        [HideInInspector] [Toggle] _TwinkleEnabled("_TwinkleEnabled", Float) = 0.0
        
        // Internal
        [HideInInspector] _SoftParticleFadeParams("_SoftParticleFadeParams", Vector) = (0.0, 0.0, 0.0, 0.0)
        [HideInInspector] _CameraFadeParams("_CameraFadeParams", Vector) = (0.0, 0.0, 0.0, 0.0)
    }

    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        Blend SrcAlpha One
        Cull Off
        ZWrite Off

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            half _Brightness;
            half _TwinkleAmount;
            half _TwinkleSpeed;
            
            struct appdata {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.fogCoord = o.vertex.z;
                return o;
            }
            
            half4 frag(v2f i) : SV_Target {
                half4 mask = tex2D(_MainTex, i.uv);
                half4 col = mask.r * i.color + mask.g * _Brightness;
                col.a = i.color.a * mask.a;
            
                #ifdef SOFTPARTICLES_ON
                    float sceneDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, i.vertex.xy).r;
                    float softFade = saturate((sceneDepth - i.vertex.z) * 0.1);
                    col.a *= softFade;
                #endif
            
                return col;
            }
            ENDHLSL            
        }
    }

    Fallback "Transparent"
    CustomEditor "Andtech.StarPack.Editor.ParticleStandardGlowShaderGUI"
}
