using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{
    /// <summary>
    /// Foi criada a classe Contexto, ela é a classe de contexto do banco de dados da estrutura de entidade
    /// A classe DBContext é para importar o pacote necessário do entity framework
    /// </summary>
    public class ApplicationDBContext : DbContext
    {
        /// <summary>
        /// Aqui foi criado o construtor e no construtor precisamos de um parâmetro do tipo DB context options
        /// Chamamos ele de Options e passamos ele para a classe Base e salvamos a classe na pasta Services
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
                
        }

        /// <summary>
        /// Propriedade que permite criar uma tabela 
        /// Propriedade do tipo DBSet 
        /// Após criar essas propriedades executar o script de migration para criar o arquivo do banco de dados
        /// e em seguida executar o update database
        /// </summary>
        public DbSet<Product> Products { get; set; }
    }
}
