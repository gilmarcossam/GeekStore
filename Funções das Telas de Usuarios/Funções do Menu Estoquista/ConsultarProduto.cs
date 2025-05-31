using MySql.Data.MySqlClient;
using System;

public static class ConsultarProduto
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void Consultar()
    {
        Console.Clear();
        Console.WriteLine("=== CONSULTAR PRODUTO ===");

        Console.Write("Digite o ID, código de barras ou nome do produto: ");
        string input = Console.ReadLine();

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT * FROM produtos WHERE " +
                    "id = @id OR codigo_barras = @codigo OR nome LIKE @nome",
                    conexao);

                comando.Parameters.AddWithValue("@id", int.TryParse(input, out int id) ? id : 0);
                comando.Parameters.AddWithValue("@codigo", input);
                comando.Parameters.AddWithValue("@nome", $"%{input}%");

                using (var reader = comando.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("\nNenhum produto encontrado!");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("\n=== DETALHES DO PRODUTO ===");
                            Console.WriteLine($"ID: {reader["id"]}");
                            Console.WriteLine($"Código de Barras: {reader["codigo_barras"]}");
                            Console.WriteLine($"Nome: {reader["nome"]}");
                            Console.WriteLine($"Categoria: {reader["categoria"]}");
                            Console.WriteLine($"Fabricante: {reader["fabricante"]}");
                            Console.WriteLine($"Quantidade: {reader["quantidade"]}");
                            Console.WriteLine($"Valor: R$ {reader["valor"]:N2}");

                            if (reader["categoria"].ToString() == "Jogo" || reader["categoria"].ToString() == "Acessório")
                            {
                                Console.WriteLine($"Plataforma: {reader["plataforma"]}");
                                Console.WriteLine($"Garantia: {reader["garantia_meses"]} meses");
                            }

                            Console.WriteLine($"Data de Cadastro: {Convert.ToDateTime(reader["data_cadastro"]):dd/MM/yyyy}");
                            Console.WriteLine("----------------------------------------");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao consultar produto: {ex.Message}");
        }
        Console.WriteLine("\nPressione qualquer tecla para voltar...");
        Console.ReadKey();
    }
}