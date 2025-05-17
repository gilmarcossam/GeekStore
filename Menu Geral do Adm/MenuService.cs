static class MenuService
{
    public static void MenuPrincipal()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        Console.WriteLine("==========Menu Principal da Administração==========");
        Console.ResetColor();

        Console.WriteLine(@"
Escolha uma opção:
1. Gerenciar Usuários
2. Lista de Usuários
3. Gerenciar Acesso de Admin
4. Sair
");


        string opcao = Console.ReadLine()!;
        switch (opcao)
        {
            case "1":
                MenuAdm1.GerenciarUsuarios();
                break;
            case "2":
                MenuAdm2.ListaUsuarios();
                break;
            case "3":
                MenuAdm3.AlterarSenha();
                break;
            case "4":
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