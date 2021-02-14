using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MiniTwitWebApi.Entities;
using MiniTwitWebApi.Models;
using static System.Net.HttpStatusCode;

namespace MiniTwitWebApi.Tests
{
    public class MiniTwitRepositoryTests
    {
        private readonly SqliteConnection _connection;
        
        private readonly MiniTwitContext _context;

		private readonly MiniTwitRepository _repository;

		public MiniTwitRepositoryTests()
		{
			//_connection = new SqliteConnection("Filename=:memory:");
			_connection = new SqliteConnection("Filename=minitwit.db");
			_connection.Open();
			var builder = new DbContextOptionsBuilder<MiniTwitContext>().UseSqlite(_connection);
			
			_context = new MiniTwitContext(builder.Options);
			_context.Database.EnsureCreated();

			_repository = new MiniTwitRepository(_context);
		}


		[Fact]
		public void test_count()
		{
			var ent = new Message();
			_context.Messages.Add(ent);
			var actual = _repository.Count();
			Assert.Equal(1 ,actual);
		}
    }
}