public static class MenuCaixa
{
    public static void MenuPrincipal(int usuarioId)
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
3. Sair
");

                string opcao = Console.ReadLine()!;

                switch (opcao)
                {
                    case "1":
                        VendaService.RealizarVenda(usuarioId);
                        break;
                    case "2":
                        CadastroCliente.Cadastrar();
                        break;
                    case "3":
                        Environment.Exit(0);
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