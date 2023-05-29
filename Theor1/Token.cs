using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theor1
{
    public enum TokenType
    {
        INTEGER, DOUBLE, STRING, DIM, AS, IF, THEN, 
        ELSE, END, PLUS, MINUS, MULTIPLY, DIVIDE, 
        OR, EQUAL, MORE, LESS, COMMA, LPAR, RPAR,
        IDENTIFIER, LITERAL, LINEBREAK, NETERM, EXPR, AND,
        MOREEQUAL, LESSEQUAL,LESSMORE
    }
    public class Token
    {
        public TokenType Type;
        public string Value;
        public Token(TokenType type)
        {
            Type = type;
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}", Type, Value);

        }
        static TokenType[] Delimiters = new TokenType[]
        {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, 
            TokenType.DIVIDE,TokenType.EQUAL, TokenType.MORE, 
            TokenType.LESS, TokenType.COMMA, TokenType.LPAR, TokenType.RPAR,
            TokenType.LINEBREAK
        };
        public static bool IsDelimiter(Token token)
        {
            return Delimiters.Contains(token.Type);
        }
        public static Dictionary<string, TokenType> SpecialWords = new Dictionary<string, TokenType>() {
            {"integer", TokenType.INTEGER},
            {"double", TokenType.DOUBLE},
            {"string", TokenType.STRING},
            {"Dim", TokenType.DIM},
            {"as", TokenType.AS},
            {"if", TokenType.IF},
            {"then", TokenType.THEN},
            {"else", TokenType.ELSE},
            {"end", TokenType.END},
            {"or", TokenType.OR},
            {"and", TokenType.AND}
        };
        public static bool IsSpecialWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            return (SpecialWords.ContainsKey(word));
        }
        public static Dictionary<char, TokenType> SpecialSymbols = new Dictionary<char, TokenType>() 
        {
            {'+', TokenType.PLUS},
            {'-', TokenType.MINUS},
            {'*', TokenType.MULTIPLY},
            {'/', TokenType.DIVIDE},
            {'=', TokenType.EQUAL},            
            {'>', TokenType.MORE},
            {'<', TokenType.LESS},
            {',', TokenType.COMMA},
            {'(', TokenType.LPAR},
            {')', TokenType.RPAR},
            {'\n',TokenType.LINEBREAK}
        };
        public static bool IsSpecialSymbol(char ch)
        {
            return SpecialSymbols.ContainsKey(ch);
        }
    }
}