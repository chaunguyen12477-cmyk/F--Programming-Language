using Xunit;
using System;
using System.IO;
using System.Collections.Generic;

namespace Fminusminus.Tests
{
    public class LexerTests
    {
        [Fact]
        public void TestImportComputer()
        {
            string code = "import computer";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            Assert.Equal(3, tokens.Count); // IMPORT, COMPUTER, EOF
            Assert.Equal(TokenType.IMPORT, tokens[0].Type);
            Assert.Equal("import", tokens[0].Lexeme);
            Assert.Equal(TokenType.COMPUTER, tokens[1].Type);
            Assert.Equal("computer", tokens[1].Lexeme);
        }

        [Fact]
        public void TestPrintAndPrintln()
        {
            string code = @"
import computer
start()
{
    print(""Hello"")
    println(""World"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            bool hasPrint = false;
            bool hasPrintln = false;
            bool hasImport = false;
            bool hasComputer = false;
            
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.PRINT) hasPrint = true;
                if (token.Type == TokenType.PRINTLN) hasPrintln = true;
                if (token.Type == TokenType.IMPORT) hasImport = true;
                if (token.Type == TokenType.COMPUTER) hasComputer = true;
            }
            
            Assert.True(hasImport, "Should have IMPORT token");
            Assert.True(hasComputer, "Should have COMPUTER token");
            Assert.True(hasPrint, "Should have PRINT token");
            Assert.True(hasPrintln, "Should have PRINTLN token");
        }

        [Fact]
        public void TestStringInterpolation()
        {
            string code = @"import computer
start()
{
    print($""Hello {name}"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            Assert.Contains(tokens, t => t.Type == TokenType.STRING_INTERPOLATED);
        }

        [Fact]
        public void TestMemorySyntax()
        {
            string code = @"
import computer
start()
{
    memory.memoryleft
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            
            Assert.Contains(tokens, t => t.Type == TokenType.MEMORY);
            Assert.Contains(tokens, t => t.Type == TokenType.DOT);
        }

        [Fact]
        public void TestRequiredReturnAndEnd()
        {
            string code = @"
import computer
start()
{
    println(""Missing return and end"")
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            
            Assert.Throws<Exception>(() => parser.Parse());
        }
    }

    public class ParserTests
    {
        [Fact]
        public void TestValidProgram()
        {
            string code = @"
import computer
start()
{
    println(""Hello F--!"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            
            var exception = Record.Exception(() => parser.Parse());
            Assert.Null(exception);
        }

        [Fact]
        public void TestMissingImport()
        {
            string code = @"
start()
{
    println(""Hello"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            
            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Fact]
        public void TestMissingEnd()
        {
            string code = @"
import computer
start()
{
    println(""Hello"")
    return(0)
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            
            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Fact]
        public void TestMissingReturn()
        {
            string code = @"
import computer
start()
{
    println(""Hello"")
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            
            Assert.Throws<Exception>(() => parser.Parse());
        }
    }

    public class InterpreterTests
    {
        [Fact]
        public void TestHelloWorld()
        {
            string code = @"
import computer
start()
{
    println(""Hello F--!"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var interpreter = new Interpreter();
            
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            
            try
            {
                int result = interpreter.Execute(ast);
                Assert.Equal(0, result);
                Assert.Contains("Hello F--!", sw.ToString());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void TestVariableAssignment()
        {
            string code = @"
import computer
start()
{
    name = ""F--""
    version = 2.0
    println($""{name} v{version}"")
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var interpreter = new Interpreter();
            
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            
            try
            {
                int result = interpreter.Execute(ast);
                Assert.Equal(0, result);
                Assert.Contains("F-- v2", sw.ToString());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void TestMemoryAccess()
        {
            string code = @"
import computer
start()
{
    memory.memoryleft
    return(0)
    end()
}";
            var lexer = new Lexer(code);
            var tokens = lexer.ScanTokens();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var interpreter = new Interpreter();
            
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            
            try
            {
                int result = interpreter.Execute(ast);
                Assert.Equal(0, result);
                Assert.Contains("memoryleft:", sw.ToString());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
