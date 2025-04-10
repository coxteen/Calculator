 using Calculator.Command;
using Calculator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Calculator.ViewModel
{
    class MainVM : INotifyPropertyChanged
    {
        #region Fields

        private bool _digitGrouping;
        private bool _programmer;
        private bool _operationPriority;
        private int _baseForCalculations;

        private string _keyPressedString;
        private string _enteredNumber;
        private ButtonPressedCommand _buttonPressedCommand;

        private List<string> EnteredKeys;
        private decimal Number = 0;
        private bool FirstNumberEntered;
        private bool FunctionPressed;
        private string SelectedFunction;
        private string PreviousNumber;
        public string PreviousEnteredKey;

        private List<decimal> _memoryStack;
        private bool _memoryGridVisible;
        private ObservableCollection<decimal> _memoryStackDisplay = new ObservableCollection<decimal>();

        #endregion

        #region Format string
        private string FormatNumber(decimal number)
        {
            if (DigitGrouping)
            {
                string numberString = number.ToString("0.##########");
                string integerPart, decimalPart = "";

                if (numberString.Contains('.'))
                {
                    string[] parts = numberString.Split('.');
                    integerPart = parts[0];
                    decimalPart = "." + parts[1];
                }
                else
                {
                    integerPart = numberString;
                }

                bool isNegative = integerPart.StartsWith('-');
                if (isNegative)
                {
                    integerPart = integerPart.Substring(1);
                }

                string result = "";
                int length = integerPart.Length;

                for (int i = 0; i < length; i++)
                {
                    if (i > 0 && (length - i) % 3 == 0)
                    {
                        result += ",";
                    }
                    result += integerPart[i];
                }

                if (isNegative)
                {
                    result = "-" + result;
                }

                return result + decimalPart;
            }
            else
            {
                return number.ToString("0.##########");
            }
        }
        #endregion

        #region Update Elements On GUI
        void UpdateEnteredKeysOnGUI()
        {
            string temp = "";
            for (int i = 0; i < EnteredKeys.Count; ++i)
            {
                temp += EnteredKeys[i];
            }
            KeyPressedString = temp;
        }
        #endregion

        #region Properties

        #region Digit Grouping
        public bool DigitGrouping
        {
            get { return _digitGrouping; }
            set
            {
                if (_digitGrouping != value)
                {
                    _digitGrouping = value;

                    decimal currentValue;
                    if (decimal.TryParse(_enteredNumber, out currentValue))
                    {
                        EnteredNumber = FormatNumber(currentValue);
                    }

                    OnPropertyChanged(nameof(DigitGrouping));
                }
            }
        }
        #endregion

        #region Programmer
        public bool Programmer
        {
            get { return _programmer; }
            set 
            { 
                if (_programmer != value)
                {
                    _programmer = value;
                    OnPropertyChanged(nameof(Programmer));
                }
            }
        }
        #endregion

        #region Operation Priority
        public bool OperationPriority
        {
            get { return _operationPriority; }
            set
            {
                if (_operationPriority != value)
                {
                    _operationPriority = value;
                    OnPropertyChanged(nameof(OperationPriority));
                }
            }
        }
        #endregion

        #region BaseForCalculations
        public int BaseForCalculations
        {
            get { return _baseForCalculations; }
            set 
            { 
                if (_baseForCalculations != value)
                {
                    _baseForCalculations = value;
                    OnPropertyChanged(nameof(BaseForCalculations));
                }
            }
        }
        #endregion

        #region Key Pressed String
        public string KeyPressedString
        {
            get { return _keyPressedString; }
            set
            {
                _keyPressedString = value;
                OnPropertyChanged(nameof(KeyPressedString));
            }
        }
        #endregion

        #region Entered Number
        public string EnteredNumber
        {
            get { return _enteredNumber; }
            set
            {
                decimal parsedValue;
                // Try to parse and format if it's a valid number
                if (decimal.TryParse(value, out parsedValue))
                {
                    _enteredNumber = DigitGrouping ? FormatNumber(parsedValue) : value;
                }
                else
                {
                    _enteredNumber = value; // Keep original string if not a valid number
                }
                OnPropertyChanged(nameof(EnteredNumber));
            }
        }
        #endregion

        #region Button Pressed Command
        public ButtonPressedCommand buttonPressedCommand
        {
            get { return _buttonPressedCommand; }
            set { _buttonPressedCommand = value; }
        }
        #endregion

        #region Memory Properties

        #region Memory Grid Visible
        public bool MemoryGridVisible
        {
            get { return _memoryGridVisible; }
            set
            {
                _memoryGridVisible = value;
                OnPropertyChanged(nameof(MemoryGridVisible));
            }
        }
        #endregion

        #region Memory Stack Display
        public ObservableCollection<decimal> MemoryStackDisplay
        {
            get { return _memoryStackDisplay; }
            set
            {
                _memoryStackDisplay = value;
                OnPropertyChanged(nameof(MemoryStackDisplay));
            }
        }
        #endregion

        #endregion

        #endregion

        #region Constructor
        public MainVM()
        {
            EnteredKeys = new List<string>();
            Number = 0;
            FirstNumberEntered = true;
            FunctionPressed = false;
            SelectedFunction = "";
            PreviousEnteredKey = "";

            EnteredNumber = "0";
            KeyPressedString = "";

            buttonPressedCommand = new ButtonPressedCommand(this);
            _memoryStack = new List<decimal>();

            MemoryGridVisible = false;
        }
        #endregion

        #region Methods
        public void GetPressedButton(string pressedButton)
        {
            #region Lambda helper functions

            bool IsNumeric(string btn) => "0123456789.".Contains(btn);
            bool IsOperator(string token) => token == "+" || token == "-" || token == "*" || token == "/" || token == "%";
            string FormatResult(decimal number) => FormatNumber(number);

            #endregion

            #region Cut Copy Paste
            if (pressedButton == "Cut")
            {
                Clipboard.SetText(EnteredNumber);
                EnteredNumber = "0";
                OnPropertyChanged(nameof(EnteredNumber));
                PreviousEnteredKey = pressedButton;
                return;
            }
            if (pressedButton == "Copy")
            {
                Clipboard.SetText(EnteredNumber);
                PreviousEnteredKey = pressedButton;
                return;
            }
            if (pressedButton == "Paste")
            {
                if (Clipboard.ContainsText())
                {
                    EnteredNumber = Clipboard.GetText();
                    EnteredKeys.Add(EnteredNumber);
                    UpdateEnteredKeysOnGUI();
                    OnPropertyChanged(nameof(EnteredNumber));
                }
                PreviousEnteredKey = pressedButton;
                return;
            }
            #endregion

            #region Number insertion
            if (IsNumeric(pressedButton))
            {
                if (FunctionPressed || EnteredNumber == "0")
                {
                    EnteredNumber = pressedButton;
                    if (FunctionPressed)
                        EnteredKeys.Add(pressedButton);
                    else if (EnteredKeys.Count == 0)
                        EnteredKeys.Add(pressedButton);
                    else
                        EnteredKeys[EnteredKeys.Count - 1] = pressedButton;
                }
                else
                {
                    EnteredNumber += pressedButton;
                    if (EnteredKeys.Count > 0)
                        EnteredKeys[EnteredKeys.Count - 1] += pressedButton;
                    else
                        EnteredKeys.Add(pressedButton);
                }
                FunctionPressed = false;
                UpdateEnteredKeysOnGUI();
                PreviousEnteredKey = pressedButton;
                return;
            }
            #endregion

            #region Memory buttons
            switch (pressedButton)
            {
                case "MC":
                    _memoryStack.Clear();
                    PreviousEnteredKey = pressedButton;
                    return;

                case "MR":
                    if (_memoryStack.Any())
                    {
                        EnteredNumber = _memoryStack.Last().ToString();
                    }
                    PreviousEnteredKey = pressedButton;
                    return;

                case "M+":
                    {
                        decimal current = Convert.ToDecimal(EnteredNumber);
                        if (_memoryStack.Any())
                        {
                            _memoryStack[_memoryStack.Count - 1] += current;
                        }
                        else
                        {
                            _memoryStack.Add(current);
                        }
                        PreviousEnteredKey = pressedButton;
                        return;
                    }

                case "M-":
                    {
                        decimal current = Convert.ToDecimal(EnteredNumber);
                        if (_memoryStack.Any())
                        {
                            _memoryStack[_memoryStack.Count - 1] -= current;
                        }
                        else
                        {
                            _memoryStack.Add(-current);
                        }
                        PreviousEnteredKey = pressedButton;
                        return;
                    }

                case "MS":
                    {
                        decimal current = Convert.ToDecimal(EnteredNumber);
                        _memoryStack.Add(current);
                        PreviousEnteredKey = pressedButton;
                        return;
                    }

                case "M":
                    if (_memoryStack.Any())
                    {
                        MemoryWindow memWindow = new MemoryWindow(_memoryStack);
                        bool? result = memWindow.ShowDialog();
                        if (result == true)
                        {
                            // Use the selected memory value.
                            EnteredNumber = memWindow.SelectedValue.ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Memory is empty.", "Memory Stack");
                    }
                    PreviousEnteredKey = pressedButton;
                    return;

                default:
                    break;
            }
            #endregion

            switch (pressedButton)
            {
                #region Clear buttons
                case "C":
                    EnteredKeys.Clear();
                    KeyPressedString = "";
                    EnteredNumber = "0";
                    Number = 0;
                    FirstNumberEntered = true;
                    PreviousEnteredKey = pressedButton;
                    return;

                case "CE":
                    EnteredNumber = "0";
                    if (EnteredKeys.Count > 0)
                    {
                        string last = EnteredKeys[EnteredKeys.Count - 1];
                        if (last.All(ch => char.IsDigit(ch) || ch == '.' || ch == '-'))
                            EnteredKeys.RemoveAt(EnteredKeys.Count - 1);
                        else
                            EnteredKeys[EnteredKeys.Count - 1] = "";
                    }
                    UpdateEnteredKeysOnGUI();
                    PreviousEnteredKey = pressedButton;
                    return;

                case "E":
                    if (EnteredNumber.Length > 1)
                        EnteredNumber = EnteredNumber.Substring(0, EnteredNumber.Length - 1);
                    else
                        EnteredNumber = "0";

                    if (EnteredKeys.Count > 0)
                    {
                        string last = EnteredKeys[EnteredKeys.Count - 1];
                        if (last.Length > 1)
                            EnteredKeys[EnteredKeys.Count - 1] = last.Substring(0, last.Length - 1);
                        else
                            EnteredKeys.RemoveAt(EnteredKeys.Count - 1);
                    }
                    UpdateEnteredKeysOnGUI();
                    PreviousEnteredKey = pressedButton;
                    return;
                #endregion

                #region Two operands operations
                case "+":
                case "-":
                case "*":
                case "/":
                case "%":
                    if (SelectedFunction == "EqualTo")
                    {
                        if (EnteredKeys.Count > 0 && EnteredKeys[EnteredKeys.Count - 1] == "=")
                        {
                            EnteredKeys.Clear();
                            EnteredKeys.Add(Number.ToString());
                        }
                        SelectedFunction = "";
                    }
                    if (EnteredKeys.Count > 0 && IsOperator(EnteredKeys[EnteredKeys.Count - 1]))
                    {
                        EnteredKeys[EnteredKeys.Count - 1] = pressedButton;
                        SelectedFunction = pressedButton switch
                        {
                            "+" => "Addition",
                            "-" => "Subtraction",
                            "*" => "Multiplication",
                            "/" => "Division",
                            "%" => "Modulo",
                            _ => ""
                        };
                    }
                    else
                    {
                        if (FirstNumberEntered)
                        {
                            Number = Convert.ToDecimal(EnteredNumber);
                            FirstNumberEntered = false;
                        }
                        else
                        {
                            Number = SelectedFunction switch
                            {
                                "Addition" => CalculatorLogic.Add(Number, Convert.ToDecimal(EnteredNumber)),
                                "Subtraction" => CalculatorLogic.Subtract(Number, Convert.ToDecimal(EnteredNumber)),
                                "Multiplication" => CalculatorLogic.Multiply(Number, Convert.ToDecimal(EnteredNumber)),
                                "Division" => CalculatorLogic.Divide(Number, Convert.ToDecimal(EnteredNumber)),
                                "Modulo" => CalculatorLogic.Modulo(Number, Convert.ToDecimal(EnteredNumber)),
                                _ => Convert.ToDecimal(EnteredNumber)
                            };
                            EnteredNumber = FormatResult(Number);
                        }
                        EnteredKeys.Add(pressedButton);
                        SelectedFunction = pressedButton switch
                        {
                            "+" => "Addition",
                            "-" => "Subtraction",
                            "*" => "Multiplication",
                            "/" => "Division",
                            "%" => "Modulo",
                            _ => ""
                        };
                    }
                    break;
                #endregion

                #region One operand operations
                case "x^2":
                    PreviousNumber = EnteredNumber;
                    Number = CalculatorLogic.Square(Convert.ToDecimal(EnteredNumber));
                    EnteredNumber = FormatResult(Number);
                    EnteredKeys.Clear();
                    EnteredKeys.Add(PreviousNumber + "^2=" + EnteredNumber);
                    break;

                case "+/-":
                    Number = CalculatorLogic.Inverse(Convert.ToDecimal(EnteredNumber));
                    EnteredNumber = FormatResult(Number);
                    if (EnteredKeys.Count > 0)
                    {
                        string last = EnteredKeys[EnteredKeys.Count - 1];
                        if (last.All(ch => char.IsDigit(ch) || ch == '.' || ch == '-'))
                            EnteredKeys[EnteredKeys.Count - 1] = EnteredNumber;
                        else
                            EnteredKeys.Add(EnteredNumber);
                    }
                    else
                    {
                        EnteredKeys.Add(EnteredNumber);
                    }
                    break;

                case "1/x":
                    PreviousNumber = EnteredNumber;
                    Number = CalculatorLogic.OneOver(Convert.ToDecimal(EnteredNumber));
                    EnteredNumber = FormatResult(Number);
                    EnteredKeys.Clear();
                    EnteredKeys.Add("1/" + PreviousNumber + "=" + EnteredNumber);
                    break;

                case "sqrt(x)":
                    PreviousNumber = EnteredNumber;
                    Number = CalculatorLogic.Sqrt(Convert.ToDecimal(EnteredNumber));
                    EnteredNumber = FormatResult(Number);
                    EnteredKeys.Clear();
                    EnteredKeys.Add("sqrt(" + PreviousNumber + ")=" + EnteredNumber);
                    break;

                case "=":
                    if (_operationPriority)
                    {
                        try
                        {
                            decimal result = ExpressionEvaluator.EvaluateInfixExpression(EnteredKeys);
                            EnteredNumber = FormatResult(result);
                            EnteredKeys.Clear();
                            EnteredKeys.Add(EnteredNumber);
                        }
                        catch (Exception ex)
                        {
                            // Handle any conversion or evaluation error here.
                            MessageBox.Show("Error evaluating expression: " + ex.Message);
                        }
                    }
                    else
                    {
                        // Process without prioritizing operations: perform the calculation as entered.
                        if (!FirstNumberEntered)
                        {
                            Number = SelectedFunction switch
                            {
                                "Addition" => CalculatorLogic.Add(Number, Convert.ToDecimal(EnteredNumber)),
                                "Subtraction" => CalculatorLogic.Subtract(Number, Convert.ToDecimal(EnteredNumber)),
                                "Multiplication" => CalculatorLogic.Multiply(Number, Convert.ToDecimal(EnteredNumber)),
                                "Division" => CalculatorLogic.Divide(Number, Convert.ToDecimal(EnteredNumber)),
                                "Modulo" => CalculatorLogic.Modulo(Number, Convert.ToDecimal(EnteredNumber)),
                                _ => Convert.ToDecimal(EnteredNumber)
                            };
                            EnteredNumber = FormatResult(Number);
                            EnteredKeys.Add("=");
                        }
                        SelectedFunction = "EqualTo";
                    }
                    break;


                #endregion

                default:
                    break;
            }

            UpdateEnteredKeysOnGUI();
            PreviousEnteredKey = pressedButton;
            FunctionPressed = true;
        }

        #endregion

        #region Inherited Methods

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
