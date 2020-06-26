using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Seguridad
{
    public class UserClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public ApplicationUser User { get; set; }
    }
}
