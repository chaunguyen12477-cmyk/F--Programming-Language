// F-- Interpreter đơn giản
using System;
using System.Collections.Generic;
using System.IO;

class FSharpMinusInterpreter
{
    static Dictionary<string, object> variables = new Dictionary<string, object>();
    static string currentFile = "";
    
    static void Main(string[] args)
    {
        string code = File.ReadAllText("hello.f--");
        Interpret(code);
    }
    
    static void Interpret(string code)
    {
        var lines = code.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains("println"))
            {
                // Xử lý println
                var content = line.Split('"')[1];
                Console.WriteLine(content);
            }
            else if (line.Contains("io.cfile"))
            {
                // Xử lý tạo file
                var fileName = line.Split('"')[1];
                currentFile = fileName + ".txt";
            }
            else if (line.Contains("io.save()"))
            {
                // Xử lý lưu file
                File.WriteAllText(currentFile, "hello");
            }
        }
    }
}
