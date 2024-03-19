using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication50.Data
{
    public class FreeGiveawaysRepository
    {
        private string _connectionString;
        public FreeGiveawaysRepository(string connectionString)
        {
            _connectionString = connectionString;       
        }

        public void AddUser(User user, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                           "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", hash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public User GetByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if(user == null)
            {
                return null;
            }
            var isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isMatch)
            {
                return null;   
            }
            return user;
        }

        public List<Listing> GetListings()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Listings";
            connection.Open();
            var result = new List<Listing>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new()
                {
                    Id = (int)reader["Id"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    Text = (string)reader["Text"],
                    Name = reader.GetOrNull<string>("Name"),
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;
        }

        public List<Listing> GetListingsForUser(string email)
        {
            var user = GetByEmail(email);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT l.Id, l.Name, l.DateCreated, l.PhoneNumber, l.Text, l.UserId FROM Listings l JOIN Users u ON l.UserId = @id";
            cmd.Parameters.AddWithValue("@id", user.Id);
            connection.Open();
            var result = new List<Listing>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new()
                {
                    Id = (int)reader["Id"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    Text = (string)reader["Text"],
                    Name = reader.GetOrNull<string>("Name"),
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;
        }

        public void AddListing(Listing listing)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Listings(DateCreated, Text, Name, PhoneNumber, UserId) " +
                "VALUES(@date, @text, @name, @phone, @userId)";
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@text", listing.Text);
            cmd.Parameters.AddWithValue("@name", listing.Name);
            cmd.Parameters.AddWithValue("@phone", listing.PhoneNumber);
            cmd.Parameters.AddWithValue("@userId", listing.UserId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteListing(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE From Listings WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

    }
    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}
