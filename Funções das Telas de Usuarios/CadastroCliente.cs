using MySql.Data.MySqlClient;
using System;

public static class CadastroCliente
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";
    private static Random random = new Random();

    // Nova versão que pode ser chamada com CPF pré-definido e retorna o ID do cliente
    public static int Cadastrar(string cpf = null)
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

        // Se o CPF foi passado como parâmetro, exibe e não pede novamente
        if (cpf != null)
        {
            Console.WriteLine($"CPF: {FormatarCPF(cpf)}");
        }
        else
        {
            Console.Write("CPF (apenas números): ");
            cpf = Console.ReadLine();
        }

        Console.Write("RG (apenas números): ");
        string rg = Console.ReadLine();

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

                // Primeiro verifica se o CPF já existe (caso o usuário tenha pulado alguma etapa)
                if (CPFExisteNoBanco(cpf))
                {
                    Console.WriteLine("\n❌ Este CPF já está cadastrado!");
                    Console.ReadKey();
                    return 0;
                }

                var comando = new MySqlCommand(
                    "INSERT INTO clientes (codigo, rg, cpf, nome, endereco, telefone, email) " +
                    "VALUES (@codigo, @rg, @cpf, @nome, @endereco, @telefone, @email); " +
                    "SELECT LAST_INSERT_ID();",  // Retorna o ID do cliente inserido
                    conexao
                );

                comando.Parameters.AddWithValue("@codigo", codigo);
                comando.Parameters.AddWithValue("@rg", rg);
                comando.Parameters.AddWithValue("@cpf", FormatarCPF(cpf));
                comando.Parameters.AddWithValue("@nome", nome);
                comando.Parameters.AddWithValue("@endereco", endereco);
                comando.Parameters.AddWithValue("@telefone", telefone);
                comando.Parameters.AddWithValue("@email", email);

                int novoClienteId = Convert.ToInt32(comando.ExecuteScalar());
                Console.WriteLine("\n✅ Cliente cadastrado com sucesso! Código: " + codigo);
                Console.ReadKey();
                return novoClienteId;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n❌ Erro: " + ex.Message);
            Console.ReadKey();
            return 0;
        }
    }

    // Mantém o método original para compatibilidade
    public static void Cadastrar()
    {
        Cadastrar(null); // Chama a nova versão sem CPF pré-definido
    }

    // Formata o CPF para o padrão 000.000.000-00
    private static string FormatarCPF(string cpf)
    {
        if (cpf.Length != 11 || !long.TryParse(cpf, out _))
            return cpf; // Retorna sem formatação se não for válido

        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    // Verifica se um CPF já existe no banco
    private static bool CPFExisteNoBanco(string cpf)
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT 1 FROM clientes WHERE cpf = @cpf",
                    conexao
                );
                comando.Parameters.AddWithValue("@cpf", FormatarCPF(cpf));
                return comando.ExecuteScalar() != null;
            }
        }
        catch
        {
            return false;
        }
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