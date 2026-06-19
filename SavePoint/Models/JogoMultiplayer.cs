//  JogoMultiplayer.cs — Subclasse concreta (HERANÇA + POLIMORFISMO)

using System;

namespace SavePoint.Models
{
    public class JogoMultiplayer : Jogo
    {
        public int  MaxJogadores     { get; private set; }
        public bool ModoCompetitivo  { get; private set; }

        public JogoMultiplayer(int id, string titulo, string genero,
                               int maxJogadores, bool modoCompetitivo)
            : base(id, titulo, genero)
        {
            if (maxJogadores < 2)
                throw new ArgumentException("Um jogo multiplayer precisa de ao menos 2 jogadores.");

            MaxJogadores    = maxJogadores;
            ModoCompetitivo = modoCompetitivo;
        }

        public override void ExibirDetalhes()
        {
            Console.WriteLine($"  [{Id}] {Titulo} ({Genero})");
            Console.WriteLine($"      Tipo           : Multiplayer");
            Console.WriteLine($"      Max. Jogadores : {MaxJogadores}");
            Console.WriteLine($"      Competitivo    : {(ModoCompetitivo ? "Sim" : "Não")}");
            Console.WriteLine($"      Jogadas        : {HorasJogadas}h");
            Console.WriteLine($"      Status         : {Status}");
            Console.WriteLine($"      Progresso      : {CalcularProgresso():F1}%");
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
