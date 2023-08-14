/* Author:  Leonardo Trevisan Silio
 * Date:    13/08/2023
 */
using System;
using System.Text;
using System.Linq;

namespace Radiance;

using Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

using RenderFunctions;

/// <summary>
/// A facede with all utils to use Radiance features.
/// </summary>
public static class RadianceUtils
{
    public static Action CreateRender(Action<RenderOperations> render)
    {
        GenericRenderFunction renderFunction = render;
        renderFunction.Load();
        return renderFunction;
    }

    public static Action<T> CreateRender<T>(Action<RenderOperations, T> render)
    {
        GenericRenderFunction<T> renderFunction = render;
        renderFunction.Load();
        return renderFunction;
    }

    public static Action<T1, T2> CreateRender<T1, T2>(Action<RenderOperations, T1, T2> render)
    {
        GenericRenderFunction<T1, T2> renderFunction = render;
        renderFunction.Load();
        return renderFunction;
    }

    internal readonly static TimeShaderInput _t = new();
    internal readonly static WidthWindowDependence _width = new();
    internal readonly static HeightWindowDependence _height = new();
    
    public static FloatShaderObject t => new(_t.Name, _t);
    public static Vector i => new(1, 0, 0); 
    public static Vector j => new(0, 1, 0); 
    public static Vector k => new(0, 0, 1);
    public static FloatShaderObject width => new(_width.Name, _width);
    public static FloatShaderObject height => new(_height.Name, _height);

    public static FloatShaderObject cos(FloatShaderObject angle)
        => floatFunc("cos", angle);

    public static FloatShaderObject cos(ShaderDependence angle)
    {
        return new FloatShaderObject($"cos({angle})", angle);
    }

    public static FloatShaderObject sin(FloatShaderObject angle)
        => floatFunc("sin", angle);

    public static FloatShaderObject tan(FloatShaderObject angle)
        => floatFunc("tan", angle);

    public static FloatShaderObject exp(FloatShaderObject angle)
        => floatFunc("exp", angle);

    public static FloatShaderObject exp2(FloatShaderObject angle)
        => floatFunc("exp2", angle);

    public static FloatShaderObject log(FloatShaderObject angle)
        => floatFunc("log", angle);

    public static FloatShaderObject log2(FloatShaderObject angle)
        => floatFunc("log2", angle);

    public static FloatShaderObject smoothstep(
        FloatShaderObject edge0,
        FloatShaderObject edge1,
        FloatShaderObject x
    )  => operation<FloatShaderObject>("smoothstep", edge0, edge1, x);

    public static FloatShaderObject smootherstep(
        FloatShaderObject edge0,
        FloatShaderObject edge1,
        FloatShaderObject x
    ) => operation<FloatShaderObject>("smootherstep", edge0, edge1, x);

    public static FloatShaderObject length(Vec2ShaderObject vec) 
        => func<FloatShaderObject, Vec2ShaderObject>("length", vec);

    public static FloatShaderObject length(Vec3ShaderObject vec) 
        => func<FloatShaderObject, Vec3ShaderObject>("length", vec);

    public static FloatShaderObject distance(Vec2ShaderObject p0, Vec2ShaderObject p1)
        => operation<Vec2ShaderObject>("distance", p0, p1);

    public static FloatShaderObject distance(Vec3ShaderObject p0, Vec3ShaderObject p1)
        => operation<Vec3ShaderObject>("distance", p0, p1);

    public static FloatShaderObject dot(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => operation<Vec3ShaderObject>("dot", v0, v1);

    public static FloatShaderObject dot(Vec2ShaderObject v0, Vec2ShaderObject v1)
        => operation<Vec2ShaderObject>("dot", v0, v1);

    public static Vec3ShaderObject cross(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => func<Vec3ShaderObject, Vec3ShaderObject, Vec3ShaderObject>("cross", v0, v1);
    
    public static FloatShaderObject round(FloatShaderObject angle)
        => floatFunc("round", angle);

    public static FloatShaderObject floor(FloatShaderObject angle)
        => floatFunc("floor", angle);

    public static FloatShaderObject ceil(FloatShaderObject angle)
        => floatFunc("ceil", angle);

    public static FloatShaderObject trunc(FloatShaderObject angle)
        => floatFunc("trunc", angle);

    public static FloatShaderObject max(FloatShaderObject x, FloatShaderObject y)
        => func<
            FloatShaderObject,
            FloatShaderObject,
            FloatShaderObject
            >("max", x, y);
    
    public static FloatShaderObject min(FloatShaderObject x, FloatShaderObject y)
        => func<
            FloatShaderObject,
            FloatShaderObject,
            FloatShaderObject
            >("min", x, y);
    
    public static T mix<T>(T x, T y, FloatShaderObject a) 
        where T : ShaderObject, new()
        => func<T, T, T, FloatShaderObject>("mix", x, y, a);

    private static FloatShaderObject operation<T>(string name, T obj1, T obj2)
        where T : ShaderObject
        => func<FloatShaderObject, T, T>(name, obj1, obj2);

    private static FloatShaderObject operation<T>(string name, T obj1, T obj2, T obj3)
        where T : ShaderObject
        => func<FloatShaderObject, T, T, T>(name, obj1, obj2, obj3);

    private static FloatShaderObject floatFunc(string name, FloatShaderObject input)
        => func<FloatShaderObject, FloatShaderObject>(name, input);

    private static R func<R, P1>(
        string name, 
        P1 obj1
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
    {
        var result = new R();

        result.Dependecies = obj1.Dependecies;
        
        result.Expression = 
            buildObject(name, obj1);

        return result;
    }

    private static R func<R, P1, P2>(
        string name, 
        P1 obj1, P2 obj2
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
        where P2 : ShaderObject
    {
        var result = new R();

        result.Dependecies = 
            obj1.Dependecies
            .Concat(obj2.Dependecies);
        
        result.Expression = 
            buildObject(name, obj1, obj2);

        return result;
    }

    private static R func<R, P1, P2, P3>(
        string name, 
        P1 obj1, P2 obj2, P3 obj3
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
        where P2 : ShaderObject
        where P3 : ShaderObject
    {
        var result = new R();

        result.Dependecies = 
            obj1.Dependecies
            .Concat(obj2.Dependecies)
            .Concat(obj3.Dependecies);
        
        result.Expression = 
            buildObject(name, obj1, obj2, obj3);

        return result;
    }

    private static string buildObject(
        string funcName,
        params object[] inputs
    )
    {
        var sb = new StringBuilder();
        sb.Append($"{funcName}(");

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            if (inputs[i] is ShaderObject input)
                sb.Append(input.Expression);
            else if (inputs[i] is ShaderDependence dependence)
                sb.Append(dependence.Name);
            sb.Append(", ");
        }

        if (inputs.Length > 0)
        {
            if (inputs[^1] is ShaderObject input)
                sb.Append(input.Expression);
            else if (inputs[^1] is ShaderDependence dependence)
                sb.Append(dependence.Name);
            sb.Append(")");
        }
            
        return sb.ToString();
    }
}