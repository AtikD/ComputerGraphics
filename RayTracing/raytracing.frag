#version 430 core
out vec4 FragColor;
in vec3 glPosition;

#define EPSILON 0.001
#define BIG 1000000.0
#define MAX_STACK_SIZE 10

const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

struct SCamera
{
    vec3 Position;
    vec3 View;
    vec3 Up;
    vec3 Side;
    vec2 Scale;
};

struct SRay
{
    vec3 Origin;
    vec3 Direction;
};

struct SLight
{
    vec3 Position;
};

struct SMaterial
{
    vec3 Color;
    vec4 LightCoeffs;
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};

struct SSphere
{
    vec3 Center;
    float Radius;
    int MaterialIdx;
};

struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialIdx;
};

struct SIntersection
{
    float Time;
    vec3 Point;
    vec3 Normal;
    vec3 Color;
    vec4 LightCoeffs;
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};

struct STracingRay
{
    SRay ray;
    float contribution;
    int depth;
};

uniform float aspect;
uniform vec3 campos;
uniform vec3 camdir;
uniform vec3 camup;
uniform vec3 camright;

SSphere spheres[2];
STriangle triangles[10];
SMaterial materials[6];
SLight light;
STracingRay stack[MAX_STACK_SIZE];
int stackSize = 0;

SRay GenerateRay(SCamera camera)
{
    vec2 coords = glPosition.xy * camera.Scale;
    vec3 direction = camera.View + camera.Side * coords.x + camera.Up * coords.y;
    return SRay(camera.Position, normalize(direction));
}

SCamera initializeDefaultCamera()
{
    SCamera camera;
    camera.Position = campos;
    camera.View = camdir;
    camera.Up = camup;
    camera.Side = camright;
    camera.Scale = vec2(aspect, 1.0);
    return camera;
}

void initializeDefaultScene()
{
    light.Position = vec3(0.0, 2.0, -4.0f);
    
    vec4 lightCoeffs = vec4(0.4, 0.9, 0.0, 512.0);
    
    materials[0].Color = vec3(0.0, 1.0, 0.0);
    materials[0].LightCoeffs = vec4(lightCoeffs);
    materials[0].ReflectionCoef = 0.5;
    materials[0].RefractionCoef = 1.0;
    materials[0].MaterialType = DIFFUSE;
    
    materials[1].Color = vec3(0.0, 0.0, 1.0);
    materials[1].LightCoeffs = vec4(lightCoeffs);
    materials[1].ReflectionCoef = 0.5;
    materials[1].RefractionCoef = 1.0;
    materials[1].MaterialType = DIFFUSE;
    
    materials[2].Color = vec3(0.8, 0.8, 0.8);
    materials[2].LightCoeffs = vec4(0.1, 0.9, 0.0, 512.0);
    materials[2].ReflectionCoef = 0.5;
    materials[2].RefractionCoef = 1.0;
    materials[2].MaterialType = DIFFUSE;
    
    materials[3].Color = vec3(1.0, 1.0, 1.0);
    materials[3].LightCoeffs = vec4(0.0, 0.0, 1.0, 512.0);
    materials[3].ReflectionCoef = 0.5;
    materials[3].RefractionCoef = 1.0;
    materials[3].MaterialType = REFLECTION;
    
    materials[4].Color = vec3(0.0, 0.0, 0.9);
    materials[4].LightCoeffs = vec4(0.0, 0.1, 0.9, 512.0);
    materials[4].ReflectionCoef = 0.1;
    materials[4].RefractionCoef = 1.5;
    materials[4].MaterialType = REFRACTION;
    
    materials[5].Color = vec3(1.0, 0.0, 0.0);
    materials[5].LightCoeffs = vec4(lightCoeffs);
    materials[5].ReflectionCoef = 0.5;
    materials[5].RefractionCoef = 1.0;
    materials[5].MaterialType = DIFFUSE;
    
    vec3 leftWallColor = vec3(1.0, 0.0, 0.0);
    vec3 rightWallColor = vec3(0.0, 1.0, 0.0);
    vec3 backWallColor = vec3(0.0, 0.0, 1.0);
    vec3 downWallColor = vec3(1.0, 1.0, 0.0);
    vec3 upWallColor = vec3(0.0, 1.0, 1.0);
    vec3 lightColor = vec3(1.0, 1.0, 1.0);
    
    spheres[0].Center = vec3(-1.0, -1.0, -2.0);
    spheres[0].Radius = 2.0;
    spheres[0].MaterialIdx = 2;
    
    spheres[1].Center = vec3(2.0, 1.0, 2.0);
    spheres[1].Radius = 1.0;
    spheres[1].MaterialIdx = 3;
    
    triangles[0].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[0].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[0].v3 = vec3(-5.0, 5.0, -5.0);
    triangles[0].MaterialIdx = 0;
    
    triangles[1].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[1].v2 = vec3(-5.0, -5.0, 5.0);
    triangles[1].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[1].MaterialIdx = 0;
    
    triangles[2].v1 = vec3(-5.0, -5.0, 5.0);
    triangles[2].v2 = vec3(5.0, -5.0, 5.0);
    triangles[2].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[2].MaterialIdx = 2;
    
    triangles[3].v1 = vec3(5.0, 5.0, 5.0);
    triangles[3].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[3].v3 = vec3(5.0, -5.0, 5.0);
    triangles[3].MaterialIdx = 2;
    
    triangles[4].v1 = vec3(5.0, -5.0, 5.0);
    triangles[4].v2 = vec3(5.0, -5.0, -5.0);
    triangles[4].v3 = vec3(5.0, 5.0, -5.0);
    triangles[4].MaterialIdx = 1;
    
    triangles[5].v1 = vec3(5.0, 5.0, -5.0);
    triangles[5].v2 = vec3(5.0, 5.0, 5.0);
    triangles[5].v3 = vec3(5.0, -5.0, 5.0);
    triangles[5].MaterialIdx = 1;
    
    triangles[6].v1 = vec3(5.0, 5.0, -5.0);
    triangles[6].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[6].v3 = vec3(-5.0, 5.0, -5.0);
    triangles[6].MaterialIdx = 1;
    
    triangles[7].v1 = vec3(5.0, 5.0, -5.0);
    triangles[7].v2 = vec3(5.0, 5.0, 5.0);
    triangles[7].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[7].MaterialIdx = 1;
    
    triangles[8].v1 = vec3(-5.0, -5.0, 5.0);
    triangles[8].v2 = vec3(-5.0, -5.0, -5.0);
    triangles[8].v3 = vec3(5.0, -5.0, -5.0);
    triangles[8].MaterialIdx = 5;
    
    triangles[9].v1 = vec3(5.0, -5.0, -5.0);
    triangles[9].v2 = vec3(5.0, -5.0, 5.0);
    triangles[9].v3 = vec3(-5.0, -5.0, 5.0);
    triangles[9].MaterialIdx = 5;
}

