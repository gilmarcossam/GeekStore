static class LoginService
{
    public static string senhaAdmin = "admin123";
    public static string usuarioAdmin = "admin";

    public static void FazerLogin() 
        {
        do
        {
            Console.Clear();
            Console.WriteLine("========== Sistema de Login - GeekStore ==========\n");
            Console.Write("Digite seu usuário: ");
            string usuario = Console.ReadLine()!;

            Console.Write("Digite sua senha: ");
            string senha = LerSenhaComAsteriscos();
            Console.WriteLine();

            // Verifica se o usuário e a senha estão corretos
            if (usuario == usuarioAdmin && senha == senhaAdmin)
            {
                Console.WriteLine("Login bem-sucedido!");
                MenuService.MenuPrincipal();
                break;
            }
            else
            {
                Console.WriteLine("Usuário ou senha incorretos. Tente novamente.");
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        } 
        while (true);
    }
    private static string LerSenhaComAsteriscos() 
    {
        string senha = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            // Ignora qualquer tecla que não seja caractere, Backspace ou Enter
            if (!char.IsControl(key.KeyChar))
            {
                senha += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && senha.Length > 0)
            {
                senha = senha.Remove(senha.Length - 1);
                Console.Write("\b \b"); // Remove o último asterisco
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine(); // Pula uma linha após digitar a senha
        return senha;
    }


}