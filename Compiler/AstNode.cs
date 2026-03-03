using System;
using System.Collections.Generic;

namespace FSharpMinus.Compiler
{
    // Base node
    public abstract class AstNode
    {
        public virtual void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}{GetType().Name}");
        }
    }

    // Program node
    public class ProgramNode : AstNode
    {
        public List<ImportNode> Imports { get; set; } = new();
        public List<UsingNode> Usings { get; set; } = new();
        public StartBlockNode StartBlock { get; set; }
        public List<StatementNode> Statements => StartBlock?.Statements ?? new();

        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Program");
            
            foreach (var import in Imports)
                import.Print(indent + 2);
                
            foreach (var using_ in Usings)
                using_.Print(indent + 2);
                
            StartBlock?.Print(indent + 2);
        }
    }

    public class ImportNode : AstNode
    {
        public string ModuleName { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Import: {ModuleName}");
        }
    }

    public class UsingNode : AstNode
    {
        public string Namespace { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Using: {Namespace}");
        }
    }

    public class StartBlockNode : AstNode
    {
        public List<StatementNode> Statements { get; set; } = new();
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}StartBlock");
            foreach (var stmt in Statements)
                stmt.Print(indent + 2);
        }
    }

    public abstract class StatementNode : AstNode { }

    public class PrintlnStatementNode : StatementNode
    {
        public bool IsInterpolated { get; set; }
        public string Value { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Println: {(IsInterpolated ? "$" : "")}\"{Value}\"");
        }
    }

    public class ReturnStatementNode : StatementNode
    {
        public int ReturnCode { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Return: {ReturnCode}");
        }
    }

    public class IOStatementNode : StatementNode
    {
        public string Operation { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new();
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}IO.{Operation}({string.Join(", ", Parameters)})");
        }
    }

    public class MemoryStatementNode : StatementNode
    {
        public string Property { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Memory.{Property}");
        }
    }

    public class AtBlockNode : StatementNode
    {
        public string FileName { get; set; }
        public List<StatementNode> Statements { get; set; } = new();
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}At \"{FileName}\"");
            foreach (var stmt in Statements)
                stmt.Print(indent + 2);
        }
    }

    public class AssignmentNode : StatementNode
    {
        public string VariableName { get; set; }
        public LiteralNode Value { get; set; }
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Assignment: {VariableName} = {Value}");
        }
    }

    public class FunctionCallNode : StatementNode
    {
        public string FunctionName { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new();
        
        public override void Print(int indent)
        {
            Console.WriteLine($"{new string(' ', indent)}Call: {FunctionName}({string.Join(", ", Parameters)})");
        }
    }

    public class LiteralNode
    {
        public string Type { get; set; }
        public string Value { get; set; }
        
        public override string ToString() => Value;
    }

    public class ParameterNode
    {
        public string Type { get; set; }
        public string Value { get; set; }
        
        public override string ToString() => Value;
    }
}
