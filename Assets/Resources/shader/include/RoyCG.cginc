// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef Roy_CG_INCLUDED
#define Roy_CG_INCLUDED

#define PI 3.14

// 计算漫反射(入射光线L在法线N上的投影)
#define GetDiffuse(Normal, LightDir) max(0.0, dot(Normal, LightDir))

// 计算高光反射
#define GetSpecular(ViewDir, ReflectDir, Gloss) pow(max(0.0, dot(ViewDir, ReflectDir)), Gloss)

// 半兰伯特
#define HalfLambert(Light) (Light * 0.5 + 0.5)

// 颜色卡通化: 把颜色都被归入到一个已知的集合中，以简化颜色
#define ColorToon(Color, Tooniness) (floor(Color * Tooniness) / Tooniness)

// Position: object space -> world space
#define GetWorldPos(objPos) mul(_object2World, objPos)

// Normal: object space -> world space
#define GetWorldNormal(objNormal) mul(objNormal, (float3x3)unity_WorldToObject)

// 计算灰度值[与Luminace(0.22, 0.707, 0.071)系数不同]
inline fixed GetGray(fixed3 color)
{
	return dot(color, fixed3(0.299, 0.587, 0.114));
}

// 滤色混合
inline fixed4 ScreenBlend(fixed4 baseColor, fixed4 blendColor)
{
	return 1.0 - (1.0 - baseColor) * (1.0 - blendColor);
}

// 单通道叠加混合
inline fixed OverlayBlendMode(fixed basePixel, fixed blendPixel)
{
	if ( basePixel < 0.5 )
	{
		return 2.0 * basePixel * blendPixel;
	}
	return 1.0 - 2.0 * (1.0 - basePixel) * (1.0 - blendPixel);
}

// 叠加混合
inline fixed4 OverlayBlend(fixed4 baseColor, fixed4 blendColor)
{
	fixed4 color;
	color.r = OverlayBlendMode(baseColor.r, blendColor.r);
	color.g = OverlayBlendMode(baseColor.g, blendColor.g);
	color.b = OverlayBlendMode(baseColor.b, blendColor.b);
	color.a = OverlayBlendMode(baseColor.a, blendColor.a);
	return color;
}

// 计算平方和
inline float SqrMagnitude(float2 xy)
{
	return xy.x * xy.x + xy.y * xy.y;
}

// 计算向量的模
inline float Magnitude(float2 xy)
{
	return sqrt(SqrMagnitude(xy));
}

// 通过dot计算cos再推导出sin
inline float GetSin(float3 dir_1, float3 dir_2)
{
	float3 normal_1 = normalize(dir_1);
	float3 normal_2 = normalize(dir_2);
	return sin(acos(max(0.0, dot(normal_1, normal_2))));
}

// rectangular coordinate to polar coordinate
inline float2 Rect2Polar(float2 uv, float2 center = 0.5)
{
    uv = uv - center;
    float radius = length(uv);
    float angle = atan2(uv.y, uv.x);
    return float2(radius, (angle / PI + 1) * 0.5);
}

#endif
