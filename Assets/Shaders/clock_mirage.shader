shader_type canvas_item;

uniform float frequency = 15;
uniform float depth = 0.00;
uniform sampler2D arrowTexture;
uniform float rotation;

vec4 layer(vec4 foreground, vec4 background) {
    return foreground * foreground.a + background * (1.0 - foreground.a);
}

vec2 rotateUV(vec2 uv)
{
    float mid = 0.5;
    return vec2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}

vec2 mirage(vec2 uv, float time)
{
	uv.x += sin(uv.y * frequency + time) * depth;
	uv.x = clamp(uv.x, 0.0, 1.0);
	
	return uv;
}

void fragment() {
	vec2 uv = UV;	
	uv = mirage(uv, TIME);
	
	vec2 arrowUv = rotateUV(uv);
	
	vec4 clock = textureLod(TEXTURE, uv, 0.0).rgba;
	vec4 arrow = textureLod(arrowTexture, arrowUv, 0.0).rgba;

	COLOR.rgba = layer(arrow, clock);
}
