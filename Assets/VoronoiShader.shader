Shader "Custom/VoronoiShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DotSize("Dot Size", Float) = 0.01
        _EdgeWidth("Edge Width", Float) = 0.005
        _GradientRadius("Gradient Radius", Float) = 0.1 // New property for gradient radius
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
                #define MAX_POINT_COUNT 50

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
                float _NumPoints;
                float _GradientRadius; // Gradient radius variable
                uniform float _Points[MAX_POINT_COUNT * 3];
                uniform float3 _Colors[MAX_POINT_COUNT];

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float3 points2[MAX_POINT_COUNT];
                    for (int p = 0; p < _NumPoints; p++)
                    {
                        points2[p] = float3(_Points[p * 3], _Points[((p * 3) + 1)], _Points[((p * 3) + 2)]);
                    }

                    float nearestDistance = 1e5;
                    float secondNearestDistance = 1e5;
                    int nearestPointIndex = 0;

                    for (int j = 0; j < _NumPoints; j++)
                    {
                        float dist = distance(i.uv, points2[j]);
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

                    if (abs(nearestDistance - secondNearestDistance) < _EdgeWidth)
                    {
                        return fixed4(0, 0, 0, 1);
                    }

                    for (int k = 0; k < _NumPoints; k++)
                    {
                        if (distance(i.uv, points2[k]) < _DotSize)
                        {
                            return fixed4(0, 0, 0, 1);
                        }
                    }

                    float pointDistance = nearestDistance;
                    // After determining the nearest point and before final color calculation:
                    float3 seedPoint = points2[nearestPointIndex];
                    float2 direction = normalize(i.uv - seedPoint.xy);
                    float angle = atan2(direction.y, direction.x);

                    // Adjust these values based on desired visual effect
                    float lineSpread = 3.14159 / 3; // 60 degrees
                    float lineThickness = 0.002; // Thickness of lines

                    for (int l = 0; l < 9; l++) {
                        float lineAngle = l * lineSpread;
                        float angleDiff = abs(angle - lineAngle);

                        // Check if the fragment is within the thickness of a line
                        if (angleDiff < lineThickness || abs(angleDiff - 2 * 3.14159) < lineThickness) {
                            // Modify this color based on desired line appearance
                            return fixed4(0, 0, 0, 1); // Drawing the line
                        }
                    }

                    fixed3 darkGrey = fixed3(0.25, 0.25, 0.25);
                    float zCoordinate = points2[nearestPointIndex].z;
                    float modifiedGradientRadius = (_GradientRadius + zCoordinate) * 1.0; 

                    modifiedGradientRadius = max(modifiedGradientRadius, 0.0);

                    float gradientFactor = smoothstep(zCoordinate, 0.0, pointDistance);
                    fixed3 gradientColor = lerp(darkGrey, _Colors[nearestPointIndex], gradientFactor);

                    return fixed4(gradientColor, 1);
                }
                ENDCG
            }
        }
}
