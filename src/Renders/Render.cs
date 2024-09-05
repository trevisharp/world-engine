/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Primitives;
using Exceptions;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render(
    Delegate function,
    params object?[] curryingArguments
    ) : DynamicObject
{
    public void Load()
    {

    }

    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        var parameters = function.Method.GetParameters();
        object[] arguments = [
            ..curryingArguments, ..args
        ];
        var argumentCount = MeasureArguments(arguments);

        if (argumentCount < parameters.Length)
        {
            result = Curry(args ?? []);
            return true;
        }

        if (argumentCount > parameters.Length)
            throw new ExcessOfArgumentsException();
        
        if (arguments.Length > 0 && arguments[0] is not Polygon)
            throw new MissingPolygonException();
        
        var ctx = RenderContext.GetContext();
        if (ctx is null)
        {
            TrueRender();
            result = null;
            return true;
        }

        FakeRender(out result);
        return true;
    }

    public Render Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..args ]);

    public static implicit operator Action(Render render)
    {
        throw new NotImplementedException();
    }

    void FakeRender(out object? result)
    {
        throw new NotImplementedException();
    }

    void TrueRender()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generate a Shader object with dependencies based on ParameterInfo.
    /// </summary>
    static ShaderObject? GenerateDependence(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        var name = parameter.Name!;

        if (parameter.ParameterType == typeof(FloatShaderObject))
        {
            var dep = new UniformFloatDependence(name);
            return new FloatShaderObject(
                name, ShaderOrigin.Global, [dep]
            );
        }

        if (parameter.ParameterType == typeof(Sampler2DShaderObject))
        {
            var dep = new TextureDependence(name);
            return new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [dep]
            );
        }
        
        return null;
    }

    /// <summary>
    /// Measure the size of parameters args when displayed based on size.
    /// </summary>
    static int MeasureArguments(object[] args)
        => args.Sum(arg => arg switch
        {
            Vec2 or Vec2ShaderObject => 2,
            Vec3 or Vec3ShaderObject => 3,
            Vec4 or Vec4ShaderObject => 4,
            float[] arr => arr.Length,
            _ => 1
        });

    /// <summary>
    /// Fill parameters data on a vector to run a function.
    /// </summary>
    static void DisplayParameters(object[] result, object[] args)
    {
        int index = 0;
        foreach (var arg in args)
            index = DisplayParameters(arg, result, index);
        
        if (index < args.Length)
            throw new Exception(); // TODO: Use custom exception
    }
    
    /// <summary>
    /// Set arr data from a index based on arg data size.
    /// </summary>
    static int DisplayParameters(object arg, object[] arr, int index)
    {
        switch (arg)
        {
            case float num:
                verify(1);
                add(num);
                break;
            
            case int num:
                verify(1);
                add((float)num);
                break;
            
            case double num:
                verify(1);
                add((float)num);
                break;
                
            case Vec2 vec:
                verify(2);
                add(vec.X);
                add(vec.Y);
                break;
                
            case Vec3 vec:
                verify(3);
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                break;
                
            case Vec4 vec:
                verify(4);
                add(vec.X);
                add(vec.Y);
                add(vec.Z);
                add(vec.W);
                break;
            
            case float[] subArray:
                verify(subArray.Length);
                foreach (var value in subArray)
                    add(value);
                break;
            
            case Texture img:
                verify(1);
                add(img);
                break;
            
            default:
                throw new Exception(); // TODO: Use custom exception
        }

        return index;

        void verify(int size)
        {
            if (index + size <= arr.Length)
                return;
            
            throw new Exception(); // TODO: Use custom exception
        }

        void add(object value)
            => arr[index++] = value;
    }
}