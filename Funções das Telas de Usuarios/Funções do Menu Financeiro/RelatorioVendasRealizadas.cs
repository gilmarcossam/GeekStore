using MySql.Data.MySqlClient;
using System;

public static class RelatorioVendasRealizadas
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void MenuVendasRealizadas(int usuarioId)
    {
        if (!VerificarPermissaoFinanceiro(usuarioId))
        {
            Console.WriteLine("Acesso negado. Somente o setor financeiro pode acessar este relatório.");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIO DE VENDAS REALIZADAS ===");
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
                    @"SELECT v.codigo_venda, c.cpf, c.nome as cliente, u.login as atendente, 
                             v.valor_total, v.forma_pagamento, v.data_venda
                      FROM vendas v
                      JOIN clientes c ON v.cliente_id = c.id
                      JOIN usuarios u ON v.atendente_id = u.id
                      WHERE DATE(v.data_venda) BETWEEN @inicio AND @fim
                      ORDER BY v.data_venda DESC",
                    conexao);

                comando.Parameters.AddWithValue("@inicio", dataInicio.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@fim", dataFim.ToString("yyyy-MM-dd"));

                using (var reader = comando.ExecuteReader())
                {
                    Console.Clear();
                    Console.WriteLine($"VENDAS REALIZADAS ENTRE {dataInicio:dd/MM/yyyy} E {dataFim:dd/MM/yyyy}");
                    Console.WriteLine("====================================================================");
                    Console.WriteLine("Código\t\tData\t\tCliente\t\tValor\t\tForma Pagto");
                    Console.WriteLine("--------------------------------------------------------------------");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda encontrada no período selecionado.");
                    }
                    else
                    {
                        decimal totalPeriodo = 0;
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["codigo_venda"]}\t" +
                                            $"{Convert.ToDateTime(reader["data_venda"]):dd/MM/yyyy}\t" +
                                            $"{reader["cliente"]}\t" +
                                            $"R$ {Convert.ToDecimal(reader["valor_total"]):N2}\t" +
                                            $"{reader["forma_pagamento"]}");
                            totalPeriodo += Convert.ToDecimal(reader["valor_total"]);
                        }
                        Console.WriteLine("--------------------------------------------------------------------");
                        Console.WriteLine($"TOTAL DO PERÍODO: R$ {totalPeriodo:N2}");
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
                    @"SELECT v.codigo_venda, v.valor_total, v.forma_pagamento, v.data_venda, 
                             u.login as atendente
                      FROM vendas v
                      JOIN clientes c ON v.cliente_id = c.id
                      JOIN usuarios u ON v.atendente_id = u.id
                      WHERE c.cpf = @cpf
                      ORDER BY v.data_venda DESC",
                    conexao);

                comando.Parameters.AddWithValue("@cpf", cpfFormatado);

                using (var reader = comando.ExecuteReader())
                {
                    Console.Clear();
                    Console.WriteLine($"VENDAS REALIZADAS PARA O CPF: {cpfFormatado}");
                    Console.WriteLine("=================================================");
                    Console.WriteLine("Código\t\tData\t\tValor\t\tForma Pagto");
                    Console.WriteLine("-------------------------------------------------");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda encontrada para este CPF.");
                    }
                    else
                    {
                        decimal totalCliente = 0;
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["codigo_venda"]}\t" +
                                            $"{Convert.ToDateTime(reader["data_venda"]):dd/MM/yyyy}\t" +
                                            $"R$ {Convert.ToDecimal(reader["valor_total"]):N2}\t" +
                                            $"{reader["forma_pagamento"]}");
                            totalCliente += Convert.ToDecimal(reader["valor_total"]);
                        }
                        Console.WriteLine("-------------------------------------------------");
                        Console.WriteLine($"TOTAL DO CLIENTE: R$ {totalCliente:N2}");
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
                    @"SELECT v.codigo_venda, c.nome as cliente, c.cpf, u.login as atendente, 
                             v.valor_total, v.forma_pagamento, v.data_venda
                      FROM vendas v
                      JOIN clientes c ON v.cliente_id = c.id
                      JOIN usuarios u ON v.atendente_id = u.id
                      WHERE v.codigo_venda = @codigo",
                    conexao);

                comandoVenda.Parameters.AddWithValue("@codigo", codigo);

                using (var readerVenda = comandoVenda.ExecuteReader())
                {
                    if (!readerVenda.HasRows)
                    {
                        Console.WriteLine("\nNenhuma venda encontrada com este código.");
                        Console.ReadKey();
                        return;
                    }

                    readerVenda.Read();
                    Console.Clear();
                    Console.WriteLine($"DETALHES DA VENDA: {readerVenda["codigo_venda"]}");
                    Console.WriteLine("=================================================");
                    Console.WriteLine($"Data/Hora: {Convert.ToDateTime(readerVenda["data_venda"]):dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"Cliente: {readerVenda["cliente"]} (CPF: {readerVenda["cpf"]})");
                    Console.WriteLine($"Atendente: {readerVenda["atendente"]}");
                    Console.WriteLine($"Forma de Pagamento: {readerVenda["forma_pagamento"]}");
                    Console.WriteLine($"Valor Total: R$ {Convert.ToDecimal(readerVenda["valor_total"]):N2}");
                }

                // Consulta os itens da venda
                var comandoItens = new MySqlCommand(
                    @"SELECT p.nome as produto, iv.quantidade, iv.valor_unitario, 
                      (iv.quantidade * iv.valor_unitario) as valor_total
                      FROM itens_venda iv
                      JOIN produtos p ON iv.produto_id = p.id
                      WHERE iv.venda_id = (SELECT id FROM vendas WHERE codigo_venda = @codigo)",
                    conexao);

                comandoItens.Parameters.AddWithValue("@codigo", codigo);

                using (var readerItens = comandoItens.ExecuteReader())
                {
                    Console.WriteLine("\nITENS DA VENDA:");
                    Console.WriteLine("-------------------------------------------------");
                    Console.WriteLine("Produto\t\tQtd\tVl. Unit.\tVl. Total");

                    while (readerItens.Read())
                    {
                        Console.WriteLine($"{readerItens["produto"]}\t" +
                                        $"{readerItens["quantidade"]}\t" +
                                        $"{Convert.ToDecimal(readerItens["valor_unitario"]):N2}\t\t" +
                                        $"{Convert.ToDecimal(readerItens["valor_total"]):N2}");
                    }
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