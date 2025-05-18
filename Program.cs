class Program
{
    static void Main(string[] args)
    {
        Database.TestarConexao();  // ⬅️ A mensagem aparece AQUI!
        LoginService.FazerLogin(); // Só roda depois do teste
    }
}