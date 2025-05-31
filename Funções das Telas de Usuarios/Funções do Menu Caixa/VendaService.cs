using MySql.Data.MySqlClient;

public static class VendaService
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void RealizarVenda(int atendenteId)
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRAR VENDA ===");

        var (clienteId, nomeCliente, cpfCliente) = SelecionarCliente();
        if (clienteId == 0) return;

        string codigoVenda = "VEN-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var produtosVenda = new List<ItemVenda>();
        decimal valorTotal = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Venda: {codigoVenda}");
            Console.WriteLine($"Cliente: {nomeCliente} (CPF: {cpfCliente})");
            Console.WriteLine($"Valor Total: R$ {valorTotal:F2}\n");

            Console.WriteLine("1. Adicionar Produto");
            Console.WriteLine("2. Remover Produto");
            Console.WriteLine("3. Finalizar Venda");
            Console.WriteLine("4. Cancelar Venda");
            Console.WriteLine("5. Consultar Produto");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1": // Adicionar produto
                    var item = AdicionarProduto();
                    if (item != null)
                    {
                        produtosVenda.Add(item);
                        valorTotal += item.ValorTotal;
                    }
                    break;

                case "2": // Remover produto
                    if (produtosVenda.Count > 0)
                    {
                        RemoverProduto(produtosVenda, ref valorTotal);
                    }
                    else
                    {
                        Console.WriteLine("Nenhum produto para remover!");
                        Console.ReadKey();
                    }
                    break;

                case "3": // Finalizar venda
                    if (produtosVenda.Count == 0)
                    {
                        Console.WriteLine("Adicione pelo menos um produto!");
                        Console.ReadKey();
                        continue;
                    }
                    FinalizarVenda(codigoVenda, clienteId, atendenteId, produtosVenda, valorTotal);
                    return;

                case "4": // Cancelar venda
                    if (AutenticarSupervisor())
                    {
                        RegistrarVendaCancelada(codigoVenda, clienteId, atendenteId, produtosVenda, valorTotal, cpfCliente);
                        Console.WriteLine("Venda cancelada com sucesso!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Autenticação falhou. Operação cancelada.");
                        Console.ReadKey();
                        continue;
                    }
                    return;
                case"5":
                    ConsultarProduto.Consultar();
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    Console.ReadKey();
                    break;
            }
        }
    }
    private static void RegistrarVendaCancelada(string codigoVenda, int clienteId, int atendenteId,
        List<ItemVenda> produtos, decimal valorTotal, string cpfCliente)
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();

                // 1. Registrar cabeçalho da venda cancelada
                var comandoVenda = new MySqlCommand(
                    "INSERT INTO vendas_canceladas (codigo_venda, cliente_id, cpf_cliente, atendente_id, valor_total, data_cancelamento) " +
                    "VALUES (@codigo, @cliente, @cpf, @atendente, @valor, NOW())",
                    conexao);

                comandoVenda.Parameters.AddWithValue("@codigo", codigoVenda);
                comandoVenda.Parameters.AddWithValue("@cliente", clienteId);
                comandoVenda.Parameters.AddWithValue("@cpf", cpfCliente);
                comandoVenda.Parameters.AddWithValue("@atendente", atendenteId);
                comandoVenda.Parameters.AddWithValue("@valor", valorTotal);

                comandoVenda.ExecuteNonQuery();

                // 2. Registrar itens da venda cancelada
                foreach (var item in produtos)
                {
                    var comandoItem = new MySqlCommand(
                        "INSERT INTO itens_venda_cancelados (codigo_venda, produto_id, nome_produto, quantidade, valor_unitario) " +
                        "VALUES (@codigo, @produto, @nome, @quantidade, @valor)",
                        conexao);

                    comandoItem.Parameters.AddWithValue("@codigo", codigoVenda);
                    comandoItem.Parameters.AddWithValue("@produto", item.ProdutoId);
                    comandoItem.Parameters.AddWithValue("@nome", item.Nome);
                    comandoItem.Parameters.AddWithValue("@quantidade", item.Quantidade);
                    comandoItem.Parameters.AddWithValue("@valor", item.ValorUnitario);

                    comandoItem.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao registrar venda cancelada: {ex.Message}");
        }
    }

    private static (int id, string nome, string cpf) SelecionarCliente()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SELECIONAR CLIENTE ===");
            Console.Write("Digite o CPF do cliente (apenas números) ou 0 para cancelar: ");
            string cpfInput = Console.ReadLine().Trim();

            // Se o usuário digitar 0, cancela a operação
            if (cpfInput == "0")
            {
                return (0, null, null);
            }

            // Validar se o CPF contém apenas números e tem 11 dígitos
            if (cpfInput.Length != 11 || !long.TryParse(cpfInput, out _))
            {
                Console.WriteLine("CPF inválido! Deve conter exatamente 11 números.");
                Console.ReadKey();
                continue;
            }

            // Formatando o CPF para o padrão 000.000.000-00
            string cpfFormatado = $"{cpfInput.Substring(0, 3)}.{cpfInput.Substring(3, 3)}.{cpfInput.Substring(6, 3)}-{cpfInput.Substring(9, 2)}";

            try
            {
                using (var conexao = new MySqlConnection(connectionString))
                {
                    conexao.Open();

                    // Buscar cliente no banco de dados
                    var comando = new MySqlCommand(
                        "SELECT id, nome, cpf FROM clientes WHERE cpf = @cpf",
                        conexao);
                    comando.Parameters.AddWithValue("@cpf", cpfFormatado);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Cliente encontrado
                            int clienteId = reader.GetInt32("id");
                            string nomeCliente = reader.GetString("nome");
                            string cpfCliente = reader.GetString("cpf");

                            Console.WriteLine($"\nCliente encontrado: {nomeCliente}");
                            Console.WriteLine($"CPF: {cpfCliente}");
                            Console.WriteLine("Pressione qualquer tecla para continuar...");
                            Console.ReadKey();
                            return (clienteId, nomeCliente, cpfCliente);
                        }
                        else
                        {
                            // Cliente não encontrado
                            Console.WriteLine($"\nCPF {cpfFormatado} não cadastrado.");
                            Console.WriteLine("Deseja cadastrar este cliente agora? (S/N)");
                            string resposta = Console.ReadLine().Trim().ToUpper();

                            if (resposta == "S")
                            {
                                // Chamar rotina de cadastro de cliente
                                Console.WriteLine("\nRedirecionando para cadastro...");
                                Console.ReadKey();

                                int novoClienteId = CadastroCliente.Cadastrar(cpfInput);

                                if (novoClienteId > 0)
                                {
                                    // Obter os dados do novo cliente cadastrado
                                    using (var conexao2 = new MySqlConnection(connectionString))
                                    {
                                        conexao2.Open();
                                        var comandoDados = new MySqlCommand(
                                            "SELECT nome, cpf FROM clientes WHERE id = @id",
                                            conexao2);
                                        comandoDados.Parameters.AddWithValue("@id", novoClienteId);

                                        using (var readerDados = comandoDados.ExecuteReader())
                                        {
                                            if (readerDados.Read())
                                            {
                                                string novoNomeCliente = readerDados.GetString("nome");
                                                string novoCpfCliente = readerDados.GetString("cpf");
                                                return (novoClienteId, novoNomeCliente, novoCpfCliente);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Cadastro não concluído. Tente novamente.");
                                    Console.ReadKey();
                                    continue;
                                }
                            }
                            else
                            {
                                // Usuário optou por não cadastrar
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar cliente: {ex.Message}");
                Console.ReadKey();
                continue;
            }
        }
    }
   
    private static ItemVenda AdicionarProduto()
    {
        // Implementar busca de produtos
        Console.Write("Digite o código de barras ou ID do produto: ");
        string input = Console.ReadLine();

        // Lógica para buscar produto no banco
        // Retorna null se cancelar

        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine());

        // Exemplo de retorno
        return new ItemVenda
        {
            ProdutoId = 1,
            Nome = "Produto Exemplo",
            ValorUnitario = 99.90m,
            Quantidade = quantidade
        };
    }
 
    private static void RemoverProduto(List<ItemVenda> produtos, ref decimal valorTotal)
    {
        Console.WriteLine("\nProdutos na venda:");
        for (int i = 0; i < produtos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {produtos[i].Nome} - {produtos[i].Quantidade}x R$ {produtos[i].ValorUnitario:F2}");
        }

        Console.Write("\nDigite o número do produto a remover: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= produtos.Count)
        {
            // Solicitar autenticação do supervisor
            if (AutenticarSupervisor())
            {
                valorTotal -= produtos[index - 1].ValorTotal;
                produtos.RemoveAt(index - 1);
                Console.WriteLine("\nProduto removido com sucesso!");
            }
            else
            {
                Console.WriteLine("Autenticação falhou. Operação cancelada.");
            }
        }
        else
        {
            Console.WriteLine("Índice inválido!");
        }
        Console.ReadKey();
    }

    private static bool AutenticarSupervisor()
    {
        Console.Clear();
        Console.WriteLine("=== AUTENTICAÇÃO DE SUPERVISOR ===");
        Console.Write("Usuário: ");
        string usuario = Console.ReadLine();

        Console.Write("Senha: ");
        string senha = LerSenhaComAsteriscos();

        // Verificar se é admin master
        if (usuario == LoginService.usuarioAdmin && senha == LoginService.senhaAdmin)
        {
            return true;
        }

        // Verificar no banco de dados
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT senha_hash FROM usuarios WHERE login = @login AND tipo_usuario = 'Supervisor'",
                    conexao);
                comando.Parameters.AddWithValue("@login", usuario);

                var senhaHash = comando.ExecuteScalar()?.ToString();
                if (senhaHash != null && BCrypt.Net.BCrypt.Verify(senha, senhaHash))
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na autenticação: {ex.Message}");
        }

        return false;
    }

    private static string LerSenhaComAsteriscos()
    {
        string senha = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (!char.IsControl(key.KeyChar))
            {
                senha += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && senha.Length > 0)
            {
                senha = senha.Remove(senha.Length - 1);
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        return senha;
    }

    private static void FinalizarVenda(string codigoVenda, int clienteId, int atendenteId,
        List<ItemVenda> produtos, decimal valorTotal)
    {
        Console.WriteLine("\nForma de pagamento:");
        Console.WriteLine("1. Dinheiro");
        Console.WriteLine("2. Cartão");
        Console.WriteLine("3. PIX");
        Console.Write("Opção: ");

        string formaPagamento = Console.ReadLine() switch
        {
            "1" => "Dinheiro",
            "2" => "Cartão",
            "3" => "PIX",
            _ => "Dinheiro"
        };

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();

                // Iniciar transação
                using (var transaction = conexao.BeginTransaction())
                {
                    try
                    {
                        // 1. Inserir venda principal
                        var comandoVenda = new MySqlCommand(
                            "INSERT INTO vendas (codigo_venda, cliente_id, valor_total, forma_pagamento, atendente_id) " +
                            "VALUES (@codigo, @cliente, @valor, @pagamento, @atendente); SELECT LAST_INSERT_ID();",
                            conexao, transaction);

                        comandoVenda.Parameters.AddWithValue("@codigo", codigoVenda);
                        comandoVenda.Parameters.AddWithValue("@cliente", clienteId);
                        comandoVenda.Parameters.AddWithValue("@valor", valorTotal);
                        comandoVenda.Parameters.AddWithValue("@pagamento", formaPagamento);
                        comandoVenda.Parameters.AddWithValue("@atendente", atendenteId);

                        int vendaId = Convert.ToInt32(comandoVenda.ExecuteScalar());

                        // 2. Inserir itens da venda
                        foreach (var item in produtos)
                        {
                            var comandoItem = new MySqlCommand(
                                "INSERT INTO itens_venda (venda_id, produto_id, quantidade, valor_unitario) " +
                                "VALUES (@venda, @produto, @quantidade, @valor)",
                                conexao, transaction);

                            comandoItem.Parameters.AddWithValue("@venda", vendaId);
                            comandoItem.Parameters.AddWithValue("@produto", item.ProdutoId);
                            comandoItem.Parameters.AddWithValue("@quantidade", item.Quantidade);
                            comandoItem.Parameters.AddWithValue("@valor", item.ValorUnitario);

                            comandoItem.ExecuteNonQuery();
                        }

                        // Confirmar transação
                        transaction.Commit();
                        Console.WriteLine("\n✅ Venda registrada com sucesso!");
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Erro ao registrar venda: {ex.Message}");
        }
        Console.ReadKey();
    }
}

public class ItemVenda
{
    public int ProdutoId { get; set; }
    public string Nome { get; set; }
    public decimal ValorUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal => ValorUnitario * Quantidade;
}