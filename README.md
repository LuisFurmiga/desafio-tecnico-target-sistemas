# Desafio Técnico — Target Sistemas

Projeto desenvolvido em **C# / .NET 8** para resolução do desafio técnico proposto pela **Target Sistemas**.

A aplicação é executada em modo console e reúne os três exercícios em um único programa, com um menu principal para selecionar a funcionalidade desejada.

## Funcionalidades

### 1. Cálculo de comissão de vendedores

O programa lê os registros de vendas fornecidos em JSON, agrupa os dados por vendedor e calcula a comissão de cada venda de acordo com as regras do desafio.

Regras utilizadas:

- Vendas abaixo de **R$ 100,00**: não geram comissão.
- Vendas a partir de **R$ 100,00** e abaixo de **R$ 500,00**: comissão de **1%**.
- Vendas a partir de **R$ 500,00**: comissão de **5%**.

Ao final, são apresentados para cada vendedor:

- quantidade de vendas;
- valor total vendido;
- comissão total calculada.

---

### 2. Movimentação de estoque

Permite realizar movimentações de **entrada** e **saída** nos produtos definidos no JSON inicial.

Cada movimentação possui:

- identificador numérico único;
- código do produto;
- descrição da movimentação;
- tipo da movimentação;
- quantidade movimentada;
- estoque final do produto.

Também são realizadas validações para:

- código de produto inexistente;
- tipo de movimentação inválido;
- quantidade inválida;
- tentativa de saída maior que o estoque disponível;
- descrição vazia.

Após cada operação, o programa informa o saldo atualizado do produto.

O estoque inicial é carregado uma única vez durante a execução da aplicação. Dessa forma, ao retornar ao menu principal e acessar novamente a opção de estoque, as movimentações realizadas anteriormente continuam refletidas no saldo atual.

> A persistência é mantida somente enquanto o programa estiver em execução. Ao encerrar e iniciar novamente a aplicação, o estoque volta aos valores originais definidos no JSON, já que o desafio não solicita persistência em banco de dados ou arquivo externo.

---

### 3. Cálculo de juros

O programa recebe:

- valor inicial;
- data de vencimento.

A taxa utilizada é de **2,5% ao dia**.

Primeiro, a aplicação verifica se existe atraso em relação à data atual. Caso o vencimento ainda não tenha ocorrido, o programa informa que não há juros e mantém o valor original.

Quando existe atraso, são disponibilizadas duas opções de cálculo:

1. **Juros simples — interpretação utilizada para atender ao enunciado**
2. **Juros compostos — simulação adicional**

#### Juros simples

O cálculo utilizado é:

```text
juros = valor × taxa diária × dias em atraso
```

O valor total é calculado por:

```text
valor total = valor original + juros
```

#### Juros compostos

Como funcionalidade adicional, a aplicação também permite simular juros compostos.

Nesse caso, a taxa de 2,5% é aplicada diariamente sobre o saldo acumulado.

> O cálculo de juros simples é a opção adotada como referência para o enunciado original. A opção de juros compostos foi adicionada apenas como recurso complementar de simulação.

---

## Estrutura do programa

O projeto foi mantido propositalmente simples, utilizando uma aplicação de console e separando cada exercício em um método específico:

```text
Program.cs
│
├── Main()
│   └── Menu principal
│
├── ExecutarParte1()
│   └── Comissão de vendedores
│
├── ExecutarParte2()
│   └── Movimentação de estoque
│
├── CarregarEstoqueInicial()
│   └── Carregamento do estoque inicial
│
└── ExecutarParte3()
    └── Cálculo de juros
```

Também são utilizadas classes para representar:

- vendas;
- produtos;
- estoque;
- movimentações.

---

## Tecnologias utilizadas

- **C#**
- **.NET 8**
- `System.Text.Json`
- LINQ
- `CultureInfo`
- Aplicação Console

---

## Pré-requisitos

É necessário possuir o **.NET 8 SDK** instalado.

Para verificar a versão instalada:

```bash
dotnet --version
```

Caso o comando retorne uma versão `8.x` ou compatível com o target `net8.0`, o ambiente está preparado.

---

## Como executar

Clone o repositório:

```bash
git clone https://github.com/LuisFurmiga/desafio-tecnico-target-sistemas.git
```

Entre na pasta do projeto:

```bash
cd desafio-tecnico-target-sistemas
```

Execute:

```bash
dotnet run
```

O menu principal será exibido:

```text
==========================================
       DESAFIO TÉCNICO - TARGET
==========================================
1 - Parte 1: Comissão de vendedores
2 - Parte 2: Movimentação de estoque
3 - Parte 3: Cálculo de juros
0 - Sair
==========================================
Escolha uma opção:
```

Digite o número correspondente à funcionalidade desejada e pressione `Enter`.

---

## Decisões de implementação

### `decimal` para valores monetários

Os valores financeiros são representados com `decimal`, evitando problemas de precisão comuns em cálculos monetários realizados com tipos de ponto flutuante.

### Desserialização de JSON

Os dados fornecidos no desafio são mantidos em formato JSON e desserializados utilizando `System.Text.Json`.

### LINQ no cálculo das vendas

LINQ é utilizado para agrupar as vendas por vendedor e calcular:

- quantidade de vendas;
- valor total vendido;
- comissão acumulada.

### Estado do estoque durante a execução

O estoque é carregado uma única vez quando a aplicação é iniciada e permanece em memória durante toda a execução.

Isso garante que as movimentações realizadas continuem refletidas no saldo mesmo que o usuário volte ao menu principal e acesse novamente a funcionalidade de estoque.

### Validação das entradas

As entradas informadas pelo usuário são validadas antes das operações, evitando situações como:

- valores inválidos;
- códigos de produtos inexistentes;
- quantidades menores ou iguais a zero;
- retirada superior ao estoque disponível;
- descrições vazias;
- datas em formato inválido;
- opções de menu inválidas.

### Cultura brasileira

A aplicação utiliza `pt-BR` para formatação monetária e tratamento dos valores apresentados no console.

### Juros simples e compostos

O cálculo de **juros simples** é utilizado como interpretação principal do enunciado, considerando a taxa de 2,5% ao dia.

A opção de **juros compostos** foi adicionada como funcionalidade complementar para permitir a comparação entre as duas formas de cálculo.

---

## Observações

- O projeto foi desenvolvido como aplicação de console para manter a solução objetiva e adequada ao escopo do desafio.
- Não foi utilizada persistência em banco de dados ou arquivo externo, pois isso não foi solicitado no enunciado.
- As regras foram mantidas separadas em métodos para facilitar leitura, manutenção e testes.

---

## Autor

**Luís Fernando da Silva Corrêa**

- GitHub: [LuisFurmiga](https://github.com/LuisFurmiga)
