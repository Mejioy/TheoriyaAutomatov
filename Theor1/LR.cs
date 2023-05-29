using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theor1
{
    public class LR
    {
        public List<Troyka> operatsii = new List<Troyka>();
        List<Token> tokens = new List<Token>();
        Stack<Token> lexemStack = new Stack<Token>();
        Stack<int> stateStack = new Stack<int>();
        int nextLex = 0;
        int state = 0;
        int lastEXPRind;
        bool firstEXPR = true;
        bool isEnd = false;
        public LR(List<Token> vvodtoken)
        {
            tokens = vvodtoken;
        }
        private Token GetLexeme(int nextLex)
        {
            return tokens[nextLex];
        }
        private void Shift()
        {
            if (nextLex == tokens.Count)
                throw new Exception("Список лексем пуст, но анализ не был завершён.");
            if ((lexemStack.Count > 0) && (tokens[nextLex].Type == lexemStack.Peek().Type))
                throw new Exception($"После {ConvertLex(lexemStack.Peek().Type)} не может следовать {ConvertLex(GetLexeme(nextLex).Type)}.");
            lexemStack.Push(GetLexeme(nextLex));
            nextLex++;
        }
        private void GoToState(int state)
        {
            stateStack.Push(state);
            this.state = state;
        }
        private void Reduce(int num, string neterm)
        {
            for (int i = 0; i < num; i++)
            {
                lexemStack.Pop();
                stateStack.Pop();
            }
            state = stateStack.Peek();
            Token k = new Token(TokenType.NETERM);
            k.Value = neterm;
            lexemStack.Push(k);            
        }
        private void State0()
        {
            if (lexemStack.Count == 0)
                Shift();
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<программа>":
                            if (nextLex == tokens.Count)
                                isEnd = true;
                            break;
                        case "<спис_опер>":
                            GoToState(1);
                            break;
                        case "<опер>":
                            GoToState(2);
                            break;
                        case "<иниц>":
                            GoToState(3);
                            break;
                        case "<услов>":
                            GoToState(4);
                            break;
                        case "<присв>":
                            GoToState(5);
                            break;
                        default:
                            Error("<программа>,<спис_опер>,<опер>,<иниц>,<услов> или <присв>");
                            break;                            
                    }
                    break;
                case TokenType.DIM:
                    GoToState(6);
                    break;
                case TokenType.IF:
                    GoToState(7);
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(8);
                    break;
                default:
                    Error("Dim, if или идентификатор");
                    break;
            }
        }
        private void State1()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис_опер>":
                            if(nextLex == tokens.Count)
                            {
                                Reduce(1, "<программа>");
                                break;
                            }
                            if (GetLexeme(nextLex).Type == TokenType.LINEBREAK)
                            {
                                Shift();
                                break;
                            }
                            else
                            {
                                Error("\\n");
                                break;
                            }                                
                        default:
                            Error("<спис_опер>");
                            break;
                    }
                    break;                    
                case TokenType.LINEBREAK:
                    GoToState(9);
                    break;
                default:
                    Error("\\n");
                    break;
            }
        }
        private void State2()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<опер>")
                Reduce(1, "<спис_опер>");
            else
                Error("<опер>");
        }
        private void State3()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<иниц>")
                Reduce(1, "<опер>");
            else
                Error("<иниц>");
        }
        private void State4()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<услов>")
                Reduce(1, "<опер>");
            else
                Error("<услов>");
        }
        private void State5()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<присв>")
                Reduce(1, "<опер>");
            else
                Error("<присв>");
        }
        private void State6()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    if (lexemStack.Peek().Value == "<спис_перем>")
                        GoToState(10);
                    else
                        Error("<спис_перем>");
                    break;
                case TokenType.DIM:
                    if (nextLex == tokens.Count)
                        Error("получить идентификатор");
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(31);
                    break;
                default:
                    Error("получить идентификатор");
                    break;
            }
        }
        private void State7()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IF:
                    Expression();
                    break;
                case TokenType.EXPR:
                    GoToState(11);
                    break;
                default:
                    Error("if или сложное логическое выражение");
                    break;
            }
        }
        private void State8()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IDENTIFIER:
                    if (nextLex == tokens.Count)
                        Error("=");
                    Shift();
                    break;
                case TokenType.EQUAL:
                    GoToState(12);
                    break;
                default:
                    Error("=");
                    break;
            }
        }
        private void State9()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<опер>":
                            GoToState(13);
                            break;
                        case "<иниц>":
                            GoToState(3);
                            break;
                        case "<услов>":
                            GoToState(4);
                            break;
                        case "<присв>":
                            GoToState(5);
                            break;
                        default:
                            Error("<опер>, <иниц>, <услов> или <присв>");
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    if (nextLex == tokens.Count)
                        Error("Dim, if или идентификатор");
                    Shift();
                    break;
                case TokenType.DIM:
                    GoToState(6);
                    break;
                case TokenType.IF:
                    GoToState(7);
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(8);
                    break;
                default:
                    Error("Dim, if или идентификатор");
                    break;
            }
        }
        private void State10()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    if (lexemStack.Peek().Value == "<спис_перем>")
                    {
                        if (nextLex == tokens.Count)
                            Error(", или as");
                        Shift();
                    }    
                    else
                        Error("<спис_перем>");
                    break;
                case TokenType.AS:
                    GoToState(14);
                    break;
                case TokenType.COMMA:
                    GoToState(41);
                    break;
                default:
                    Error(", или as");
                    break;
            }
        }
        private void State11()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.EXPR:
                    if (nextLex == tokens.Count)
                        Error("then");
                    Shift();
                    break;
                case TokenType.THEN:
                    GoToState(15);
                    break;
                default:
                    Error("then");
                    break;
            }
        }
        private void State12()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    if (lexemStack.Peek().Value == "<операнд>")
                        GoToState(16);
                    else
                        Error("<операнд>");
                    break;
                case TokenType.EQUAL:
                    if (nextLex == tokens.Count)
                        Error("получить идентификатор или литерал");
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(32);
                    break;
                case TokenType.LITERAL:
                    GoToState(33);
                    break;
                default:
                    Error("получить идентификатор или литерал");
                    break;
            }
        }
        private void State13()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<опер>")
                Reduce(3, "<спис_опер>");
            else
                Error("<опер>");
        }
        private void State14()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    if (lexemStack.Peek().Value == "<тип>")
                        GoToState(17);
                    else
                        Error("<тип>");
                    break;
                case TokenType.AS:
                    if (nextLex == tokens.Count)
                        Error("integer, double или string");
                    Shift();
                    break;
                case TokenType.INTEGER:
                    GoToState(34);
                    break;
                case TokenType.DOUBLE:
                    GoToState(35);
                    break;
                case TokenType.STRING:
                    GoToState(36);
                    break;
                default:
                    Error("integer, double или string");
                    break;
            }
        }
        private void State15()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.THEN:
                    if (nextLex == tokens.Count)
                        Error("\\n");
                    Shift();
                    break;
                case TokenType.LINEBREAK:
                    GoToState(18);
                    break;
                default:
                    Error("\\n");
                    break;
            }
        }
        private void State16()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<операнд>":
                            if (nextLex == tokens.Count)
                            {
                                Reduce(3, "<присв>");
                                break;
                            }                        
                            switch (GetLexeme(nextLex).Type)
                            {
                                case TokenType.PLUS:
                                    Shift();
                                    break;
                                case TokenType.MINUS:
                                    Shift();
                                    break;
                                case TokenType.MULTIPLY:
                                    Shift();
                                    break;
                                case TokenType.DIVIDE:
                                    Shift();
                                    break;
                                case TokenType.LINEBREAK:
                                    Reduce(3, "<присв>");
                                    break;                                
                                default:
                                    Error("\\n");
                                    break;
                            }
                            break;
                        case "<знак>":
                            GoToState(19);
                            break;                        
                        default:
                            Error("<знак> или <операнд>");
                            break;
                    }
                    break;
                case TokenType.PLUS:
                    GoToState(37);
                    break;
                case TokenType.MINUS:
                    GoToState(38);
                    break;
                case TokenType.MULTIPLY:
                    GoToState(39);
                    break;
                case TokenType.DIVIDE:
                    GoToState(40);
                    break;
                default:
                    Error("+, -, * или /");
                    break;
            }
            }
            private void State17()
            {
                if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<тип>")
                    Reduce(4, "<иниц>");
                else
                    Error("<тип>");
            }
            private void State18()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.NETERM:
                        switch (lexemStack.Peek().Value)
                        {
                            case "<спис_опер>":
                                GoToState(20);
                                break;
                            case "<опер>":
                                GoToState(2);
                                break;
                            case "<иниц>":
                                GoToState(3);
                                break;
                            case "<услов>":
                                GoToState(4);
                                break;
                            case "<присв>":
                                GoToState(5);
                                break;
                            default:
                                Error("<спис_опер>, <опер>, <иниц>, <услов> или <присв>");
                                break;
                        }
                        break;
                    case TokenType.LINEBREAK:
                        if (nextLex == tokens.Count)
                            Error("Dim, if или идентификатор");
                            Shift();
                        break;
                    case TokenType.DIM:
                        GoToState(6);
                        break;
                    case TokenType.IF:
                        GoToState(7);
                        break;
                    case TokenType.IDENTIFIER:
                        GoToState(8);
                        break;
                    default:
                        Error("Dim, if или идентификатор");
                        break;
                }
            }
            private void State19()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.NETERM:
                        switch (lexemStack.Peek().Value)
                        {
                            case "<знак>":
                                if (nextLex == tokens.Count)
                                    Error("<операнд>");
                                Shift();
                                break;
                            case "<операнд>":
                                GoToState(21);
                                break;
                            default:
                                Error("<операнд>");
                                break;
                        }
                        break;
                    case TokenType.IDENTIFIER:
                        GoToState(32);
                        break;
                    case TokenType.LITERAL:
                        GoToState(33);
                        break;
                    default:
                        Error("получить идентификатор или литерал");
                        break;
                }
            }
            private void State20()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.NETERM:
                        if (lexemStack.Peek().Value == "<спис_опер>")
                        {
                        if (nextLex == tokens.Count)
                            Error("\\n");
                        Shift();
                        }
                        else
                            Error("<спис_опер>");
                        break;
                   case TokenType.LINEBREAK:
                        GoToState(22);
                        break;
                    default:
                        Error("\\n");
                        break;
                }
            }
            private void State21()
            {
                if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<операнд>")
                    Reduce(5, "<присв>");
                else
                    Error("<операнд>");
            }
            private void State22()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.NETERM:
                        switch (lexemStack.Peek().Value)
                        {
                            case "<опер>":
                                GoToState(13);
                                break;
                            case "<иниц>":
                                GoToState(3);
                                break;
                            case "<услов>":
                                GoToState(4);
                                break;
                            case "<присв>":
                                GoToState(5);
                                break;
                            default:
                                Error("<опер>, <иниц>, <услов> или <присв>");
                                break;
                        }
                        break;
                    case TokenType.LINEBREAK:
                        if (nextLex == tokens.Count)
                            Error("Dim, if, end, else или идентификатор");
                        Shift();
                        break;
                    case TokenType.DIM:
                        GoToState(6);
                        break;
                    case TokenType.IF:
                        GoToState(7);
                        break;
                    case TokenType.IDENTIFIER:
                        GoToState(8);
                        break;
                    case TokenType.END:
                        GoToState(23);
                        break;
                    case TokenType.ELSE:
                        GoToState(24);
                        break;               
                    default:
                        Error("Dim, if, end, else или идентификатор");
                        break;
                }
            }
            private void State23()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.END:
                        if (nextLex == tokens.Count)
                            Error("if");
                        Shift();
                        break;
                    case TokenType.IF:
                        GoToState(25);
                        break;
                    default:
                        Error("if");
                        break;
                }
            }
            private void State24()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.ELSE:
                        if (nextLex == tokens.Count)
                            Error("else или \\n");
                        Shift();
                        break;
                    case TokenType.LINEBREAK:
                        GoToState(26);
                        break;
                    default:
                        Error("\\n или else");
                        break;
                }
            }
            private void State25()
            {
                if (lexemStack.Peek().Type == TokenType.IF)
                    Reduce(8, "<услов>");
                else
                    Error("if");
            }
            private void State26()
            {
                switch (lexemStack.Peek().Type)
                {
                    case TokenType.NETERM:
                        switch (lexemStack.Peek().Value)
                        {
                            case "<спис_опер>":
                                GoToState(27);
                                break;
                            case "<опер>":
                                GoToState(2);
                                break;
                            case "<иниц>":
                                GoToState(3);
                                break;
                            case "<услов>":
                                GoToState(4);
                                break;
                            case "<присв>":
                                GoToState(5);
                                break;
                            default:
                                Error("<спис_опер>, <опер>, <иниц>, <услов> или <присв>");
                                break;
                        }
                        break;
                    case TokenType.LINEBREAK:
                        if (nextLex == tokens.Count)
                            Error("Dim, if или идентификатор");
                        Shift();
                        break;
                    case TokenType.DIM:
                        GoToState(6);
                        break;
                    case TokenType.IF:
                        GoToState(7);
                        break;
                    case TokenType.IDENTIFIER:
                        GoToState(8);
                        break;
                    default:
                        Error("Dim, if или идентификатор");
                        break;
                }
            }
        private void State27()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    if (lexemStack.Peek().Value == "<спис_опер>")
                    {
                        if (nextLex == tokens.Count)
                            Error("\\n");
                        Shift();
                    }
                    else
                        Error("<спис_опер>");
                    break;
                case TokenType.LINEBREAK:
                    GoToState(28);
                    break;
                default:
                    Error("\\n");
                    break;
            }
        }
        private void State28()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<опер>":
                            GoToState(13);
                            break;
                        case "<иниц>":
                            GoToState(3);
                            break;
                        case "<услов>":
                            GoToState(4);
                            break;
                        case "<присв>":
                            GoToState(5);
                            break;
                        default:
                            Error("<опер>, <иниц>, <услов>, <присв>");
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    if (nextLex == tokens.Count)
                        Error("Dim, if , end или идентификатор");
                    Shift(); 
                    break;
                case TokenType.DIM:
                    GoToState(6);
                    break;
                case TokenType.IF:
                    GoToState(7);
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(8);
                    break;
                case TokenType.END:
                    GoToState(29);
                    break;
                default:
                    Error("Dim, if , end или идентификатор");
                    break;
            }
        }
        private void State29()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.END:
                    if (nextLex == tokens.Count)
                        Error("if");
                    Shift();
                    break;
                case TokenType.IF:
                    GoToState(30);
                    break;
                default:
                    Error("if");
                    break;
            }
        }
        private void State30()
        {
            if (lexemStack.Peek().Type == TokenType.IF)
                Reduce(12, "<услов>");
            else
                Error("if");
        }
        private void State31()
        {
            if (lexemStack.Peek().Type == TokenType.IDENTIFIER)
                Reduce(1, "<спис_перем>");
            else
                Error("получить идентификатор");
        }
        private void State32()
        {
            if (lexemStack.Peek().Type == TokenType.IDENTIFIER)
                Reduce(1, "<операнд>");
            else
                Error("получить идентификатор");
        }
        private void State33()
        {
            if (lexemStack.Peek().Type == TokenType.LITERAL)
                Reduce(1, "<операнд>");
            else
                Error("получить литерал");
        }
        private void State34()
        {
            if (lexemStack.Peek().Type == TokenType.INTEGER)
                Reduce(1, "<тип>");
            else
                Error("integer");
        }
        private void State35()
        {
            if (lexemStack.Peek().Type == TokenType.DOUBLE)
                Reduce(1, "<тип>");
            else
                Error("double");
        }
        private void State36()
        {
            if (lexemStack.Peek().Type == TokenType.STRING)
                Reduce(1, "<тип>");
            else
                Error("string");
        }
        private void State37()
        {
            if (lexemStack.Peek().Type == TokenType.PLUS)
                Reduce(1, "<знак>");
            else
                Error("+");
        }
        private void State38()
        {
            if (lexemStack.Peek().Type == TokenType.MINUS)
                Reduce(1, "<знак>");
            else
                Error("-");
        }
        private void State39()
        {
            if (lexemStack.Peek().Type == TokenType.MULTIPLY)
                Reduce(1, "<знак>");
            else
                Error("*");
        }
        private void State40()
        {
            if (lexemStack.Peek().Type == TokenType.DIVIDE)
                Reduce(1, "<знак>");
            else
                Error("/");
        }
        private void State41()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.COMMA:
                    if (nextLex == tokens.Count)
                        Error("получить идентификатор");
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(42);
                    break;
                default:
                    Error("получить идентификатор");
                    break;
            }
        }
        private void State42()
        {
            if (lexemStack.Peek().Type == TokenType.IDENTIFIER)
                Reduce(3, "<спис_перем>");
            else
                Error("получить идентификатор");
        }
        
        private void Expression()
        {
            List<Token> inmet = new List<Token>();
            while (nextLex != tokens.Count && GetLexeme(nextLex).Type != TokenType.THEN)
            {
                inmet.Add(GetLexeme(nextLex));
                nextLex++;
            }
            if (firstEXPR == true)
            {
                Bower a = new Bower(inmet);
                a.Start();
                foreach (Troyka troyka in a.troyka)
                    operatsii.Add(troyka);
                lastEXPRind = a.Lastindex;
                firstEXPR = false;
            }
            else
            {
                Bower a = new Bower(inmet,lastEXPRind);
                a.Start();
                foreach (Troyka troyka in a.troyka)
                    operatsii.Add(troyka);
                lastEXPRind = a.Lastindex;
            }            
            Token k = new Token(TokenType.EXPR);
            lexemStack.Push(k);
        }
        public void Start()
        {
            stateStack.Push(0);
            while (isEnd != true)
                switch (state)
                {
                    case 0:
                        State0();
                        break;
                    case 1:
                        State1();
                        break;
                    case 2:
                        State2();
                        break;
                    case 3:
                        State3();
                        break;
                    case 4:
                        State4();
                        break;
                    case 5:
                        State5();
                        break;
                    case 6:
                        State6();
                        break;
                    case 7:
                        State7();
                        break;
                    case 8:
                        State8();
                        break;
                    case 9:
                        State9();
                        break;
                    case 10:
                        State10();
                        break;
                    case 11:
                        State11();
                        break;
                    case 12:
                        State12();
                        break;
                    case 13:
                        State13();
                        break;
                    case 14:
                        State14();
                        break;
                    case 15:
                        State15();
                        break;
                    case 16:
                        State16();
                        break;
                    case 17:
                        State17();
                        break;
                    case 18:
                        State18();
                        break;
                    case 19:
                        State19();
                        break;
                    case 20:
                        State20();
                        break;
                    case 21:
                        State21();
                        break;
                    case 22:
                        State22();
                        break;
                    case 23:
                        State23();
                        break;
                    case 24:
                        State24();
                        break;
                    case 25:
                        State25();
                        break;
                    case 26:
                        State26();
                        break;
                    case 27:
                        State27();
                        break;
                    case 28:
                        State28();
                        break;
                    case 29:
                        State29();
                        break;
                    case 30:
                        State30();
                        break;
                    case 31:
                        State31();
                        break;
                    case 32:
                        State32();
                        break;
                    case 33:
                        State33();
                        break;
                    case 34:
                        State34();
                        break;
                    case 35:
                        State35();
                        break;
                    case 36:
                        State36();
                        break;
                    case 37:
                        State37();
                        break;
                    case 38:
                        State38();
                        break;
                    case 39:
                        State39();
                        break;
                    case 40:
                        State40();
                        break;
                    case 41:
                        State41();
                        break;
                    case 42:
                        State42();
                        break;
                }
        }
        private void Error(string ojid)
        {
            if(nextLex == tokens.Count)
            {
                if (lexemStack.Peek().Type == TokenType.NETERM)
                    throw new Exception($"Ожидалось {ojid}, но последний элемент анализируемой последовательности {lexemStack.Peek().Value}. Состояние:{state}");
                else
                    throw new Exception($"Ожидалось {ojid}, но последний элемент анализируемой последовательности {ConvertLex(lexemStack.Peek().Type)}. Состояние:{state}");
            }
            else
            {
            if (lexemStack.Peek().Type == TokenType.NETERM)
                throw new Exception($"Ожидалось {ojid}, но было получено {lexemStack.Peek().Value}. Состояние:{state}");
            else
                throw new Exception($"Ожидалось {ojid}, но было получено {ConvertLex(lexemStack.Peek().Type)}. Состояние:{state}");
            }
        }
        public static string ConvertLex(TokenType type)
        {
            string s = "";
            switch (type)
            {
                case TokenType.IDENTIFIER:
                    s = "идентификатор";
                    break;
                case TokenType.LITERAL:
                    s = "литерал";
                    break;
                case TokenType.PLUS:
                    s = "+";
                    break;
                case TokenType.MINUS:
                    s = "-";
                    break;
                case TokenType.MULTIPLY:
                    s = "*";
                    break;
                case TokenType.DIVIDE:
                    s = "/";
                    break;
                case TokenType.EQUAL:
                    s = "=";
                    break;
                case TokenType.MORE:
                    s = ">";
                    break;
                case TokenType.LESS:
                    s = "<";
                    break;
                case TokenType.LESSEQUAL:
                    s = "<=";
                    break;
                case TokenType.LESSMORE:
                    s = "<>";
                    break;
                case TokenType.MOREEQUAL:
                    s = ">=";
                    break;
                case TokenType.COMMA:
                    s = ",";
                    break;
                case TokenType.LPAR:
                    s = "(";
                    break;
                case TokenType.RPAR:
                    s = ")";
                    break;
                case TokenType.LINEBREAK:
                    s = "\\n";
                    break;
                case TokenType.INTEGER:
                    s = "integer";
                    break;
                case TokenType.DOUBLE:
                    s = "double";
                    break;
                case TokenType.STRING:
                    s = "string";
                    break;
                case TokenType.DIM:
                    s = "Dim";
                    break;
                case TokenType.AS:
                    s = "as";
                    break;
                case TokenType.IF:
                    s = "if";
                    break;
                case TokenType.THEN:
                    s = "then";
                    break;
                case TokenType.ELSE:
                    s = "else";
                    break;
                case TokenType.END:
                    s = "end";
                    break;
                case TokenType.OR:
                    s = "or";
                    break;
                case TokenType.AND:
                    s = "and";
                    break;
                case TokenType.EXPR:
                    s = "Сложное логическое выражение";
                    break;
                default:
                    break;
            }
            return s;
        }
    }
}