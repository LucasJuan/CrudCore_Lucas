using System;
using System.Collections.Generic;

namespace ExemploCore3.Models
{
    public partial class Acessa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public DateTime? DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string Chave { get; set; }
        public string Ativo { get; set; }
        public string Role { get; set; }
    }

}
