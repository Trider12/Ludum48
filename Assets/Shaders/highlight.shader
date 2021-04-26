shader_type canvas_item;

uniform bool enabled = false;
uniform int width = 5;
uniform vec4 highlight = vec4(1f, 0f, 0f, 1f);

void fragment() 
{
	vec4 color =  texture(TEXTURE, UV);
	
	bool painted = false;
	
	if(enabled)
	{	
		if(color.a != 0f)
		{
		    for (int i = -width; i <= +width && !painted; i++)
		    {
		        for (int j = -width; j <= +width && !painted; j++)
		        {
		            if (i == 0 && j == 0)
		            {
		                continue;
		            }

		            vec2 offset = vec2(float(i), float(j)) * TEXTURE_PIXEL_SIZE;
					vec2 pos = UV + offset;

		            if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f || texture(TEXTURE, pos).a == 0f)
		            {
		                COLOR.rgba = highlight;
		                painted = true;
		            }
					else
					{
						COLOR = color;
					}
		        }
		    }
		}
		else
		{
			discard;
		}
	}
	else
	{
		COLOR = color;
	}
}
