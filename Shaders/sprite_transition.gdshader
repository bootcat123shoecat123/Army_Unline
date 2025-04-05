shader_type canvas_item;

uniform sampler2D old_texture;
uniform sampler2D new_texture;
uniform float transition_progress : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 old_color = texture(old_texture, UV);
    vec4 new_color = texture(new_texture, UV);
    
    // 使用平滑过渡
    COLOR = mix(old_color, new_color, transition_progress);
} 