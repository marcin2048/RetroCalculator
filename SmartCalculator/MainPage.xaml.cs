using System.Globalization;

namespace RetroCalculator
{
    public partial class MainPage : ContentPage
    {
        private decimal _firstOperand;
        private decimal _memory;
        private string? _pendingOperator;
        private bool _waitingForOperand = true;

        private Label Display => this.FindByName<Label>("DisplayLabel");

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            var key = button.Text;

            if (key is "MC" or "MR" or "M−" or "M+")
            {
                HandleMemoryKey(key);
                return;
            }

            if (key is "0" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9")
            {
                EnterDigit(key);
                return;
            }

            switch (key)
            {
                case ",":
                    EnterDecimalSeparator();
                    break;
                case "C":
                    Clear();
                    break;
                case "±":
                    ToggleSign();
                    break;
                case "%":
                    SetDisplay(ParseDisplay() / 100m);
                    break;
                case "+":
                case "−":
                case "×":
                case "÷":
                    SetOperator(key);
                    break;
                case "=":
                    CalculateResult();
                    break;
            }
        }

        private void HandleMemoryKey(string key)
        {
            switch (key)
            {
                case "MC":
                    _memory = 0;
                    break;
                case "MR":
                    SetDisplay(_memory);
                    _waitingForOperand = true;
                    break;
                case "M+":
                    _memory += ParseDisplay();
                    break;
                case "M−":
                    _memory -= ParseDisplay();
                    break;
            }
        }

        private void EnterDigit(string digit)
        {
            if (_waitingForOperand || Display.Text == "0")
            {
                Display.Text = digit;
                _waitingForOperand = false;
                return;
            }

            if (Display.Text!.Length < 16)
                Display.Text += digit;
        }

        private void EnterDecimalSeparator()
        {
            if (_waitingForOperand)
            {
                Display.Text = "0,";
                _waitingForOperand = false;
            }
            else if (!Display.Text!.Contains(',') && !Display.Text.Contains('.'))
            {
                Display.Text += ",";
            }
        }

        private void SetOperator(string operatorKey)
        {
            var value = ParseDisplay();

            if (_pendingOperator is not null && !_waitingForOperand)
            {
                value = ApplyOperation(_firstOperand, value, _pendingOperator);
                SetDisplay(value);
            }

            _firstOperand = value;
            _pendingOperator = operatorKey;
            _waitingForOperand = true;
        }

        private void CalculateResult()
        {
            if (_pendingOperator is null)
                return;

            var result = ApplyOperation(_firstOperand, ParseDisplay(), _pendingOperator);
            SetDisplay(result);
            _pendingOperator = null;
            _waitingForOperand = true;
        }

        private decimal ApplyOperation(decimal left, decimal right, string operatorKey)
        {
            return operatorKey switch
            {
                "+" => left + right,
                "−" => left - right,
                "×" => left * right,
                "÷" => right == 0 ? 0 : left / right,
                _ => right
            };
        }

        private void ToggleSign()
        {
            SetDisplay(-ParseDisplay());
        }

        private void Clear()
        {
            Display.Text = "0";
            _firstOperand = 0;
            _pendingOperator = null;
            _waitingForOperand = true;
        }

        private decimal ParseDisplay()
        {
            var normalized = Display.Text!.Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                ? value
                : 0;
        }

        private void SetDisplay(decimal value)
        {
            Display.Text = value.ToString("0.###############", CultureInfo.InvariantCulture).Replace('.', ',');
        }
    }
}



