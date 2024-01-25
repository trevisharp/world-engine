/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Shaders;

using Dependencies;
using static ShaderOrigin;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public abstract record ShaderObject(
    ShaderType Type,
    string Expression,
    ShaderOrigin Origin,
    IEnumerable<ShaderDependence> Dependencies
) {
    public override string ToString()
        => Expression;

    public static implicit operator ShaderObject(float value) => value;
    public static implicit operator ShaderObject(double value) => (float)value;
    public static implicit operator ShaderObject(int value) => value;
    public static implicit operator ShaderObject(bool value) => value;
    public static implicit operator ShaderObject((float x, float y) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z, float w) value) => value;

    public static T Union<T>(string newExpression, T obj1, T obj2)
        where T : ShaderObject
    {
        var deps = obj1.Dependencies.Concat(obj2.Dependencies);

        var origin = (obj1.Origin, obj2.Origin) switch
        {
            (VertexShader, VertexShader) => VertexShader,
            (VertexShader, Global) => VertexShader,
            (Global, VertexShader) => VertexShader,

            (FragmentShader, FragmentShader) => FragmentShader,
            (FragmentShader, Global) => FragmentShader,
            (Global, FragmentShader) => FragmentShader,

            (FragmentShader, VertexShader) => FragmentShader,
            (VertexShader, FragmentShader) => FragmentShader,

            _ => Global
        };

        bool hasConflitct = 
            obj1.Origin != Global && 
            obj2.Origin != Global && 
            obj1.Origin != obj2.Origin;
        if (hasConflitct)
        {
            (ShaderObject vertObj, ShaderObject fragObj) = 
                obj1.Origin == VertexShader ?
                (obj1, obj2) : (obj2, obj1);

            var output = new OutputDependence(vertObj);
            
            deps = deps.Append(output);
        }

        var newObj = Activator.CreateInstance(
            typeof(T), obj1.Type, newExpression, origin, deps
        );
        return newObj as T;
    }
}