using System;
using System.IO;

namespace Fminusminus
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine(@"
    ╔══════════════════════════════════╗
    ║  ███████╗  ███╗   ███╗  ██╗██╗  ║
    ║  ██╔════╝  ████╗ ████║  ██║██║  ║
    ║  █████╗    ██╔████╔██║  ██║██║  ║
    ║  ██╔══╝    ██║╚██╔╝██║  ██║██║  ║
    ║  ██║       ██║ ╚═╝ ██║  ██║██║  ║
    ║  ╚═╝       ╚═╝     ╚═╝  ╚═╝╚═╝  ║
    ║                                  ║
    ║     F-- PROGRAMMING LANGUAGE     ║
    ║        Version 2.0.0             ║
    ║     Created by RealMG (13)       ║
    ╚══════════════════════════════════╝
    ");

            if (args.Length == 0)
            {
                ShowHelp();
                return 1;
            }

            string command = args[0].ToLower();
            
            try
            {
                switch (command)
                {
                    case "run":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Missing filename");
                            return 1;
                        }
                        return RunFile(args[1]);
                        
                    case "ast":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Missing filename");
                            return 1;
                        }
                        return ShowAST(args[1]);
                        
                    case "--version":
                        ShowVersion();
                        return 0;
                        
                    case "--help":
                        ShowHelp();
                        return 0;
                        
                    default:
                        // Assume it's a filename
                        return RunFile(args[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001b[31m{ex.Message}\u001b[0m");
                return 1;
            }
        }

        static int RunFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"fmm004: File not found: {filename}");
                return 1;
            }

            Console.WriteLine($"\u001b[36mRunning: {filename}\u001b[0m\n");
            
            string code = File.ReadAllText(filename);
            
            // Lexer
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            // Parser
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            
            // Interpreter
            var interpreter = new Interpreter();
            int result = interpreter.Execute(ast);
            
            Console.WriteLine($"\n\u001b[32mProgram completed with exit code: {result}\u001b[0m");
            return result;
        }

        static int ShowAST(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"fmm004: File not found: {filename}");
                return 1;
            }

            string code = File.ReadAllText(filename);
            
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            
            Console.WriteLine("\n\u001b[36m=== Abstract Syntax Tree ===\u001b[0m\n");
            ast.Print();
            
            return 0;
        }

        static void ShowVersion()
        {
            Console.WriteLine("F-- Programming Language v2.0.0");
            Console.WriteLine("Copyright (c) 2026 RealMG");
            Console.WriteLine("License: MIT");
            Console.WriteLine("\n\"The backward step of humanity, but forward step in creativity!\"");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: fminus <command> [options]");
            Console.WriteLine("\nCommands:");
            Console.WriteLine("  run <file>     Run F-- program");
            Console.WriteLine("  ast <file>     Show AST tree");
            Console.WriteLine("  --version      Show version");
            Console.WriteLine("  --help         Show this help");
            Console.WriteLine("\nExamples:");
            Console.WriteLine("  fminus run hello.f--");
            Console.WriteLine("  fminus ast hello.f--");
            Console.WriteLine("\nF-- Syntax:");
            Console.WriteLine("  import computer");
            Console.WriteLine("  start() {");
            Console.WriteLine("      println(\"Hello!\");");
            Console.WriteLine("      print($\"Value: {var}\");");
            Console.WriteLine("      return(0);");
            Console.WriteLine("      end()");
            Console.WriteLine("  }");
        }
    }
}
