﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataLoader.GraphQL.StarWars.Schema;
using GraphQL;
using Microsoft.AspNetCore.Mvc;

namespace DataLoader.GraphQL.StarWars.Controllers
{
    [Route("api/graphql")]
    public class GraphQLController : Controller
    {
        private readonly IDocumentExecuter _executer = new DocumentExecuter();
        private readonly StarWarsSchema _schema = new StarWarsSchema();

        public GraphQLController()
        {
            Console.WriteLine($"GraphQLController created");
        }

        [HttpPost]
        public async Task<ExecutionResult> Post([FromBody] GraphQLRequest request)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Executing GraphQL query");

            var sw = Stopwatch.StartNew();

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = request.Query;
                _.UserContext = new GraphQLUserContext();
                _.Listeners.Add(new DataLoaderListener());
            });

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Finished query ({sw.ElapsedMilliseconds}ms)");
            sw.Stop();

            return result;
        }
    }

    public class GraphQLRequest
    {
        public string Query { get; set; }
        public object Variables { get; set; }
    }
}