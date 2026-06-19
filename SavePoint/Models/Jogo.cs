//  Jogo.cs — Classe abstrata base (ABSTRAÇÃO + ENCAPSULAMENTO)

using System;
using System.Collections.Generic;

namespace SavePoint.Models
{
    public abstract class Jogo
    {
        public int    Id              { get; protected set; }
        public string Titulo          { get; protected set; }
        public string Genero          { get; protected set; }
        public string Status          { get; protected set; }   
        public double HorasJogadas    { get; private set; }

        private readonly List<Conquista> _conquistas = new List<Conquista>();
        public IReadOnlyList<Conquista> Conquistas => _conquistas.AsReadOnly();

        protected Jogo(int id, string titulo, string genero)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título do jogo não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(genero))
                throw new ArgumentException("O gênero do jogo não pode ser vazio.");

            Id     = id;
            Titulo = titulo;
            Genero = genero;
            Status = "Não Iniciado";
        }

        public void AdicionarHoras(double horas)
        {
            if (horas <= 0)
                throw new ArgumentException("A quantidade de horas deve ser positiva.");
            HorasJogadas += horas;
        }

        public void AtualizarStatus(string novoStatus)
        {
            string[] statusValidos = { "Não Iniciado", "Em Andamento", "Concluído", "Platinado" };
            bool valido = false;
            foreach (string s in statusValidos)
                if (s == novoStatus) { valido = true; break; }

            if (!valido)
                throw new ArgumentException($"Status inválido: '{novoStatus}'.");
            Status = novoStatus;
        }

        public void DesbloquearConquista(string nomeConquista)
        {
            foreach (Conquista c in _conquistas)
            {
                if (c.Nome.Equals(nomeConquista, StringComparison.OrdinalIgnoreCase))
                {
                    c.Desbloquear();
                    Console.WriteLine($"  🏆 Conquista desbloqueada: {c.Nome}");
                    return;
                }
            }
            throw new InvalidOperationException($"Conquista '{nomeConquista}' não encontrada.");
        }

        public void AdicionarConquista(Conquista conquista)
        {
            if (conquista == null)
                throw new ArgumentNullException(nameof(conquista));
            _conquistas.Add(conquista);
        }

        public abstract void ExibirDetalhes();

        public abstract double CalcularProgresso();
    }
}
