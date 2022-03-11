Shader "ENOZ/SurfaceLightmap"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_LightTex("LightTex", 2D) = "white" {}
		_LightBlend("LightmapBlend", Range(0,1)) = 0.5
		_Intensity("Intensity", Range(0,100)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

		Lighting On

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		//#include "./PBSLighting.cginc"
		#include "UnityPBSLighting.cginc"
        #pragma surface surf Standard2// fullforwardshadows //NoLighting

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _LightTex;
		float4 _LightTexST;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv2_LightTex;
        };

        half _Glossiness;
        half _Metallic;
		half _LightBlend;
		half _Intensity;
        fixed4 _Color;
		float newUV;

		//inline half4 LightingStandard(SurfaceOutputStandard s, float3 viewDir)
		//{
		//	//half NdotL = dot(s.Normal, lightDir);
		//	half4 c; c.rgb = viewDir;// *(NdotL * atten);
		//	c.a = s.Alpha;
		//	return c;
		//}

		//inline fixed4 LightingStandard_SingleLightmap(SurfaceOutputStandard s, fixed4 color)
		//{
		//	half3 lm = DecodeLightmap(tex2D(_LightTex, newUV));
		//	return fixed4(1 - lm, 0);
		//}

		//inline fixed4 LightingStandard_DualLightmap(SurfaceOutputStandard s, fixed4 totalColor, fixed4 indirectOnlyColor, half indirectFade)
		//{
		//	half3 lm = lerp(DecodeLightmap(indirectOnlyColor), DecodeLightmap(totalColor), indirectFade);
		//	return fixed4(1 - lm, 0);
		//}

		//inline fixed4 LightingStandard_StandardLightmap(SurfaceOutputStandard s, fixed4 color, fixed4 scale, bool surfFuncWritesNormal)
		//{
		//	UNITY_DIRBASIS

		//		half3 lm = DecodeLightmap(color);
		//		half3 scalePerBasisVector = DecodeLightmap(scale);

		//		if (surfFuncWritesNormal)
		//		{
		//			half3 normalInRnmBasis = saturate(mul(unity_DirBasis, s.Normal));
		//			lm *= dot(normalInRnmBasis, scalePerBasisVector);
		//		}

		//		return fixed4(1 - lm, 0);
		//}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		//fixed4 LightingNoLighting(SurfaceOutputStandard s, fixed3 lightDir, fixed atten)
		//{
		//	fixed4 c;
		//	c.rgb = s.Albedo;
		//	c.a = s.Alpha;
		//	return c;
		//}

		inline half4 LightingStandard2(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
		{
			s.Normal = normalize(s.Normal);

			half oneMinusReflectivity;
			half3 specColor;
			s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

			// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
			// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
			half outputAlpha;
			s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

			UnityLight newLight = gi.light;
			UnityIndirect newIn = gi.indirect;

			//newLight.color *= 0;
			//newLight.dir *= 0;
			//newLight.ndotl = 0;

			//newIn.diffuse *= 0;
			//newIn.specular *= 0;

			half4 c;// = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, newLight, newIn);
			c.rgb = s.Albedo +((1 - s.Albedo) * 2 * _Intensity * s.Smoothness * newIn.diffuse * newIn.specular);

			//half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, newLight, newIn);

			c.a = outputAlpha;
			return c;
		}

		inline void LightingStandard2_GI(
			SurfaceOutputStandard s,
			UnityGIInput data,
			inout UnityGI gi)
		{
#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
			gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
#else
			Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, lerp(unity_ColorSpaceDielectricSpec.rgb, s.Albedo, s.Metallic));

			UnityGIInput newData = data;

			//newData.ambient *= 0;
			//newData.worldViewDir *= 0;
			//newData.atten = 1;

			gi = UnityGlobalIllumination(newData, s.Occlusion, s.Normal, g);
#endif
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 l = tex2D(_LightTex, IN.uv2_LightTex) * _Color;
            //o.Albedo = (c.rgb * (1 - _LightBlend)) + (l.rgb * _LightBlend);
			//o.Albedo = c.rgb * l.rgb;
			//o.Albedo = (1 - (1 - c.rgb)*(1 - l.rgb));
			//o.Albedo = c.rgb + l.rgb;
			//o.Albedo = c.rgb;// +l.rgb;
			newUV = IN.uv2_LightTex * _LightTexST.xy + _LightTexST.zw;
			fixed3 d = DecodeLightmap(tex2D(_LightTex, IN.uv2_LightTex));

			float blendVal = 1 - _LightBlend;
			//o.Albedo = (c.rgb * (1-d.rgb)) + (c.rgb * d.rgb * _LightBlend);// DecodeLightmap(tex2D(_LightTex, IN.uv2_LightTex));
			o.Albedo = ((1 - d.rgb) * blendVal) * c.rgb + (d.rgb) * c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
