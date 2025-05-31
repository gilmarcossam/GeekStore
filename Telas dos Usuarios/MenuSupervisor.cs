public static class MenuSupervisor
{
    public static void MenuPrincipal(int usuarioId)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU SUPERVISOR ===");
            Console.WriteLine("1. Relatório de Vendas Canceladas");
            Console.WriteLine("2. Produtos do Estoque");
            Console.WriteLine("3. Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    RelatorioVendasCanceladas.MenuVendasCanceladas(usuarioId);
                    break;
                case "2":
                    ListarProdutos.Listar();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
