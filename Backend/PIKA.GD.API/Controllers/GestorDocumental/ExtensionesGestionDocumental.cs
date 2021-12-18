using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Modelo.Seguridad;
using System.Globalization;

namespace PIKA.GD.API.Controllers.GestorDocumental
{
    public static class ExtensionesGestionDocumental
    {
        public static UsuarioPrestamo AUsuarioPrestamo(this PropiedadesUsuario u)
        {
            UsuarioPrestamo up = new UsuarioPrestamo() { Email ="", Nombre ="", Puesto ="", Telefono ="" };
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            if (u != null)
            {
                up.Nombre = $"{(u.name ?? "")} {(u.family_name ?? "")} {(u.given_name ?? "")}".TrimEnd();
                up.Nombre = ti.ToTitleCase(up.Nombre);
                up.Puesto = "";
                up.Telefono = "";
            }

            return up;
        }

    }
}
