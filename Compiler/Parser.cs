using System;
using System.Collections.Generic;
using System.Linq;

namespace FSharpMinus.Compiler
{
    /// <summary>
    /// Parser cho F-- 
    /// Chuyển đổi tokens thành Abstract Syntax Tree (AST)
    /// </summary>
    public class Parser
    {
        private List<Token> _tokens;
        private int _position = 0;
        private List<string> _errors = new List<string>();

        // Các từ khóa của F--
        private readonly HashSet<string> _keywords = new HashSet<string>
        {
            "import", "using", "namespace", "start", "return",
            "println", "memory", "io", "at", "if", "else",
            "true", "false", "null"
        };

        public Parser(List<Token> tokens)
        {
            _tokens = tokens ?? new List<Token>();
        }

        /// <summary>
        /// Parse chương trình F-- thành AST
        /// </summary>
        public ProgramNode Parse()
        {
            var program = new ProgramNode();

            while (!IsAtEnd())
            {
                try
                {
                    if (Match(TokenType.IMPORT))
                    {
                        program.Imports.Add(ParseImport());
                    }
                    else if (Match(TokenType.USING))
                    {
                        program.Usings.Add(ParseUsing());
                    }
                    else if (Check(TokenType.IDENTIFIER) && Peek().Value == "start")
                    {
                        program.StartBlock = ParseStartBlock();
                    }
                    else
                    {
                        // Bỏ qua những thứ không hiểu (comment, whitespace)
                        Advance();
                    }
                }
                catch (ParseException ex)
                {
                    _errors.Add($"fmm001: {ex.Message} at line {Previous().Line}");
                    // Recovery - skip to next line
                    while (!IsAtEnd() && Peek().Type != TokenType.NEWLINE)
                        Advance();
                }
            }

            if (_errors.Any())
            {
                throw new ParseException(string.Join("\n", _errors));
            }

            return program;
        }

        /// <summary>
        /// Parse import statement: import system
        /// </summary>
        private ImportNode ParseImport()
        {
            var import = new ImportNode
            {
                ImportToken = Previous()
            };

            if (Match(TokenType.IDENTIFIER))
            {
                import.ModuleName = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected module name after 'import'");
            }

            Consume(TokenType.NEWLINE, "Expected newline after import");
            return import;
        }

        /// <summary>
        /// Parse using statement: using namespace sys
        /// </summary>
        private UsingNode ParseUsing()
        {
            var usingNode = new UsingNode
            {
                UsingToken = Previous()
            };

            if (Match(TokenType.NAMESPACE))
            {
                // Đã có từ khóa namespace
            }

            if (Match(TokenType.IDENTIFIER))
            {
                usingNode.Namespace = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected namespace name after 'using'");
            }

            Consume(TokenType.NEWLINE, "Expected newline after using");
            return usingNode;
        }

        /// <summary>
        /// Parse start block: start() { ... }
        /// </summary>
        private StartBlockNode ParseStartBlock()
        {
            var start = new StartBlockNode();

            // Parse 'start'
            Consume(TokenType.IDENTIFIER, "Expected 'start'");
            
            // Parse '('
            Consume(TokenType.LPAREN, "Expected '(' after start");
            
            // Parse ')'
            Consume(TokenType.RPAREN, "Expected ')' after start(");

            // Parse '{'
            Consume(TokenType.LBRACE, "Expected '{' to start block");

            // Parse các statements bên trong
            while (!Check(TokenType.RBRACE) && !IsAtEnd())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    start.Statements.Add(statement);
                }
            }

            // Parse '}'
            Consume(TokenType.RBRACE, "Expected '}' to end block");

