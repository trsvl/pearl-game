Shader "Unlit/ColorBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(None,0,Add,1,Multiply,2, Subtract,3)] _Blend ("Blend mode subset", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            
            // All material properties must be in UnityPerMaterial for SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                int _Blend;
            CBUFFER_END

            // Define instanced color property
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                // Get texture color
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Get per-instance color
                float4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                // Apply blend mode
                switch (_Blend)
                {
                    case 1: col += instanceColor; break;  // Add
                    case 2: col *= instanceColor; break;  // Multiply
                    case 3: col -= instanceColor; break;  // Subtract
                }

                return col;
            }
            ENDCG
        }
    }
}
