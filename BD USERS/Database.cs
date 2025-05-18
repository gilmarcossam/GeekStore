using MySql.Data.MySqlClient;
using System;

public static class Database
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void TestarConexao()
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                Console.WriteLine("✅ Conectado ao MySQL!");
                conexao.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }
}