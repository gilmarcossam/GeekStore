using System;
using System.Collections.Generic;

public static class CadastroUser
{
    private static List<Usuario> usuariosCadastrados = new List<Usuario>();

    public static void CadastrarUsuario()
    {
        Console.Clear();
        Console.WriteLine("=== CADASTRO DE NOVO USUÁRIO ===");

        // Selecionar tipo de usuário
        string tipoUsuario = SelecionarTipoUsuario();

        // Solicitar credenciais
        var (nomeUsuario, senha) = SolicitarCredenciais();

        // Criar e armazenar o novo usuário
        usuariosCadastrados.Add(new Usuario(nomeUsuario, senha, tipoUsuario));

        Console.WriteLine($"\nUsuário {tipoUsuario} cadastrado com sucesso!");
        Console.WriteLine("Pressione qualquer tecla para voltar...");
        Console.ReadKey();
    }

    private static string SelecionarTipoUsuario()
    {
        Console.WriteLine("\nSelecione o tipo de usuário:");
        Console.WriteLine("1 - Cliente");
        Console.WriteLine("2 - Caixa");
        Console.WriteLine("3 - Estoquista");
        Console.WriteLine("4 - Supervisor");
        Console.WriteLine("5 - Financeiro");

        while (true)
        {
            Console.Write("Opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": return "Cliente";
                case "2": return "Caixa";
                case "3": return "Estoquista";
                case "4": return "Supervisor";
                case "5": return "Financeiro";
                default:
                    Console.WriteLine("Opção inválida! Digite um número de 1 a 5");
                    break;
            }
        }
    }

    private static (string nomeUsuario, string senha) SolicitarCredenciais()
    {
        string nomeUsuario;
        do
        {
            Console.Write("\nNome de usuário: ");
            nomeUsuario = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(nomeUsuario))
            {
                Console.WriteLine("Nome de usuário não pode ser vazio!");
            }
            else if (UsuarioExiste(nomeUsuario))
            {
                Console.WriteLine("Usuário já existe! Escolha outro nome.");
                nomeUsuario = null;
            }
        } while (string.IsNullOrEmpty(nomeUsuario));

        string senha;
        do
        {
            Console.Write("Senha (mínimo 6 caracteres): ");
            senha = Console.ReadLine();

            if (senha.Length < 6)
            {
                Console.WriteLine("A senha deve ter no mínimo 6 caracteres!");
            }
        } while (senha.Length < 6);

        // Confirmar senha
        string confirmacao;
        do
        {
            Console.Write("Confirme a senha: ");
            confirmacao = Console.ReadLine();

            if (senha != confirmacao)
            {
                Console.WriteLine("As senhas não coincidem!");
            }
        } while (senha != confirmacao);

        return (nomeUsuario, senha);
    }

    private static bool UsuarioExiste(string nomeUsuario)
    {
        return usuariosCadastrados.Exists(u =>
            u.NomeUsuario.Equals(nomeUsuario, StringComparison.OrdinalIgnoreCase));
    }
}

// Classe Usuario (coloque em um arquivo separado se necessário)
public class Usuario
{
    public string NomeUsuario { get; set; }
    public string Senha { get; set; }
    public string TipoUsuario { get; set; }

    public Usuario(string nomeUsuario, string senha, string tipoUsuario)
    {
        NomeUsuario = nomeUsuario;
        Senha = senha;
        TipoUsuario = tipoUsuario;
    }
}