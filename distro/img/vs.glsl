#version 100

attribute vec2 in_position;
attribute vec2 in_uv;

varying vec2 position;
varying vec2 uv;
varying float hitcircle_radius;
varying float alpha;

uniform int mode;
uniform mat4 projection;
uniform vec2 center;
uniform vec2 off_center;
uniform float current_time;
uniform float start_time;
uniform float end_time;

uniform float AR;
uniform float CS;
uniform float slider_velocity;

// this is the way osu maps difficulty levels from 0 -> 10 to output
float DifficultyRange(float difficulty, float v0, float v1, float v2) {
	if (difficulty > 5.0)
		return v1 + (v2 - v1) * (difficulty - 5.0) / 5.0;
	if (difficulty < 5.0)
		return v1 - (v1 - v0) * (5.0 - difficulty) / 5.0;
	return v1;
}

float Range(float x, float in_min, float in_max, float out_min, float out_max) {
	return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

void main(void) {
	uv = in_uv;
	gl_Position = projection * vec4(in_position, 0.0, 1.0);
	position = in_position.xy;
	if (mode != 0) {
		hitcircle_radius = 54.4 - 4.48 * CS;
		float preempt = DifficultyRange(AR, 1800.0, 1200.0, 450.0);
		float fadein_start = start_time - preempt;
		float fadein_end = start_time - (preempt / 3.0);
		float fadeout_start = end_time;
		float fadeout_end = end_time + (preempt * 2.0 / 3.0);
		if (current_time < fadein_start || current_time > fadeout_end) {
			alpha = 0.0;
		} else if (current_time < fadein_end) {
			alpha = Range(current_time, fadein_start, fadein_end, 0.0, 1.0);
		} else if (current_time > fadeout_start) {
			alpha = Range(current_time, fadeout_start, fadeout_end, 1.0, 0.0);
		} else {
			alpha = 1.0;
		}
	}
}
