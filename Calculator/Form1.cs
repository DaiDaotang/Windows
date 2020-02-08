using Calculatordll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private LinkedList<Exp> exp = new LinkedList<Exp>();
        private bool hasPoint = false;
        private int hasLeftBrac = 0;
        public String last;        

        public Form1()
        {
            InitializeComponent();
        }
        private void num_click(object sender, EventArgs e)
        {
            Button a = sender as Button;//判断按下的是哪个按钮
            if (input.Text == "")//若是第一次输入数字
            {
                input.Text = a.Text;
                exp.AddLast(new Exp("num", a.Text));                
            }
            else
            {
                if (last.Equals(")"))//若之前是右括号，后面不能跟着数字
                {
                    error.Text = "')'后不允许输入数字";
                    return;
                }
                else
                {
                    if (last.Equals("/") && a.Text.Equals("0"))//若前面是除号，则不能跟着0
                    {
                        error.Text = "'0'不能作为除数";
                        return;
                    }
                    else
                    {
                        input.Text += a.Text;
                        exp.AddLast(new Exp("num", a.Text));
                    }
                }
            }
            last = "num";
            error.Text = "";
            input.Focus();
            input.Select(input.TextLength, 0);
            input.ScrollToCaret();          
        }
        private void op_Click(object sender, EventArgs e)
        {
            Button a = sender as Button;//判断按下的是哪个按钮
            if (a.Text.Equals("*")|| a.Text.Equals("/"))//先考虑乘除
            {
                if(input.Text == "")
                {
                    error.Text = "乘除只能跟在数字和右括号后";
                    return;
                }
                else if(last.Equals("num")|| last.Equals(")"))//乘除只能跟在数字和右括号后
                {
                    input.Text += a.Text;
                    exp.AddLast(new Exp("op", a.Text));
                }
                else
                {
                    error.Text = "乘除只能跟在数字和右括号后";
                    return;
                }
            }
            else//考虑加减法
            {
                if(input.Text==""||last.Equals("("))//若是第一个数字或左括号内是正负
                {
                    input.Text += a.Text;
                    exp.AddLast(new Exp("sign", a.Text));
                }
                else if (last.Equals("num") || last.Equals(")"))//若在数字和右括号后则是加减
                {
                    input.Text += a.Text;
                    exp.AddLast(new Exp("op", a.Text));
                }
                else
                {
                    error.Text = "'+''-'号使用错误";
                    return;
                }
            }
            last = a.Text;
            error.Text = "";
            hasPoint = false;
            input.Focus();
            input.Select(input.TextLength, 0);
            input.ScrollToCaret();
        }

        private void point_Click(object sender, EventArgs e)
        {
            if(!hasPoint&&last.Equals("num"))
            {
                input.Text += point.Text;
                exp.AddLast(new Exp("point", point.Text));
            }
            else
            {
                error.Text = "小数点添加错误";
                return;
            }
            last = "point";
            hasPoint = true;
            error.Text = "";
            input.Focus();
            input.Select(input.TextLength, 0);
            input.ScrollToCaret();
        }

        private void leftBrac_Click(object sender, EventArgs e)
        {   // 左括号只允许在开头，或者运算符后，或者左括号后面
            if(input.Text.Equals("")||last.Equals("+") || last.Equals("-") || last.Equals("*")
                || last.Equals("/") || last.Equals("("))
            {
                input.Text += leftBrac.Text;
                exp.AddLast(new Exp("leftBrac", leftBrac.Text));
            }
            else
            {
                error.Text = "'('使用不合法";
                return;
            }
            last = "(";
            hasLeftBrac ++;
            error.Text = "";
            input.Focus();
            input.Select(input.TextLength, 0);
            input.ScrollToCaret();
        }

        private void rightBrac_Click(object sender, EventArgs e)
        {
            if(hasLeftBrac==0)
            {
                error.Text = "缺少左括号";
                return;
            }
            else
            {
                if(last.Equals(")")||last.Equals("num"))//右括号只允许跟在右括号或者数字后面
                {
                    input.Text += rightBrac.Text;
                    exp.AddLast(new Exp("rightBrac", rightBrac.Text));
                }
                else
                {
                    error.Text = "右括号使用不合法";
                    return;
                }
            }
            last = ")";
            hasLeftBrac--;
            error.Text = "";
            input.Focus();
            input.Select(input.TextLength, 0);
            input.ScrollToCaret();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if(input.Text!="")
            {
                input.Text = input.Text.Substring(0, input.Text.Length - 1);
                if(exp.Count()==1)
                {
                    exp.RemoveLast();
                }
                else
                {
                    Exp expr = exp.Last();
                    if (expr.type.Equals("point"))
                    {
                        hasPoint = false;
                    }
                    else if (expr.type.Equals("rightBrac"))
                    {
                        hasLeftBrac++;
                    }
                    else if (expr.type.Equals("leftBrac"))
                    {
                        hasLeftBrac--;
                    }
                    exp.RemoveLast();
                    Exp expr2 = exp.Last();
                    if (expr2.type == "num")
                    {
                        last = "num";
                    }
                    else
                    {
                        last = expr2.value;
                    }
                }               
            }
            else
            {
                error.Text = "已经全部删除";
                return;
            }            
        }

        private void equal_Click(object sender, EventArgs e)
        {
            if(hasLeftBrac==0)
            {
                Calculate calculate = new Calculate(exp);
                Exp exp2 = calculate.result();
                result.Text = exp2.value;
            }
            else
            {
                error.Text = "左括号未闭合";
                return;
            }
        }

        private void input_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void deleteAll_Click(object sender, EventArgs e)
        {
            exp = new LinkedList<Exp>();
            input.Text = "";
            hasPoint = false;
        }
    }

  
}
