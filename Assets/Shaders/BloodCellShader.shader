Shader "Custom/BloodCellShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1) // White
        _MidColor("Middle Color", Color) = (0,0,0,1) // Black
        _TipColor("Tip Color", Color) = (1,0,0,1) // Red
        _MidPoint("Middle Point", Range(0,1)) = 0.9 // Point where the base transitions to the middle
        _TopPoint("Top Point", Range(0,1)) = 1.0  // Point where the middle transitions to the tip
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
                float3 position : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _BaseColor;
            fixed4 _MidColor;
            fixed4 _TipColor;
            float _MidPoint;
            float _TopPoint;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float height = saturate(i.position.y); // Normalize the y value
                fixed4 color;

                // Determine the position of the color based on the height
                if (height < _MidPoint) {
                    // Base to Middle Color
                    color = lerp(_BaseColor, _MidColor, height / _MidPoint);
                }
                else if (height >= _MidPoint && height < _TopPoint) {
                    // Middle to Tip Color
                    
    // Smooth transition from middle to tip color
    float transitionProgress = (height - _MidPoint) / (_TopPoint - _MidPoint);
    transitionProgress = clamp(transitionProgress, 0.0, 1.0); // Ensure the progress stays within bounds
    color = lerp(_MidColor, _TipColor, transitionProgress);
    
                }
                else {
                    // Tip Color (no interpolation needed here)
                    color = _TipColor;
                }

                return color;
            }
            ENDCG
        }
    }
}
