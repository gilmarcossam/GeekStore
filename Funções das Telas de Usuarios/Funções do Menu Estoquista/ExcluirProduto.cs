using MySql.Data.MySqlClient;
using System;

public static class ExcluirProduto
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void Excluir()
    {
        Console.Clear();
        Console.WriteLine("=== EXCLUIR PRODUTO ===");

        Console.Write("Digite o ID ou código de barras do produto: ");
        string input = Console.ReadLine();

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();

                // Verificar se o produto existe
                var comandoVerificar = new MySqlCommand(
                    "SELECT nome FROM produtos WHERE id = @id OR codigo_barras = @codigo",
                    conexao);

                comandoVerificar.Parameters.AddWithValue("@id", int.TryParse(input, out int id) ? id : 0);
                comandoVerificar.Parameters.AddWithValue("@codigo", input);

                string nomeProduto = comandoVerificar.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(nomeProduto))
                {
                    Console.WriteLine("Produto não encontrado!");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nTem certeza que deseja excluir o produto: {nomeProduto}? (S/N)");
                string confirmacao = Console.ReadLine().ToUpper();

                if (confirmacao == "S")
                {
                    var comandoExcluir = new MySqlCommand(
                        "DELETE FROM produtos WHERE id = @id OR codigo_barras = @codigo",
                        conexao);

                    comandoExcluir.Parameters.AddWithValue("@id", id);
                    comandoExcluir.Parameters.AddWithValue("@codigo", input);

                    int linhasAfetadas = comandoExcluir.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("Produto excluído com sucesso!");
                    else
                        Console.WriteLine("Nenhum produto foi excluído.");
                }
                else
                {
                    Console.WriteLine("Operação cancelada.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao excluir produto: {ex.Message}");
        }
        Console.ReadKey();
    }
}