using MySql.Data.MySqlClient;
using System;

public static class LoginService
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    // Credenciais do Admin Master (fixas)
    public static string usuarioAdmin = "admin";
    public static string senhaAdmin = "admin123";

    public static void FazerLogin()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("========== Sistema de Login - GeekStore ==========\n");
            Console.Write("Digite seu usuário: ");
            string usuario = Console.ReadLine()!;

            Console.Write("Digite sua senha: ");
            string senha = LerSenhaComAsteriscos();
            Console.WriteLine();

            // Primeiro verifica se é o Admin Master
            if (usuario == usuarioAdmin && senha == senhaAdmin)
            {
                Console.WriteLine("Login bem-sucedido como Admin Master!");
                MenuService.MenuPrincipal(); // Menu específico para Admin Master
                break;
            }

            // Se não for Admin Master, verifica no banco de dados
            Usuario usuarioLogado = VerificarUsuarioNoBanco(usuario, senha);

            if (usuarioLogado != null)
            {
                Console.WriteLine($"Login bem-sucedido como {usuarioLogado.TipoUsuario}!");
                RedirecionarParaMenu(usuarioLogado);
                break;
            }
            else
            {
                Console.WriteLine("Usuário ou senha incorretos. Tente novamente.");
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }
        while (true);
    }

    private static Usuario VerificarUsuarioNoBanco(string login, string senha)
    {
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "SELECT id, login, senha_hash, tipo_usuario FROM usuarios WHERE login = @login",
                    conexao);
                comando.Parameters.AddWithValue("@login", login);

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string senhaHash = reader["senha_hash"].ToString();

                        // Verifica se a senha está correta usando BCrypt
                        if (BCrypt.Net.BCrypt.Verify(senha, senhaHash))
                        {
                            return new Usuario(
                                Convert.ToInt32(reader["id"]),
                                reader["login"].ToString(),
                                reader["tipo_usuario"].ToString()
                            );
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar usuário: {ex.Message}");
        }
        return null;
    }

    private static void RedirecionarParaMenu(Usuario usuario)
    {
        switch (usuario.TipoUsuario)
        {
            case "Cliente":
                MenuCliente.MenuPrincipal(usuario.Id);
                break;
            case "Caixa":
                MenuCaixa.MenuPrincipal(usuario.Id);
                break;
            case "Estoquista":
                MenuEstoquista.MenuPrincipal(usuario.Id);
                break;
            case "Supervisor":
                MenuSupervisor.MenuPrincipal(usuario.Id);
                break;
            case "Financeiro":
                MenuFinanceiro.MenuPrincipal(usuario.Id);
                break;
            default:
                Console.WriteLine("Tipo de usuário não reconhecido.");
                break;
        }
    }

    private static string LerSenhaComAsteriscos()
    {
        string senha = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (!char.IsControl(key.KeyChar))
            {
                senha += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && senha.Length > 0)
            {
                senha = senha.Remove(senha.Length - 1);
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return senha;
    }
}

// Classe auxiliar para representar o usuário
public class Usuario
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string TipoUsuario { get; set; }

    public Usuario(int id, string login, string tipoUsuario)
    {
        Id = id;
        Login = login;
        TipoUsuario = tipoUsuario;
    }
}