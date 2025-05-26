Shader "Custom/UVReveal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0

        _LightDirection("Light Direction", Vector) = (0,0,1,0)
        _LightPosition("Light Position", Vector) = (0,0,0,0)
        _LightAngle("Light Angle", Range(0,180)) = 90
        _StrengthScalar("Strength", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Color;
            //float _Smoothness;
            //float _Metallic;

            float _LightAngle;
            float _StrengthScalar;
            float4 _LightPosition;
            float4 _LightDirection;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample base texture
                half4 texSample = tex2D(_MainTex, IN.uv);
                half3 albedo = texSample.rgb * _Color.rgb;
                half baseAlpha = texSample.a * _Color.a;

                // light direction
                float3 lightDir = normalize(_LightPosition.xyz - IN.worldPos);
                float3 lightForward = normalize(-_LightDirection.xyz);

                float spotDot = dot(lightDir, lightForward);
                float cosAngle = cos(radians(max(_LightAngle * 0.5, 0.1)));
                float inSpotlight = smoothstep(cosAngle - 0.05, 1.0, spotDot); 

                // float distance = length(lightDir);
                // float atten = smoothstep(10, 0, distance); // 10m max range

                // strength 
                float strength = inSpotlight /* * atten */ * _StrengthScalar;
                strength = saturate(strength);

                // Final Output
                // half3 finalColor = albedo;
                // half finalAlpha = baseAlpha * strength;
                //return half4(finalColor, finalAlpha);



                return half4(albedo, strength);
            }
            ENDHLSL
        }
    //     HLSLPROGRAM
    //     // Physically based Standard lighting model, and enable shadows on all light types
    //     #pragma surface SurfaceReveal Standard fullforwardshadows alpha:fade

    //     // Use shader model 3.0 target, to get nicer looking lighting
    //     #pragma target 3.0
    //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    //     sampler2D _MainTex;

    //     struct Input
    //     {
    //         float2 uv_MainTex;
    //         float3 worldPos;
    //     };

    //     half _Glossiness;
    //     half _Metallic;
    //     fixed4 _Color;
    //     float4 _LightDirection;
    //     float4 _LightPosition;
    //     float _LightAngle;
    //     float _StrengthScalor;

    //     // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    //     // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    //     // #pragma instancing_options assumeuniformscaling
    //     UNITY_INSTANCING_BUFFER_START(Props)
    //     // put more per-instance properties here
    //     UNITY_INSTANCING_BUFFER_END(Props)

    //     void SurfaceReveal (Input IN, inout SurfaceOutputStandard o)
    //     {
    //         float3 Dir = normalize(_LightDirection - IN.worldPos);
    //         float Scale = dot(Dir, _LightDirection);
    //         float Strength = Scale - cos(_LightAngle * (3.14 / 360.0));
    //         Strength = min(max(Strength * _StrengthScalor, 0), 1);
    //         fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //         o.Albedo = c.rgb;
    //         o.Emission = c.rgb * c.a * Strength;
    //         o.Metallic = _Metallic;
    //         o.Smoothness = _Glossiness;
    //         o.Alpha = Strength * c.a;
    //     }
    //     ENDHLSL
    }
    FallBack "Diffuse"
}