bool IntersectSphere(SSphere sphere, SRay ray, float start, float final, out float time)
{
    ray.Origin -= sphere.Center;
    float A = dot(ray.Direction, ray.Direction);
    float B = dot(ray.Direction, ray.Origin);
    float C = dot(ray.Origin, ray.Origin) - sphere.Radius * sphere.Radius;
    float D = B * B - A * C;
    
    if (D > 0.0)
    {
        D = sqrt(D);
        
        float t1 = (-B - D) / A;
        float t2 = (-B + D) / A;
        
        if(t1 < 0 && t2 < 0)
            return false;
        
        if(min(t1, t2) < 0)
        {
            time = max(t1,t2);
            return true;
        }
        
        time = min(t1, t2);
        return true;
    }
    
    return false;
}

bool IntersectTriangle(SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time)
{
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    vec3 N = cross(A, B);
    
    float NdotRayDirection = dot(N, ray.Direction);
    if (abs(NdotRayDirection) < EPSILON)
        return false;
    
    float d = dot(N, v1);
    float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;
    
    if (t < 0) 
        return false;
    
    vec3 P = ray.Origin + t * ray.Direction;
    
    vec3 C;
    vec3 edge1 = v2 - v1;
    vec3 VP1 = P - v1;
    C = cross(edge1, VP1);
    if (dot(N, C) < 0)
        return false;
    
    vec3 edge2 = v3 - v2;
    vec3 VP2 = P - v2;
    C = cross(edge2, VP2);
    if (dot(N, C) < 0) 
        return false;
    
    vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < 0) 
        return false;
    
    time = t;
    return true;
}

bool Raytrace(SRay ray, float start, float final, inout SIntersection intersect)
{
    bool result = false;
    float test;
    
    for(int i = 0; i < 2; i++)
    {
        if(IntersectSphere(spheres[i], ray, start, final, test) && test < intersect.Time)
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize(intersect.Point - spheres[i].Center);
            
            SMaterial material = materials[spheres[i].MaterialIdx];
            intersect.Color = material.Color;
            intersect.LightCoeffs = material.LightCoeffs;
            intersect.ReflectionCoef = material.ReflectionCoef;
            intersect.RefractionCoef = material.RefractionCoef;
            intersect.MaterialType = material.MaterialType;
            
            result = true;
        }
    }
    
    for(int i = 0; i < 10; i++)
    {
        if(IntersectTriangle(ray, triangles[i].v1, triangles[i].v2, triangles[i].v3, test) 
           && test < intersect.Time)
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize(cross(triangles[i].v1 - triangles[i].v2, 
                                             triangles[i].v3 - triangles[i].v2));
            
            SMaterial material = materials[triangles[i].MaterialIdx];
            intersect.Color = material.Color;
            intersect.LightCoeffs = material.LightCoeffs;
            intersect.ReflectionCoef = material.ReflectionCoef;
            intersect.RefractionCoef = material.RefractionCoef;
            intersect.MaterialType = material.MaterialType;
            
            result = true;
        }
    }
    
    return result;
}

