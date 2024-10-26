Shader "Mobile/HGWater"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.2, 0.5, 0.8, 0.5)
        _MainTex ("Base Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _WaveSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv + _WaveSpeed * _Time.y;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                return texColor * _Color;
            }
            ENDCG
        }
    }
}