#ifndef UNIVERSAL_LIT_BRDF_INCLUDED
#define UNIVERSAL_LIT_BRDF_INCLUDED

// 大部分的BRDF相关的在Core RP Library

struct BxDFContext
{
    float NoV;
    float NoL;
    float VoL;
    float NoH;
    float VoH;
    float XoV;
    float XoL;
    float XoH;
    float YoV;
    float YoL;
    float YoH;
};

void InitBxDFContext( inout BxDFContext Context, half3 N, half3 V, half3 L )
{
    Context.NoL = dot(N, L);
    Context.NoV = dot(N, V);
    Context.VoL = dot(V, L);
    float InvLenH = rsqrt( 2 + 2 * Context.VoL );
    Context.NoH = saturate( ( Context.NoL + Context.NoV ) * InvLenH );
    Context.VoH = saturate( InvLenH + InvLenH * Context.VoL );
    //NoL = saturate( NoL );
    //NoV = saturate( abs( NoV ) + 1e-5 );

    Context.XoV = 0.0f;
    Context.XoL = 0.0f;
    Context.XoH = 0.0f;
    Context.YoV = 0.0f;
    Context.YoL = 0.0f;
    Context.YoH = 0.0f;
}

#endif 