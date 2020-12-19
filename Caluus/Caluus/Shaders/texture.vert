#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 inverse_model;
uniform mat4 lightSpaceMatrix;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;
out vec3 Tangent;
out vec4 FragPosLightSpace;

void main()
{
    mat3 Model3 = mat3(inverse_model);
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    FragPos = vec3(vec4(aPos, 1.0) * model);
    Normal = normalize(aNormal * Model3);
    TexCoords = aTexCoords; 
    Tangent = normalize(aTangent * Model3);
    FragPosLightSpace = lightSpaceMatrix * vec4(FragPos, 1.0);
}