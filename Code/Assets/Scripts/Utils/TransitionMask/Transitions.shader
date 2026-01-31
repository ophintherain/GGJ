Shader "Unlit/Transitions"

{

    Properties

    {

        _MainTex ("Texture", 2D) = "white" {}

         _Color ("Tint", Color) = (1,1,1,1)

        _Color2 ("Tint", Color) = (1,1,1,1)

        _Color3 ("Tint", Color) = (1,1,1,1)


        _Progress ("Progress", Range(0, 1)) = 0

        _ScaleFactor ("Scale Factor", Range(1, 10)) = 10

        _AlphaClip ("Alpha Clip", Range(0, 1)) = 0.01

        // UI相关属性

        _StencilComp ("Stencil Comparison", Float) = 8

        _Stencil ("Stencil ID", Float) = 0

        _StencilOp ("Stencil Operation", Float) = 0

        _StencilWriteMask ("Stencil Write Mask", Float) = 255

        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

    }

    

    SubShader

    {

        Tags 

        { 

            "Queue"="Transparent" 

            "IgnoreProjector"="True" 

            "RenderType"="Transparent" 

            "PreviewType"="Plane"

            "CanUseSpriteAtlas"="True"

        }

        

        // UI所需的Stencil设置

        Stencil

        {

            Ref [_Stencil]

            Comp [_StencilComp]

            Pass [_StencilOp]

            ReadMask [_StencilReadMask]

            WriteMask [_StencilWriteMask]

        }

        

        Cull Off

        Lighting Off

        ZWrite Off

        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha

        ColorMask [_ColorMask]



        Pass

        {

            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            #pragma target 2.0



            #include "UnityCG.cginc"

            #include "UnityUI.cginc"



            struct appdata

            {

                float4 vertex : POSITION;

                float2 uv : TEXCOORD0;

                float4 color : COLOR;

            };



            struct v2f

            {

                float2 uv : TEXCOORD0;

                float4 vertex : SV_POSITION;

                float4 color : COLOR;

                float4 worldPosition : TEXCOORD1;

                // 在顶点着色器中预计算缩放后的UV

                float2 scaledUV1 : TEXCOORD2;

                float2 scaledUV2 : TEXCOORD3;

                float2 scaledUV3 : TEXCOORD4;

                float2 scaledUV4 : TEXCOORD5;

            };



            sampler2D _MainTex;

            float4 _MainTex_ST;

            fixed4 _Color;

            fixed4 _Color2;

            fixed4 _Color3;

            float _Progress;

            float _ScaleFactor;

            float _AlphaClip;

            float4 _ClipRect;



            // 计算缩放UV

            float2 ScaleUVFromCenter(float2 uv, float scale)

            {

                float2 centeredUV = uv - 0.5;

                return centeredUV / scale + 0.5;

            }



            v2f vert (appdata v)

            {

                v2f o;

                o.worldPosition = v.vertex;

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.color = v.color * _Color;



                // 在顶点着色器中预计算所有缩放UV

                float currentScale1 = lerp(0.001, _ScaleFactor, _Progress);

                float currentScale2 = lerp(0.001, _ScaleFactor, saturate(_Progress - 0.1));

                float currentScale3 = lerp(0.001, _ScaleFactor, saturate(_Progress - 0.25));

                float currentScale4 = lerp(0.001, _ScaleFactor, saturate(_Progress - 0.55));



                o.scaledUV1 = ScaleUVFromCenter(o.uv, currentScale1);

                o.scaledUV2 = ScaleUVFromCenter(o.uv, currentScale2);

                o.scaledUV3 = ScaleUVFromCenter(o.uv, currentScale3);

                o.scaledUV4 = ScaleUVFromCenter(o.uv, currentScale4);



                return o;

            }



            fixed4 frag (v2f i) : SV_Target

            {

                // 使用预计算的UV进行采样

                fixed4 col1 = tex2D(_MainTex, i.scaledUV1) * _Color;

                fixed4 col2 = tex2D(_MainTex, i.scaledUV2) * _Color2;

                fixed4 col3 = tex2D(_MainTex, i.scaledUV3) * _Color3;

                fixed4 col4 = tex2D(_MainTex, i.scaledUV4);

                

                // 第四层alpha反转

                col4.a = 1.0 - col4.a;

                col4 *= _Color;



                // Alpha混合

                fixed4 col = col1;

                col.rgb = lerp(col.rgb, col2.rgb, col2.a);

                col.rgb = lerp(col.rgb, col3.rgb, col3.a);

               

                // 应用颜色和UI遮罩

                col *= i.color;

                

                #ifdef UNITY_UI_CLIP_RECT

                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);

                #endif



                col.a = lerp(col.a, col4.a, step(0.55, _Progress));

                col.a = lerp(0, col.a, step(0.001, _Progress));



                clip(col.a - _AlphaClip);



                return col;

            }

            ENDCG

        }

    }

}