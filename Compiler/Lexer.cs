using System;
using System.Collections.Generic;
using System.Text;

namespace Fminusminus
{
    public enum TokenType
    {
        // Keywords
        IMPORT, COMPUTER, START, RETURN, END,
        PRINT, PRINTLN, AT, IO, MEMORY,
        
        // Literals
        IDENTIFIER, STRING, STRING_INTERPOLATED, NUMBER,
        
        // Punctuation
        LPAREN, RPAREN, LBRACE, RBRACE, LBRACKET, RBRACKET,
        DOT, COMMA, SEMICOLON, COLON, ASSIGN,
        
        // Operators
        PLUS, MINUS, STAR, SLASH, PERCENT,
        
        // Comparison
        EQUAL, NOT_EQUAL, LESS, LESS_EQUAL, GREATER, GREATER_EQUAL,
        
        // Special
        NEWLINE, COMMENT, EOF, ERROR
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(TokenType type, string lexeme, object literal, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Type} '{Lexeme}' at {Line}:{Column}";
        }
    }

    public class Lexer
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private int _column = 1;
        private readonly List<string> _errors = new();

        private static readonly Dictionary<string, TokenType> _keywords = new()
        {
            { "import", TokenType.IMPORT },
            { "computer", TokenType.COMPUTER },
            { "start", TokenType.START },
            { "return", TokenType.RETURN },
            { "end", TokenType.END },
            { "print", TokenType.PRINT },
            { "println", TokenType.PRINTLN },
            { "at", TokenType.AT },
            { "io", TokenType.IO },
            { "memory", TokenType.MEMORY }
        };

        public Lexer(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line, _column));
            
            if (_errors.Count > 0)
                throw new Exception(string.Join("\n", _errors));
            
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                // Single character tokens
                case '(': AddToken(TokenType.LPAREN); break;
                case ')': AddToken(TokenType.RPAREN); break;
                case '{': AddToken(TokenType.LBRACE); break;
                case '}': AddToken(TokenType.RBRACE); break;
                case '[': AddToken(TokenType.LBRACKET); break;
                case ']': AddToken(TokenType.RBRACKET); break;
                case '.': AddToken(TokenType.DOT); break;
                case ',': AddToken(TokenType.COMMA); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case ':': AddToken(TokenType.COLON); break;
                
                // Operators
                case '+': AddToken(TokenType.PLUS); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '*': AddToken(TokenType.STAR); break;
                case '/': 
                    if (Match('/'))
                    {
                        // Comment đến hết dòng
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                        AddToken(TokenType.COMMENT);
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case '%': AddToken(TokenType.PERCENT); break;
                
                // Assignment
                case '=': 
                    if (Match('='))
                        AddToken(TokenType.EQUAL);
                    else
                        AddToken(TokenType.ASSIGN);
                    break;
                
                // Comparison
                case '!':
                    if (Match('='))
                        AddToken(TokenType.NOT_EQUAL);
                    else
                        AddToken(TokenType.ERROR, "Unexpected '!'");
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                
                // String literals
                case '"':
                    StringLiteral();
                    break;
                
                // String interpolation
                case '$':
                    if (Peek() == '"')
                    {
                        Advance();
                        StringInterpolation();
                    }
                    else
                    {
                        AddToken(TokenType.IDENTIFIER, "$");
                    }
                    break;
                
                // Whitespace
                case ' ':
                case '\r':
                case '\t':
                    // Ignore
                    break;
                case '\n':
                    _line++;
                    _column = 1;
                    AddToken(TokenType.NEWLINE);
                    break;
                
                default:
                    if (char.IsDigit(c))
                    {
                        Number();
                    }
                    else if (char.IsLetter(c) || c == '_')
                    {
                        Identifier();
                    }
                    else
                    {
                        _errors.Add($"fmm002: Unexpected character '{c}' at line {_line}, column {_column}");
                    }
                    break;
            }
        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_') Advance();
            
            string text = _source.Substring(_start, _current - _start);
            TokenType type = _keywords.TryGetValue(text, out var keyword) ? keyword : TokenType.IDENTIFIER;
            
            AddToken(type, text);
        }

        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();
            
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                Advance(); // Consume '.'
                while (char.IsDigit(Peek())) Advance();
                AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
            }
            else
            {
                AddToken(TokenType.NUMBER, int.Parse(_source.Substring(_start, _current - _start)));
            }
        }

        private void StringLiteral()
        {
            StringBuilder sb = new();
            
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                if (Peek() == '\\')
                {
                    Advance();
                    sb.Append(HandleEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Advance();
                }
            }
            
            if (IsAtEnd())
            {
                _errors.Add($"fmm002: Unterminated string at line {_line}");
                return;
            }
            
            Advance(); // Consume closing "
            AddToken(TokenType.STRING, sb.ToString());
        }

        private void StringInterpolation()
        {
            StringBuilder sb = new();
            
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                
                if (Peek() == '{')
                {
                    sb.Append('{');
                    Advance();
                    
                    int braceCount = 1;
                    while (braceCount > 0 && !IsAtEnd())
                    {
                        if (Peek() == '{') braceCount++;
                        if (Peek() == '}') braceCount--;
                        if (Peek() == '\n') _line++;
                        
                        sb.Append(Peek());
                        Advance();
                    }
                }
                else if (Peek() == '\\')
                {
                    Advance();
                    sb.Append(HandleEscapeSequence());
                }
                else
                {
                    sb.Append(Peek());
                    Advance();
                }
            }
            
            if (IsAtEnd())
            {
                _errors.Add($"fmm002: Unterminated interpolated string at line {_line}");
                return;
            }
            
            Advance(); // Consume closing "
            AddToken(TokenType.STRING_INTERPOLATED, sb.ToString());
        }

        private char HandleEscapeSequence()
        {
            return Peek() switch
            {
                'n' => '\n',
                't' => '\t',
                'r' => '\r',
                '"' => '"',
                '\\' => '\\',
                '{' => '{',
                '}' => '}',
                _ => Peek()
            };
        }

        private bool Match(char expected)
        {
            if (IsAtEnd() || _source[_current] != expected) return false;
            _current++;
            _column++;
            return true;
        }

        private char Peek() => IsAtEnd() ? '\0' : _source[_current];
        private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
        private bool IsAtEnd() => _current >= _source.Length;
        
        private char Advance()
        {
            _current++;
            _column++;
            return _source[_current - 1];
        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line, _column - text.Length));
        }
    }
}
