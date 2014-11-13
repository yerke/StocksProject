using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Stocks.WpfClient.ValidationRules
{
    /// <summary>
    /// Validates that a string is no longer than MaxLength characters.
    /// </summary>
    class StringMaxLengthRule : ValidationRule
    {
        public int MaxLength { get; set; }

        public override ValidationResult Validate(
            object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null && value.ToString().Length > MaxLength)
                return new ValidationResult(false, "Cannot exceed " 
                    + MaxLength + " characters.");
            return ValidationResult.ValidResult;
        }
    }
}
