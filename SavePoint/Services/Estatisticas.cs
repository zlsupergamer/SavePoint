//  Estatisticas.cs — Relatório agregado da biblioteca

using System;
using System.Collections.Generic;
using SavePoint.Models;

namespace SavePoint.Services
{
    public class Estatisticas
    {
        private readonly Biblioteca _biblioteca;

        public Estatisticas(Biblioteca biblioteca)
        {
            _biblioteca = biblioteca ?? throw new ArgumentNullException(nameof(biblioteca));
        }

        public void Exibir()
        {
            List<Jogo> jogos = _biblioteca.ObterTodos();

            if (jogos.Count == 0)
            {
                Console.WriteLine("  Nenhum jogo na biblioteca para gerar estatísticas.");
                return;
            }

            double totalHoras         = 0;
            int    totalConquistas    = 0;
            int    conquistasDesbloq  = 0;
            int    naoIniciados       = 0;
            int    emAndamento        = 0;
            int    concluidos         = 0;
            int    platinados         = 0;

            foreach (Jogo j in jogos)
            {
                totalHoras += j.HorasJogadas;

                foreach (Conquista c in j.Conquistas)
                {
                    totalConquistas++;
                    if (c.Desbloqueada) conquistasDesbloq++;
                }

                switch (j.Status)
                {
                    case "Não Iniciado": naoIniciados++; break;
                    case "Em Andamento": emAndamento++;  break;
                    case "Concluído":    concluidos++;   break;
                    case "Platinado":    platinados++;   break;
                }
            }

            double pctConquistas = totalConquistas > 0
                ? (conquistasDesbloq / (double)totalConquistas) * 100.0
                : 0;

            Console.WriteLine("  ════════════════ ESTATÍSTICAS ════════════════");
            Console.WriteLine($"  Total de jogos       : {jogos.Count}");
            Console.WriteLine($"  Total de horas       : {totalHoras:F1}h");
            Console.WriteLine($"  Conquistas           : {conquistasDesbloq}/{totalConquistas} ({pctConquistas:F1}%)");
            Console.WriteLine($"  Não Iniciados        : {naoIniciados}");
            Console.WriteLine($"  Em Andamento         : {emAndamento}");
            Console.WriteLine($"  Concluídos           : {concluidos}");
            Console.WriteLine($"  Platinados           : {platinados}");
            Console.WriteLine("  ═══════════════════════════════════════════════");
        }
    }
}
