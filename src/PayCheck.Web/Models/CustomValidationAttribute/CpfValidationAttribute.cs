namespace PayCheck.Web.Models.CustomValidationAttribute
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class responsible for the <see cref="CpfValidationAttribute"/> "Validação do CPF" attribute.
    /// </summary>
    public sealed class CpfValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpfValidationAttribute"/> class.
        /// </summary>
        public CpfValidationAttribute()
        { }

        /// <summary>
        /// Checks if a value passed is a valid CPF.
        /// </summary>
        /// <param name="value">CPF value to be tested.</param>
        /// <returns>True if the value is valid, False otherwise.</returns>
        public override bool IsValid(object value)
        {
            try
            {
                if (value != null)
                    return ARVTech.Shared.Verifiers.CpfVerifier.IsValid(
                        value.ToString());

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
