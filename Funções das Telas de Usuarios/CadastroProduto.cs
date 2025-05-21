using MySql.Data.MySqlClient;
using System;

public static class CadastroProduto
{
    private static string connectionString = "Server=localhost;Database=sistema_usuarios;Uid=root;Pwd=255S@m255;";

    public static void CadastrarNovoProduto()
    {
        Console.Clear();
        Console.WriteLine("=== CADASTRO DE PRODUTO ===");

        // Dados básicos
        Console.Write("Código de barras: ");
        string codigoBarras = Console.ReadLine();

        Console.Write("Nome do produto: ");
        string nome = Console.ReadLine();

        Console.WriteLine("Categoria (1-Jogo | 2-Acessório | 3-Produto Geek): ");
        int categoriaOp = int.Parse(Console.ReadLine());
        string categoria = categoriaOp switch
        {
            1 => "Jogo",
            2 => "Acessório",
            3 => "Produto Geek",
            _ => "Produto Geek"
        };

        Console.Write("Fabricante: ");
        string fabricante = Console.ReadLine();

        Console.Write("Quantidade em estoque: ");
        int quantidade = int.Parse(Console.ReadLine());

        Console.Write("Valor unitário (R$): ");
        decimal valor = decimal.Parse(Console.ReadLine());

        // Campos específicos (para Jogos/Acessórios)
        string plataforma = null;
        int? garantiaMeses = null;

        if (categoria == "Jogo" || categoria == "Acessório")
        {
            Console.Write("Plataforma (ex: PS5, Xbox, PC): ");
            plataforma = Console.ReadLine();

            Console.Write("Garantia (meses): ");
            garantiaMeses = int.Parse(Console.ReadLine());
        }

        // Salva no banco
        try
        {
            using (var conexao = new MySqlConnection(connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand(
                    "INSERT INTO produtos (codigo_barras, nome, categoria, fabricante, quantidade, valor, plataforma, garantia_meses) " +
                    "VALUES (@codigo, @nome, @categoria, @fabricante, @quantidade, @valor, @plataforma, @garantia)",
                    conexao
                );

                comando.Parameters.AddWithValue("@codigo", codigoBarras);
                comando.Parameters.AddWithValue("@nome", nome);
                comando.Parameters.AddWithValue("@categoria", categoria);
                comando.Parameters.AddWithValue("@fabricante", fabricante);
                comando.Parameters.AddWithValue("@quantidade", quantidade);
                comando.Parameters.AddWithValue("@valor", valor);
                comando.Parameters.AddWithValue("@plataforma", plataforma ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@garantia", garantiaMeses ?? (object)DBNull.Value);

                comando.ExecuteNonQuery();
                Console.WriteLine("\n✅ Produto cadastrado com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Erro: {ex.Message}");
        }
        Console.ReadKey();
    }
}