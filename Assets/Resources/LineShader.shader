Shader "LineShader"
{
	SubShader
	{
		Pass
		{
			Blend SrcAlpha
			OneMinusSrcAlpha
			ZWrite On
			Cull Off
			Fog
			{
				Mode Off
			}
			BindChannels
			{
				Bind "vertex", vertex
				Bind "color", color
			}
		} 
	}
}