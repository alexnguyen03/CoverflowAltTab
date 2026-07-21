using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace CoverflowAltTab.Extensibility;

public sealed class ExtensionLoader
{
    private readonly ExtensionRegistry _registry;
    private readonly ILogger _logger;

    public ExtensionLoader(ExtensionRegistry registry, ILogger logger)
    {
        _registry = registry;
        _logger = logger;
    }

    public IReadOnlyList<ExtensionLoadResult> LoadFromDirectory(string extensionsDirectory)
    {
        Directory.CreateDirectory(extensionsDirectory);

        var results = new List<ExtensionLoadResult>();
        var loadedAssemblyPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var manifestPath in Directory.EnumerateFiles(extensionsDirectory, "*.manifest.json", SearchOption.TopDirectoryOnly))
        {
            results.AddRange(LoadFromManifest(extensionsDirectory, manifestPath, loadedAssemblyPaths));
        }

        foreach (var dllPath in Directory.EnumerateFiles(extensionsDirectory, "*.dll", SearchOption.TopDirectoryOnly))
        {
            var fullDllPath = Path.GetFullPath(dllPath);
            if (loadedAssemblyPaths.Contains(fullDllPath))
            {
                continue;
            }

            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fullDllPath);
                foreach (var type in GetExtensionTypes(assembly))
                {
                    if (Activator.CreateInstance(type) is not IExtension extension)
                    {
                        continue;
                    }

                    var summary = new ExtensionRegistrationSummary();
                    try
                    {
                        extension.Initialize(new HostContext(_registry, _logger, summary));
                        _logger.Info($"Loaded extension '{extension.Id}' from '{dllPath}'.");
                        results.Add(new ExtensionLoadResult(
                            dllPath,
                            extension.Id,
                            extension.Name,
                            "1.0.0",
                            string.Empty,
                            true,
                            "Loaded successfully.",
                            summary.RegisteredWindowFilters,
                            summary.RegisteredSortStrategies,
                            summary.RegisteredOverlayDismissBehaviors));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Extension '{type.FullName}' failed during initialization.", ex);
                        results.Add(new ExtensionLoadResult(
                            dllPath,
                            type.FullName ?? type.Name,
                            type.Name,
                            "1.0.0",
                            string.Empty,
                            false,
                            ex.Message,
                            summary.RegisteredWindowFilters,
                            summary.RegisteredSortStrategies,
                            summary.RegisteredOverlayDismissBehaviors));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load extension assembly '{dllPath}'.", ex);
                results.Add(new ExtensionLoadResult(dllPath, string.Empty, Path.GetFileNameWithoutExtension(dllPath), "1.0.0", string.Empty, false, ex.Message, 0, 0, 0));
            }
        }

        return results;
    }

    private IReadOnlyList<ExtensionLoadResult> LoadFromManifest(
        string extensionsDirectory,
        string manifestPath,
        HashSet<string> loadedAssemblyPaths)
    {
        try
        {
            var manifest = JsonSerializer.Deserialize<ExtensionManifest>(
                File.ReadAllText(manifestPath),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            if (manifest is null)
            {
                return
                [
                    new ExtensionLoadResult(manifestPath, string.Empty, Path.GetFileNameWithoutExtension(manifestPath), "1.0.0", string.Empty, false, "Manifest could not be parsed.", 0, 0, 0),
                ];
            }

            var assemblyPath = Path.GetFullPath(Path.Combine(extensionsDirectory, manifest.Assembly));
            loadedAssemblyPaths.Add(assemblyPath);

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            var types = GetExtensionTypes(assembly);
            if (!string.IsNullOrWhiteSpace(manifest.EntryType))
            {
                types = types.Where(type => string.Equals(type.FullName, manifest.EntryType, StringComparison.Ordinal)).ToArray();
            }

            var results = new List<ExtensionLoadResult>();
            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is not IExtension extension)
                {
                    continue;
                }

                var summary = new ExtensionRegistrationSummary();
                try
                {
                    extension.Initialize(new HostContext(_registry, _logger, summary));
                    _logger.Info($"Loaded extension '{manifest.Id}' from manifest '{manifestPath}'.");
                    results.Add(new ExtensionLoadResult(
                        manifestPath,
                        manifest.Id,
                        manifest.Name,
                        manifest.Version,
                        manifest.Description,
                        true,
                        "Loaded successfully.",
                        summary.RegisteredWindowFilters,
                        summary.RegisteredSortStrategies,
                        summary.RegisteredOverlayDismissBehaviors));
                }
                catch (Exception ex)
                {
                    _logger.Error($"Extension '{manifest.Id}' failed during initialization.", ex);
                    results.Add(new ExtensionLoadResult(
                        manifestPath,
                        manifest.Id,
                        manifest.Name,
                        manifest.Version,
                        manifest.Description,
                        false,
                        ex.Message,
                        summary.RegisteredWindowFilters,
                        summary.RegisteredSortStrategies,
                        summary.RegisteredOverlayDismissBehaviors));
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to load extension manifest '{manifestPath}'.", ex);
            return
            [
                new ExtensionLoadResult(manifestPath, string.Empty, Path.GetFileNameWithoutExtension(manifestPath), "1.0.0", string.Empty, false, ex.Message, 0, 0, 0),
            ];
        }
    }

    private static IReadOnlyList<Type> GetExtensionTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes().Where(type =>
                typeof(IExtension).IsAssignableFrom(type) &&
                type is { IsAbstract: false, IsInterface: false } &&
                type.GetConstructor(Type.EmptyTypes) is not null).ToArray();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .OfType<Type>()
                .Where(type =>
                    typeof(IExtension).IsAssignableFrom(type) &&
                    type is { IsAbstract: false, IsInterface: false } &&
                    type.GetConstructor(Type.EmptyTypes) is not null)
                .ToArray();
        }
    }
}
