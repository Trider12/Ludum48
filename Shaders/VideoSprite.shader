shader_type canvas_item;

uniform sampler2D Diffuse;

void fragment(){
    //The color value is the sampled pixel color
    COLOR = texture(Diffuse, UV);
	
	if(UV.y <= 0.5)
	{
		vec4 color = texture(Diffuse, vec2(UV.x, UV.y+0.5));
		float alpha = (color.r + color.g + color.b)/3.0;
		COLOR.a = alpha;
	}
	else
	{
		COLOR.a = 0f;
	}
}