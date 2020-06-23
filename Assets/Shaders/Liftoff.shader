// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Liftoff"
{
	Properties
	{
		_ToonRamp("Toon Ramp", 2D) = "white" {}
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (0,0,0,0)
		_RimPower("Rim Power", Range( 0 , 1)) = 1
		_RimTint("Rim Tint", Color) = (1,1,1,0)
		_Falloff("Falloff", Float) = 0
		_Intensity("Intensity", Range( 0 , 2)) = 0
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Tranistionspecular("Tranistion specular", Range( 0 , 1)) = 0
		shadowMeter("Shadow Overide", Range( 0 , 1)) = 0
		colorMeter("Albedo Flood", Range( 0 , 1)) = 1
		_ColorBlock("Color Block", Color) = (1,0,0,0)
		frenSelect("Selection", Range( 0 , 1)) = 0
		_SelectionColou("Selection Colou", Color) = (1,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _SelectionColou;
		uniform float frenSelect;
		uniform float4 _AlbedoTint;
		uniform sampler2D _AlbedoMap;
		uniform float4 _AlbedoMap_ST;
		uniform sampler2D _ToonRamp;
		uniform float _RimPower;
		uniform float4 _RimTint;
		uniform float _Falloff;
		uniform sampler2D _SpecularMap;
		uniform float4 _SpecularMap_ST;
		uniform float _Tranistionspecular;
		uniform float _Intensity;
		uniform float shadowMeter;
		uniform float colorMeter;
		uniform float4 _ColorBlock;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV149 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode149 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV149, 5.0 ) );
			float2 uv_AlbedoMap = i.uv_texcoord * _AlbedoMap_ST.xy + _AlbedoMap_ST.zw;
			float4 Albedo23 = ( _AlbedoTint * tex2D( _AlbedoMap, uv_AlbedoMap ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult4 = dot( ase_worldNormal , ase_worldlightDir );
			float NormalLightDir9 = dotResult4;
			float2 temp_cast_0 = ((NormalLightDir9*0.5 + 0.5)).xx;
			float4 Shadow15 = ( Albedo23 * tex2D( _ToonRamp, temp_cast_0 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi32 = gi;
			float3 diffNorm32 = ase_worldNormal;
			gi32 = UnityGI_Base( data, 1, diffNorm32 );
			float3 indirectDiffuse32 = gi32.indirect.diffuse + diffNorm32 * 0.0001;
			float4 Lighting30 = ( Shadow15 * ( ase_lightColor * float4( ( indirectDiffuse32 + ase_lightAtten ) , 0.0 ) ) );
			float dotResult8 = dot( ase_worldNormal , ase_worldViewDir );
			float NormalViewDir10 = dotResult8;
			float4 Rim46 = ( saturate( ( pow( ( 1.0 - saturate( ( 0.5 + NormalViewDir10 ) ) ) , _RimPower ) * ( NormalLightDir9 * ase_lightAtten ) ) ) * ( ase_lightColor * _RimTint ) );
			float dotResult72 = dot( ( ase_worldViewDir + _WorldSpaceLightPos0.xyz ) , ase_worldNormal );
			float smoothstepResult77 = smoothstep( 1.1 , 1.12 , pow( dotResult72 , _Falloff ));
			float2 uv_SpecularMap = i.uv_texcoord * _SpecularMap_ST.xy + _SpecularMap_ST.zw;
			float4 color92 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 lerpResult95 = lerp( color92 , ase_lightColor , _Tranistionspecular);
			float4 Spec84 = ( ase_lightAtten * ( ( smoothstepResult77 * ( tex2D( _SpecularMap, uv_SpecularMap ) * lerpResult95 ) ) * _Intensity ) );
			float temp_output_136_0 = ( 1.0 - shadowMeter );
			float Shadowoveride144 = temp_output_136_0;
			c.rgb = ( ( _SelectionColou * ( ( frenSelect * pow( fresnelNode149 , 2.3 ) ) * 15.0 ) ) + ( max( ( ( Lighting30 + Rim46 ) + Spec84 ) , ( ( Shadowoveride144 / 2.25 ) * ( pow( colorMeter , 3.0 ) * _ColorBlock ) ) ) * pow( temp_output_136_0 , 3.0 ) ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
312;205;1280;675;811.7284;522.9865;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;12;-2750.52,361.8042;Inherit;False;861.2402;383.9924;Comment;4;6;7;8;10;Normal.View;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;11;-2672.58,-262.11;Inherit;False;799.6313;373.8002;Comment;4;5;3;4;9;Normal.Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;6;-2694.914,411.8041;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;7;-2700.52,557.7963;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;5;-2622.58,-71.30984;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;3;-2617.68,-212.11;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;8;-2371.901,522.9137;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-2113.28,501.9237;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-2328.581,-151.0098;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;55;-1463.771,1256.847;Inherit;False;1738.535;818.7089;Comment;17;46;49;50;48;47;56;54;52;53;51;45;44;43;42;40;41;39;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;24;-2649.249,-798.8635;Inherit;False;796.7937;477.1555;Comment;4;20;21;22;23;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1398.67,1306.847;Inherit;False;Constant;_RimOffset;Rim Offset;3;0;Create;True;0;0;False;0;False;0.5;0.55;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;85;-3319.619,2291.936;Inherit;False;2181.26;1127.367;Comment;22;96;81;87;92;93;94;95;84;83;82;80;77;88;74;79;78;72;75;73;68;69;66;Spec;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-1413.771,1424.381;Inherit;False;10;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-2554.945,-748.8635;Inherit;False;Property;_AlbedoTint;Albedo Tint;2;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-2599.249,-551.7081;Inherit;True;Property;_AlbedoMap;Albedo Map;1;0;Create;True;0;0;False;0;False;-1;None;58218fbbf65bdd54f996706a7c959596;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-2081.712,-153.1658;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;19;-1659.133,27.49875;Inherit;False;1072.712;480.7521;;7;26;17;15;25;14;18;13;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-1611.695,110.3731;Inherit;False;9;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;69;-3269.619,2507.036;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1149.669,1372.847;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;66;-3224.119,2341.937;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-2255.888,-627.0258;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1609.133,323.8369;Inherit;False;Constant;_ToonmapTiling;Toonmap Tiling;1;0;Create;True;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;42;-1013.669,1374.847;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;73;-3183.819,2634.437;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-2051.455,-634.5191;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2925.119,2449.837;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;18;-1391.117,284.2511;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;92;-3153.961,3002.853;Inherit;False;Constant;_Specularcolor;Specular color;10;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-1033.589,1473.967;Inherit;False;Property;_RimPower;Rim Power;3;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-1011.018,1643.83;Inherit;False;9;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1190.643,238.5143;Inherit;True;Property;_ToonRamp;Toon Ramp;0;0;Create;True;0;0;False;0;False;-1;10e567d6fd94a7d4bb631508c8050e49;0ad51d3af2bacb8439602317a1b2ec50;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;72;-2726.219,2561.635;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;94;-3196.159,3166.581;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;53;-1010.778,1718.762;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-1151.771,92.07541;Inherit;False;23;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;38;-2936.099,944.9615;Inherit;False;1274.319;525.06;Comment;8;31;32;28;33;34;29;30;27;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-2736.619,2683.836;Inherit;False;Property;_Falloff;Falloff;5;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;43;-861.6687,1375.847;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-3008.798,3262.796;Inherit;False;Property;_Tranistionspecular;Tranistion specular;8;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-2560.902,2815.238;Inherit;False;Constant;_Max;Max;8;0;Create;True;0;0;False;0;False;1.12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2557.002,2726.837;Inherit;False;Constant;_Min;Min;9;0;Create;True;0;0;False;0;False;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;31;-2765.103,1359.021;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;95;-2532.799,3090.626;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;74;-2469.322,2605.91;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-888.1192,103.3277;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;32;-2774.103,1292.021;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;44;-698.6686,1371.847;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;88;-2924.541,2816.131;Inherit;True;Property;_SpecularMap;Specular Map;7;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-765.6844,1665.153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;47;-490.3717,1672.258;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-775.8715,245.2913;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-516.0907,1385.716;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-2430.103,1298.021;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;28;-2740.696,1103.679;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;49;-533.6708,1872.099;Inherit;False;Property;_RimTint;Rim Tint;4;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;77;-2247.819,2620.137;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2556.331,2949.444;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2304.103,1197.021;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-2886.099,994.9615;Inherit;False;15;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-545.6943,792.498;Inherit;False;Property;shadowMeter;Shadow Overide;9;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-2033.826,2801.109;Inherit;False;Property;_Intensity;Intensity;6;0;Create;True;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-295.5256,1693.908;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-2059.804,2617.457;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;56;-332.4915,1385.51;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-137.3166,1397.474;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1885.825,2688.009;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;82;-1809.125,2589.209;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2199.553,1077.913;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;136;-267.3362,689.3093;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-1418.024,594.342;Inherit;False;Property;colorMeter;Albedo Flood;10;0;Create;False;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;38.74915,1369.63;Inherit;False;Rim;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1549.125,2690.609;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1885.781,1108.399;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;134;-1290.651,685.819;Inherit;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;474.5437,822.6354;Inherit;False;Shadowoveride;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-180.5863,-99.6221;Inherit;False;Constant;_Float3;Float 3;14;0;Create;True;0;0;False;0;False;2.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;110;-1357.528,796.8574;Inherit;False;Property;_ColorBlock;Color Block;11;0;Create;True;0;0;False;0;False;1,0,0,0;0,0.979866,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;16;-564.179,272.8397;Inherit;False;46;Rim;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;149;-485.8389,-368.2311;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-777.3618,600.5226;Inherit;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;False;0;False;2.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-1424.918,2702.036;Inherit;False;Spec;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;-825.8235,524.8024;Inherit;False;144;Shadowoveride;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-569.4085,69.33856;Inherit;False;30;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;133;-1078.949,523.5811;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;160;42.63098,-153.4094;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-796.8422,695.7452;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-76.93094,-418.1443;Inherit;False;Property;frenSelect;Selection;12;0;Create;False;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-553.8021,346.0697;Inherit;False;84;Spec;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-370.2152,143.498;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;147;-622.8926,523.7929;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-535.057,573.2635;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;219.622,-163.3813;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-224.0562,259.9667;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-490.1896,895.0365;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;303.4994,61.73965;Inherit;False;Constant;_Float4;Float 4;14;0;Create;True;0;0;False;0;False;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;139;-114.4179,406.3384;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;497.1336,-47.17963;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;141;-62.72067,812.8173;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;154;468.6463,-280.8879;Inherit;False;Property;_SelectionColou;Selection Colou;13;0;Create;True;0;0;False;0;False;1,0,0,0;0,1,0.4358647,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;86.93961,434.7206;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;751.1399,-138.0687;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-246.4593,5.231912;Inherit;False;23;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;153;602.0886,324.8316;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;158;945.509,56.60146;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Liftoff;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;10;0;8;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;9;0;4;0
WireConnection;40;0;41;0
WireConnection;40;1;39;0
WireConnection;22;0;21;0
WireConnection;22;1;20;0
WireConnection;42;0;40;0
WireConnection;23;0;22;0
WireConnection;68;0;66;0
WireConnection;68;1;69;1
WireConnection;18;0;13;0
WireConnection;18;1;17;0
WireConnection;18;2;17;0
WireConnection;14;1;18;0
WireConnection;72;0;68;0
WireConnection;72;1;73;0
WireConnection;43;0;42;0
WireConnection;95;0;92;0
WireConnection;95;1;94;0
WireConnection;95;2;93;0
WireConnection;74;0;72;0
WireConnection;74;1;75;0
WireConnection;26;0;25;0
WireConnection;26;1;14;0
WireConnection;44;0;43;0
WireConnection;44;1;45;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;15;0;26;0
WireConnection;54;0;44;0
WireConnection;54;1;52;0
WireConnection;33;0;32;0
WireConnection;33;1;31;0
WireConnection;77;0;74;0
WireConnection;77;1;78;0
WireConnection;77;2;79;0
WireConnection;96;0;88;0
WireConnection;96;1;95;0
WireConnection;34;0;28;0
WireConnection;34;1;33;0
WireConnection;48;0;47;0
WireConnection;48;1;49;0
WireConnection;87;0;77;0
WireConnection;87;1;96;0
WireConnection;56;0;54;0
WireConnection;50;0;56;0
WireConnection;50;1;48;0
WireConnection;80;0;87;0
WireConnection;80;1;81;0
WireConnection;29;0;27;0
WireConnection;29;1;34;0
WireConnection;136;0;100;0
WireConnection;46;0;50;0
WireConnection;83;0;82;0
WireConnection;83;1;80;0
WireConnection;30;0;29;0
WireConnection;144;0;136;0
WireConnection;84;0;83;0
WireConnection;133;0;99;0
WireConnection;133;1;134;0
WireConnection;160;0;149;0
WireConnection;160;1;161;0
WireConnection;127;0;133;0
WireConnection;127;1;110;0
WireConnection;60;0;57;0
WireConnection;60;1;16;0
WireConnection;147;0;145;0
WireConnection;147;1;148;0
WireConnection;146;0;147;0
WireConnection;146;1;127;0
WireConnection;151;0;152;0
WireConnection;151;1;160;0
WireConnection;89;0;60;0
WireConnection;89;1;86;0
WireConnection;139;0;89;0
WireConnection;139;1;146;0
WireConnection;162;0;151;0
WireConnection;162;1;163;0
WireConnection;141;0;136;0
WireConnection;141;1;140;0
WireConnection;124;0;139;0
WireConnection;124;1;141;0
WireConnection;156;0;154;0
WireConnection;156;1;162;0
WireConnection;153;0;156;0
WireConnection;153;1;124;0
WireConnection;158;13;153;0
ASEEND*/
//CHKSM=E1FC71DC3488AA0DC962BD435A0B0E330712F168