using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System;

public static class CadastroUser
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void CadastrarUsuario()
    {
        Console.Clear();
        Console.WriteLine("=== CADASTRO DE USUÁRIO ===");

        // Pede os dados
        Console.Write("Digite o login: ");
        string login = Console.ReadLine();

        Console.Write("Digite a senha: ");
        string senha = Console.ReadLine();

        Console.WriteLine("Escolha o tipo:");
        Console.WriteLine("1 - Cliente | 2 - Caixa | 3 - Estoquista | 4 - Supervisor | 5 - Financeiro");
        int tipo = int.Parse(Console.ReadLine());

        string tipoUsuario = tipo switch
        {
            1 => "Cliente",
            2 => "Caixa",
            3 => "Estoquista",
            4 => "Supervisor",
            5 => "Financeiro",
            _ => "Cliente"
        };

        // Criptografa a senha com BCrypt
        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        // Salva no banco
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "INSERT INTO usuarios (login, senha_hash, tipo_usuario) VALUES (@login, @senha, @tipo)",
                    conexao
                );
                comando.Parameters.AddWithValue("@login", login);
                comando.Parameters.AddWithValue("@senha", senhaHash);
                comando.Parameters.AddWithValue("@tipo", tipoUsuario);
                comando.ExecuteNonQuery();
            }
            Console.WriteLine("✅ Usuário cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }
}