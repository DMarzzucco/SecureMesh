using System;
using System.Security.Cryptography;

namespace IdentifyService.Utils.Helper;

public class CodeGeneration
{
    public string InvokeCodeGeneration()
    {
        int length = 6;
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var code = BitConverter.ToUInt32(bytes, 0) % 1000000;

        return code.ToString("D6");
    }
}
