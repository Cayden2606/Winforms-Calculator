using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Media;
using System.Speech.Synthesis;
using System.Xml.Linq;

namespace Calculator_230833F
{
    public partial class MainForm_230833F : Form
    {
        SpeechSynthesizer syn = new SpeechSynthesizer();
        public MainForm_230833F()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void lblID_Click(object sende, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            Clipboard.SetText(attribute.Value.ToString());
        }

        private void MainForm_230833F_Load(object sender, EventArgs e)
        {
            this.Size = new Size(373, 574);
            // To force the window into a specific size to fix scaling issues
            SoundPlayer player; // Play Boot Up Sequence Sound
            player = new SoundPlayer(Properties.Resources.Boot_sound_new);
            player.Play();
        }

        private void numPad_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            Button btn = (Button)sender;

            string last_butt = btn.Text;
            string numbers = txtbox_input.Text;
            string last_number = lastNumber(numbers); // Get last number of string (incl. decimals, -ve & whole numbers)

            switch (last_butt)
            {
                case ".":
                    if (!last_number.Contains(".") || txtbox_input.Text.EndsWith("+") || txtbox_input.Text.EndsWith("-") || txtbox_input.Text.EndsWith("÷") || txtbox_input.Text.EndsWith("×") || txtbox_input.Text.EndsWith("^")) // If last number inputted, doesn't have a "." it inputs a "."
                    {
                        if (txtbox_input.Text.Length <= 0)
                        {
                            txtbox_input.Text = "0.";
                        }
                        else if (txtbox_input.Text[txtbox_input.Text.Length - 1] == '+' || txtbox_input.Text[txtbox_input.Text.Length - 1] == '-' || txtbox_input.Text[txtbox_input.Text.Length - 1] == '÷' || txtbox_input.Text[txtbox_input.Text.Length - 1] == '×' || txtbox_input.Text[txtbox_input.Text.Length - 1] == '×' || txtbox_input.Text[txtbox_input.Text.Length - 1] == '^')
                        {
                            txtbox_input.Text += "0.";
                        }
                        else
                        {
                            txtbox_input.Text += ".";
                        }
                    }
                    break;
                default:
                    txtbox_input.Text += btn.Text; // Add the last inputted number key
                    break;
            }
            btn_Equals.Focus();
        }
        string opr = "";

        private void btn_Equals_Click(object sender, EventArgs e)
        {
            Equals();
            Speech(this, lbl_Results.Text);
            btn_Equals.Focus();
        }

