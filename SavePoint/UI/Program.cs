// ============================================================
//  Program.cs — Menu principal e ponto de entrada
// ============================================================
using System;
using SavePoint.Models;
using SavePoint.Services;

namespace SavePoint
{
    class Program
    {
        // ── Instâncias globais do sistema ───────────────────────────────────
        private static readonly Biblioteca    _biblioteca    = new Biblioteca();
        private static readonly GerenciadorJson _gerenciador = new GerenciadorJson();
        private static readonly Estatisticas  _estatisticas  = new Estatisticas(_biblioteca);
        private const  string                 ARQUIVO_JSON   = "biblioteca.json";

        static void Main(string[] args)
        {
            // Registra a biblioteca no gerenciador
            GerenciadorJson.DefinirBiblioteca(_biblioteca);

            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║          💾  SavePoint               ║");
            Console.WriteLine("║   Gerenciador de Jogos e Conquistas  ║");
            Console.WriteLine("╚══════════════════════════════════════╝");

            // Tenta carregar dados existentes ao iniciar
            Console.WriteLine("\n  Carregando dados salvos...");
            _gerenciador.Carregar(ARQUIVO_JSON);

            bool executando = true;
            while (executando)
            {
                ExibirMenuPrincipal();
                string opcao = Console.ReadLine()?.Trim() ?? "";

                switch (opcao)
                {
                    case "1": MenuAdicionarJogo();         break;
                    case "2": MenuListarJogos();           break;
                    case "3": MenuAtualizarStatus();       break;
                    case "4": MenuAdicionarHoras();        break;
                    case "5": MenuGerenciarConquistas();   break;
                    case "6": MenuRemoverJogo();           break;
                    case "7": _estatisticas.Exibir();      break;
                    case "8": _gerenciador.Salvar(ARQUIVO_JSON); break;
                    case "0":
                        Console.WriteLine("\n  Salvando antes de sair...");
                        _gerenciador.Salvar(ARQUIVO_JSON);
                        executando = false;
                        Console.WriteLine("\n  Até mais! 🎮");
                        break;
                    default:
                        Console.WriteLine("  ⚠  Opção inválida. Tente novamente.");
                        break;
                }

                if (executando)
                {
                    Console.WriteLine("\n  Pressione ENTER para continuar...");
                    Console.ReadLine();
                }
            }
        }

        // ── Menu principal ──────────────────────────────────────────────────
        static void ExibirMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══════════ MENU PRINCIPAL ═══════════");
            Console.WriteLine("  [1] Adicionar jogo");
            Console.WriteLine("  [2] Listar biblioteca");
            Console.WriteLine("  [3] Atualizar status de jogo");
            Console.WriteLine("  [4] Registrar horas jogadas");
            Console.WriteLine("  [5] Gerenciar conquistas");
            Console.WriteLine("  [6] Remover jogo");
            Console.WriteLine("  [7] Ver estatísticas");
            Console.WriteLine("  [8] Salvar dados");
            Console.WriteLine("  [0] Sair");
            Console.WriteLine("  ══════════════════════════════════════");
            Console.Write("  Opção: ");
        }

        // ── Adicionar jogo ──────────────────────────────────────────────────
        static void MenuAdicionarJogo()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ ADICIONAR JOGO ═══");
            Console.WriteLine("  [1] Single-Player");
            Console.WriteLine("  [2] Multiplayer");
            Console.Write("  Tipo: ");
            string tipo = Console.ReadLine()?.Trim() ?? "";

            try
            {
                Console.Write("  Título : "); string titulo = Console.ReadLine() ?? "";
                Console.Write("  Gênero : "); string genero = Console.ReadLine() ?? "";

                if (tipo == "1")
                {
                    Console.Write("  Dificuldade (Fácil/Médio/Difícil): ");
                    string dif = Console.ReadLine() ?? "Médio";

                    Console.Write("  Tempo estimado (horas): ");
                    double tempo = double.Parse(Console.ReadLine() ?? "0");

                    var jogo = new JogoSingle(_biblioteca.ProximoId(), titulo, genero, dif, tempo);
                    AdicionarConquistasIniciais(jogo);
                    _biblioteca.AdicionarJogo(jogo);
                    Console.WriteLine($"\n  ✅ '{titulo}' adicionado como Single-Player!");
                }
                else if (tipo == "2")
                {
                    Console.Write("  Máx. jogadores: ");
                    int max = int.Parse(Console.ReadLine() ?? "2");

                    Console.Write("  Modo competitivo? (s/n): ");
                    bool comp = (Console.ReadLine()?.Trim().ToLower() == "s");

                    var jogo = new JogoMultiplayer(_biblioteca.ProximoId(), titulo, genero, max, comp);
                    AdicionarConquistasIniciais(jogo);
                    _biblioteca.AdicionarJogo(jogo);
                    Console.WriteLine($"\n  ✅ '{titulo}' adicionado como Multiplayer!");
                }
                else
                {
                    Console.WriteLine("  ⚠  Tipo inválido.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("  ❌ Valor numérico inválido. Tente novamente.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ❌ Erro: {ex.Message}");
            }
        }

        // ── Listar jogos ────────────────────────────────────────────────────
        static void MenuListarJogos()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ BIBLIOTECA ═══\n");
            _biblioteca.ListarJogos();
        }

