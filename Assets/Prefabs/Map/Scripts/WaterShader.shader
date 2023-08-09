Shader"Custom/WaterShader"
{
    Properties
    {
        _WaveHeight ("Wave Height", Range(0, 1)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 1
        _WaterColor ("Water Color", Color) = (0.0, 0.5, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
LOD100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

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

float _WaveHeight;
float _WaveSpeed;
float _WaveFrequency;
fixed4 _WaterColor;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex + float4(0, 0, sin(_Time.y * _WaveSpeed + v.vertex.x * _WaveFrequency) * _WaveHeight, 0));
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    return _WaterColor;
}
            ENDCG
        }
    }
}