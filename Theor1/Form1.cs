using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Theor1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Token> tokens = new List<Token>();
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                Analys s = new Analys(richTextBox1.Text.Trim());
                s.Razbor();
                foreach (Lex a in s.lexes)
                    listBox1.Items.Add($"{a.Lexema}    :    {a.Type}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Error! {ex.Message}");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            
            try 
            { 
                listBox1.Items.Clear();
                tokens.Clear();
                ZapolneniyeListaTokenov();
                foreach (Token a in tokens)
                {
                    if (a.Type == TokenType.IDENTIFIER || a.Type == TokenType.LITERAL)
                        listBox1.Items.Add($"{a.Type}    :    {a.Value}");
                    else
                        listBox1.Items.Add($"{a.Type}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Error! {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы txt (*.txt)|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader rdr = new StreamReader(fileDialog.FileName);
                string line = rdr.ReadToEnd();
                rdr.Close();
                richTextBox1.Text = line;
            }
        }      

        private void button5_Click(object sender, EventArgs e)
        {            
            try
            {
                listBox1.Items.Clear();                
                tokens.Clear();
                ZapolneniyeListaTokenov();
                foreach (Token a in tokens)
                {
                    if (a.Type == TokenType.IDENTIFIER || a.Type == TokenType.LITERAL)
                        listBox1.Items.Add($"{a.Type}    :    {a.Value}");
                    else
                        listBox1.Items.Add($"{a.Type}");
                }
                LR rule = new LR(tokens);
                dataGridView1.DataSource = null;
                dataGridView1.RowCount = 1;
                rule.Start();
                int index = 0;
                foreach (Troyka z in rule.operatsii)
                {
                    dataGridView1.RowCount++;
                    dataGridView1[0, index].Value = $"m{index}";
                    dataGridView1[1, index].Value = $"{LR.ConvertLex(z.deystvie.Type)}"; 
                    dataGridView1[2, index].Value = $"{z.operand1.Value}";
                    dataGridView1[3, index].Value = $"{z.operand2.Value}";
                    index++;
                }
                MessageBox.Show("Разбор завершён");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Error! {ex.Message}");
            }
        }
        private void ZapolneniyeListaTokenov()
        {
            Analys s = new Analys(richTextBox1.Text.Trim());
            s.Razbor();            
            for (int i = 0; i < s.lexes.Count; i++)
            {
                string currentLexem = s.lexes[i].Lexema;
                if (s.lexes[i].Type == "идентификатор")
                {
                    if (Token.IsSpecialWord(s.lexes[i].Lexema))
                    {
                        Token token = new Token(Token.SpecialWords[s.lexes[i].Lexema]);
                        tokens.Add(token);
                    }
                    else
                    {
                        Token token = new Token(TokenType.IDENTIFIER);
                        token.Value = currentLexem;
                        tokens.Add(token);
                    }
                }
                else if (s.lexes[i].Type == "литерал")
                {
                    Token token = new Token(TokenType.LITERAL);
                    token.Value = currentLexem;
                    tokens.Add(token);
                }
                else
                {
                    if (i == s.lexes.Count - 1)
                    {
                        if (Token.IsSpecialSymbol(Convert.ToChar(s.lexes[i].Lexema)))
                        {
                            Token token = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                            tokens.Add(token);
                        }
                    }
                    else
                    {
                        if (Token.IsSpecialSymbol(Convert.ToChar(s.lexes[i].Lexema)))
                        {
                            switch (Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)])
                            {
                                case TokenType.LESS:
                                    if (s.lexes[i + 1].Type == "литерал" || s.lexes[i + 1].Type == "идентификатор")
                                    {
                                        Token token1 = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                                        tokens.Add(token1);
                                        break;
                                    }
                                    if (Token.IsSpecialSymbol(Convert.ToChar(s.lexes[i + 1].Lexema)) == true && Token.SpecialSymbols[Convert.ToChar(s.lexes[i+1].Lexema)] == TokenType.EQUAL)
                                    {
                                        Token token2 = new Token(TokenType.LESSEQUAL);
                                        token2.Value = "<=";
                                        tokens.Add(token2);
                                        i++;
                                    }
                                    else if (Token.IsSpecialSymbol(Convert.ToChar(s.lexes[i + 1].Lexema)) == true && s.lexes[i + 1].Type != "литерал" && s.lexes[i + 1].Type != "идентификатор" && Token.SpecialSymbols[Convert.ToChar(s.lexes[i+1].Lexema)] == TokenType.MORE)
                                    {
                                        Token token2 = new Token(TokenType.LESSMORE);
                                        token2.Value = "<>";
                                        tokens.Add(token2);
                                        i++;
                                    }
                                    else
                                    {
                                        Token token2 = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                                        tokens.Add(token2);
                                    }
                                    break;
                                case TokenType.MORE:
                                    if(s.lexes[i + 1].Type == "литерал" || s.lexes[i + 1].Type == "идентификатор")
                                    {
                                        Token token1 = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                                        tokens.Add(token1);
                                        break;
                                    }
                                    if (Token.IsSpecialSymbol(Convert.ToChar(s.lexes[i+1].Lexema))==true && Token.SpecialSymbols[Convert.ToChar(s.lexes[i+1].Lexema)] == TokenType.EQUAL)
                                    {
                                        Token token2 = new Token(TokenType.MOREEQUAL);
                                        token2.Value = ">=";
                                        tokens.Add(token2);
                                        i++;
                                    }
                                    else
                                    {
                                        Token token2 = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                                        tokens.Add(token2);
                                    }                                    
                                        break;
                                default:
                                    Token token = new Token(Token.SpecialSymbols[Convert.ToChar(s.lexes[i].Lexema)]);
                                    tokens.Add(token);
                                    break;
                            }
                        }
                    }
                }
            }            
        }
    }
}