vec3 Phong(SIntersection intersect, SLight currLight, float shadow)
{
    vec3 light = normalize(currLight.Position - intersect.Point);
    float diffuse = max(dot(light, intersect.Normal), 0.0);
    
    vec3 view = normalize(initializeDefaultCamera().Position - intersect.Point);
    vec3 reflected = reflect(-view, intersect.Normal);
    float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);
    
    return intersect.LightCoeffs.x * intersect.Color +
           intersect.LightCoeffs.y * diffuse * intersect.Color * shadow +
           intersect.LightCoeffs.z * specular * vec3(1.0) * shadow;
}

float Shadow(SLight currLight, SIntersection intersect)
{
    float shadowing = 1.0;
    
    vec3 direction = normalize(currLight.Position - intersect.Point);
    
    float distanceLight = distance(currLight.Position, intersect.Point);
    
    SRay shadowRay = SRay(intersect.Point + direction * EPSILON, direction);
    
    SIntersection shadowIntersect;
    shadowIntersect.Time = BIG;
    
    if(Raytrace(shadowRay, 0, distanceLight, shadowIntersect))
    {
        shadowing = 0.0;
    }
    
    return shadowing;
}

void pushRay(STracingRay ray)
{
    if(stackSize < MAX_STACK_SIZE)
    {
        stack[stackSize++] = ray;
    }
}

STracingRay popRay()
{
    return stack[--stackSize];
}

bool isEmpty()
{
    return stackSize == 0;
}

void main()
{
    float start = 0;
    float final = BIG;
    
    SCamera camera = initializeDefaultCamera();
    SRay ray = GenerateRay(camera);
    
    vec3 resultColor = vec3(0,0,0);
    
    initializeDefaultScene();
    
    STracingRay trRay = STracingRay(ray, 1.0, 0);
    pushRay(trRay);
    
    while(!isEmpty())
    {
        trRay = popRay();
        ray = trRay.ray;
        
        SIntersection intersect;
        intersect.Time = BIG;
        
        if(Raytrace(ray, start, final, intersect))
        {
            switch(intersect.MaterialType)
            {
                case DIFFUSE:
                {
                    float shadowing = Shadow(light, intersect);
                    resultColor += trRay.contribution * Phong(intersect, light, shadowing);
                    break;
                }
                
                case REFLECTION:
                {
                    if(intersect.ReflectionCoef < 1.0)
                    {
                        float contribution = trRay.contribution * (1.0 - intersect.ReflectionCoef);
                        float shadowing = Shadow(light, intersect);
                        resultColor += contribution * Phong(intersect, light, shadowing);
                    }
                    
                    vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
                    float contribution = trRay.contribution * intersect.ReflectionCoef;
                    STracingRay reflectRay = STracingRay(
                        SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),
                        contribution,
                        trRay.depth + 1
                    );
                    pushRay(reflectRay);
                    break;
                }
                
                case REFRACTION:
                {
                    float contribution = trRay.contribution * (1.0 - intersect.ReflectionCoef);
                    float shadowing = Shadow(light, intersect);
                    resultColor += contribution * Phong(intersect, light, shadowing);
                    
                    vec3 refractedDir = refract(ray.Direction, intersect.Normal, 1.0 / intersect.RefractionCoef);
                    float refractionContrib = trRay.contribution * (1.0 - intersect.ReflectionCoef);
                    
                    STracingRay refractRay = STracingRay(
                        SRay(intersect.Point + refractedDir * EPSILON, refractedDir),
                        refractionContrib,
                        trRay.depth + 1
                    );
                    pushRay(refractRay);
                    
                    vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
                    float reflectionContrib = trRay.contribution * intersect.ReflectionCoef;
                    
                    if(reflectionContrib > 0.0)
                    {
                        STracingRay reflectRay = STracingRay(
                            SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),
                            reflectionContrib,
                            trRay.depth + 1
                        );
                        pushRay(reflectRay);
                    }
                    break;
                }
            }
        }
    }
    
    FragColor = vec4(resultColor, 1.0);
}