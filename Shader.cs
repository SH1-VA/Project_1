using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader
{
    public int Handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public Shader(string vertexPath, string fragmentPath)
    {
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        var VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);
        var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        GL.CompileShader(VertexShader);
        GL.CompileShader(FragmentShader);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);
        GL.LinkProgram(Handle);

        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);

        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        _uniformLocations = new Dictionary<string, int>();

        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);

            var location = GL.GetUniformLocation(Handle, key);

            _uniformLocations.Add(key, location);
        }
    }
    private static void CompileShader(int shader)
    {
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }
    private static void LinkProgram(int program)
    {
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            throw new Exception($"Error occurred whilst linking Program({program})");
        }
    }
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }
    public void SetInt(string name, int data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(_uniformLocations[name], data);
    }
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(_uniformLocations[name], data);
    }
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(Handle);
        GL.Uniform3(_uniformLocations[name], data);
    }
}