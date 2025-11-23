Shader "Custom/BoxifyVertexOnly"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Strength ("Boxify Strength", Range(0,1)) = 1
        _BoxScale ("Boxify Scale", Float) = 1.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags{ "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            //T//ags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Strength;
            float _BoxScale;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 Boxify(float3 pos, float strength, float3 centerOffset, float axisScale)
            {
                // ①元の位置
                float3 original = pos - centerOffset;

                // ②正規化して -0.5～0.5 でClamp
                float3 scaled = original * axisScale;
                float3 normalized = normalize(scaled);    
                float3 clamped = clamp(normalized, -0.3, 0.3);

                // ③strengthでLerp
                float3 boxified =  lerp(original, clamped, strength);
                return boxified + float3(0, 0.5, 0);
            }

            v2f vert (appdata v)
            {
                /*v2f o;

                float3 original = v.vertex.xyz;
                float3 boxed = BoxifyPosition(original);

                float3 finalPos = lerp(original, boxed, _Strength);

                o.pos = UnityObjectToClipPos(float4(finalPos, 1.0));
                o.uv = v.uv;

                return o;*/
                v2f o;
                float3 pos = v.vertex.xyz;
                float3 centerOffset = float3(0.0, 0.8, -0.1);
                float3 axisScale    = float3(1, 1, 1);   // X/Z方向は通常、Y方向は縦長に
                // Boxify変形
                pos = Boxify(pos, 0.95, centerOffset, axisScale);

                o.pos = UnityObjectToClipPos(float4(pos, 1.0));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                // --- Cutout 判定 ---
                clip(col.a - _Cutoff);
                return col;
            }

            ENDCG
        }
    }

    //FallBack "Diffuse"
}
