public class MenuAdm3
{
    public static void AlterarSenha()
    {
        bool continuar = true;

        while (continuar)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("==========Gerenciar Acesso de Admin==========");
            Console.ResetColor();

            Console.WriteLine(@"
Escolha uma opção:
1. Alterar Usuário
2. Alterar Senha
3. Adicionar Admin
4. Excluir Admin
5. Voltar/Sair
");

            string opcao = Console.ReadLine()!;

            switch (opcao)
            {
                case "1":
                    Console.WriteLine("Funcionalidade de Gerenciar Usuários seria implementada aqui.");
                    Console.ReadLine();
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
                    Console.WriteLine("Alteração de Admin seria feita aqui.");
                    Console.ReadLine();
                    break;
                case "5":
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