        private void Equals()
        {
            click_sound("Enter");
            lbl_Results.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            if (txtbox_input.Text.Count() == 0)
            {
                lbl_Results.Text = "Input Empty";
            }
            else
            {
                string txtbox = txtbox_input.Text; // if last character is an operator or "." it gives an error when press equals
                if (txtbox[txtbox.Length - 1].ToString() == "-" || txtbox[txtbox.Length - 1].ToString() == "+" || txtbox[txtbox.Length - 1].ToString() == "×" || txtbox[txtbox.Length - 1].ToString() == "÷" || txtbox[txtbox.Length - 1].ToString() == ".")
                {
                    lbl_Results.Text = "Syntax Error";

                }
                else
                {
                    string answer = calculate(txtbox_input.Text); // Calculate the Entire equation at once
                    lbl_Results.Text = answer;
                    if (double.TryParse(answer, out _))
                    {
                        Ans = answer;
                    }
                    else
                    {
                        Ans = "";
                    }
                }
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            click_sound("Space"); // Clear Input and make Output "0"
            opr = "";
            lbl_Results.Text = "0";
            txtbox_input.Text = "";
        }

        private void u_operatorClick(object sender, EventArgs e)
        {
            click_sound("Normal"); // If equation has an operator
            if (txtbox_input.Text.Contains("+")|| txtbox_input.Text.Contains("-")|| txtbox_input.Text.Contains("÷") || txtbox_input.Text.Contains("×") || txtbox_input.Text.Contains("/") || txtbox_input.Text.Contains("%") || txtbox_input.Text.Contains("^") || txtbox_input.Text.Contains("e") || txtbox_input.Text.Contains("π"))
            {
                Equals(); // Calcuates that equation before carrying on
            }
            else
            {
                if (double.TryParse(txtbox_input.Text, out double value))
                {
                    lbl_Results.Text = value.ToString(); 
                }

                lbl_Results.Text = lastNumber(lbl_Results.Text); // Gets only the number
            }
            Button btn = (Button)sender;
            string u_opr = btn.Tag.ToString();
            if (txtbox_input.Text.Length == 0)
            {
                lbl_Results.Text = "Empty"; // Output Empty if input is empty
            }
            else
            {
                if (double.TryParse(lbl_Results.Text, out double value)) 
                {
                }
                else
                {
                    lbl_Results.Text = "Syntax Error"; // if results contains things that's not a number
                    Speech(this, lbl_Results.Text);
                    return;
                }
                string results;
                switch (u_opr) // All the Unary Operator Calculations
                {
                    case "Sqrt":
                        results = Math.Sqrt(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = "√" + lbl_Results.Text;
                        lbl_Results.Text = results.TrimEnd('0').TrimEnd('.');
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Square":
                        results = Math.Pow(value, 2).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = lbl_Results.Text + "^2";
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Reciprocal":
                        results = Math.Pow(value, -1).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = "1/" + lbl_Results.Text;
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Plus_Minus":
                        results = (value * -1).ToString().TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = results;
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Sin":
                        if (deg_rad) // Degree Mode
                        {
                            if (btn_Sin.Text == "sin⁻¹") // Inverse Sine
                            {
                                results = (Math.Asin(value) * (180.0 / Math.PI)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Sin⁻¹(" + lbl_Results.Text + ")";
                            }
                            else // Regular Sine
                            {
                                results = Math.Sin(value * (Math.PI / 180)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Sin(" + lbl_Results.Text + ")";
                            }
                        }
                        else // Radian Mode
                        {
                            if (btn_Sin.Text == "sin⁻¹") // Inverse Sine
                            {
                                results = Math.Asin(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Sin⁻¹(" + lbl_Results.Text + ")";

                            }
                            else // Regular Sine
                            {
                                results = Math.Sin(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Sin(" + lbl_Results.Text + ")";

                            }

                        }
                        
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Cos":
                        if (deg_rad) // Degree Mode
                        {
                            if (btn_Cos.Text == "cos⁻¹") // Inverse Cosine
                            {
                                results = (Math.Acos(value) * (180.0 / Math.PI)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Cos⁻¹(" + lbl_Results.Text + ")";
                            }
                            else // Regular Cosine
                            {
                                results = Math.Cos(value * (Math.PI / 180)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Cos(" + lbl_Results.Text + ")";
                            }
                        }
                        else // Radian Mode
                        {
                            if (btn_Cos.Text == "cos⁻¹") // Inverse Cosine
                            {
                                results = Math.Acos(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Cos⁻¹(" + lbl_Results.Text + ")";
                            }
                            else // Regular Cosine
                            {
                                results = Math.Cos(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Cos(" + lbl_Results.Text + ")";

                            }
                        }
                        
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Tan":
                        if (deg_rad) // Degree Mode
                        {
                            if (btn_Tan.Text == "tan⁻¹") // Inverse Tangent
                            {
                                results = (Math.Atan(value) * (180.0 / Math.PI)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Tan⁻¹(" + lbl_Results.Text + ")";
                            }
                            else // Regular Tangent
                            {
                                results = Math.Tan(value * (Math.PI / 180)).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Tan(" + lbl_Results.Text + ")";
                            }
                        }
                        else // Radian Mode
                        {
                            if (btn_Tan.Text == "tan⁻¹") // Inverse Tangent
                            {
                                results = Math.Atan(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Tan⁻¹(" + lbl_Results.Text + ")";
                            }
                            else // Regular Tangent
                            {
                                results = Math.Tan(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                                txtbox_input.Text = "Tan(" + lbl_Results.Text + ")";

                            }
                        }
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Ln":
                        results = Math.Log(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = "ln(" + lbl_Results.Text + ")";
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Log":
                        results = Math.Log10(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = "log₁₀(" + lbl_Results.Text + ")";
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "eˣ":
                        results = Math.Exp(value).ToString("N10").TrimEnd('0').TrimEnd('.');
                        if (Math.Abs(double.Parse(results) - Math.Truncate(double.Parse(results))) < 0.000000001) // If the number is like 3.00000001 it makes it 3
                        {
                            int intresults = (int)Math.Round(double.Parse(results));
                            results = intresults.ToString();
                        }

                        txtbox_input.Text = "e^" + lbl_Results.Text;
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "10ˣ":
                        results = Math.Pow(10, value).ToString("N10").TrimEnd('0').TrimEnd('.');
                        txtbox_input.Text = "10^"+lbl_Results.Text;
                        lbl_Results.Text = results;
                        Speech(this, lbl_Results.Text);
                        break;
                    case "Round":
                        long rounded_results = (long)Math.Round(value);
                        txtbox_input.Text = "Rnd(" + lbl_Results.Text + ")";
                        lbl_Results.Text = rounded_results.ToString().TrimEnd('0').TrimEnd('.');
                        Speech(this, lbl_Results.Text);
                        break;
                }
            }
            btn_Equals.Focus();
        }

        private void operator_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            Button btn = (Button)sender;
            opr = btn.Tag.ToString();
            switch (opr) // Visually adds Operators in the Input Text Box, with an if statement not allowing multiple operators next to each other, other than ×- and ÷- (Times neg num/ Divide neg num)
            {
                case "Add":
                    if (txtbox_input.Text == "")
                    {

                    }
                    else if (char.IsDigit(txtbox_input.Text[txtbox_input.Text.Length - 1]) || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "e" || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "π")
                    {
                        txtbox_input.Text += "+";

                    }
                    break;
                case "Minus":
                    if (txtbox_input.Text == "")
                    {
                        txtbox_input.Text = "0";
                    }
                    if (txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() != "+" && txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() != "-" && txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() != ".")
                    {
                        txtbox_input.Text += "-";
                    }
                    break;
                case "Multiply":
                    if (txtbox_input.Text == "")
                    {

                    }
                    else if (char.IsDigit(txtbox_input.Text[txtbox_input.Text.Length - 1]) || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "e" || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "π")
                    {
                        txtbox_input.Text += "×";
                    }
                    break;
                case "Divide":
                    if (txtbox_input.Text == "")
                    {

                    }
                    else if (char.IsDigit(txtbox_input.Text[txtbox_input.Text.Length - 1]) || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "e" || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "π")
                    {
                        txtbox_input.Text += "÷";
                    }
                    break;
                case "Modulus":
                    if (txtbox_input.Text == "")
                    {

                    }
                    else if (char.IsDigit(txtbox_input.Text[txtbox_input.Text.Length - 1]) || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "e" || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "π")
                    {
                        txtbox_input.Text += "%";
                    }
                    break;
                case "Power":
                    if (txtbox_input.Text == "")
                    {

                    }
                    else if (char.IsDigit(txtbox_input.Text[txtbox_input.Text.Length - 1]) || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "e" || txtbox_input.Text[txtbox_input.Text.Length - 1].ToString() == "π")
                    {
                        txtbox_input.Text += "^";
                    }
                    break;
                default:
                    break;
            }
            btn_Equals.Focus();
       }


        static string lastNumber(string numbers) // Gets the last number of the Input decimals, negative numbers and whole numbers
        {
            string[] number = Regex.Split(numbers, @"[^\d.]+");

            string lastNumber = "";

            foreach (string num in number)
            {
                if (!string.IsNullOrEmpty(num))
                {
                    lastNumber = num;
                }
            }
            Console.WriteLine(lastNumber);
            return lastNumber;
        }

        static string calculate(string expression)
        {
            expression = expression.Replace("/", "÷");
            expression = expression.Replace("e", Math.E.ToString()); // Replaces pi and e to their respective values
            expression = expression.Replace("π", Math.PI.ToString());

            // To split the string to be numbers (-ve, whole numbers, decimals) and operators and place them in a list. (Doesn't consider - as an operator)
            string pattern = @"-?\d+\.\d+|-?\d+|[+\×÷%^]"; 
            List<string> expressionList = new List<string>();
            MatchCollection matches1 = Regex.Matches(expression, pattern);
            foreach (Match var in matches1)
            {
                expressionList.Add(var.Value.ToString()); // Add to list
            }
            int list_count = expressionList.Count; // Get total number of items in list
            if (list_count == 1) // if expression doesn't have any operator and have 1 number, it returns the number
            {
                return expressionList[0];
            }

            int c = 0;
            while (c < list_count - 1) // loops the list
            {
                if (double.TryParse(expressionList[c], out _)) // if value is number
                {
                    if (double.TryParse(expressionList[c + 1], out double numb)) // if next value is also a number
                    {
                        if (numb < 0) // if it is a -ve number
                        {
                            expressionList.Insert(c + 1, "+"); // insert a "+" infront of the -ve number e.g  if the list is ["1", "-3"] , it be ["1", "+", "-3"]
                            Console.WriteLine("Added: " + expressionList[c].ToString());
                            c++;
                            list_count++;
                        }
                    }
                }
                Console.WriteLine("expression: " + expressionList[c].ToString());
                c++;
            }

            expression = string.Join("", expressionList); // Join the list into 1 string again
            int count = 0;
            while (true)
            {
                MatchCollection matches = Regex.Matches(expression, pattern); // Splits the string again, numbers (-ve, decimal, whole num) and plus, times, divide etc.

                foreach (Match matchs in matches)
                {
                    Console.WriteLine("Matches: " + matchs.Value.ToString());
                }

                double first_num = 0;
                string operation = "";
                double second_num = 0;
                int i = 0;
                count++;

                if (i < matches.Count && double.TryParse(matches[i].Value, out first_num)) // if first value is a number
                {
                    Console.WriteLine("First Num: " + matches[i].Value);
                    i++;
                }

                if (i < matches.Count && !double.TryParse(matches[i].Value, out _)) // if second value is an operator
                {
                    operation = matches[i].Value;
                    Console.WriteLine("Operation: " + matches[i].Value);
                    i++;
                }

                if (i < matches.Count && double.TryParse(matches[i].Value, out second_num)) // if third value is a number
                {
                    Console.WriteLine("Second Num: " + matches[i].Value);
                    i++;
                }
                Console.WriteLine("First Num: " + first_num + ", Second Num: " + second_num);

                double result;
                switch (operation) // Main calculations
                {
                    case "+":
                        result = first_num + second_num;
                        break;
                    case "×":
                        result = first_num * second_num;
                        break;
                    case "÷":
                        result = first_num / second_num;
                        break;
                    case "%":
                        result = first_num % second_num;
                        break;
                    case "^":
                        result = Math.Pow(first_num, second_num);
                        break;
                    default:
                        return "Error";
                }

                expression = result + expression.Substring(matches[i - 1].Index + matches[i - 1].Length); // Calculates from left to right, with every loop

                if (matches.Count == i)
                {
                    return expression.ToString(); // Return final answer
                }
            }
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            click_sound("Delete");
            if (txtbox_input.Text.Count() > 0) // Deletes last character
            {
                txtbox_input.Text = txtbox_input.Text.Remove(txtbox_input.Text.Length - 1);
            }
            if (txtbox_input.Text.Count() <= 0) // If input's character length is <=0, output = 0
            {

                lbl_Results.Text = "0";
            }

            lbl_Results.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            btn_Equals.Focus();


        }
        string Ans = "";
        private void btn_Ans_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            txtbox_input.Text += Ans; // Answer button takes the last Answer output
            btn_Equals.Focus();

        }


        private void btn_hamburger_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            if (panel_menu.Visible) // On and off the pannel with more buttons and radio buttons
            {
                panel_menu.Visible = false;
                btn_hamburger.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            }
            else
            {
                panel_menu.Visible = true;
                btn_hamburger.BackColor = System.Drawing.Color.FromArgb(44, 44, 44);
            }
        }
        private void Menu_Change_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            opr = btn.Tag.ToString();
            switch (opr)
            {
                case "Standard": // Moves/resize/hide/unhide the buttons according to the different modes.
                    lbl_Calc_Type.Text = "Standard";
                    side_line_aesthetic.Visible = false;
                    btn_deg_rad.Visible = false;
                    lbl_deg_rad.Visible = false;
                    btn_Shift.Visible = false;

                    foreach (Button btn_ in new Button[] { btn_ln, btn_log, btn_10_Power_x, btn_e_Power_x, btn_expo, btn_pi, btn_e, btn_Sin, btn_Cos, btn_Tan, btn_Round }) 
                    {
                        btn_.Visible = false;
                    }
                    btn_Mod.Location = new Point(4, 171);
                    btn_Clear.Location = new Point(92, 171);
                    btn_Ans.Location = new Point(180, 171);
                    btn_Del.Location = new Point(268, 171);
                    btn_reciprocal.Location = new Point(4, 228);
                    btn_Square.Location = new Point(92, 228);
                    btn_Sqrt.Location = new Point(180, 228);
                    btn_Divide.Location = new Point(268, 228);
                    btn_7.Location = new Point(4, 285);
                    btn_8.Location = new Point(92, 285);
                    btn_9.Location = new Point(180, 285);
                    btn_Multiply.Location = new Point(268, 285);
                    btn_4.Location = new Point(4, 342);
                    btn_5.Location = new Point(92, 342);
                    btn_6.Location = new Point(180, 342);
                    btn_Minus.Location = new Point(268, 342);
                    btn_1.Location = new Point(4, 399);
                    btn_2.Location = new Point(92, 399);
                    btn_3.Location = new Point(180, 399);
                    btn_Plus.Location = new Point(268, 399);
                    btn_Plus_Minus.Location = new Point(4, 456);
                    btn_0.Location = new Point(92, 456);
                    btn_dot.Location = new Point(180, 456);
                    btn_Equals.Location = new Point(268, 456);

                    foreach (Button btn_ in new Button[] {btn_Mod, btn_Clear, btn_Ans, btn_Del, btn_reciprocal, btn_Square, btn_Sqrt, btn_Divide, btn_7, btn_8, btn_9, btn_Multiply, btn_4, btn_5, btn_6, btn_Minus, btn_1, btn_2, btn_3, btn_Plus, btn_Plus_Minus, btn_0, btn_dot, btn_Equals})
                    { 
                        btn_.Size = new Size(85, 54);
                    }

                    btn_hamburger.PerformClick();
                    break;
                case "Scientific":
                    lbl_Calc_Type.Text = "Scientific";
                    side_line_aesthetic.Visible = true;
                    btn_deg_rad.Visible = true;
                    lbl_deg_rad.Visible = true;
                    btn_Shift.Visible = true;

                    btn_Del.Location = new Point(287, 177);
                    btn_Ans.Location = new Point(218, 177);
                    btn_Clear.Location = new Point(149, 177);
                    btn_e.Location = new Point(80, 177);
                    btn_pi.Location = new Point(11, 177);

                    btn_Round.Location = new Point(287, 225);
                    btn_Tan.Location = new Point(218, 225);
                    btn_Cos.Location = new Point(149, 225);
                    btn_Sin.Location = new Point(80, 225);
                    btn_Mod.Location = new Point(11, 225);

                    btn_Divide.Location = new Point(287, 273);
                    btn_Sqrt.Location = new Point(218, 273);
                    btn_Square.Location = new Point(149, 273);
                    btn_reciprocal.Location = new Point(80, 273);
                    btn_expo.Location = new Point(11, 273);

                    btn_Multiply.Location = new Point(287, 321);
                    btn_9.Location = new Point(218, 321);
                    btn_8.Location = new Point(149, 321);
                    btn_7.Location = new Point(80, 321);
                    btn_e_Power_x.Location = new Point(11, 321);


                    btn_Minus.Location = new Point(287, 369);
                    btn_6.Location = new Point(218, 369);
                    btn_5.Location = new Point(149, 369);
                    btn_4.Location = new Point(80, 369);
                    btn_10_Power_x.Location = new Point(11, 369);

                    btn_Plus.Location = new Point(287, 417);
                    btn_3.Location = new Point(218, 417);
                    btn_2.Location = new Point(149, 417);
                    btn_1.Location = new Point(80, 417);
                    btn_log.Location = new Point(11, 417);

                    btn_Equals.Location = new Point(287, 465);
                    btn_dot.Location = new Point(218, 465);
                    btn_0.Location = new Point(149, 465);
                    btn_Plus_Minus.Location = new Point(80, 465);
                    btn_ln.Location = new Point(11, 465);


                    foreach (Button btn_ in new Button[] { btn_Mod, btn_Clear, btn_Ans, btn_Del, btn_reciprocal, btn_Square, btn_Sqrt, btn_Divide, btn_7, btn_8, btn_9, btn_Multiply, btn_4, btn_5, btn_6, btn_Minus, btn_1, btn_2, btn_3, btn_Plus, btn_Plus_Minus, btn_0, btn_dot, btn_Equals, btn_ln, btn_log, btn_10_Power_x, btn_e_Power_x, btn_expo, btn_pi, btn_e, btn_Sin, btn_Cos, btn_Tan, btn_Round })
                    {
                        btn_.Size = new Size(66, 45);
                    }
                    foreach (Button btn_ in new Button[] { btn_ln, btn_log, btn_10_Power_x, btn_e_Power_x, btn_expo, btn_pi, btn_e, btn_Sin, btn_Cos, btn_Tan, btn_Round, btn_deg_rad, btn_Shift })
                    {
                        btn_.Visible = true;
                    }
                    btn_hamburger.PerformClick();
                    break;
                default:
                    break;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // All the keyboard inputs
            if (e.Shift && e.KeyCode == Keys.D8)
            {
                btn_Multiply.PerformClick();
                return;
            }
            if (e.Shift && e.KeyCode == Keys.D5)
            {
                btn_Mod.PerformClick();
                return;
            }
            if (e.Shift && e.KeyCode == Keys.Back)
            {
                btn_Clear.PerformClick();
                return;
            }
            if (e.Shift && e.KeyCode == Keys.D6)
            {
                btn_expo.PerformClick();
                return;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                btn_Copy.PerformClick();
                return;
            }
            if (e.Shift)
            {
                btn_Shift.PerformClick();
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    btn_hamburger.PerformClick();
                    return;
                case Keys.Enter:
                    btn_Equals.PerformClick();
                    return;
                case Keys.D0:
                    btn_0.PerformClick();
                    return;
                case Keys.D1:
                    btn_1.PerformClick();
                    return;
                case Keys.D2:
                    btn_2.PerformClick();
                    return;
                case Keys.D3:
                    btn_3.PerformClick();
                    return;
                case Keys.D4:
                    btn_4.PerformClick();
                    return;
                case Keys.D5:
                    btn_5.PerformClick();
                    return;
                case Keys.D6:
                    btn_6.PerformClick();
                    return;
                case Keys.D7:
                    btn_7.PerformClick();
                    return;
                case Keys.D8:
                    btn_8.PerformClick();
                    return;
                case Keys.D9:
                    btn_9.PerformClick();
                    return;
                case Keys.Back:
                    btn_Del.PerformClick();
                    return;
                case Keys.OemPeriod:
                    btn_dot.PerformClick();
                    return;
                case Keys.Oemplus:
                    btn_Plus.PerformClick();
                    return;
                case Keys.OemMinus:
                    btn_Minus.PerformClick();
                    return;
                case Keys.OemQuestion:
                    btn_Divide.PerformClick();
                    return;
                case Keys.S:
                    btn_Sin.PerformClick();
                    return;
                case Keys.C:
                    btn_Cos.PerformClick();
                    return;
                case Keys.T:
                    btn_Tan.PerformClick();
                    return;
                case Keys.D:
                    btn_deg_rad.PerformClick();
                    break;
                case Keys.R:
                    btn_deg_rad.PerformClick();
                    break;
            }
            click_sound("Normal");
        }

        private void btn_pi_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            txtbox_input.Text += "π";
        }

        private void btn_e_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            txtbox_input.Text += "e";
        }

        private void click_sound(string type)
        {
            if (!Mute_Button)
            {
                SoundPlayer player; // Play button sounds
                switch (type)
                {
                    case "Normal":
                        System.IO.Stream click_normal = Properties.Resources.GENERIC_R1_press;
                        player = new SoundPlayer(click_normal);
                        player.Play();
                        break;
                    case "Delete":
                        System.IO.Stream click_del = Properties.Resources.BACKSPACE_press;
                        player = new SoundPlayer(click_del);
                        player.Play();
                        break;
                    case "Enter":
                        System.IO.Stream click_enter = Properties.Resources.ENTER_press;
                        player = new SoundPlayer(click_enter);
                        player.Play();
                        break;
                    case "Space":
                        System.IO.Stream click_space = Properties.Resources.SPACE_press;
                        player = new SoundPlayer(click_space);
                        player.Play();
                        break;
                    default:
                        return;
                }
                player.Dispose();
                Console.WriteLine(type + " is played");
            }
        }

        private void Speech(MainForm_230833F talk, string results)
        {
            if (!Mute_Speech)
            {
                talk.syn.SpeakAsync(results); // Output speech synthesizer
            }
        }

        bool Mute_Button = false;
        bool Mute_Speech = false;
        private void Mute_CheckedChanged(object sender, EventArgs e) // Visual status bar to see if Button/Speech is muted or not
        {
            click_sound("Space");
            if (Rbutt_All.Checked)
            {
                PBox_Btn.Image = Properties.Resources.Unmute;
                PBox_Spk.Image = Properties.Resources.Unmute;
                Mute_Button = false;
                Mute_Speech = false;
            }
            else if (Rbutt_Butt.Checked)
            {
                PBox_Btn.Image = Properties.Resources.Unmute;
                PBox_Spk.Image = Properties.Resources.Mute;
                Mute_Button = false;
                Mute_Speech = true;
            }
            else if (Rbutt_Speak.Checked)
            {
                PBox_Btn.Image = Properties.Resources.Mute;
                PBox_Spk.Image = Properties.Resources.Unmute;
                Mute_Button = true;
                Mute_Speech = false;
            }
            else if (Rbutt_No.Checked)
            {
                PBox_Btn.Image = Properties.Resources.Mute;
                PBox_Spk.Image = Properties.Resources.Mute;
                Mute_Button = true;
                Mute_Speech = true;
            }
        }

        bool deg_rad = false;
        private void btn_deg_rad_Click(object sender, EventArgs e)
        {
            click_sound("Normal"); 

            if (deg_rad) // Visual status bar for Rad/Deg
            {
                deg_rad = false;
                btn_deg_rad.Text = "Rad";
                lbl_deg_rad.Text = "Rad";
            }
            else
            {
                deg_rad = true;
                btn_deg_rad.Text = "Deg";
                lbl_deg_rad.Text = "Deg";
            }

            if (txtbox_input.Text.Length > 0) //  Perform the trigo button click of its respective trigo functions sin/cos/tan 
            {
                if (txtbox_input.Text[0].ToString() == "S")
                {
                    string temp_num = lastNumber(txtbox_input.Text);
                    txtbox_input.Text = temp_num;
                    btn_Sin.PerformClick();
                }
                else if (txtbox_input.Text[0].ToString() == "C")
                {
                    string temp_num = lastNumber(txtbox_input.Text);
                    txtbox_input.Text = temp_num;
                    btn_Cos.PerformClick();
                }
                else if (txtbox_input.Text[0].ToString() == "T")
                {
                    string temp_num = lastNumber(txtbox_input.Text);
                    txtbox_input.Text = temp_num;
                    btn_Tan.PerformClick();
                }
            }
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lbl_Results.Text); // Copy results to clipboard
        }

        private bool isShiftToggled = false;
        private void btn_Shift_Click(object sender, EventArgs e)
        {
            click_sound("Normal");
            // Visual change of trigo and inverse trigo
            isShiftToggled = !isShiftToggled;
            if (isShiftToggled)
            {
                btn_Shift.BackColor = Color.FromArgb(50, 50, 50);
                btn_Sin.Text = "sin⁻¹";
                btn_Cos.Text = "cos⁻¹";
                btn_Tan.Text = "tan⁻¹";
            }
            else
            {
                btn_Shift.BackColor = Color.FromArgb(32, 32, 32);
                btn_Sin.Text = "sin(θ)";
                btn_Cos.Text = "cos(θ)";
                btn_Tan.Text = "tan(θ)";
            }
        }
    }
}
