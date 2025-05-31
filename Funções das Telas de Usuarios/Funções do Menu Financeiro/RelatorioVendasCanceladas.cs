using MySql.Data.MySqlClient;

public static class RelatorioVendasCanceladas
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void MenuVendasCanceladas(int usuarioId)
    {
        // Verificar se o usuário tem permissão (Financeiro)
        if (!VerificarPermissaoFinanceiro(usuarioId))
        {
            Console.WriteLine("Acesso negado. Somente o setor financeiro pode acessar este relatório.");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIO DE VENDAS CANCELADAS ===");
            Console.WriteLine("1. Consultar por período");
            Console.WriteLine("2. Consultar por CPF do cliente");
            Console.WriteLine("3. Consultar por código da venda");
            Console.WriteLine("4. Voltar");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    ConsultarPorPeriodo();
                    break;
                case "2":
                    ConsultarPorCPF();
                    break;
                case "3":
                    ConsultarPorCodigo();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static bool VerificarPermissaoFinanceiro(int usuarioId)
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT tipo_usuario FROM usuarios WHERE id = @id",
                    conexao);
                comando.Parameters.AddWithValue("@id", usuarioId);

                var tipoUsuario = comando.ExecuteScalar()?.ToString();
                return tipoUsuario == "Financeiro" || tipoUsuario == "Admin";
            }
        }
        catch
        {
            return false;
        }
    }

    private static void ConsultarPorPeriodo()
    {
        Console.Clear();
        Console.WriteLine("=== CONSULTAR POR PERÍODO ===");

        Console.Write("Data inicial (DD/MM/AAAA): ");
        DateTime dataInicio;
        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio))
        {
            Console.WriteLine("Formato inválido! Use DD/MM/AAAA");
            Console.Write("Data inicial (DD/MM/AAAA): ");
        }

        Console.Write("Data final (DD/MM/AAAA): ");
        DateTime dataFim;
        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim))
        {
            Console.WriteLine("Formato inválido! Use DD/MM/AAAA");
            Console.Write("Data final (DD/MM/AAAA): ");
        }

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    @"SELECT vc.codigo_venda, vc.cpf_cliente, vc.valor_total, vc.data_cancelamento, 
                             c.nome as nome_cliente, u.login as atendente
                      FROM vendas_canceladas vc
                      LEFT JOIN clientes c ON vc.cliente_id = c.id
                      LEFT JOIN usuarios u ON vc.atendente_id = u.id
                      WHERE DATE(vc.data_cancelamento) BETWEEN @inicio AND @fim
                      ORDER BY vc.data_cancelamento DESC",
                    conexao);

                comando.Parameters.AddWithValue("@inicio", dataInicio.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@fim", dataFim.ToString("yyyy-MM-dd"));

                using (var reader = comando.ExecuteReader())
                {
                    Console.Clear();
                    Console.WriteLine($"VENDAS CANCELADAS ENTRE {dataInicio:dd/MM/yyyy} E {dataFim:dd/MM/yyyy}");
                    Console.WriteLine("==================================================================");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda cancelada no período selecionado.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"\nCódigo: {reader["codigo_venda"]}");
                            Console.WriteLine($"Data: {Convert.ToDateTime(reader["data_cancelamento"]):dd/MM/yyyy HH:mm}");
                            Console.WriteLine($"Cliente: {reader["nome_cliente"]} (CPF: {reader["cpf_cliente"]})");
                            Console.WriteLine($"Atendente: {reader["atendente"]}");
                            Console.WriteLine($"Valor Total: R$ {Convert.ToDecimal(reader["valor_total"]):N2}");
                            Console.WriteLine("------------------------------------------------------------------");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar vendas: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    private static void ConsultarPorCPF()
    {
        Console.Clear();
        Console.WriteLine("=== CONSULTAR POR CPF ===");
        Console.Write("Digite o CPF (apenas números): ");
        string cpf = Console.ReadLine();

        if (cpf.Length != 11 || !long.TryParse(cpf, out _))
        {
            Console.WriteLine("CPF inválido! Deve conter 11 números.");
            Console.ReadKey();
            return;
        }

        string cpfFormatado = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    @"SELECT vc.codigo_venda, vc.cpf_cliente, vc.valor_total, vc.data_cancelamento, 
                             c.nome as nome_cliente, u.login as atendente
                      FROM vendas_canceladas vc
                      LEFT JOIN clientes c ON vc.cliente_id = c.id
                      LEFT JOIN usuarios u ON vc.atendente_id = u.id
                      WHERE vc.cpf_cliente = @cpf
                      ORDER BY vc.data_cancelamento DESC",
                    conexao);

                comando.Parameters.AddWithValue("@cpf", cpfFormatado);

                using (var reader = comando.ExecuteReader())
                {
                    Console.Clear();
                    Console.WriteLine($"VENDAS CANCELADAS PARA O CPF: {cpfFormatado}");
                    Console.WriteLine("================================================");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda cancelada encontrada para este CPF.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"\nCódigo: {reader["codigo_venda"]}");
                            Console.WriteLine($"Data: {Convert.ToDateTime(reader["data_cancelamento"]):dd/MM/yyyy HH:mm}");
                            Console.WriteLine($"Cliente: {reader["nome_cliente"]}");
                            Console.WriteLine($"Atendente: {reader["atendente"]}");
                            Console.WriteLine($"Valor Total: R$ {Convert.ToDecimal(reader["valor_total"]):N2}");
                            Console.WriteLine("----------------------------------------");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar vendas: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    private static void ConsultarPorCodigo()
    {
        Console.Clear();
        Console.WriteLine("=== CONSULTAR POR CÓDIGO ===");
        Console.Write("Digite o código da venda: ");
        string codigo = Console.ReadLine();

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();

                // Consulta o cabeçalho da venda
                var comandoVenda = new MySqlCommand(
                    @"SELECT vc.codigo_venda, vc.cpf_cliente, vc.valor_total, vc.data_cancelamento, 
                             c.nome as nome_cliente, u.login as atendente
                      FROM vendas_canceladas vc
                      LEFT JOIN clientes c ON vc.cliente_id = c.id
                      LEFT JOIN usuarios u ON vc.atendente_id = u.id
                      WHERE vc.codigo_venda = @codigo",
                    conexao);

                comandoVenda.Parameters.AddWithValue("@codigo", codigo);

                using (var readerVenda = comandoVenda.ExecuteReader())
                {
                    if (!readerVenda.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda cancelada encontrada com este código.");
                        Console.ReadKey();
                        return;
                    }

                    readerVenda.Read();
                    Console.Clear();
                    Console.WriteLine($"DETALHES DA VENDA CANCELADA: {readerVenda["codigo_venda"]}");
                    Console.WriteLine("==================================================================");
                    Console.WriteLine($"Data/Hora: {Convert.ToDateTime(readerVenda["data_cancelamento"]):dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"Cliente: {readerVenda["nome_cliente"]} (CPF: {readerVenda["cpf_cliente"]})");
                    Console.WriteLine($"Atendente: {readerVenda["atendente"]}");
                    Console.WriteLine($"Valor Total: R$ {Convert.ToDecimal(readerVenda["valor_total"]):N2}");
                }

                // Consulta os itens da venda
                var comandoItens = new MySqlCommand(
                    @"SELECT nome_produto, quantidade, valor_unitario, 
                      (quantidade * valor_unitario) as valor_total
                      FROM itens_venda_cancelados
                      WHERE codigo_venda = @codigo",
                    conexao);

                comandoItens.Parameters.AddWithValue("@codigo", codigo);

                using (var readerItens = comandoItens.ExecuteReader())
                {
                    Console.WriteLine("\nITENS DA VENDA:");
                    Console.WriteLine("------------------------------------------------------------------");
                    Console.WriteLine("Produto\t\tQtd\tVl. Unit.\tVl. Total");

                    decimal totalGeral = 0;
                    while (readerItens.Read())
                    {
                        Console.WriteLine($"{readerItens["nome_produto"]}\t" +
                                        $"{readerItens["quantidade"]}\t" +
                                        $"{Convert.ToDecimal(readerItens["valor_unitario"]):N2}\t\t" +
                                        $"{Convert.ToDecimal(readerItens["valor_total"]):N2}");
                        totalGeral += Convert.ToDecimal(readerItens["valor_total"]);
                    }

                    Console.WriteLine("------------------------------------------------------------------");
                    Console.WriteLine($"TOTAL GERAL:\t\t\t\tR$ {totalGeral:N2}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar venda: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }
}