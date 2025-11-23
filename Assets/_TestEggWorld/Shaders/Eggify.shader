Shader "Custom/EggifyVertexOnly"
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

            float MapYToBodyParts(float y, float splitY,
                      float headMin, float headMax,
                      float bodyMin, float bodyMax)
            {
                if (y >= splitY)
                {
                // 卵の上半分 → 頭領域に線形マッピング
                float t = (y - splitY) / (1.0 - splitY);  // 0〜1
                return lerp(headMin, headMax, t);
                }
                else
                {
                // 卵の下半分 → 身体領域に線形マッピング
                float t = (y - (-1.0)) / (splitY - (-1.0)); // 0〜1
                return lerp(bodyMin, bodyMax, t);
                }
            }


            float3 Eggify(float3 pos, float strength, float3 centerOffset, float axisScale)
            {
                // --- 球化 ---
                // ①元の位置
                float3 original = pos - centerOffset;

                // ②正規化
                float3 scaled = original * axisScale;
                float3 normalized = normalize(scaled);

                // ③Lerp
                float3 sphere = lerp(original, normalized, strength);    

                // --- 卵化（XZ 方向だけ縮小） ---
                // y=0（お腹付近）で1、上や下に離れるほど0になる係数
                float t = 1.0 - saturate(abs(scaled.y + 0.3) * 0.7);

                // XZ を縮小（Y は変えない）
                float3 egg = sphere;
                egg.x *= t;
                egg.z *= t;

                // 身体領域の再配置
                egg.y = MapYToBodyParts(
                /*egg.y,
                splitY,
                headMin, headMax,
                bodyMin, bodyMax*/
                egg.y,
                0.5,    // splitY
                0.4, 1.0,   // headMin, headMax
                -1.0, 0.4   // bodyMin, bodyMax
                );

                return egg + centerOffset;
            }

            v2f vert (appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;
                float3 centerOffset = float3(0.0, 0.88, 0.0);
                float3 axisScale    = float3(0.5, 0.5, 0.5);   // X/Z方向は通常、Y方向は縦長に
                // Eggify変形
                //pos = Eggify(pos, _Strength, centerOffset, axisScale);
                pos = Eggify(pos, 0.7, centerOffset, axisScale);

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
