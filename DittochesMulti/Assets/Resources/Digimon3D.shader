Shader "Dittoches/Digimon3D"
{
    Properties { _Color ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _Color;
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 vertex : SV_POSITION; float lighting : TEXCOORD0; };
            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                float3 normal = UnityObjectToWorldNormal(input.normal);
                output.lighting = .42 + .58 * saturate(dot(normal, normalize(float3(-.35,.8,-.45))));
                return output;
            }
            fixed4 frag(v2f input) : SV_Target { return fixed4(_Color.rgb * input.lighting, _Color.a); }
            ENDCG
        }
    }
}
