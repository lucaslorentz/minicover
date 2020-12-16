using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniCover.Core.FileSystem;
using MiniCover.Core.Hits;
using MiniCover.Core.Instrumentation;
using MiniCover.Core.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMiniCoverCore(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.TryAddSingleton<IFileSystem, FileSystem>();
            services.TryAddSingleton<DepsJsonUtils>();
            services.TryAddSingleton<IFileReader, CachedFileReader>();
            services.TryAddSingleton<IHitsResetter, HitsResetter>();
            services.TryAddSingleton<IHitsReader, HitsReader>();
            services.TryAddSingleton<IInstrumenter, Instrumenter>();
            services.TryAddSingleton<IUninstrumenter, Uninstrumenter>();
            services.TryAddSingleton<IAssemblyInstrumenter, AssemblyInstrumenter>();
            services.TryAddSingleton<ITypeInstrumenter, TypeInstrumenter>();
            services.TryAddSingleton<IMethodInstrumenter, MethodInstrumenter>();

            return services;
        }
    }
}
