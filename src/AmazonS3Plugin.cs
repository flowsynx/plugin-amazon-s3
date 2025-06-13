using FlowSynx.PluginCore;
using FlowSynx.Plugins.Amazon.S3.Models;
using FlowSynx.Plugins.Amazon.S3.Services;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.PluginCore.Helpers;

namespace FlowSynx.Plugins.Amazon.S3;

public class AmazonS3Plugin : IPlugin
{
    private IAmazonS3Manager _manager = null!;
    private AmazonS3Specifications _s3Specifications = null!;
    private bool _isInitialized;

    public PluginMetadata Metadata { 
        get
        {
            return new PluginMetadata
            {
                Id = Guid.Parse("b961131b-04cb-48df-9554-4252dc66c04c"),
                Name = "Amazon.S3",
                CompanyName = "FlowSynx",
                Description = Resources.ConnectorDescription,
                Version = new PluginVersion(1, 1, 0),
                Namespace = PluginNamespace.Connectors,
                Authors = new List<string> { "FlowSynx" },
                Copyright = "© FlowSynx. All rights reserved.",
                Icon = "flowsynx.png",
                ReadMe = "README.md",
                RepositoryUrl = "https://github.com/flowsynx/plugin-amazon-s3",
                ProjectUrl = "https://flowsynx.io",
                Tags = new List<string>() { "FlowSynx", "Amazon", "S3", "Cloud" }
            };
        }
    }

    public PluginSpecifications? Specifications { get; set; }
    public Type SpecificationsType => typeof(AmazonS3Specifications);

    public Task Initialize(IPluginLogger logger)
    {
        if (ReflectionHelper.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        ArgumentNullException.ThrowIfNull(logger);
        var connection = new AmazonS3Connection();
        _s3Specifications = Specifications.ToObject<AmazonS3Specifications>();
        var client = connection.Connect(_s3Specifications);
        _manager = new AmazonS3Manager(logger, client, _s3Specifications.Bucket);
        _isInitialized = true;
        return Task.CompletedTask;
    }

    public Task<object?> ExecuteAsync(PluginParameters parameters, CancellationToken cancellationToken)
    {
        if (ReflectionHelper.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var operationParameter = parameters.ToObject<OperationParameter>();
        var operation = operationParameter.Operation;

        if (OperationMap.TryGetValue(operation, out var handler))
        {
            return handler(parameters, cancellationToken);
        }

        throw new NotSupportedException($"Amazon S3 plugin: Operation '{operation}' is not supported.");
    }

    private Dictionary<string, Func<PluginParameters, CancellationToken, Task<object?>>> OperationMap => new(StringComparer.OrdinalIgnoreCase)
    {
        ["create"] = async (parameters, cancellationToken) => { await _manager.Create(parameters, cancellationToken); return null; },
        ["delete"] = async (parameters, cancellationToken) => { await _manager.Delete(parameters, cancellationToken); return null; },
        ["exist"] = async (parameters, cancellationToken) => await _manager.Exist(parameters, cancellationToken),
        ["list"] = async (parameters, cancellationToken) => await _manager.List(parameters, cancellationToken),
        ["purge"] = async (parameters, cancellationToken) => { await _manager.Purge(parameters, cancellationToken); return null; },
        ["read"] = async (parameters, cancellationToken) => await _manager.Read(parameters, cancellationToken),
        ["write"] = async (parameters, cancellationToken) => { await _manager.Write(parameters, cancellationToken); return null; },
    };

    public IReadOnlyCollection<string> SupportedOperations => OperationMap.Keys;
}