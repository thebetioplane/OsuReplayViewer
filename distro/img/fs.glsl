#version 100

precision mediump float;

const vec3 HITCIRCLE_COLOR = vec3(0.8, 0.8, 0.8);
const vec3 SLIDER_BODY_COLOR = vec3(0.1, 0.1, 0.1);
const vec3 SLIDER_EDGE_COLOR = vec3(0.8, 0.8, 0.8);
const vec3 SHADER_ERROR_COLOR = vec3(1.0, 0.0, 1.0);

varying vec2 position;
varying vec2 uv;
varying float hitcircle_radius;
varying float alpha;

// 0 -> texture
// 1 -> hitcircle
// 2 -> slider body
// 3 -> slider ball
// 4 -> spinner
uniform int mode;
uniform sampler2D my_texture;
uniform vec2 center;
uniform vec2 off_center;

float Circle(void) {
	return distance(position, center) - hitcircle_radius;
}

float Obround(void) {
	float a = distance(position, center) - hitcircle_radius;
	float b = distance(position, off_center) - hitcircle_radius;
	if (distance(center, off_center) < 0.001) {
		return a;
	}
	vec2 u = position - off_center;
	vec2 v = center - off_center;
	vec2 m = (dot(u, v) / dot(v, v)) * v;
	if (dot(v, m) > 0.0 && length(m) < length(v)) {
		return distance(position, m + off_center) - hitcircle_radius;
	}
	return min(a, b);
}

void main(void) {
	if (mode == 0) {
		gl_FragColor = texture2D(my_texture, uv);
	} else if (mode == 1) {
		if (Circle() > 0.0)
			discard;
		gl_FragColor = vec4(HITCIRCLE_COLOR, alpha);
	} else if (mode == 2) {
		if (alpha <= 0.0)
			discard;
		float dst = Obround();
		if (dst > 0.0)
			discard;
		if (dst >= -5.0)
			gl_FragColor = vec4(SLIDER_EDGE_COLOR, 1.0);
		else
			gl_FragColor = vec4(SLIDER_BODY_COLOR, 1.0);

	} else {
		gl_FragColor = vec4(SHADER_ERROR_COLOR, 1.0);
	}
}
