//  JogoSingle.cs — Subclasse concreta (HERANÇA + POLIMORFISMO)

using System;

namespace SavePoint.Models
{
    public class JogoSingle : Jogo
    {
        public string Dificuldade         { get; private set; }  
        public double TempoEstimadoHoras  { get; private set; }

        public JogoSingle(int id, string titulo, string genero,
                          string dificuldade, double tempoEstimadoHoras)
            : base(id, titulo, genero)   
        {
            if (string.IsNullOrWhiteSpace(dificuldade))
                throw new ArgumentException("A dificuldade não pode ser vazia.");
            if (tempoEstimadoHoras <= 0)
                throw new ArgumentException("O tempo estimado deve ser positivo.");

            Dificuldade        = dificuldade;
            TempoEstimadoHoras = tempoEstimadoHoras;
        }

        public override void ExibirDetalhes()
        {
            Console.WriteLine($"  [{Id}] {Titulo} ({Genero})");
            Console.WriteLine($"      Tipo       : Single-Player");
            Console.WriteLine($"      Dificuldade: {Dificuldade}");
            Console.WriteLine($"      Tempo Est. : {TempoEstimadoHoras}h");
            Console.WriteLine($"      Jogadas    : {HorasJogadas}h");
            Console.WriteLine($"      Status     : {Status}");
            Console.WriteLine($"      Progresso  : {CalcularProgresso():F1}%");
        }

        public override double CalcularProgresso()
        {
            if (Conquistas.Count == 0) return 0;
            int desbloqueadas = 0;
            foreach (Conquista c in Conquistas)
                if (c.Desbloqueada) desbloqueadas++;
            return (desbloqueadas / (double)Conquistas.Count) * 100.0;
        }
    }
}