            return start;
        }

        /// <summary>
        /// Parse một câu lệnh
        /// </summary>
        private StatementNode ParseStatement()
        {
            // Bỏ qua newline
            while (Match(TokenType.NEWLINE)) { }

            if (IsAtEnd()) return null;

            if (Match(TokenType.IDENTIFIER))
            {
                var identifier = Previous().Value;

                switch (identifier)
                {
                    case "println":
                        return ParsePrintlnStatement();
                    case "return":
                        return ParseReturnStatement();
                    case "io":
                        return ParseIOStatement();
                    case "memory":
                        return ParseMemoryStatement();
                    case "at":
                        return ParseAtBlockStatement();
                    default:
                        // Có thể là biến hoặc function call
                        if (Check(TokenType.ASSIGN))
                        {
                            return ParseAssignmentStatement(identifier);
                        }
                        else if (Check(TokenType.LPAREN))
                        {
                            return ParseFunctionCall(identifier);
                        }
                        break;
                }
            }
            else if (Match(TokenType.COMMENT))
            {
                // Skip comments
                return null;
            }

            // Nếu không hiểu, bỏ qua dòng này
            while (!Check(TokenType.NEWLINE) && !IsAtEnd())
                Advance();

            return null;
        }

        /// <summary>
        /// Parse println statement: println("Hello") hoặc println($"Hello {var}")
        /// </summary>
        private PrintlnStatementNode ParsePrintlnStatement()
        {
            var println = new PrintlnStatementNode();

            Consume(TokenType.LPAREN, "Expected '(' after println");

            // Kiểm tra string interpolation
            if (Match(TokenType.STRING_INTERPOLATED))
            {
                println.IsInterpolated = true;
                println.Value = Previous().Value;
            }
            else if (Match(TokenType.STRING))
            {
                println.IsInterpolated = false;
                println.Value = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected string after println(");
            }

            Consume(TokenType.RPAREN, "Expected ')' after string");

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return println;
        }

        /// <summary>
        /// Parse return statement: return(0)
        /// </summary>
        private ReturnStatementNode ParseReturnStatement()
        {
            var ret = new ReturnStatementNode();

            Consume(TokenType.LPAREN, "Expected '(' after return");

            if (Match(TokenType.NUMBER))
            {
                ret.ReturnCode = int.Parse(Previous().Value);
            }
            else
            {
                throw new ParseException("Expected return code after return(");
            }

            Consume(TokenType.RPAREN, "Expected ')' after return code");

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return ret;
        }

        /// <summary>
        /// Parse IO statement: io.cfile(), io.println(), io.save()
        /// </summary>
        private IOStatementNode ParseIOStatement()
        {
            var io = new IOStatementNode();

            Consume(TokenType.DOT, "Expected '.' after io");

            if (Match(TokenType.IDENTIFIER))
            {
                io.Operation = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected IO operation after io.");
            }

            // Parse parameters nếu có
            if (Match(TokenType.LPAREN))
            {
                io.Parameters = ParseParameters();
                Consume(TokenType.RPAREN, "Expected ')' after parameters");
            }

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return io;
        }

        /// <summary>
        /// Parse memory statement: memory.memoryleft
        /// </summary>
        private MemoryStatementNode ParseMemoryStatement()
        {
            var memory = new MemoryStatementNode();

            Consume(TokenType.DOT, "Expected '.' after memory");

            if (Match(TokenType.IDENTIFIER))
            {
                memory.Property = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected memory property after memory.");
            }

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return memory;
        }

        /// <summary>
        /// Parse at block: at "file.txt" { ... }
        /// </summary>
        private AtBlockNode ParseAtBlockStatement()
        {
            var atBlock = new AtBlockNode();

            if (Match(TokenType.STRING))
            {
                atBlock.FileName = Previous().Value;
            }
            else
            {
                throw new ParseException("Expected filename after 'at'");
            }

            Consume(TokenType.LBRACE, "Expected '{' to start at block");

            // Parse các statements bên trong block
            while (!Check(TokenType.RBRACE) && !IsAtEnd())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    atBlock.Statements.Add(statement);
                }
            }

            Consume(TokenType.RBRACE, "Expected '}' to end at block");

            return atBlock;
        }

        /// <summary>
        /// Parse assignment: variable = value
        /// </summary>
        private AssignmentNode ParseAssignmentStatement(string identifier)
        {
            var assignment = new AssignmentNode
            {
                VariableName = identifier
            };

            Consume(TokenType.ASSIGN, "Expected '=' in assignment");

            if (Match(TokenType.STRING))
            {
                assignment.Value = new LiteralNode { Type = "string", Value = Previous().Value };
            }
            else if (Match(TokenType.NUMBER))
            {
                assignment.Value = new LiteralNode { Type = "number", Value = Previous().Value };
            }
            else
            {
                throw new ParseException("Expected value after '='");
            }

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return assignment;
        }

        /// <summary>
        /// Parse function call: functionName()
        /// </summary>
        private FunctionCallNode ParseFunctionCall(string functionName)
        {
            var call = new FunctionCallNode
            {
                FunctionName = functionName
            };

            Consume(TokenType.LPAREN, "Expected '(' after function name");
            call.Parameters = ParseParameters();
            Consume(TokenType.RPAREN, "Expected ')' after parameters");

            // Optional semicolon
            Match(TokenType.SEMICOLON);

            return call;
        }

        /// <summary>
        /// Parse parameters: (param1, param2, ...)
        /// </summary>
        private List<ParameterNode> ParseParameters()
        {
            var parameters = new List<ParameterNode>();

            if (!Check(TokenType.RPAREN))
            {
                do
                {
                    if (Match(TokenType.STRING))
                    {
                        parameters.Add(new ParameterNode
                        {
                            Type = "string",
                            Value = Previous().Value
                        });
                    }
                    else if (Match(TokenType.NUMBER))
                    {
                        parameters.Add(new ParameterNode
                        {
                            Type = "number",
                            Value = Previous().Value
                        });
                    }
                    else if (Match(TokenType.IDENTIFIER))
                    {
                        parameters.Add(new ParameterNode
                        {
                            Type = "identifier",
                            Value = Previous().Value
                        });
                    }
                } while (Match(TokenType.COMMA));
            }

            return parameters;
        }

        // ==================== Helper Methods ====================

        private Token Peek() => _tokens[_position];
        private Token Previous() => _tokens[_position - 1];
        private bool IsAtEnd() => _position >= _tokens.Count;
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        private Token Advance()
        {
            if (!IsAtEnd()) _position++;
            return Previous();
        }

        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new ParseException($"{message} - found '{Peek().Value}' at line {Peek().Line}");
        }
    }

    // ==================== AST Nodes ====================

    public class ProgramNode
    {
        public List<ImportNode> Imports { get; set; } = new();
        public List<UsingNode> Usings { get; set; } = new();
        public StartBlockNode StartBlock { get; set; }
    }

    public class ImportNode
    {
        public Token ImportToken { get; set; }
        public string ModuleName { get; set; }
    }

    public class UsingNode
    {
        public Token UsingToken { get; set; }
        public string Namespace { get; set; }
    }

    public class StartBlockNode
    {
        public List<StatementNode> Statements { get; set; } = new();
    }

    public abstract class StatementNode { }

    public class PrintlnStatementNode : StatementNode
    {
        public bool IsInterpolated { get; set; }
        public string Value { get; set; }
    }

    public class ReturnStatementNode : StatementNode
    {
        public int ReturnCode { get; set; }
    }

    public class IOStatementNode : StatementNode
    {
        public string Operation { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new();
    }

    public class MemoryStatementNode : StatementNode
    {
        public string Property { get; set; }
    }

    public class AtBlockNode : StatementNode
    {
        public string FileName { get; set; }
        public List<StatementNode> Statements { get; set; } = new();
    }

    public class AssignmentNode : StatementNode
    {
        public string VariableName { get; set; }
        public LiteralNode Value { get; set; }
    }

    public class FunctionCallNode : StatementNode
    {
        public string FunctionName { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new();
    }

    public class LiteralNode
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class ParameterNode
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    // ==================== Exception ====================

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
