public static class MenuCaixa
{
    public static void MenuPrincipal()
    {
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
1. Realizar Venda
2. Cadastrar um Cliente
3. Consultar um Produto
4. Cancelar uma Venda
5. Fechar Caixa
6. Voltar/Sair
");

                string opcao = Console.ReadLine()!;

                switch (opcao)
                {
                    case "1":
                        //CadastroUser.CadastrarUsuario();
                        break;
                    case "2":
                        CadastroCliente.Cadastrar();
                        break;
                    case "3":
                        Console.WriteLine("Alteração de Admin seria feita aqui.");
                        Console.ReadLine();
                        break;
                    case "6":
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
}