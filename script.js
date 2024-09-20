let currentInput = "";
let operator = null;
let firstOperand = null;

function appendValue(value) {
  currentInput += value;
  document.getElementById("calculator-screen").value = currentInput;
}

function appendOperator(op) {
  if (currentInput === "") return;

  firstOperand = parseFloat(currentInput);
  operator = op;
  currentInput = "";
}

function calculate() {
  if (operator === null || currentInput === "") return;

  let secondOperand = parseFloat(currentInput);
  let result = 0;

  switch (operator) {
    case "+":
      result = firstOperand + secondOperand;
      break;
    case "-":
      result = firstOperand - secondOperand;
      break;
    case "*":
      result = firstOperand * secondOperand;
      break;
    case "/":
      result = firstOperand / secondOperand;
      break;
  }

  document.getElementById("calculator-screen").value = result;
  currentInput = result.toString();
  operator = null;
}

function clearScreen() {
  currentInput = "";
  operator = null;
  firstOperand = null;
  document.getElementById("calculator-screen").value = "";
}
