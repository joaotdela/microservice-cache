using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using web_app_performance.Model;

namespace web_app_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private static ConnectionMultiplexer redis;
        [HttpGet]
        public async Task<IActionResult> GetProduto()
        {
            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
            string user =await db.StringGetAsync(key);
            if (!string.IsNullOrEmpty(user)) {
                return Ok(user);
            
            }
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "select id, nome, preco, quantidade_estoque, data_criacao from produtos;";
            var produtos = await connection.QueryAsync<Produto>(query);
            string produtosJson= JsonConvert.SerializeObject(produtos);
            await db.StringSetAsync(key,produtosJson);

            return Ok(produtos);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
           
            
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = "insert into produtos(nome, preco, quantidade_estoque, data_criacao) values(@Nome, @Preco, @QuantidadeEstoque, @DataCriacao);";
            await connection.ExecuteAsync(sql,produto);

            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);




            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Produto produto)
        {


            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = "Update produtos set Nome = @nome, preco = @preco, quantidade_estoque=@QuantidadeEstoque,data_criacao = @DataCriacao where Id=@id";
            await connection.ExecuteAsync(sql, produto);

            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);




            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {


            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = @"delete from produtos where Id=@id";
            await connection.ExecuteAsync(sql, new { id });

            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);




            return Ok();
        }
    }
}
