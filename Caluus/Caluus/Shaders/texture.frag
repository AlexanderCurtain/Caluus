#version 330 core
//In this tutorial it might seem like a lot is going on, but really we just combine the last tutorials, 3 pieces of source code into one
//and added 3 extra point lights.
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D normalmap;
    float     shininess;
    float     opacity;
};
//This is the directional light struct, where we only need the directions
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform DirLight dirLight;
//This is our pointlight where we need the position aswell as the constants defining the attenuation of the light.
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
//We have a total of 4 point lights now, so we define a preprossecor directive to tell the gpu the size of our point light array
#define NR_POINT_LIGHTS 16
uniform int NumberOfPointLights;
uniform PointLight pointLights[NR_POINT_LIGHTS];
//This is our spotlight where we need the position, attenuation along with the cutoff and the outer cutoff. Plus the direction of the light
struct SpotLight{
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};
#define NR_SPOT_LIGHTS 16
uniform int NumberOfSpotLights;
uniform SpotLight spotLight[NR_SPOT_LIGHTS];

uniform Material material;
uniform vec3 viewPos;

uniform sampler2D shadowMap;

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;
in vec3 Tangent;
in vec4 FragPosLightSpace;

//Here we have some function prototypes, these are the signatures the gpu will use to know how the
//parameters of each light calculation is layed out.
//We have one function per light, since this makes it so we dont have to take up to much space in the main function.
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float shadowCalc();

void main()
{
    vec4 NormalMapValue = (texture(material.normalmap, TexCoords));

    vec3 Bitangent = cross(Tangent, Normal);
    mat3 TBN = mat3(
    Tangent.x, Bitangent.x, Normal.x,
    Tangent.y, Bitangent.y, Normal.y,
    Tangent.z, Bitangent.z, Normal.z
    );

    vec4 Texture = vec4(texture(material.diffuse, TexCoords));
    if (Texture.a < 0.2f) {
    discard;
    }


    vec3 UnitNormal = normalize(NormalMapValue.rgb);
    //properties
    vec3 norm = vec3(UnitNormal *2 -1) * TBN;
    //norm = Normal;

    vec3 viewDir = normalize(viewPos - FragPos);

    //phase 1: Directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    
    //phase 2: Point lights
    for(int i = 0; i < NumberOfPointLights; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
    //phase 3: Spot light
    for(int i = 0; i < NumberOfSpotLights; i++)
        result += CalcSpotLight(spotLight[i], norm, FragPos, viewDir);  

    FragColor = vec4(result, material.opacity * Texture.a);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    //combine results
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    float shadow = shadowCalc();
    return ((ambient + diffuse) + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    //attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));
    //combine results
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
} 

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{

    //diffuse shading
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //attenuation
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //spotlight intensity
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    //combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}

float shadowCalc()
{
    vec3 pos = FragPosLightSpace.xyz * 0.5 + 0.5;
    float depth = texture(shadowMap, pos.xy).r;
    return depth < pos.z ? 0.0 : 1.0;
}