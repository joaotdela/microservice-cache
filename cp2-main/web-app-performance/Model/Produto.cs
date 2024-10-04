﻿using System.Data;

namespace web_app_performance.Model
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Preco { get; set; }

        public double QuantidadeEstoque { get; set; } 

        public DateTime DataCriacao { get; set; }  


    }
}
