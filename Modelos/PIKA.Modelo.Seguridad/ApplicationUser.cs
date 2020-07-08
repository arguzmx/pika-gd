using Microsoft.AspNetCore.Identity;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad.Base;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Seguridad
{
    public class ApplicationUser : IdentityUser, IEntidadEliminada
    {

        public ApplicationUser() {
            this.UsuariosDominio = new HashSet<UsuarioDominio>();
        }

        //
        // Resumen:
        //     Gets or sets the date and time, in UTC, when any user lockout ends.
        //
        // Comentarios:
        //     A value in the past means the user is not locked out.
        public override DateTimeOffset? LockoutEnd { get; set; }
        
        //
        // Resumen:
        //     Gets or sets a flag indicating if two factor authentication is enabled for this
        //     user.
        //
        // Valor:
        //     True if 2fa is enabled, otherwise false.
        public override bool TwoFactorEnabled { get; set; }

        //
        // Resumen:
        //     Gets or sets a flag indicating if a user has confirmed their telephone address.
        //
        // Valor:
        //     True if the telephone number has been confirmed, otherwise false.
        public override bool PhoneNumberConfirmed { get; set; }
        
        //
        // Resumen:
        //     Gets or sets a telephone number for the user.
        public override string PhoneNumber { get; set; }

        //
        // Resumen:
        //     A random value that must change whenever a user is persisted to the store
        public override string ConcurrencyStamp { get; set; }

        //
        // Resumen:
        //     A random value that must change whenever a users credentials change (password
        //     changed, login removed)
        public override string SecurityStamp { get; set; }

        //
        // Resumen:
        //     Gets or sets a salted and hashed representation of the password for this user.
        public override string PasswordHash { get; set; }

        //
        // Resumen:
        //     Gets or sets a flag indicating if a user has confirmed their email address.
        //
        // Valor:
        //     True if the email address has been confirmed, otherwise false.
        public override bool EmailConfirmed { get; set; }

        //
        // Resumen:
        //     Gets or sets the normalized email address for this user.
        public override string NormalizedEmail { get; set; }

        //
        // Resumen:
        //     Gets or sets the email address for this user.
        public override string Email { get; set; }

        //
        // Resumen:
        //     Gets or sets the normalized user name for this user.
        public override string NormalizedUserName { get; set; }

        //
        // Resumen:
        //     Gets or sets the user name for this user.
        public override string UserName { get; set; }

        //
        // Resumen:
        //     Gets or sets the primary key for this user.

        public override string Id { get; set; }
        
        //
        // Resumen:
        //     Gets or sets a flag indicating if the user could be locked out.
        //
        // Valor:
        //     True if the user could be locked out, otherwise false.

        public override bool LockoutEnabled { get; set; }

        //
        // Resumen:
        //     Gets or sets the number of failed login attempts for the current user.
        public override int AccessFailedCount { get; set; }


        /// <summary>
        /// Especifica si la cuenta se encuentra ainactiva
        /// </summary>
        public bool Inactiva { get; set; }

        /// <summary>
        /// Especifica si la cuenta ha sido marcada para eliminar
        /// </summary>
        public bool Eliminada { get; set; }

        public bool GlobalAdmin { get; set; }

        public virtual ICollection<UsuarioDominio>  UsuariosDominio { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual PropiedadesUsuario Propiedades { get; set; }
        
    }
}
