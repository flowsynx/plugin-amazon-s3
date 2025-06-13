# Amazon S3 Plugin – FlowSynx Platform Integration

**Version:** 1.0.0  
**Author:** FlowSynx  
**License:** © FlowSynx. All rights reserved.  
**Repository:** [github.com/flowsynx/plugin-amazon-s3](https://github.com/flowsynx/plugin-amazon-s3)

## Overview

The **Amazon S3 Plugin** is a pre-packaged, plug-and-play integration component for the FlowSynx engine. It enables secure and configurable access to Amazon S3 cloud storage as part of FlowSynx’s no-code/low-code automation workflows.

This plugin can be installed automatically by the FlowSynx engine when selected within the platform. It is not intended for manual installation or developer use outside the FlowSynx environment.

---

## Purpose

This plugin allows FlowSynx users to interact with Amazon S3 for a variety of storage-related operations—without writing code. Once installed, the plugin becomes available as a storage connector within the platform's workflow builder.

---

## Supported Operations

The plugin supports the following operations, which can be selected and configured within the FlowSynx UI:

| Operation | Description                          |
|----------|--------------------------------------|
| `create` | Upload a new object to the S3 bucket. |
| `delete` | Remove an object from the bucket.     |
| `exist`  | Check if an object exists.            |
| `list`   | List all objects in the bucket.       |
| `purge`  | Remove all contents in the bucket.    |
| `read`   | Read and return the contents of an object. |
| `write`  | Overwrite or write new data to an object. |

---

## Plugin Metadata

| Field         | Value                                           |
|---------------|-------------------------------------------------|
| Plugin ID     | `b961131b-04cb-48df-9554-4252dc66c04c`          |
| Name          | `Amazon.S3`                                     |
| Company       | `FlowSynx`                                      |
| Namespace     | `Connectors`                                    |
| Version       | `1.0.0`                                         |
| Authors       | `FlowSynx`                                      |
| Tags          | `FlowSynx`, `Amazon`, `S3`, `Cloud`             |
| License       | © FlowSynx. All rights reserved.                |

---

## Notes

- This plugin is only supported on the FlowSynx platform.
- It is installed automatically by the FlowSynx engine.
- All operational logic is encapsulated and executed under FlowSynx control.
- Credentials are configured through platform-managed settings.

---

## License

© FlowSynx. All rights reserved.