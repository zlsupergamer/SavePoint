//  Biblioteca.cs — Gerenciador da coleção de jogos

using System;
using System.Collections.Generic;
using SavePoint.Models;

namespace SavePoint.Services
{
    public class Biblioteca
    {
        private readonly List<Jogo> _jogos = new List<Jogo>();
        private int _proximoId = 1;


        public void AdicionarJogo(Jogo jogo)
        {
            if (jogo == null)
                throw new ArgumentNullException(nameof(jogo), "Jogo não pode ser nulo.");
            _jogos.Add(jogo);
            _proximoId++;
        }

        public void RemoverJogo(int id)
        {
            Jogo jogo = BuscarPorId(id);   
            _jogos.Remove(jogo);
            Console.WriteLine($"  ✅ '{jogo.Titulo}' removido da biblioteca.");
        }

        public Jogo BuscarPorId(int id)
        {
            foreach (Jogo j in _jogos)
                if (j.Id == id) return j;
            throw new KeyNotFoundException($"Jogo com Id {id} não encontrado.");
        }

        public void ListarJogos()
        {
            if (_jogos.Count == 0)
            {
                Console.WriteLine("  Biblioteca vazia.");
                return;
            }
            foreach (Jogo j in _jogos)
            {
                j.ExibirDetalhes();   
                Console.WriteLine();
            }
        }

        public int ProximoId() => _proximoId;

        public List<Jogo> ObterTodos() => _jogos;
    }
}
