using MySql.Data.MySqlClient;
using System;

public static class AlterarProduto
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void Alterar()
    {
        Console.Clear();
        Console.WriteLine("=== ALTERAR PRODUTO ===");

        Console.Write("Digite o ID ou código de barras do produto: ");
        string input = Console.ReadLine();

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();

                // Buscar produto
                var comandoBusca = new MySqlCommand(
                    "SELECT * FROM produtos WHERE id = @id OR codigo_barras = @codigo",
                    conexao);

                comandoBusca.Parameters.AddWithValue("@id", int.TryParse(input, out int id) ? id : 0);
                comandoBusca.Parameters.AddWithValue("@codigo", input);

                using (var reader = comandoBusca.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine("Produto não encontrado!");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine($"\nEditando produto: {reader["nome"]}");
                    Console.WriteLine("Deixe em branco para manter o valor atual.\n");

                    Console.Write($"Código de Barras [{reader["codigo_barras"]}]: ");
                    string codigoBarras = Console.ReadLine();
                    if (string.IsNullOrEmpty(codigoBarras)) codigoBarras = reader["codigo_barras"].ToString();

                    Console.Write($"Nome [{reader["nome"]}]: ");
                    string nome = Console.ReadLine();
                    if (string.IsNullOrEmpty(nome)) nome = reader["nome"].ToString();

                    Console.WriteLine($"Categoria atual: {reader["categoria"]}");
                    Console.WriteLine("Nova Categoria (1-Jogo, 2-Acessório, 3-Produto Geek, Enter-manter): ");
                    string categoriaInput = Console.ReadLine();
                    string categoria = string.IsNullOrEmpty(categoriaInput) ?
                        reader["categoria"].ToString() :
                        categoriaInput switch
                        {
                            "1" => "Jogo",
                            "2" => "Acessório",
                            "3" => "Produto Geek",
                            _ => reader["categoria"].ToString()
                        };

                    Console.Write($"Fabricante [{reader["fabricante"]}]: ");
                    string fabricante = Console.ReadLine();
                    if (string.IsNullOrEmpty(fabricante)) fabricante = reader["fabricante"].ToString();

                    Console.Write($"Quantidade [{reader["quantidade"]}]: ");
                    string quantidadeInput = Console.ReadLine();
                    int quantidade = string.IsNullOrEmpty(quantidadeInput) ?
                        Convert.ToInt32(reader["quantidade"]) :
                        int.Parse(quantidadeInput);

                    Console.Write($"Valor [{reader["valor"]}]: ");
                    string valorInput = Console.ReadLine();
                    decimal valor = string.IsNullOrEmpty(valorInput) ?
                        Convert.ToDecimal(reader["valor"]) :
                        decimal.Parse(valorInput);

                    string plataforma = reader["plataforma"]?.ToString();
                    int? garantia = reader["garantia_meses"] as int?;

                    if (categoria == "Jogo" || categoria == "Acessório")
                    {
                        Console.Write($"Plataforma [{plataforma}]: ");
                        string plataformaInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(plataformaInput)) plataforma = plataformaInput;

                        Console.Write($"Garantia (meses) [{garantia}]: ");
                        string garantiaInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(garantiaInput)) garantia = int.Parse(garantiaInput);
                    }

                    // Fechar o reader antes de executar o update
                    reader.Close();

                    var comandoUpdate = new MySqlCommand(
                        "UPDATE produtos SET " +
                        "codigo_barras = @codigo, nome = @nome, categoria = @categoria, " +
                        "fabricante = @fabricante, quantidade = @quantidade, valor = @valor, " +
                        "plataforma = @plataforma, garantia_meses = @garantia " +
                        "WHERE id = @id",
                        conexao);

                    comandoUpdate.Parameters.AddWithValue("@codigo", codigoBarras);
                    comandoUpdate.Parameters.AddWithValue("@nome", nome);
                    comandoUpdate.Parameters.AddWithValue("@categoria", categoria);
                    comandoUpdate.Parameters.AddWithValue("@fabricante", fabricante);
                    comandoUpdate.Parameters.AddWithValue("@quantidade", quantidade);
                    comandoUpdate.Parameters.AddWithValue("@valor", valor);
                    comandoUpdate.Parameters.AddWithValue("@plataforma", plataforma ?? (object)DBNull.Value);
                    comandoUpdate.Parameters.AddWithValue("@garantia", garantia ?? (object)DBNull.Value);
                    comandoUpdate.Parameters.AddWithValue("@id", reader["id"]);

                    int linhasAfetadas = comandoUpdate.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("\nProduto atualizado com sucesso!");
                    else
                        Console.WriteLine("\nNenhuma alteração foi realizada.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao alterar produto: {ex.Message}");
        }
        Console.ReadKey();
    }
}