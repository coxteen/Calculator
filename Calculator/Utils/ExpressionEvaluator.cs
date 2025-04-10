using System;
using System.Collections.Generic;
using System.Globalization;

public class ExpressionEvaluator
{
    // Define operator precedence
    private static Dictionary<string, int> precedence = new Dictionary<string, int>
    {
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 },
        { "%", 2 }
    };

    // Convert an infix expression (list of tokens) into a postfix expression.
    public static List<string> InfixToPostfix(List<string> tokens)
    {
        List<string> output = new List<string>();
        Stack<string> opStack = new Stack<string>();

        foreach (string token in tokens)
        {
            if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                // Token is a number.
                output.Add(token);
            }
            else if (precedence.ContainsKey(token))
            {
                // Token is an operator.
                while (opStack.Count > 0 && precedence.ContainsKey(opStack.Peek()) &&
                       precedence[opStack.Peek()] >= precedence[token])
                {
                    output.Add(opStack.Pop());
                }
                opStack.Push(token);
            }
            // Optionally, add support for parentheses if needed.
        }

        while (opStack.Count > 0)
        {
            output.Add(opStack.Pop());
        }

        return output;
    }

    // Evaluate a postfix expression.
    public static decimal EvaluatePostfix(List<string> postfixTokens)
    {
        Stack<decimal> stack = new Stack<decimal>();

        foreach (string token in postfixTokens)
        {
            if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
            {
                stack.Push(number);
            }
            else if (precedence.ContainsKey(token))
            {
                decimal b = stack.Pop();
                decimal a = stack.Pop();
                decimal result = token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    "%" => a % b,
                    _ => throw new Exception("Unsupported operator")
                };
                stack.Push(result);
            }
        }

        return stack.Pop();
    }

    // This helper method combines the two steps.
    public static decimal EvaluateInfixExpression(List<string> tokens)
    {
        List<string> postfix = InfixToPostfix(tokens);
        return EvaluatePostfix(postfix);
    }
}
