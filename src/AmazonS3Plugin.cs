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
                Version = new PluginVersion(1, 0, 0),
                Namespace = PluginNamespace.Connectors,
                Authors = new List<string> { "FlowSynx" },
                Copyright = "© FlowSynx. All rights reserved.",
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

    public async Task<object?> ExecuteAsync(PluginParameters parameters, CancellationToken cancellationToken)
    {
        if (ReflectionHelper.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var operationParameter = parameters.ToObject<OperationParameter>();
        var operation = operationParameter.Operation;

        switch (operation.ToLower())
        {
            case "create":
                await _manager.Create(parameters, cancellationToken).ConfigureAwait(false);
                return null;
            case "delete":
                await _manager.Delete(parameters, cancellationToken).ConfigureAwait(false);
                return null;
            case "exist":
                return await _manager.Exist(parameters, cancellationToken).ConfigureAwait(false);
            case "list":
                return await _manager.List(parameters, cancellationToken).ConfigureAwait(false);
            case "purge":
                await _manager.Purge(parameters, cancellationToken).ConfigureAwait(false);
                return null;
            case "read":
                return await _manager.Read(parameters, cancellationToken).ConfigureAwait(false);
            case "write":
                await _manager.Write(parameters, cancellationToken).ConfigureAwait(false);
                return null;
            default:
                throw new NotSupportedException($"Amazon S3 plugin: Operation '{operation}' is not supported.");
        }
    }
}