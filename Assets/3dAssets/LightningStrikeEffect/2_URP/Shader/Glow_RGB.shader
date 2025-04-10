Shader "LightningStrikeEffect/URP/Glow_RGB"
{
    Properties
    {
        _DepthFadeDistance("Fade Distance", float) = 1.0
        [Header(Texture)]
        _MainTex("Texture", 2D) = "white" {}
        _LightIntensity("LightIntensity", float) = 1.0




    }
        SubShader
        {

            Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
            HLSLINCLUDE
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                CBUFFER_START(UnityPerMaterial)

                    float4 _MainTex_ST;
                    float _LightIntensity;
                    float _DepthFadeDistance;
                CBUFFER_END
            ENDHLSL





            Pass
            {

                    //Blend SrcAlpha OneMinusSrcAlpha //AlphaBlend
                    Blend One One // Additive
                    //Blend OneMinusDstColor One // Soft additive
                    Cull Off
                    ZWrite Off



                    HLSLPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma multi_compile_fog
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

                    struct Attributes
                    {
                        half4 positionOS	: POSITION;
                        half2 uv : TEXCOORD0;
                        half4 color : COLOR;
                    };

                    struct Varyings
                    {
                        half4 color : COLOR;
                        half4 positionCS	: SV_POSITION;
                        float2 uv : TEXCOORD0;
                        real fogFactor : TEXCOORD1;
                        float3 positionVS : TEXCOORD2;
                        float4 screenPos : TEXCOORD3;

                    };

                    TEXTURE2D(_MainTex);
                    SAMPLER(sampler_MainTex);



                    Varyings vert(Attributes IN)
                    {
                        Varyings OUT;

                        VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                        OUT.positionCS = positionInputs.positionCS;
                        OUT.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                        OUT.color = IN.color;
                        OUT.uv = IN.uv;
                        float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                        OUT.positionVS = TransformWorldToView(positionWS);
                        OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                        return OUT;
                    }

                    half4 frag(Varyings IN) : SV_Target
                    {
                        half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(IN.uv, _MainTex));


                        col.rgb *= IN.color.rgb;
                        col.rgb *= _LightIntensity;
                        col.rgb *= IN.color.a;


                        float fragmentEyeDepth = -IN.positionVS.z;
                        float rawDepth = SampleSceneDepth(IN.screenPos.xy / IN.screenPos.w);
                        float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                        float depthFade = saturate((sceneEyeDepth - fragmentEyeDepth) / _DepthFadeDistance);
                        col.rgb *= depthFade;

                        col.rgb = MixFogColor(col.rgb, float3(0, 0, 0), IN.fogFactor);


                        return col;
                    }
                    ENDHLSL
                }
        }

}
