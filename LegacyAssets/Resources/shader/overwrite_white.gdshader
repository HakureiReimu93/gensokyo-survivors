shader_type canvas_item;

uniform vec4 color: source_color = vec4(1);

void fragment() {
	vec4 rgba = texture(TEXTURE, UV);
	COLOR.rgb = color.rgb;
	COLOR.a = COLOR.a * color.a;
}
