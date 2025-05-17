public class MenuAdm1
{
    public static void GerenciarUsuarios()
    {
        bool continuar = true;

        while (continuar)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("==========Gerenciar Usuários do Sistema==========");
            Console.ResetColor();

            Console.WriteLine(@"
Escolha uma opção:
1. Casdastrar novo Usuário
2. Alterar Usuário
3. Excluir Usuário
4. Voltar/Sair
");

            string opcao = Console.ReadLine()!;

            switch (opcao)
            {
                case "1":
                    CadastroUser.CadastrarUsuario();
                    break;
                case "2":
                    Console.WriteLine("Lista de Usuários seria exibida aqui.");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("Alteração de Admin seria feita aqui.");
                    Console.ReadLine();
                    break;
                case "4":
                    MenuService.MenuPrincipal();
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    Console.ReadLine();
                    break;
            }
        }
    }
}