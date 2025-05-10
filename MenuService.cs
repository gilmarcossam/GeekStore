static class MenuService
{
    public static void MenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("==========Menu Principal da Administração==========\n");
        Console.WriteLine("Escolha uma opção:");
        Console.WriteLine("1.Gerenciar Usuários"); // Cadastro, Alteração e Exclusão de Usuários
        Console.WriteLine("2.Lista de Usuários");
        Console.WriteLine("3. Alterar Usuário e Senha do Admin");
        Console.WriteLine("4. Sair");


        string opcao = Console.ReadLine()!;
        switch (opcao)
        {
            case "1":
                Console.WriteLine("Você escolheu a Opção 1.");
                // GerenciarUsuarios();
                break;
            case "2":
                Console.WriteLine("Você escolheu a Opção 2.");
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Opção inválida. Pressione qualquer tecla para continuar...");
                Console.ReadKey();
                MenuPrincipal();
                break;
        }
    }
}