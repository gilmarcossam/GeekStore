using MySql.Data.MySqlClient;
using System;

public static class CadastroCliente
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";
    private static Random random = new Random();

    public static void Cadastrar()
    {
        Console.Clear();
        Console.WriteLine("=== CADASTRO DE CLIENTE ===");

        // Gera um código aleatório de 6 dígitos e verifica se já existe
        string codigo;
        do
        {
            codigo = GerarCodigoAleatorio();
        } while (CodigoExisteNoBanco(codigo));

        Console.WriteLine($"Código do cliente gerado: {codigo}");

        Console.Write("RG (apenas números): ");
        string rg = Console.ReadLine();

        Console.Write("CPF (apenas números): ");
        string cpf = Console.ReadLine();

        Console.Write("Nome completo: ");
        string nome = Console.ReadLine();

        Console.Write("Endereço: ");
        string endereco = Console.ReadLine();

        Console.Write("Telefone: ");
        string telefone = Console.ReadLine();

        Console.Write("E-mail: ");
        string email = Console.ReadLine();

        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "INSERT INTO clientes (codigo, rg, cpf, nome, endereco, telefone, email) " +
                    "VALUES (@codigo, @rg, @cpf, @nome, @endereco, @telefone, @email)",
                    conexao
                );

                comando.Parameters.AddWithValue("@codigo", codigo);
                comando.Parameters.AddWithValue("@rg", rg);
                comando.Parameters.AddWithValue("@cpf", cpf);
                comando.Parameters.AddWithValue("@nome", nome);
                comando.Parameters.AddWithValue("@endereco", endereco);
                comando.Parameters.AddWithValue("@telefone", telefone);
                comando.Parameters.AddWithValue("@email", email);

                comando.ExecuteNonQuery();
                Console.WriteLine("\n✅ Cliente cadastrado com sucesso! Código: " + codigo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n❌ Erro: " + ex.Message);
        }
        Console.ReadKey();
    }

    // Gera um código de 6 dígitos aleatório
    private static string GerarCodigoAleatorio()
    {
        return random.Next(100000, 999999).ToString();
    }

    // Verifica se o código já existe no banco
    private static bool CodigoExisteNoBanco(string codigo)
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT 1 FROM clientes WHERE codigo = @codigo",
                    conexao
                );
                comando.Parameters.AddWithValue("@codigo", codigo);
                return comando.ExecuteScalar() != null;
            }
        }
        catch
        {
            return false;
        }
    }
}