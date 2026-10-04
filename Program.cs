using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesafioTarget
{
    // ============================================================
    // MODELOS
    // ============================================================

    public class Venda
    {
        [JsonPropertyName("vendedor")]
        public string Vendedor { get; set; } = string.Empty;

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }

    public class VendasJson
    {
        [JsonPropertyName("vendas")]
        public List<Venda> Vendas { get; set; } = new();
    }

    public class Produto
    {
        [JsonPropertyName("codigoProduto")]
        public int CodigoProduto { get; set; }

        [JsonPropertyName("descricaoProduto")]
        public string DescricaoProduto { get; set; } = string.Empty;

        [JsonPropertyName("estoque")]
        public int Estoque { get; set; }
    }

    public class EstoqueJson
    {
        [JsonPropertyName("estoque")]
        public List<Produto> Produtos { get; set; } = new();
    }

    public class Movimentacao
    {
        public int Id { get; set; }
        public int CodigoProduto { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public int EstoqueFinal { get; set; }
    }

    internal class Program
    {
        private static readonly List<Movimentacao> Movimentacoes = new();
        private static int proximoIdMovimentacao = 1;

        private static readonly EstoqueJson EstoqueAtual = CarregarEstoqueInicial();

        static void Main()
        {
            CultureInfo culturaBrasileira = new("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culturaBrasileira;
            CultureInfo.DefaultThreadCurrentUICulture = culturaBrasileira;

            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("       DESAFIO TÉCNICO - TARGET");
                Console.WriteLine("==========================================");
                Console.WriteLine("1 - Parte 1: Comissão de vendedores");
                Console.WriteLine("2 - Parte 2: Movimentação de estoque");
                Console.WriteLine("3 - Parte 3: Cálculo de juros");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("==========================================");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine() ?? string.Empty;

                Console.Clear();

                switch (opcao)
                {
                    case "1":
                        ExecutarParte1();
                        break;

                    case "2":
                        ExecutarParte2();
                        break;

                    case "3":
                        ExecutarParte3();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Pressione ENTER para voltar ao menu.");
                Console.ReadLine();
            }
        }

        // ============================================================
        // PARTE 1 - COMISSÃO DE VENDEDORES
        //
        // Regras aplicadas individualmente a cada venda:
        // - Venda abaixo de R$ 100,00: 0% de comissão.
        // - Venda de R$ 100,00 até abaixo de R$ 500,00: 1%.
        // - Venda a partir de R$ 500,00: 5%.
        // ============================================================

        private static void ExecutarParte1()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("PARTE 1 - COMISSÃO DE VENDEDORES");
            Console.WriteLine("==========================================");

            string json = """
            {
              "vendas": [
                { "vendedor": "João Silva", "valor": 1200.50 },
                { "vendedor": "João Silva", "valor": 950.75 },
                { "vendedor": "João Silva", "valor": 1800.00 },
                { "vendedor": "João Silva", "valor": 1400.30 },
                { "vendedor": "João Silva", "valor": 1100.90 },
                { "vendedor": "João Silva", "valor": 1550.00 },
                { "vendedor": "João Silva", "valor": 1700.80 },
                { "vendedor": "João Silva", "valor": 250.30 },
                { "vendedor": "João Silva", "valor": 480.75 },
                { "vendedor": "João Silva", "valor": 320.40 },

                { "vendedor": "Maria Souza", "valor": 2100.40 },
                { "vendedor": "Maria Souza", "valor": 1350.60 },
                { "vendedor": "Maria Souza", "valor": 950.20 },
                { "vendedor": "Maria Souza", "valor": 1600.75 },
                { "vendedor": "Maria Souza", "valor": 1750.00 },
                { "vendedor": "Maria Souza", "valor": 1450.90 },
                { "vendedor": "Maria Souza", "valor": 400.50 },
                { "vendedor": "Maria Souza", "valor": 180.20 },
                { "vendedor": "Maria Souza", "valor": 90.75 },

                { "vendedor": "Carlos Oliveira", "valor": 800.50 },
                { "vendedor": "Carlos Oliveira", "valor": 1200.00 },
                { "vendedor": "Carlos Oliveira", "valor": 1950.30 },
                { "vendedor": "Carlos Oliveira", "valor": 1750.80 },
                { "vendedor": "Carlos Oliveira", "valor": 1300.60 },
                { "vendedor": "Carlos Oliveira", "valor": 300.40 },
                { "vendedor": "Carlos Oliveira", "valor": 500.00 },
                { "vendedor": "Carlos Oliveira", "valor": 125.75 },

                { "vendedor": "Ana Lima", "valor": 1000.00 },
                { "vendedor": "Ana Lima", "valor": 1100.50 },
                { "vendedor": "Ana Lima", "valor": 1250.75 },
                { "vendedor": "Ana Lima", "valor": 1400.20 },
                { "vendedor": "Ana Lima", "valor": 1550.90 },
                { "vendedor": "Ana Lima", "valor": 1650.00 },
                { "vendedor": "Ana Lima", "valor": 75.30 },
                { "vendedor": "Ana Lima", "valor": 420.90 },
                { "vendedor": "Ana Lima", "valor": 315.40 }
              ]
            }
            """;

            VendasJson? dados = JsonSerializer.Deserialize<VendasJson>(json);

            if (dados == null || dados.Vendas.Count == 0)
            {
                Console.WriteLine("Não foi possível carregar os dados de vendas.");
                return;
            }

            var resultadoPorVendedor = dados.Vendas
                .GroupBy(venda => venda.Vendedor)
                .Select(grupo => new
                {
                    Vendedor = grupo.Key,
                    QuantidadeVendas = grupo.Count(),
                    TotalVendido = grupo.Sum(venda => venda.Valor),
                    ComissaoTotal = grupo.Sum(venda => CalcularComissao(venda.Valor))
                })
                .OrderBy(resultado => resultado.Vendedor);

            foreach (var resultado in resultadoPorVendedor)
            {
                Console.WriteLine();
                Console.WriteLine($"Vendedor: {resultado.Vendedor}");
                Console.WriteLine($"Quantidade de vendas: {resultado.QuantidadeVendas}");
                Console.WriteLine($"Total vendido: {resultado.TotalVendido:C2}");
                Console.WriteLine($"Comissão total: {resultado.ComissaoTotal:C2}");
                Console.WriteLine("------------------------------------------");
            }
        }

        private static decimal CalcularComissao(decimal valorVenda)
        {
            if (valorVenda < 100m)
            {
                return 0m;
            }

            if (valorVenda < 500m)
            {
                return valorVenda * 0.01m;
            }

            return valorVenda * 0.05m;
        }

        // ============================================================
        // PARTE 2 - MOVIMENTAÇÃO DE ESTOQUE
        // ============================================================

        private static EstoqueJson CarregarEstoqueInicial()
        {
            string json = """
            {
            "estoque": [
                {
                "codigoProduto": 101,
                "descricaoProduto": "Caneta Azul",
                "estoque": 150
                },
                {
                "codigoProduto": 102,
                "descricaoProduto": "Caderno Universitário",
                "estoque": 75
                },
                {
                "codigoProduto": 103,
                "descricaoProduto": "Borracha Branca",
                "estoque": 200
                },
                {
                "codigoProduto": 104,
                "descricaoProduto": "Lápis Preto HB",
                "estoque": 320
                },
                {
                "codigoProduto": 105,
                "descricaoProduto": "Marcador de Texto Amarelo",
                "estoque": 90
                }
            ]
            }
            """;

            return JsonSerializer.Deserialize<EstoqueJson>(json)
                ?? new EstoqueJson();
        }

        private static void ExecutarParte2()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("PARTE 2 - MOVIMENTAÇÃO DE ESTOQUE");
            Console.WriteLine("==========================================");

            if (EstoqueAtual.Produtos.Count == 0)
            {
                Console.WriteLine("Erro ao carregar o estoque.");
                return;
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Produtos disponíveis:");
                Console.WriteLine("------------------------------------------");

                foreach (Produto produto in EstoqueAtual.Produtos)
                {
                    Console.WriteLine(
                        $"{produto.CodigoProduto} - {produto.DescricaoProduto} - Estoque: {produto.Estoque}"
                    );
                }

                Console.WriteLine("------------------------------------------");
                Console.Write("Código do produto (0 para encerrar): ");

                if (!int.TryParse(Console.ReadLine(), out int codigo))
                {
                    Console.WriteLine("Código inválido.");
                    continue;
                }

                if (codigo == 0)
                {
                    break;
                }

                Produto? produtoSelecionado =
                    EstoqueAtual.Produtos.FirstOrDefault(
                        produto => produto.CodigoProduto == codigo
                    );

                if (produtoSelecionado == null)
                {
                    Console.WriteLine("Produto não encontrado.");
                    continue;
                }

                Console.Write("Tipo da movimentação (E = Entrada / S = Saída): ");
                string tipo = (Console.ReadLine() ?? string.Empty)
                    .Trim()
                    .ToUpperInvariant();

                if (tipo != "E" && tipo != "S")
                {
                    Console.WriteLine("Tipo de movimentação inválido.");
                    continue;
                }

                Console.Write("Quantidade: ");

                if (!int.TryParse(Console.ReadLine(), out int quantidade)
                    || quantidade <= 0)
                {
                    Console.WriteLine("Quantidade inválida.");
                    continue;
                }

                if (tipo == "S" && quantidade > produtoSelecionado.Estoque)
                {
                    Console.WriteLine(
                        $"Estoque insuficiente. Estoque atual: {produtoSelecionado.Estoque}"
                    );

                    continue;
                }

                Console.Write("Descrição da movimentação: ");
                string descricao = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(descricao))
                {
                    Console.WriteLine("A descrição da movimentação é obrigatória.");
                    continue;
                }

                if (tipo == "E")
                {
                    produtoSelecionado.Estoque += quantidade;
                }
                else
                {
                    produtoSelecionado.Estoque -= quantidade;
                }

                Movimentacao movimentacao = new()
                {
                    Id = proximoIdMovimentacao++,
                    CodigoProduto = produtoSelecionado.CodigoProduto,
                    Descricao = descricao,
                    Tipo = tipo == "E" ? "Entrada" : "Saída",
                    Quantidade = quantidade,
                    EstoqueFinal = produtoSelecionado.Estoque
                };

                Movimentacoes.Add(movimentacao);

                Console.WriteLine();
                Console.WriteLine("Movimentação realizada com sucesso.");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine($"ID da movimentação: {movimentacao.Id}");
                Console.WriteLine($"Produto: {produtoSelecionado.DescricaoProduto}");
                Console.WriteLine($"Tipo: {movimentacao.Tipo}");
                Console.WriteLine($"Descrição: {movimentacao.Descricao}");
                Console.WriteLine($"Quantidade movimentada: {movimentacao.Quantidade}");
                Console.WriteLine($"Estoque final: {movimentacao.EstoqueFinal}");
                Console.WriteLine("------------------------------------------");
            }
        }

        // ============================================================
        // PARTE 3 - CÁLCULO DE JUROS
        //
        // Considerando 2,5% ao dia:
        // juros = valor x 0,025 x dias em atraso
        // ============================================================

        private static void ExecutarParte3()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("PARTE 3 - CÁLCULO DE JUROS");
            Console.WriteLine("==========================================");

            decimal valor;

            while (true)
            {
                Console.Write("Informe o valor: R$ ");
                string entradaValor = (Console.ReadLine() ?? string.Empty).Trim();

                if (decimal.TryParse(
                        entradaValor,
                        NumberStyles.Number,
                        new CultureInfo("pt-BR"),
                        out valor)
                    && valor > 0)
                {
                    break;
                }

                Console.WriteLine("Valor inválido. Exemplo válido: 1500,50");
            }

            DateTime vencimento;

            while (true)
            {
                Console.Write("Informe a data de vencimento (dd/MM/yyyy): ");
                string entradaData = Console.ReadLine() ?? string.Empty;

                if (DateTime.TryParseExact(
                        entradaData,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out vencimento))
                {
                    break;
                }

                Console.WriteLine("Data inválida.");
            }

            DateTime hoje = DateTime.Today;
            int diasAtraso = (hoje - vencimento.Date).Days;

            if (diasAtraso <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("O título não possui atraso.");
                Console.WriteLine($"Data de vencimento: {vencimento:dd/MM/yyyy}");
                Console.WriteLine("Dias em atraso: 0");
                Console.WriteLine("Juros: R$ 0,00");
                Console.WriteLine($"Valor total: {valor:C2}");
                return;
            }

            // Taxa definida no enunciado: 2,5% ao dia.
            const decimal taxaDiaria = 0.025m;

            int tipoJuros;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Escolha o tipo de cálculo:");
                Console.WriteLine("1 - Juros simples (Enunciado)");
                Console.WriteLine("2 - Juros compostos (Simulação adicional)");
                Console.Write("Opção: ");

                if (int.TryParse(Console.ReadLine(), out tipoJuros)
                    && (tipoJuros == 1 || tipoJuros == 2))
                {
                    break;
                }

                Console.WriteLine("Opção inválida. Digite 1 ou 2.");
            }

            decimal jurosCalculados;
            decimal valorTotal;
            string descricaoTipoJuros;

            if (tipoJuros == 1)
            {
                // Juros simples:
                // juros = valor × taxa × dias em atraso
                jurosCalculados = valor * taxaDiaria * diasAtraso;
                valorTotal = valor + jurosCalculados;

                descricaoTipoJuros = "Simples";
            }
            else
            {
                // Juros compostos - funcionalidade adicional.
                // A taxa é aplicada sobre o saldo acumulado a cada dia.
                valorTotal = valor;

                for (int dia = 0; dia < diasAtraso; dia++)
                {
                    valorTotal += valorTotal * taxaDiaria;
                }

                jurosCalculados = valorTotal - valor;

                descricaoTipoJuros = "Compostos";
            }

            Console.WriteLine();
            Console.WriteLine("Resultado:");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Tipo de juros: {descricaoTipoJuros}");
            Console.WriteLine($"Data de hoje: {hoje:dd/MM/yyyy}");
            Console.WriteLine($"Data de vencimento: {vencimento:dd/MM/yyyy}");
            Console.WriteLine($"Valor original: {valor:C2}");
            Console.WriteLine($"Dias em atraso: {diasAtraso}");
            Console.WriteLine($"Taxa diária: {taxaDiaria:P1}");
            Console.WriteLine($"Juros: {jurosCalculados:C2}");
            Console.WriteLine($"Valor total: {valorTotal:C2}");
            Console.WriteLine("------------------------------------------");
        }
    }
}