        // ── Atualizar status ─────────────────────────────────────────────────
        static void MenuAtualizarStatus()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ ATUALIZAR STATUS ═══");
            try
            {
                Console.Write("  Id do jogo: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                Jogo jogo = _biblioteca.BuscarPorId(id);

                Console.WriteLine("  Status disponíveis:");
                Console.WriteLine("  [1] Não Iniciado  [2] Em Andamento  [3] Concluído  [4] Platinado");
                Console.Write("  Opção: ");
                string opt = Console.ReadLine()?.Trim() ?? "";

                string[] statusMap = { "Não Iniciado", "Em Andamento", "Concluído", "Platinado" };
                if (int.TryParse(opt, out int idx) && idx >= 1 && idx <= 4)
                {
                    jogo.AtualizarStatus(statusMap[idx - 1]);
                    Console.WriteLine($"  ✅ Status de '{jogo.Titulo}' atualizado para '{jogo.Status}'.");
                }
                else
                {
                    Console.WriteLine("  ⚠  Opção inválida.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("  ❌ Id inválido.");
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
        }

        // ── Adicionar horas ──────────────────────────────────────────────────
        static void MenuAdicionarHoras()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ REGISTRAR HORAS ═══");
            try
            {
                Console.Write("  Id do jogo: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                Jogo jogo = _biblioteca.BuscarPorId(id);

                Console.Write($"  Quantas horas jogou em '{jogo.Titulo}'? ");
                double horas = double.Parse(Console.ReadLine() ?? "0");

                jogo.AdicionarHoras(horas);
                Console.WriteLine($"  ✅ {horas}h adicionadas. Total: {jogo.HorasJogadas}h.");
            }
            catch (FormatException)
            {
                Console.WriteLine("  ❌ Valor inválido.");
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
        }

        // ── Gerenciar conquistas ─────────────────────────────────────────────
        static void MenuGerenciarConquistas()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ CONQUISTAS ═══");
            try
            {
                Console.Write("  Id do jogo: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                Jogo jogo = _biblioteca.BuscarPorId(id);

                Console.WriteLine($"\n  Conquistas de '{jogo.Titulo}':");
                if (jogo.Conquistas.Count == 0)
                    Console.WriteLine("  Nenhuma conquista cadastrada.");
                else
                    foreach (Conquista c in jogo.Conquistas)
                        Console.WriteLine($"    • {c}");

                Console.WriteLine("\n  [1] Adicionar conquista  [2] Desbloquear conquista  [0] Voltar");
                Console.Write("  Opção: ");
                string opt = Console.ReadLine()?.Trim() ?? "";

                if (opt == "1")
                {
                    Console.Write("  Nome     : "); string nome = Console.ReadLine() ?? "";
                    Console.Write("  Descrição: "); string desc = Console.ReadLine() ?? "";
                    jogo.AdicionarConquista(new Conquista(nome, desc));
                    Console.WriteLine($"  ✅ Conquista '{nome}' adicionada.");
                }
                else if (opt == "2")
                {
                    Console.Write("  Nome da conquista: ");
                    string nome = Console.ReadLine() ?? "";
                    jogo.DesbloquearConquista(nome);
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("  ❌ Id inválido.");
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
        }

        // ── Remover jogo ─────────────────────────────────────────────────────
        static void MenuRemoverJogo()
        {
            Console.Clear();
            Console.WriteLine("\n  ═══ REMOVER JOGO ═══");
            try
            {
                Console.Write("  Id do jogo a remover: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                _biblioteca.RemoverJogo(id);
            }
            catch (FormatException)
            {
                Console.WriteLine("  ❌ Id inválido.");
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                Console.WriteLine($"  ❌ {ex.Message}");
            }
        }

        // ── Helper: adiciona conquistas iniciais ao criar jogo ───────────────
        static void AdicionarConquistasIniciais(Jogo jogo)
        {
            Console.Write("\n  Deseja adicionar conquistas agora? (s/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "s") return;

            bool continuar = true;
            while (continuar)
            {
                Console.Write("  Nome da conquista (ou ENTER para parar): ");
                string nome = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(nome)) break;

                Console.Write("  Descrição: ");
                string desc = Console.ReadLine() ?? "";
                jogo.AdicionarConquista(new Conquista(nome, desc));

                Console.Write("  Adicionar outra? (s/n): ");
                continuar = (Console.ReadLine()?.Trim().ToLower() == "s");
            }
        }
    }
}
