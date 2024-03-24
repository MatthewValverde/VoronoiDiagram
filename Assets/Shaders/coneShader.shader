Shader "Custom/coneShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.depth = o.pos.z / o.pos.w;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Normalize the depth value
                float depth = i.depth * 0.5 + 0.5;

            // Create a color gradient based on depth
            fixed4 color = lerp(fixed4(1, 0, 0, 1), fixed4(0, 0, 1, 1), depth);
            return color * _Color;
        }
        ENDCG
    }
    }
        FallBack "Diffuse"
}
