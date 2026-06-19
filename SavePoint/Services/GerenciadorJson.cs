//  GerenciadorJson.cs — Persistência em JSON (try/catch/finally)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SavePoint.Models;
using SavePoint.Services;

namespace SavePoint.Services
{
    public class GerenciadorJson : IPersistivel
    {
        private readonly string _caminhoPadrao;

        public GerenciadorJson(string caminhoPadrao = "biblioteca.json")
        {
            _caminhoPadrao = caminhoPadrao;
        }

        public void Salvar(string caminho)
        {
            try
            {
                List<Jogo> jogos = ObterBibliotecaAtual();

                var lista = new List<object>();
                foreach (Jogo j in jogos)
                {
                    if (j is JogoSingle js)
                    {
                        lista.Add(new {
                            tipo              = "single",
                            id                = js.Id,
                            titulo            = js.Titulo,
                            genero            = js.Genero,
                            status            = js.Status,
                            horasJogadas      = js.HorasJogadas,
                            dificuldade       = js.Dificuldade,
                            tempoEstimado     = js.TempoEstimadoHoras,
                            conquistas        = SerializarConquistas(js)
                        });
                    }
                    else if (j is JogoMultiplayer jm)
                    {
                        lista.Add(new {
                            tipo            = "multiplayer",
                            id              = jm.Id,
                            titulo          = jm.Titulo,
                            genero          = jm.Genero,
                            status          = jm.Status,
                            horasJogadas    = jm.HorasJogadas,
                            maxJogadores    = jm.MaxJogadores,
                            modoCompetitivo = jm.ModoCompetitivo,
                            conquistas      = SerializarConquistas(jm)
                        });
                    }
                }

                var opcoes  = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(lista, opcoes);
                File.WriteAllText(caminho, json, Encoding.UTF8);

                Console.WriteLine($"  ✅ Dados salvos em '{caminho}'.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("  ❌ Erro: sem permissão para gravar o arquivo.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"  ❌ Erro de I/O ao salvar: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Erro inesperado ao salvar: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("  💾 Operação de salvamento concluída.");
            }
        }

        public void Carregar(string caminho)
        {
            try
            {
                if (!File.Exists(caminho))
                    throw new FileNotFoundException($"Arquivo '{caminho}' não encontrado.");

                string json = File.ReadAllText(caminho, Encoding.UTF8);
                JsonArray array = JsonNode.Parse(json)!.AsArray();

                Biblioteca bib = ObterBibliotecaInstancia();

                foreach (JsonNode? node in array)
                {
                    if (node == null) continue;
                    string tipo = node["tipo"]!.GetValue<string>();

                    Jogo jogo;
                    if (tipo == "single")
                    {
                        jogo = new JogoSingle(
                            node["id"]!.GetValue<int>(),
                            node["titulo"]!.GetValue<string>(),
                            node["genero"]!.GetValue<string>(),
                            node["dificuldade"]!.GetValue<string>(),
                            node["tempoEstimado"]!.GetValue<double>()
                        );
                    }
                    else if (tipo == "multiplayer")
                    {
                        jogo = new JogoMultiplayer(
                            node["id"]!.GetValue<int>(),
                            node["titulo"]!.GetValue<string>(),
                            node["genero"]!.GetValue<string>(),
                            node["maxJogadores"]!.GetValue<int>(),
                            node["modoCompetitivo"]!.GetValue<bool>()
                        );
                    }
                    else continue;

                    RestaurarCampo(jogo, "HorasJogadas",
                        node["horasJogadas"]?.GetValue<double>() ?? 0);
                    jogo.AtualizarStatus(node["status"]!.GetValue<string>());

                    if (node["conquistas"] is JsonArray conquistas)
                        foreach (JsonNode? cNode in conquistas)
                        {
                            if (cNode == null) continue;
                            var c = new Conquista(
                                cNode["nome"]!.GetValue<string>(),
                                cNode["descricao"]!.GetValue<string>()
                            );
                            if (cNode["desbloqueada"]!.GetValue<bool>())
                                c.Desbloquear();
                            jogo.AdicionarConquista(c);
                        }

                    bib.AdicionarJogo(jogo);
                }

                Console.WriteLine($"  ✅ Dados carregados de '{caminho}'.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"  ⚠  {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"  ❌ Erro ao interpretar o JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Erro inesperado ao carregar: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("  📂 Operação de carregamento concluída.");
            }
        }


        private static List<object> SerializarConquistas(Jogo jogo)
        {
            var lista = new List<object>();
            foreach (Conquista c in jogo.Conquistas)
                lista.Add(new {
                    nome          = c.Nome,
                    descricao     = c.Descricao,
                    desbloqueada  = c.Desbloqueada,
                    dataDesblob   = c.DataDesbloqueio?.ToString("o")
                });
            return lista;
        }

        private static void RestaurarCampo(Jogo jogo, string campo, object valor)
        {
            var prop = typeof(Jogo).GetProperty(campo,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            prop?.SetValue(jogo, Convert.ChangeType(valor, prop.PropertyType));
        }

        private static Biblioteca _bibliotecaRef;
        public  static void        DefinirBiblioteca(Biblioteca bib) => _bibliotecaRef = bib;
        private static Biblioteca  ObterBibliotecaInstancia()        => _bibliotecaRef;
        private static List<Jogo>  ObterBibliotecaAtual()            => _bibliotecaRef.ObterTodos();

        void IPersistivel.Salvar(string caminho)   => Salvar(caminho);
        void IPersistivel.Carregar(string caminho) => Carregar(caminho);
    }
}
