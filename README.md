# FlowSynx Amazon S3 Plugin

The Amazon S3 Plugin is a pre-packaged, plug-and-play integration component for the FlowSynx engine. It enables interacting with Amazon S3 buckets, allowing workflows to read, write, list, and manage S3 objects securely. Designed for FlowSynx’s no-code/low-code automation workflows, this plugin simplifies cloud storage integration for data pipelines and automation tasks.

This plugin is automatically installed by the FlowSynx engine when selected within the platform. It is not intended for manual installation or standalone developer use outside the FlowSynx environment.

---

## Purpose

The Amazon S3 Plugin allows FlowSynx users to:

- Upload data to Amazon S3 buckets.
- Download or read data from S3 objects.
- List, delete, or check the existence of S3 objects.
- Purge S3 buckets when needed.
- Integrate Amazon S3 operations into automation workflows without writing code.

---

## Supported Operations

- **create**: Creates a new object in the specified bucket and path.
- **delete**: Deletes an object at the specified path in the bucket.
- **exist**: Checks if an object exists at the specified path.
- **list**: Lists objects under a specified path (prefix), with filtering and optional metadata.
- **purge**: Deletes all objects under the specified path, optionally forcing deletion.
- **read**: Reads and returns the contents of an object at the specified path.
- **write**: Writes data to a specified path in the bucket, with support for overwrite.

---

## Plugin Specifications

The plugin requires the following configuration:

- `AccessKey` (string): **Required.** The AWS Access Key ID for authentication.
- `SecretKey` (string): **Required.** The AWS Secret Access Key for authentication.
- `Region` (string): **Required.** The AWS region of the S3 bucket (e.g., `us-east-1`).
- `Bucket` (string): **Required.** The name of the target S3 bucket.
- `SessionToken` (string): Optional. The AWS session token if using temporary credentials.

### Example Configuration

```json
{
  "AccessKey": "AKIAEXAMPLE123456",
  "SecretKey": "abc123secretkeyexample",
  "Region": "us-east-1",
  "Bucket": "my-flowsynx-bucket",
  "SessionToken": "FQoGZXIvYXdzEJr..."
}
```

---

## Input Parameters

Each operation accepts specific parameters:

### Create
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path where the new object is created.|

### Delete
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the object to delete.        |

### Exist
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the object to check.         |

### List
| Parameter         | Type    | Required | Description                                         |
|--------------------|---------|----------|-----------------------------------------------------|
| `Path`             | string  | Yes      | The prefix path to list objects from.              |
| `Filter`           | string  | No       | A filter pattern for object names.                 |
| `Recurse`          | bool    | No       | Whether to list recursively. Default: `false`.     |
| `CaseSensitive`    | bool    | No       | Whether the filter is case-sensitive. Default: `false`. |
| `IncludeMetadata`  | bool    | No       | Whether to include object metadata. Default: `false`. |
| `MaxResults`       | int     | No       | Maximum number of objects to list. Default: `2147483647`. |

### Purge
| Parameter     | Type    | Required | Description                                    |
|---------------|---------|----------|------------------------------------------------|
| `Path`        | string  | Yes      | The path prefix to purge.                     |
| `Force`       | bool    | No       | Whether to force deletion without confirmation.|

### Read
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the object to read.          |

### Write
| Parameter     | Type    | Required | Description                                                  |
|---------------|---------|----------|--------------------------------------------------------------|
| `Path`        | string  | Yes      | The path where data should be written.                      |
| `Data`        | object  | Yes      | The data to write to the S3 object.                          |
| `Overwrite`   | bool    | No       | Whether to overwrite if the object already exists. Default: `false`. |

### Example input (Write)

```json
{
  "Operation": "write",
  "Path": "documents/report.json",
  "Data": {
    "title": "Monthly Report",
    "content": "This is the report content."
  },
  "Overwrite": true
}
```

---

## Debugging Tips

- Ensure `AccessKey`, `SecretKey`, and `Region` are correct and the credentials have appropriate S3 permissions.
- Verify the `Bucket` exists and is accessible from the FlowSynx environment.
- Use the `Exist` operation to confirm object presence before performing `Read` or `Delete`.
- For large listings, adjust `MaxResults` to limit returned data.
- When using temporary credentials, ensure `SessionToken` is valid and not expired.

---

## Amazon S3 Considerations

- **Object Key Case Sensitivity**: S3 object keys are case-sensitive. Ensure correct casing when specifying paths.
- **Eventual Consistency**: Deletions and listings may have a short delay due to eventual consistency.
- **Overwrite Behavior**: The `Write` operation will fail if `Overwrite` is `false` and the object exists.
- **Region Specificity**: Buckets exist within regions. Cross-region operations are not supported by this plugin.
- **Large Objects**: For objects over 5 GB, consider using multipart upload (not supported directly in this plugin).

---

## Security Notes

- AWS credentials are used only during plugin execution and are not persisted by FlowSynx.
- Access is controlled by FlowSynx platform user roles and permissions.
- Always use IAM users with least privilege to limit S3 access to only required buckets and actions.

---

## License

© FlowSynx. All rights reserved.