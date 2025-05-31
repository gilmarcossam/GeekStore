public static class MenuFinanceiro
{
    public static void MenuPrincipal(int usuarioId)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU FINANCEIRO ===");
            Console.WriteLine("1. Relatório de Vendas Canceladas");
            Console.WriteLine("2. Relatório de Vendas Realizadas");
            Console.WriteLine("3. Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine())
            {
                case "1":
                    RelatorioVendasCanceladas.MenuVendasCanceladas(usuarioId);
                    break;
                case "2":
                    RelatorioVendasRealizadas.MenuVendasRealizadas(usuarioId);
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
            }
        }
    }
}