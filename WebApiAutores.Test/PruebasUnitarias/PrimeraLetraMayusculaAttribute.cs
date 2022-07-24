using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinusculaRetornaError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "jeisson";
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.AreEqual("La primera letra debe ser mayuscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNullNoRetornaError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayusculaNoRetornaError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Jeisson";
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }
    }
}