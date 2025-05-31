public static class MenuEstoquista
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
                Console.WriteLine("==========Gerenciar Produtos do Estoque==========");
                Console.ResetColor();

                Console.WriteLine(@"
Escolha uma opção:
1. Cadastrar um Produto
2. Excluir um Produto
3. Listar Produtos
4. Alterar Produto 
5. Consultar Produto 
6. Voltar/Sair
");

                string opcao = Console.ReadLine()!;

                switch (opcao)
                {
                    case "1":
                        CadastroProduto.CadastrarNovoProduto();
                        break;
                    case "2":
                        ExcluirProduto.Excluir();
                        break;
                    case "3":
                        ListarProdutos.Listar();
                        break;
                    case "4":
                        AlterarProduto.Alterar();
                        break;
                    case "5":
                        ConsultarProduto.Consultar();
                        break;
                    case "6":
                       Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
