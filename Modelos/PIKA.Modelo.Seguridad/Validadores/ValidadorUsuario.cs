using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace PIKA.Modelo.Seguridad.Validadores
{

    public static class ValidadorUsuario
    {

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool UsernameValido(string username) {
            bool r = false;
            bool hasNoneAlpha = false;
            int minlen = 2;

            if (username.Length >= minlen)
            {
                foreach (char c in username)
                {
                    if (char.IsWhiteSpace(c)) hasNoneAlpha = true;
                    if (char.IsPunctuation(c) && (c != '-')) hasNoneAlpha = true;


                }
                if (!hasNoneAlpha ) r = true;
            }

            return r;
        }


    //options.Password.RequireDigit = true;
    //options.Password.RequireLowercase = true;
    //options.Password.RequireNonAlphanumeric = true;
    //options.Password.RequireUppercase = true;
    //options.Password.RequiredLength = 6;
    //options.Password.RequiredUniqueChars = 1;

        public static bool ContrasenaValida(string contrasena, ILogger logger)
        {
            bool hasDigit = false;
            bool hasLcase = false;
            bool hasNoneAlpha = false;
            bool hasUcase = false;
            int maxRequiredUniqueChars = 1;

            bool r = false;
            if (contrasena.Length >= 6)
            {
                foreach (char c in contrasena)
                {
                    if (char.IsDigit(c)) hasDigit = true;
                    if (char.IsLower(c)) hasLcase = true;
                    if (char.IsUpper(c)) hasUcase= true;
                    if (char.IsPunctuation(c) || char.IsSymbol(c)) hasNoneAlpha = true;

                }
                var result = contrasena
                    .ToUpper()
                    .GroupBy(c => c)
                    .Select(g => new { Letter = g.Key, Count = g.Count() });

                if (result.Where(x => x.Count == 1).Count() >= maxRequiredUniqueChars)
                {
                    
                    if (hasDigit && hasLcase && hasNoneAlpha && hasUcase) r = true;
                }

            }

            return r;
        }
    }
}
