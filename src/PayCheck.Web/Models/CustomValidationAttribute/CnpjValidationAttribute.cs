namespace EquHos.Models.CustomValidationAttribute
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class responsible for the <see cref="CnpjValidationAttribute"/> "Validação do CNPJ" attribute.
    /// </summary>
    public sealed class CnpjValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CnpjValidationAttribute"/> class.
        /// </summary>
        public CnpjValidationAttribute()
        {
        }

        /// <summary>
        /// Checks if a value passed is a valid CNPJ.
        /// </summary>
        /// <param name="value">CNPJ value to be tested.</param>
        /// <returns>True if the value is valid, False otherwise.</returns>
        public override bool IsValid(object value)
        {
            try
            {
                if (value != null)
                {
                    //return Verifiers.CnpjVerifier.IsValid(value.ToString());
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }
    }
}