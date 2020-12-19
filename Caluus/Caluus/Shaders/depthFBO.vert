#version 330 core
layout (location = 0) in vec3 aPos;


uniform mat4 model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	mat4 lightSpace = Projection * View;
	gl_Position = lightSpace * model * vec4(aPos, 1);
}