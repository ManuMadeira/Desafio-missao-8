using System;

namespace ProdutoApp
{
    // Exceção personalizada para regras de domínio
    public class DomainException : Exception
    {
        public DomainException() { }
        
        public DomainException(string message) : base(message) { }
        
        public DomainException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    public class Produto
    {
        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public int Estoque { get; private set; }

        public Produto(string nome, decimal preco, int estoqueInicial)
        {
            ValidarParametros(nome, preco, estoqueInicial);
            
            Nome = nome;
            Preco = preco;
            Estoque = estoqueInicial;
        }

        private void ValidarParametros(string nome, decimal preco, int estoque)
        {
            // Validação para ArgumentOutOfRangeException
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser vazio ou nulo", nameof(nome));
            
            if (preco < 0)
                throw new ArgumentOutOfRangeException(nameof(preco), "Preço não pode ser negativo");
            
            if (estoque < 0)
                throw new ArgumentOutOfRangeException(nameof(estoque), "Estoque inicial não pode ser negativo");
        }

        public void AdicionarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantidade), 
                    "Quantidade para adicionar deve ser maior que zero"
                );

            Estoque += quantidade;
        }

        public void RemoverEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantidade), 
                    "Quantidade para remover deve ser maior que zero"
                );

            // Validação de regra de domínio - DomainException
            if (quantidade > Estoque)
                throw new DomainException(
                    $"Estoque insuficiente. Estoque atual: {Estoque}, tentativa de remover: {quantidade}"
                );

            Estoque -= quantidade;
        }

        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(novoPreco), 
                    "Preço não pode ser negativo"
                );

            Preco = novoPreco;
        }

        public override string ToString()
        {
            return $"Produto: {Nome}, Preço: {Preco:C}, Estoque: {Estoque}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Teste com valores válidos
                var produto = new Produto("Notebook", 2500.00m, 10);
                Console.WriteLine(produto);

                // Adicionar estoque
                produto.AdicionarEstoque(5);
                Console.WriteLine($"Após adicionar estoque: {produto}");

                // Remover estoque
                produto.RemoverEstoque(8);
                Console.WriteLine($"Após remover estoque: {produto}");

                // Teste com estoque insuficiente (DomainException)
                Console.WriteLine("\n--- Testando DomainException ---");
                produto.RemoverEstoque(20); // Isso lançará DomainException

            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Erro de argumento: {ex.Message}");
            }
            catch (DomainException ex)
            {
                Console.WriteLine($"Erro de domínio: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }

            Console.WriteLine("\n--- Testando ArgumentOutOfRangeException ---");
            
            try
            {
                // Teste com preço negativo
                var produtoInvalido = new Produto("Tablet", -100.00m, 5);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Erro ao criar produto: {ex.Message}");
            }

            try
            {
                // Teste com estoque negativo
                var produtoInvalido = new Produto("Mouse", 50.00m, -2);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Erro ao criar produto: {ex.Message}");
            }
        }
    }
}