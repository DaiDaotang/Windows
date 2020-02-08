using System;
using System.Collections.Generic;

namespace Calculatordll
{
    /*语法树
     * exp -> term [add-op term]*
     * term -> factor [mul-op factor]*
     * factor -> ( exp ) | number | +- ( exp )
     * add-op -> + | -
     * mul-op -> * | /
     * */
    public class Calculate
    {
        private LinkedList<Exp> e;
        private LinkedListNode<Exp> node;

        public Calculate(LinkedList<Exp> e)
        {
            this.e = e;
        }
        
        public void excuteExp(LinkedList<Exp> exp)
        {
            //处理传入的LinkedList，把数字合并
            LinkedListNode<Exp> linkNodeExp = e.First;
            while (linkNodeExp != null)
            {
                if(linkNodeExp.Value.type=="op"|| linkNodeExp.Value.type == "leftBrac"|| linkNodeExp.Value.type == "rightBrac")
                {
                    Exp exp1 = new Exp(linkNodeExp.Value.type, linkNodeExp.Value.value);
                    exp.AddLast(exp1);
                }
                else if(linkNodeExp.Value.type == "num")
                {
                    String s = linkNodeExp.Value.value;
                    while(linkNodeExp.Next!=null&&linkNodeExp.Next.Value.type == "num")
                    {
                        linkNodeExp = linkNodeExp.Next;
                        s += linkNodeExp.Value.value;
                    }
                    if(linkNodeExp.Next != null && linkNodeExp.Next.Value.type == "point")
                    {
                        linkNodeExp = linkNodeExp.Next;
                        s += linkNodeExp.Value.value;
                        while (linkNodeExp.Next != null && linkNodeExp.Next.Value.type == "num")
                        {
                            linkNodeExp = linkNodeExp.Next;
                            s += linkNodeExp.Value.value;
                        }
                        Exp exp2 = new Exp("double", s);
                        exp.AddLast(exp2);
                    }
                    else
                    {
                        Exp exp3 = new Exp("int", s);
                        exp.AddLast(exp3);
                    }
                }
                else if(linkNodeExp.Value.type == "sign")
                {
                    linkNodeExp = linkNodeExp.Next;
                    if(linkNodeExp.Value.value=="(")
                    {
                        linkNodeExp = linkNodeExp.Previous;
                        exp.AddLast(new Exp(linkNodeExp.Value.type, linkNodeExp.Value.value));
                    }
                    else if(linkNodeExp.Value.type == "num")
                    {
                        linkNodeExp = linkNodeExp.Previous;
                        String s = linkNodeExp.Value.value;
                        linkNodeExp = linkNodeExp.Next;
                        s += linkNodeExp.Value.value;
                        while (linkNodeExp.Next != null && linkNodeExp.Next.Value.type == "num")
                        {
                            linkNodeExp = linkNodeExp.Next;
                            s += linkNodeExp.Value.value;
                        }
                        if (linkNodeExp.Next != null && linkNodeExp.Next.Value.type == "point")
                        {
                            linkNodeExp = linkNodeExp.Next;
                            s += linkNodeExp.Value.value;
                            while (linkNodeExp.Next != null && linkNodeExp.Next.Value.type == "num")
                            {
                                linkNodeExp = linkNodeExp.Next;
                                s += linkNodeExp.Value.value;
                            }
                            Exp exp2 = new Exp("double", s);
                            exp.AddLast(exp2);
                        }
                        else
                        {
                            Exp exp3 = new Exp("int", s);
                            exp.AddLast(exp3);
                        }
                    }                   
                }
                linkNodeExp = linkNodeExp.Next;
            }
        }
        public Exp calculateExp()
        {
            bool isDouble = false;
            Exp exp = calculateTerm();
            if (exp.value == "")
            {
                return new Exp("", "");
            }
            if (exp.type == "double")
            {
                isDouble = true;
            }
            double result = double.Parse(exp.value);
            while (node.Next!=null&&(node.Next.Value.value == "+" || node.Next.Value.value == "-"))
            {
                node = node.Next;
                if (node.Value.value == "+")
                {
                    node = node.Next;
                    Exp exp2 = calculateTerm();
                    if (exp2.type == "double")
                    {
                        isDouble = true;
                    }
                    double value = double.Parse(exp2.value);
                    result = result + value;
                }
                else if (node.Value.value == "-")
                {
                    node = node.Next;
                    Exp exp2 = calculateTerm();
                    if (exp2.type == "double")
                    {
                        isDouble = true;
                    }
                    double value = double.Parse(exp2.value);
                    result = result - value;
                }
            }
            if (isDouble)
            {
                return new Exp("double", Convert.ToString(result));
            }
            else
            {
                int t = Convert.ToInt32(result);
                return new Exp("int", Convert.ToString(t));
            }
        }
        public Exp calculateTerm()
        {
            bool isDouble = false;
            Exp exp = calculateFactor();
            if(exp.value=="")
            {
                return new Exp("", "");
            }
            if(exp.type=="double")
            {
                isDouble = true;
            }
            double result = double.Parse(exp.value);           
            while (node.Next != null && (node.Next.Value.value == "*" || node.Next.Value.value == "/"))
            {
                node = node.Next;
                if (node.Value.value == "*")
                {
                    node = node.Next;
                    Exp exp2 = calculateFactor();
                    if (exp2.type == "double")
                    {
                        isDouble = true;
                    }
                    double value = double.Parse(exp2.value);
                    result = result * value;
                }
                else if (node.Value.value == "/")
                {
                    node = node.Next;
                    Exp exp2 = calculateFactor();
                    isDouble = true;                    
                    double value = double.Parse(exp2.value);
                    result = result / value;
                }
            }
            if(isDouble)
            {
                return new Exp("double", Convert.ToString(result));
            }
            else
            {
                return new Exp("int", Convert.ToString(result));
            }                  
        }
        public Exp calculateFactor()
        {
            if(node==null)
            {
                return new Exp("", "");
            }
            else if(node.Value.value=="(")
            {
                node = node.Next;
                Exp exp = calculateExp();
                node = node.Next;
                return exp;
            }
            else if(node.Value.type == "sign")
            {
                String s = node.Value.value;
                node = node.Next;
                node = node.Next;
                Exp exp = calculateExp();
                node = node.Next;
                s += exp.value;
                exp.value = s;
                return exp;
            }
            else if (node.Value.type == "int"|| node.Value.type == "double")
            {
                Exp exp = new Exp(node.Value.type, node.Value.value);
                return exp;               
            }
            else
            {
                return null;
            }
        }
        public Exp result()
        {
            LinkedList<Exp> exp = new LinkedList<Exp>();
            excuteExp(exp);
            node = exp.First;
            return calculateExp();
        }
    }
}
