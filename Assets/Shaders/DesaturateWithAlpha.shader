Shader "Custom/DesaturateWithAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Saturation ("Saturation", Range(0, 1)) = 1
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _Saturation;
            float _Alpha;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 color = tex2D(_MainTex, i.uv);

                // Compute grayscale color
                float luminance = dot(color.rgb, float3(0.3, 0.59, 0.11));
                float3 grayscale = float3(luminance, luminance, luminance);

                // Apply saturation
                color.rgb = lerp(grayscale, color.rgb, _Saturation);

                // Apply alpha control
                color.a *= _Alpha;

                return color;
            }
            ENDCG
        }
    }
}
