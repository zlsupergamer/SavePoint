//  Conquista.cs — Entidade simples (ENCAPSULAMENTO)

using System;

namespace SavePoint.Models
{
    public class Conquista
    {
        public string    Nome            { get; private set; }
        public string    Descricao       { get; private set; }
        public bool      Desbloqueada    { get; private set; }
        public DateTime? DataDesbloqueio { get; private set; }

        public Conquista(string nome, string descricao)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da conquista não pode ser vazio.");

            Nome         = nome;
            Descricao    = descricao ?? "";
            Desbloqueada = false;
        }

        public void Desbloquear()
        {
            if (Desbloqueada)
            {
                Console.WriteLine($"  ⚠  Conquista '{Nome}' já estava desbloqueada.");
                return;
            }
            Desbloqueada    = true;
            DataDesbloqueio = DateTime.Now;
        }

        public override string ToString()
        {
            string status = Desbloqueada
                ? $"✅ {DataDesbloqueio:dd/MM/yyyy HH:mm}"
                : "🔒 Bloqueada";
            return $"{Nome} — {status}";
        }
    }
}
