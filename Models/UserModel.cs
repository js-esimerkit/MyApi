using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace MyApi
{
    public class User
    {
        public int id_user { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        internal Database Db { get; set; }

        public User()
        {
        }

        internal User(Database db)
        {
            Db = db;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User ;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<User> FindOneAsync(int id_User)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  User  WHERE  id_User  = @id_User";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_User",
                DbType = DbType.Int32,
                Value = id_User,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }


        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  User ";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }
    

        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO  User  ( username ,  password ) VALUES (@username, @password);";
            BindParams(cmd);
            try
            {
                await cmd.ExecuteNonQueryAsync();
                int id_user = (int) cmd.LastInsertedId;
                return id_user; 
            }
            catch (System.Exception)
            {   
                return 0;
            } 
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE  User  SET  username  = @username,  password  = @password WHERE  id_user  = @id_User;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  User  WHERE  id_user  = @id_user;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<User>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<User>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new User(Db)
                    {
                        id_user = reader.GetInt32(0),
                        username = reader.GetString(1),
                        password = reader.GetString(2)
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
        
        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_user",
                DbType = DbType.Int32,
                Value = id_user,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@username",
                DbType = DbType.String,
                Value = username,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@password",
                DbType = DbType.String,
                Value = password,
            });

        }
    }
}