Shader "Custom/VoronoiShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DotSize("Dot Size", Float) = 0.01
        _EdgeWidth("Edge Width", Float) = 0.005
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
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _DotSize;
                float _EdgeWidth;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Example: Hardcoded points for demonstration purposes
                    float2 points[3] = {float2(0.3, 0.3), float2(0.7, 0.5), float2(0.4, 0.8)};
                    fixed3 colors[3] = {fixed3(1, 0, 0), fixed3(0, 1, 0), fixed3(0, 0, 1)};

                    float nearestDistance = 1e5; // Large initial distance
                    float secondNearestDistance = 1e5; // Second nearest
                    int nearestPointIndex = 0;

                    for (int j = 0; j < 3; j++)
                    {
                        float dist = distance(i.uv, points[j]);
                        if (dist < nearestDistance)
                        {
                            secondNearestDistance = nearestDistance;
                            nearestDistance = dist;
                            nearestPointIndex = j;
                        }
                        else if (dist < secondNearestDistance)
                        {
                            secondNearestDistance = dist;
                        }
                    }

                    // Draw edges if distances to two nearest points are close to each other
                    if (abs(nearestDistance - secondNearestDistance) < _EdgeWidth)
                    {
                        return fixed4(0, 0, 0, 1); // Black edge
                    }

                    // Draw dots for Voronoi points
                    for (int k = 0; k < 3; k++)
                    {
                        if (distance(i.uv, points[k]) < _DotSize)
                        {
                            return fixed4(0, 0, 0, 1); // Black dot
                        }
                    }

                    // Color the pixel based on the nearest Voronoi point
                    fixed3 color = colors[nearestPointIndex];
                    return fixed4(color, 1);
                }
                ENDCG
            }
        }
}
