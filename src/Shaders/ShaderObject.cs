/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
using System.Collections.Generic;

namespace Radiance.Shaders;

using System.Linq;
using Objects;
using Radiance.Shaders.Dependencies;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public class ShaderObject
{
    public virtual ShaderType Type { get; set; }
    public virtual string Expression { get; set; }
    public ShaderOrigin Origin { get; set; } = ShaderOrigin.Global;
    public virtual IEnumerable<ShaderDependence> Dependecies { get; set; }

    public static implicit operator ShaderObject(float value) => value;
    public static implicit operator ShaderObject(double value) => (float)value;
    public static implicit operator ShaderObject(int value) => value;
    public static implicit operator ShaderObject(bool value) => value;
    public static implicit operator ShaderObject((float x, float y) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z, float w) value) => value;

    public static T Union<T>(string newExpression, T obj1, T obj2)
        where T : ShaderObject, new()
    {
        var deps = obj1.Dependecies.Concat(obj2.Dependecies);

        var origin = (obj1.Origin, obj2.Origin) switch
        {
            (ShaderOrigin.VertexShader, ShaderOrigin.VertexShader) => ShaderOrigin.VertexShader,
            (ShaderOrigin.VertexShader, ShaderOrigin.Global) => ShaderOrigin.VertexShader,
            (ShaderOrigin.Global, ShaderOrigin.VertexShader) => ShaderOrigin.VertexShader,

            (ShaderOrigin.FragmentShader, ShaderOrigin.FragmentShader) => ShaderOrigin.FragmentShader,
            (ShaderOrigin.FragmentShader, ShaderOrigin.Global) => ShaderOrigin.FragmentShader,
            (ShaderOrigin.Global, ShaderOrigin.FragmentShader) => ShaderOrigin.FragmentShader,

            (ShaderOrigin.FragmentShader, ShaderOrigin.VertexShader) => ShaderOrigin.FragmentShader,
            (ShaderOrigin.VertexShader, ShaderOrigin.FragmentShader) => ShaderOrigin.FragmentShader,

            _ => ShaderOrigin.Global
        };

        bool hasConflitct = 
            obj1.Origin != ShaderOrigin.Global && 
            obj2.Origin != ShaderOrigin.Global && 
            obj1.Origin != obj2.Origin;
        if (hasConflitct)
        {
            (ShaderObject vertObj, ShaderObject fragObj) = 
                obj1.Origin == ShaderOrigin.VertexShader ?
                (obj1, obj2) : (obj2, obj1);

            // TODO
            vertObj.Dependecies = vertObj.Dependecies
                .Append(null);

            deps = deps.Append(new InputDependence(vertObj.Expression, null));
        }

        return new T
        {
            Expression = newExpression,
            Dependecies = deps,
            Origin = origin
        };;
    }

    public override string ToString()
        => Expression;
    
    public static string GetStringName<T>()
        where T : ShaderObject
    {
        var type = typeof(T);
        if (type == typeof(FloatShaderObject))
            return "float";

        if (type == typeof(Vec2ShaderObject))
            return "vec2";

        if (type == typeof(Vec3ShaderObject))
            return "vec3";

        if (type == typeof(Vec4ShaderObject))
            return "vec4";
        
        return "unk";
    }
}