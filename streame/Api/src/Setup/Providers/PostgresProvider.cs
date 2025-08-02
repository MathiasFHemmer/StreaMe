using api.Core.Repository;
using Streame.Data.Repositories;
using Streame.Data.UnitOfWork;
using Streame.Providers.Postgres.Repositories;

public static class PostgresProvider
{
    public static void AddPostgresProvider(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
            .AddTransient<IMovieRepository, MovieRepository>();
    }
}