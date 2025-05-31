using MySql.Data.MySqlClient;
using System;

public static class ListarProdutos
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void Listar()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE PRODUTOS ===");

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT id, codigo_barras, nome, categoria, quantidade, valor FROM produtos ORDER BY nome",
                    conexao);

                using (var reader = comando.ExecuteReader())
                {
                    Console.WriteLine("ID  Código Barras\tNome\t\tCategoria\tQtd\tValor");
                    Console.WriteLine("--------------------------------------------------------------");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["id"]}\t" +
                                        $"{reader["codigo_barras"]}\t" +
                                        $"{reader["nome"]}\t" +
                                        $"{reader["categoria"]}\t" +
                                        $"{reader["quantidade"]}\t" +
                                        $"R$ {reader["valor"]}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao listar produtos: {ex.Message}");
        }
        Console.WriteLine("\nPressione qualquer tecla para voltar...");
        Console.ReadKey();
    }
}